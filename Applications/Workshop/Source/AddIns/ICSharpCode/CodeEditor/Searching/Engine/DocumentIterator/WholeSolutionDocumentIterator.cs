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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.TextEditor.Searching
{
    public sealed class WholeSolutionDocumentIterator : IDocumentIterator
    {
        #region Private Fields

        private int _curIndex;
		private List<string> _files;

        #endregion

        #region Constructors and Destructor

        public WholeSolutionDocumentIterator()
		{
            _files    = new List<string>();
            _curIndex = -1;

			Reset();
        }

        #endregion

        #region Public Properties

        public string CurrentFileName 
        {
			get 
            {
				if (_curIndex < 0 || _curIndex >= _files.Count) 
                {
					return null;
				}
				
				return _files[_curIndex];
			}
		}
				
		public SearchDocumentInfo Current {
			get {
				if (_curIndex < 0 || _curIndex >= _files.Count) {
					return null;
				}
				if (!File.Exists(_files[_curIndex])) {
					++_curIndex;
					return this.Current;
				}
				ITextDocument document;
				string fileName = _files[_curIndex];
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.PrimaryFileName != null &&
					    FileUtility.IsEqualFileName(content.PrimaryFileName, fileName) &&
					    content is ITextEditorControlProvider)
					{
						document = (((ITextEditorControlProvider)content).TextEditorControl).Document;
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

        #endregion

        #region Public Methods

        public bool MoveForward() 
		{
			return ++_curIndex < _files.Count;
		}
		
		public bool MoveBackward()
		{
			if (_curIndex == -1) {
				_curIndex = _files.Count - 1;
				return true;
			}
			return --_curIndex >= -1;
		}
		
		public void Reset() 
		{
			_files.Clear();

			if (ProjectService.OpenSolution != null) 
            {
				foreach (IProject project in ProjectService.OpenSolution.Projects) 
                {
					foreach (ProjectItem item in project.Items) 
                    {
						if (item is FileProjectItem && SearchReplaceUtilities.IsSearchable(item.FileName)) {
							_files.Add(item.FileName);
						}
					}
				}
			}
			
			_curIndex = -1;
        }

        #endregion
    }
}
