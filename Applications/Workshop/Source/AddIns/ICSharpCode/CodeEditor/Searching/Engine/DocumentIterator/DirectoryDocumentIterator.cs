// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.TextEditor.Searching
{
	public sealed class DirectoryDocumentIterator : IDocumentIterator
	{
        private string[] _searchDirs;
        private string _searchDirectory;
        private string _fileMask;
        private bool _searchSubdirectories;
		
		private List<string> _searchFiles;
        private int _curFileIndex;
        private int _curDirIndex;
		
        private DirectoryDocumentIterator()
        {
            _curFileIndex = -1;
            _curDirIndex  = -1;
        }

		public DirectoryDocumentIterator(string searchDirectory, 
            string fileMask, bool searchSubdirectories) : this()
		{
            if (!String.IsNullOrEmpty(searchDirectory))
            {
                _searchDirs = searchDirectory.Split(';');

                _curDirIndex = 0;

                this._searchDirectory = _searchDirs[_curDirIndex];
                this._fileMask = fileMask;
                this._searchSubdirectories = searchSubdirectories;

                if (String.IsNullOrEmpty(this._fileMask))
                {
                    this._fileMask = "*.*";
                }
            }
			
			Reset();
		}
		
		public string CurrentFileName {
			get {
				if (_searchFiles == null || _curFileIndex < 0 || _curFileIndex >= _searchFiles.Count) {
					return null;
				}
				
				return _searchFiles[_curFileIndex];
			}
		}
				
		public SearchDocumentInfo Current {
			get {
				if (_curFileIndex < 0 || _curFileIndex >= _searchFiles.Count) {
					return null;
				}
				string fileName = _searchFiles[_curFileIndex];
				if (!File.Exists(fileName) || !SearchReplaceUtilities.IsSearchable(fileName)) {
					++_curFileIndex;
					return Current;
				}
				ITextDocument document;
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.PrimaryFileName != null &&
					    FileUtility.IsEqualFileName(content.PrimaryFileName, fileName) &&
					    content is ITextEditorControlProvider) {
						document = ((ITextEditorControlProvider)content).TextEditorControl.Document;
						return new SearchDocumentInfo(document,
						                                       fileName,
						                                       0);
					}
				}
				ITextBufferStrategy strategy = null;
				try {
					strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
				} catch (Exception) {
					return null;
				}
				return new SearchDocumentInfo(strategy, 
				                                       fileName, 
				                                       0);
			}
		}
		
		public bool MoveForward() 
		{
			if (_curFileIndex == -1) 
            {
                // The Directory.GetFiles used by this method does not support
                // multiple filters, so we split this operation...
                if (_searchFiles == null)
                {
                    _searchFiles = new List<string>();
                }
                string[] filters = this._fileMask.Split(';');
                int itemCount = (filters == null) ? 0 : filters.Length;
                for (int i = 0; i < itemCount; i++ )
                {
                    IList<string> fileList = FileUtility.SearchDirectory(this._searchDirectory,
                        filters[i], this._searchSubdirectories);
                    if (fileList != null && fileList.Count != 0)
                    {
                        _searchFiles.AddRange(fileList);
                    }
                }
			}
            if (_searchFiles != null && _curFileIndex >= _searchFiles.Count)
            {   
                if (this.NextDirectory())
                {
                    _curFileIndex = -1;
                    _searchFiles  = null;

                    return this.MoveForward();
                }
            }

			return ++_curFileIndex < _searchFiles.Count;
		}
		
		public bool MoveBackward()
		{
			if (_curFileIndex == -1) 
            {
				_curFileIndex = _searchFiles.Count - 1;
				return true;
			}

			return --_curFileIndex >= -1;
		}     		
		
		public void Reset() 
		{
			_curFileIndex = -1;
            _curDirIndex  = -1;
        }

        private bool NextDirectory()
        {
            _curDirIndex++;
            if (_searchDirs != null && _curDirIndex < _searchDirs.Length)
            {
                if (Directory.Exists(_searchDirs[_curDirIndex]))
                {
                    _searchDirectory = _searchDirs[_curDirIndex];

                    return true;
                }
                else
                {
                    return this.NextDirectory();
                }
            }

            return false;
        }

        private bool PreviousDirectory()
        {
            _curDirIndex--;
            if (_searchDirs != null && _curDirIndex >= 0 && 
                _curDirIndex < _searchDirs.Length)
            {
                if (Directory.Exists(_searchDirs[_curDirIndex]))
                {
                    _searchDirectory = _searchDirs[_curDirIndex];

                    return true;
                }
                else
                {
                    return this.PreviousDirectory();
                }
            }

            return false;
        }
	}
}
