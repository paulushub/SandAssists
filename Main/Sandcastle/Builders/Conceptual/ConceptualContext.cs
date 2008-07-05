using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Items;

namespace Sandcastle.Builders.Conceptual
{
    [Serializable]
    public sealed class ConceptualContext : BuildContext
    {
        #region Private Fields

        private bool   _runHelp;
        private bool   _itemsLoads;
        private bool   _cleanUp;
        private bool   _siteMap;
        private bool   _docIncludeTopic;

        private Guid   _docID;
        private Guid   _projectID;
        private Guid   _repositoryID;

        private string _workingDir;
        private string _documentsDir;
        private string _projectFile;
        private string _projectDir;
        private string _projectName;
        private string _projectTitle;

        private string _siteMapDir;

        private string _docWriter;
        private string _docEditor;
        private string _docManager;

        private ConceptualItemList   _listItems;
        private ConceptualFilterList _listFilters;

        #endregion

        #region Constructors and Destructor

        public ConceptualContext()
        {   
            _projectTitle    = "ProjectTitle";
            _projectName     = "ProjectName";
            _docID           = Guid.NewGuid();
            _projectID       = Guid.NewGuid();
            _repositoryID    = Guid.NewGuid();

            _docWriter       = String.Empty;
            _docEditor       = String.Empty;
            _docManager      = String.Empty;

            _cleanUp         = true;
            _docIncludeTopic = false;

            _listItems       = new ConceptualItemList();
            _listFilters     = new ConceptualFilterList();
        }

        public ConceptualContext(ConceptualContext source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public bool ItemsLoaded
        {
            get
            {
                return _itemsLoads;
            }
        }

        public IList<ConceptualItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        public IList<ConceptualFilter> Filters
        {
            get
            {
                return _listFilters;
            }
        }

        public Guid ProjectID
        {
            get
            {
                return _projectID;
            }

            set
            {
                _projectID = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectName = value;
                }
            }
        }

        public string ProjectTitle
        {
            get
            {
                return _projectTitle;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectTitle = value;
                }
            }
        }

        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectFile = value;
                }
            }
        }

        public string ProjectDirectory
        {
            get
            {
                return _projectDir;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectDir = value;
                }
            }
        }

        public Guid RepositoryID
        {
            get
            {
                return _repositoryID;
            }

            set
            {
                _repositoryID = value;
            }
        }

        public Guid DocumentID
        {
            get
            {
                return _docID;
            }

            set
            {
                _docID = value;
            }
        }

        public string DocumentWriter
        {
            get
            {
                return _docWriter;
            }

            set
            {
                _docWriter = value;
            }
        }

        public string DocumentEditor
        {
            get
            {
                return _docEditor;
            }

            set
            {
                _docEditor = value;
            }
        }

        public string DocumentManager
        {
            get
            {
                return _docManager;
            }

            set
            {
                _docManager = value;
            }
        }

        public string DocumentDirectory
        {
            get
            {
                return _documentsDir;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _documentsDir = value;
                }
            }
        }

        public bool DocumentIncludesTopic
        {
            get
            {
                return _docIncludeTopic;
            }

            set
            {
                _docIncludeTopic = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _workingDir;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _workingDir = value;
                }
            }
        }

        public bool RunHelp
        {
            get
            {
                return _runHelp;
            }

            set
            {
                _runHelp = value;
            }
        }

        public bool CleanIntermediates
        {
            get
            {
                return _cleanUp;
            }

            set
            {
                _cleanUp = value;
            }
        }

        public bool SiteMap
        {
            get
            {
                return _siteMap;
            }

            set
            {
                _siteMap = value;
            }
        }

        public string SiteMapDirectory
        {
            get
            {
                return _siteMapDir;
            }

            set
            {
                _siteMapDir = value;
            }
        }

        #endregion

        #region Public Methods

        public bool LoadItems(HelpLogger logger)
        {
            if (_itemsLoads)
            {
                return _itemsLoads;
            }

            _itemsLoads = false;

            if (String.IsNullOrEmpty(_documentsDir))
            {
                return _itemsLoads;
            }
            _documentsDir = Path.GetFullPath(_documentsDir);
            if (!Directory.Exists(_documentsDir))
            {
                return _itemsLoads;
            }
            if (String.IsNullOrEmpty(_projectFile))
            {
                return _itemsLoads;
            }
            _projectFile = Path.GetFullPath(_projectFile);
            if (!File.Exists(_projectFile))
            {
                return _itemsLoads;
            }

            if (String.IsNullOrEmpty(_projectDir))
            {
                _projectDir = Path.GetDirectoryName(_projectFile);
            }

            try
            {
                ConceptualItemReader reader = new ConceptualItemReader(_listFilters,
                    _projectFile, _documentsDir);
                _listItems  = reader.Read();

                reader.Dispose();
                reader = null;

                _itemsLoads = (_listItems != null && _listItems.Count > 0);
            }
            catch (Exception ex)
            {
                _itemsLoads = false;

                if (logger != null)
                {
                    logger.WriteLine(ex, HelpLoggerLevel.Error);
                }
            }

            return _itemsLoads;
        }

        public void ClearItems()
        {
            _listItems  = null;
            _itemsLoads = false;
        }

        #endregion

        #region ICloneable Members

        public override BuildContext Clone()
        {
            ConceptualContext context = new ConceptualContext(this);

            return context;
        }

        #endregion

        #region Private Classes

        private sealed class ConceptualFilterList : ItemList<ConceptualFilter>
        {
            public ConceptualFilterList()
            {   
            }
        }

        #endregion
    }
}
