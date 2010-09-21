// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2365 $</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// it checks, if the files exists at every creation, and if not it doesn't list them in the 
	/// recent files, and they'll not be saved during the next option save.
	/// </summary>
	public sealed class RecentOpen : IXmlSerializable
    {
        #region Public Const Fields

        /// <summary>
		/// This variable is the maximal length of last file/open entries
		/// must be > 0
		/// </summary>
		public const int MaxDisplayedSize      = 32;
        public const int MaxRecentOpenSize     = 36;

        public const string XmlTagName         = "recentOpen";
        public const string FileCategory       = "RecentFiles";
        public const string ProjectCategory    = "RecentProjects";
        public const string RecentOpenFileName = "RecentOpen.xml";

        #endregion

        #region Private Fields

        private int _maximumLength;
        private RecentOpenList _recentFiles;
        private RecentOpenList _recentProjects;

        #endregion

        #region Private Interop

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern bool PathCompactPathEx([Out] StringBuilder pszOut, 
            string szPath, int cchMax, int dwFlags);

        #endregion

        #region Constructors and Destructor

        public RecentOpen()
        {
            _maximumLength  = 64;
            _recentFiles    = new RecentOpenList(FileCategory);
            _recentProjects = new RecentOpenList(ProjectCategory);
        }

        public RecentOpen(Properties p)
            : this()
        {
            if (p.Contains("Files"))
            {
                string[] files = p["Files"].Split(',');
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        _recentFiles.Add(new RecentOpenItem(false, file));
                    }
                }
            }

            if (p.Contains("Projects"))
            {
                string[] projects = p["Projects"].Split(',');
                foreach (string file in projects)
                {
                    if (File.Exists(file))
                    {
                        _recentProjects.Add(new RecentOpenItem(false, file));
                    }
                }
            }
        }

        #endregion

        #region Public Events

        public event EventHandler RecentFileChanged;
		public event EventHandler RecentProjectChanged;

        #endregion

        #region Public Properties

        public int MaximumLength
        {
            get
            {
                return _maximumLength;
            }   
            set
            {
                if (value > 16 && value < 256)
                {
                    _maximumLength = value;
                }
            }
        }

        public int DisplayableFiles
        {
            get
            {
                if (_recentFiles != null)
                {
                    return _recentFiles.Displayable;
                }

                return 10;
            }
            set
            {
                if (_recentFiles != null && value >= 0)
                {
                    _recentFiles.Displayable = value;
                }
            }
        }

        public int DisplayableProjects
        {
            get
            {
                if (_recentProjects != null)
                {
                    return _recentProjects.Displayable;
                }

                return 10;
            }
            set
            {
                if (_recentProjects != null && value >= 0)
                {
                    _recentProjects.Displayable = value;
                }
            }
        }

        public IList<RecentOpenItem> RecentFiles
        {
			get 
            {
				Debug.Assert(_recentFiles != null, 
                    "RecentOpen : set string[] LastFile (value == null)");
				return _recentFiles;
            }
		}

        public IList<RecentOpenItem> RecentProjects 
        {
			get 
            {
				Debug.Assert(_recentProjects != null, 
                    "RecentOpen : set string[] LastProject (value == null)");
				return _recentProjects;
			}
        }

        public IList<RecentOpenItem> RecentDisplayableProjects
        {
            get
            {
                Debug.Assert(_recentProjects != null,
                    "RecentOpen : set string[] LastProject (value == null)");

                return this.GetDisplayableProjects();
            }
        }

        #endregion

        #region Private Properties

        private IList<string> RecentFilePaths 
        {
			get 
            {
				Debug.Assert(_recentFiles != null, 
                    "RecentOpen : set string[] LastFile (value == null)");

                List<string> recentFiles = new List<string>(
                    _recentFiles.Count);
                for (int i = 0; i < _recentFiles.Count; i++)
                {
                    recentFiles.Add(_recentFiles[i].FullPath);
                }

                return recentFiles;
            }
		}

		private IList<string> RecentProjectPaths 
        {
			get 
            {
				Debug.Assert(_recentProjects != null, 
                    "RecentOpen : set string[] LastProject (value == null)");

                List<string> recentProject = new List<string>(
                    _recentProjects.Count);
                for (int i = 0; i < _recentProjects.Count; i++)
                {
                    recentProject.Add(_recentProjects[i].FullPath);
                }

                return recentProject;
			}
        }

        #endregion

        #region Public Methods

        public void AddLastFile(string name)
		{
            RecentOpenItem lastItem = null;
			for (int i = 0; i < _recentFiles.Count; ++i) 
            {
                RecentOpenItem recentItem = _recentFiles[i];
				if (String.Equals(recentItem.FullPath, name, 
                    StringComparison.OrdinalIgnoreCase)) 
                {
					_recentFiles.RemoveAt(i);
                    lastItem = recentItem;

                    break;
				}
			}

            while (_recentFiles.Count >= MaxRecentOpenSize) 
            {
				_recentFiles.RemoveAt(_recentFiles.Count - 1);
			}

            bool isPinned = false;
            if (lastItem != null)
            {
                isPinned = lastItem.Pinned;
            }
			
			if (_recentFiles.Count > 0) 
            {
                _recentFiles.Insert(0, new RecentOpenItem(isPinned, name));
			} 
            else 
            {
                _recentFiles.Add(new RecentOpenItem(isPinned, name));
			}
			
			OnRecentFileChange();
		}
		
		public void ClearRecentFiles()
		{
			_recentFiles.Clear();
			
			OnRecentFileChange();
		}
		
		public void ClearRecentProjects()
		{
			_recentProjects.Clear();
			
			OnRecentProjectChange();
		}
		
		public void AddLastProject(string name)
		{
            RecentOpenItem lastItem = null;
            for (int i = 0; i < _recentProjects.Count; ++i) 
            {
                RecentOpenItem recentItem = _recentProjects[i];
                if (String.Equals(recentItem.FullPath, name, 
                    StringComparison.OrdinalIgnoreCase)) 
                {
					_recentProjects.RemoveAt(i);

                    lastItem = recentItem;
                    break;
				}
			}

            while (_recentProjects.Count >= MaxRecentOpenSize) 
            {
                int removeAt = _recentProjects.Count - 1;
                RecentOpenItem removeItem = _recentProjects[removeAt];
                // Try looking to an unpinned recent project item...
                while (removeItem.Pinned && removeAt > 0)
                {
                    removeAt--;
                    if (removeAt > 0)
                    {
                        removeItem = _recentProjects[removeAt];
                    }
                }
                if (!removeItem.Pinned && removeAt > 0)
                {
                    _recentProjects.RemoveAt(removeAt);
                }
                else
                {
                    // Most likely, no unpinned is found, just remove the last...
                    _recentProjects.RemoveAt(_recentProjects.Count - 1);
                }
			}

            bool isPinned = false;
            if (lastItem != null)
            {
                isPinned = lastItem.Pinned;
            }
			
			if (_recentProjects.Count > 0) 
            {
                _recentProjects.Insert(0, new RecentOpenItem(isPinned, name));
			} 
            else 
            {
                _recentProjects.Add(new RecentOpenItem(isPinned, name));			
			}
			OnRecentProjectChange();
		}

        public void UpdateFilesPinnedState()
        {
            if (_recentFiles != null)
            {
                _recentFiles.UpdatePinnedState();
            }
        }

        public void UpdateProjectsPinnedState()
        {
            if (_recentProjects != null)
            {
                _recentProjects.UpdatePinnedState();
            }
        }

        public void RemoveFile(string fileName)
        {
            bool isSuccessful = false;

            for (int i = 0; i < _recentFiles.Count; ++i)
            {
                RecentOpenItem recentItem = _recentFiles[i];
                if (String.Equals(fileName, recentItem.FullPath,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _recentFiles.RemoveAt(i);

                    isSuccessful = true;
                    break;
                }
            }

            if (isSuccessful)
            {
                OnRecentFileChange();
            }
        }

        public void RenameFile(string sourceFile, string targetFile)
        {
            bool isSuccessful = false;

            for (int i = 0; i < _recentFiles.Count; ++i)
            {
                RecentOpenItem recentItem = _recentFiles[i];
                string file = recentItem.FullPath;
                if (String.Equals(sourceFile, file,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _recentFiles.RemoveAt(i);
                    _recentFiles.Insert(i, 
                        new RecentOpenItem(recentItem.Pinned, targetFile));

                    isSuccessful = true;
                    break;
                }
            }

            if (isSuccessful)
            {
                OnRecentFileChange();
            }
        }

        public void RemoveProject(string projectName)
        {
            bool isSuccessful = false;

            for (int i = 0; i < _recentProjects.Count; ++i)
            {
                RecentOpenItem recentItem = _recentProjects[i];
                if (String.Equals(projectName, recentItem.FullPath,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _recentProjects.RemoveAt(i);

                    isSuccessful = true;
                    break;
                }
            }

            if (isSuccessful)
            {
                OnRecentProjectChange();
            }
        }

        public void RenameProject(string sourceProject, string targetProject)
        {
            bool isSuccessful = false;

            for (int i = 0; i < _recentProjects.Count; ++i)
            {
                RecentOpenItem recentItem = _recentProjects[i];
                string file = recentItem.FullPath;
                if (String.Equals(sourceProject, file,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _recentProjects.RemoveAt(i);
                    _recentProjects.Insert(i, 
                        new RecentOpenItem(recentItem.Pinned, targetProject));

                    isSuccessful = true;
                    break;
                }
            }

            if (isSuccessful)
            {
                OnRecentProjectChange();
            }
        }

        public Properties ToProperties()
        {
            Properties p  = new Properties();
            p["Files"]    = String.Join(",", this.RecentFilePaths.ToArray());
            p["Projects"] = String.Join(",", this.RecentProjectPaths.ToArray());

            return p;
        }

        public static string CompactPath(string longPathName, int wantedLength)
        {
            StringBuilder sb = new StringBuilder(wantedLength + 1);
            if (PathCompactPathEx(sb, longPathName, wantedLength + 1, 0))
            {
                return sb.ToString();
            }

            return longPathName;
        }

        #endregion

        #region Private Methods

        private void OnRecentFileChange()
        {
            if (this.RecentFileChanged != null)
            {
                this.RecentFileChanged(this, null);
            }
        }

        private void OnRecentProjectChange()
        {
            if (this.RecentProjectChanged != null)
            {
                this.RecentProjectChanged(this, null);
            }
        }

        private RecentOpenList GetDisplayableProjects()
        {
            int displayable = _recentProjects.Displayable;
            if (displayable == 0)
            {
                return new RecentOpenList();
            }

            int itemCount   = _recentProjects.Count;  
            if (itemCount == 0 || itemCount <= displayable)
            {
                return _recentProjects;
            }
            int pinnedCount = _recentProjects.Pinned;
            if (pinnedCount == 0 || pinnedCount == itemCount)
            {
                RecentOpenList projectItems = new RecentOpenList();
                projectItems.Displayable = displayable;

                int listCount = Math.Min(displayable, itemCount);

                for (int i = 0; i < listCount; i++)
                {
                    projectItems.Add(_recentProjects[i]);
                }

                return projectItems;
            }
            else if (pinnedCount >= displayable)
            {
                RecentOpenList projectItems = new RecentOpenList();
                projectItems.Displayable = displayable;

                for (int i = 0; i < itemCount; i++)
                {
                    RecentOpenItem recentItem = _recentProjects[i];
                    if (recentItem.Pinned)
                    {
                        projectItems.Add(recentItem);
                    }

                    if (projectItems.Count >= displayable)
                    {
                        break;
                    }
                }

                return projectItems;
            }
            else if (pinnedCount < displayable)
            {
                RecentOpenList projectItems = new RecentOpenList();
                projectItems.Displayable = displayable;

                int unpinnedCount = displayable - pinnedCount;
                for (int i = 0; i < itemCount; i++)
                {
                    RecentOpenItem recentItem = _recentProjects[i];
                    if (recentItem.Pinned)
                    {
                        if (pinnedCount > 0)
                        {
                            projectItems.Add(recentItem);
                            pinnedCount--;
                        }
                    }
                    else
                    {
                        if (unpinnedCount > 0)
                        {
                            projectItems.Add(recentItem);
                            unpinnedCount--;
                        }
                    }
                                      
                    if (projectItems.Count >= displayable)
                    {
                        break;
                    }
                }

                return projectItems;
            }

            return _recentProjects;
        }

        #endregion

        #region IXmlSerializable Members

        public bool LoadXml(string fileName)
        {   
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return false;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;

            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                this.ReadXml(reader);
            }

            return true;
        }

        public bool SaveXml(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return false;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent   = true;
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                this.WriteXml(writer);
            }

            return true;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {   
                    if (String.Equals(reader.Name, RecentOpenList.XmlTagName))
                    {
                        string category = reader.GetAttribute("category");
                        if (!String.IsNullOrEmpty(category))
                        {
                            if (String.Equals(category, FileCategory))
                            {
                                if (_recentFiles == null || _recentFiles.Count != 0)
                                {
                                    _recentFiles = new RecentOpenList(FileCategory);
                                }

                                _recentFiles.ReadXml(reader);
                            }
                            else if (String.Equals(category, ProjectCategory))
                            {
                                if (_recentProjects == null || _recentProjects.Count != 0)
                                {
                                    _recentProjects = new RecentOpenList(ProjectCategory);
                                }

                                _recentProjects.ReadXml(reader);
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, XmlTagName))
                    {
                        break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement(XmlTagName);

            if (_recentFiles != null)
            {
                _recentFiles.WriteXml(writer);
            }
            if (_recentProjects != null)
            {
                _recentProjects.WriteXml(writer);
            }

            writer.WriteEndElement();  
        }

        #endregion
    }
}
