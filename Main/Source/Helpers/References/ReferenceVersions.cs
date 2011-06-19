using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    public sealed class ReferenceVersions : BuildObject, IBuildNamedItem
    {
        #region Private Fields

        private string _versionsId;
        private string _versionsTitle;
        private string _versionsDir;

        private BuildList<string> _workingDirs;

        private BuildKeyedList<ReferenceVersionSource> _listSources;

        #endregion

        #region Constructors and Destrutor

        public ReferenceVersions()
        {
            _versionsId  = Guid.NewGuid().ToString();
            _workingDirs = new BuildList<string>();
            _listSources = new BuildKeyedList<ReferenceVersionSource>();
        }

        public ReferenceVersions(string versionsId, string versionsTitle)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(versionsId, "versionsId");
            BuildExceptions.NotNullNotEmpty(versionsTitle, "versionsTitle");

            _versionsId    = versionsId;
            _versionsTitle = versionsTitle;
        }

        #endregion

        #region Public Properties

        public string VersionsId
        {
            get
            {
                return _versionsId;
            }
            //set
            //{
            //    _versionsId = value;
            //}
        }

        public string VersionsTitle
        {
            get
            {
                return _versionsTitle;
            }
            set
            {
                _versionsTitle = value;
            }
        }

        public string VersionsDir
        {
            get
            {
                return _versionsDir;
            }
            set
            {
                _versionsDir = value;
            }
        }

        public IList<string> WorkingDirs
        {
            get
            {
                return _workingDirs;
            }
        }

        public int Count
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources.Count;
                }

                return 0;
            }
        }

        public ReferenceVersionSource this[int index]
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources[index];
                }

                return null;
            }
        }

        public ReferenceVersionSource this[string sourceId]
        {
            get
            {
                if (String.IsNullOrEmpty(sourceId))
                {
                    return null;
                }

                if (_listSources != null)
                {
                    return _listSources[sourceId];
                }

                return null;
            }
        }

        public IBuildNamedList<ReferenceVersionSource> Sources
        {
            get
            {
                return _listSources;
            }
        }

        #endregion

        #region Public Methods

        public void Add(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }

            _listSources.Add(source);
        }

        public void Add(IList<ReferenceVersionSource> sources)
        {
            BuildExceptions.NotNull(sources, "sources");

            int sourceCount = sources.Count;
            if (sourceCount == 0)
            {
                return;
            }

            for (int i = 0; i < sourceCount; i++)
            {
                this.Add(sources[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.RemoveAt(index);
        }

        public void Remove(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Remove(source);
        }

        public bool Contains(ReferenceVersionSource source)
        {
            if (source == null || _listSources == null || _listSources.Count == 0)
            {
                return false;
            }

            return _listSources.Contains(source);
        }

        public void Clear()
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Clear();
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _versionsId; 
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return null;
        }

        #endregion
    }
}
