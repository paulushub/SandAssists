using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConceptualGroup : BuildGroup
    {
        #region Private Fields

        private Guid _docID;
        private Guid _projectID;
        private Guid _repositoryID;

        private string _projectName;
        private string _projectTitle;

        private string _docWriter;
        private string _docEditor;
        private string _docManager;

        private string _defaultTopic;

        private string _contentsDir;
        private string _contentsFile;

        private Version _fileVersion;

        // main files creates
        private string _buildManifestFile;

        private List<ConceptualItem> _listItems;

        private ConceptualMetadataContent  _metadata;
        private ConceptualCategoryContent  _categories;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class to the
        /// default parameters.
        /// </summary>
        public ConceptualGroup()
        {
            _fileVersion   = new Version(1, 0, 0, 0);
            _projectTitle  = "ProjectTitle";
            _projectName   = "ProjectName";
            _docID         = Guid.NewGuid();
            _projectID     = Guid.NewGuid();
            _repositoryID  = Guid.NewGuid();

            _metadata      = new ConceptualMetadataContent();
            _categories    = new ConceptualCategoryContent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class with
        /// parameters copied from the specified argument, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualGroup"/> class specifying the initial 
        /// properties and states for this newly created instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualGroup(ConceptualGroup source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this group is empty.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the group is empty; otherwise, 
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsEmpty
        {
            get
            {
                if (_listItems == null || _listItems.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value specifying the type of this group.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildGroupType"/> specifying the
        /// type of this group. This property will always return
        /// <see cref="BuildGroupType.Conceptual"/>
        /// </value>
        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Conceptual;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid ProjectID
        {
            get
            {
                return _projectID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid RepositoryID
        {
            get
            {
                return _repositoryID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid DocumentID
        {
            get
            {
                return _docID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<ConceptualItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualCategoryContent Categories
        {
            get
            {
                return _categories;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualMetadataContent Metadata
        {
            get
            {
                return _metadata;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Version FileVersion
        {
            get
            {
                return _fileVersion;
            }
            set
            {
                if (value == null)
                {
                    _fileVersion = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DefaultTopic
        {
            get
            {
                return _defaultTopic;
            }
            set
            {
                _defaultTopic = value;
            }
        }

        #endregion

        #region Internal Properties

        internal string ManifestFile
        {
            get
            {
                return _buildManifestFile;
            }
        }

        #endregion

        #region Public Methods

        #region AddItems Method

        public bool AddItems(string contentsDir, string contentsFile)
        {
            _contentsDir  = null;
            _contentsFile = null;
            _listItems    = null;

            if (String.IsNullOrEmpty(contentsDir))
            {
                return false;
            }
            contentsDir = Path.GetFullPath(contentsDir);
            if (!Directory.Exists(contentsDir))
            {
                return false;
            }
            if (String.IsNullOrEmpty(contentsFile))
            {
                return false;
            }
            contentsFile = Path.GetFullPath(contentsFile);
            if (!File.Exists(contentsFile))
            {
                return false;
            }

            _contentsDir  = contentsDir;
            _contentsFile = contentsFile;

            if (_listItems == null)
            {
                _listItems = new List<ConceptualItem>();
            }

            ConceptualContent contents = new ConceptualContent(contentsFile, contentsDir);
            
            contents.Read(this);

            return true;
        }

        #endregion

        #region Initialize Method

        public override bool Initialize(BuildContext context)
        {
            base.Initialize(context);
            BuildSettings settings = context.Settings;
            string workingDir = this.WorkingDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = context.WorkingDirectory;
                this.WorkingDirectory = workingDir;
            }

            if (String.IsNullOrEmpty(_contentsDir) ||
                !Directory.Exists(_contentsDir))
            {
                return false;
            }
            if (String.IsNullOrEmpty(_contentsDir) ||
                !Directory.Exists(_contentsDir))
            {
                return false;
            }
            if (String.IsNullOrEmpty(_contentsFile) ||
                !File.Exists(_contentsFile))
            {
                return false;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return false;
            }

            string dduexmlDir = Path.Combine(workingDir, "DdueXml");
            string compDir    = Path.Combine(workingDir, "XmlComp");

            if (!Directory.Exists(dduexmlDir))
            {
                Directory.CreateDirectory(dduexmlDir);
            }
            if (!Directory.Exists(compDir))
            {
                Directory.CreateDirectory(compDir);
            }

            int itemCount = _listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualItem projItem = _listItems[i];
                if (projItem == null)
                {
                    continue;
                }

                projItem.CreateFiles(dduexmlDir, compDir);
            }

            // 1. Write the table of contents
            ConceptualTableOfContents tableOfContents = new ConceptualTableOfContents();
            tableOfContents.Write(this, settings);

            // 2. Write the project settings
            ConceptualSettingsLoc projectSettings = new ConceptualSettingsLoc();
            projectSettings.Write(this, settings);

            // 3. Write the project content metadata
            if (_metadata == null)
            {
                _metadata = new ConceptualMetadataContent();
            }
            _metadata.Write(this, settings);

            // 4. Write the project build manifest
            ConceptualBuildManifest buildManifest = new ConceptualBuildManifest();
            buildManifest.Write(this, settings);

            _buildManifestFile = buildManifest.FilePath;

            return true;
        }

        #endregion

        #region Uninitialize Method

        public override void Uninitialize()
        {
            _contentsDir  = null;
            _contentsFile = null;
            _listItems    = null;

            _buildManifestFile = null;

            base.Uninitialize();
        }

        #endregion

        #region Category Methods

        public void AddCategory(string name, string description)
        {
            this.AddCategory(new ConceptualCategoryItem(name, description));
        }

        public void AddCategory(ConceptualCategoryItem category)
        {
            BuildExceptions.NotNull(category, "category");

            if (_categories == null)
            {
                _categories = new ConceptualCategoryContent();
            }

            _categories.Add(category);
        }

        public void RemoveCategory(int index)
        {
            if (_categories != null)
            {
                _categories.Remove(index);
            }
        }

        public void RemoveCategory(ConceptualCategoryItem category)
        {
            if (_categories != null && category != null)
            {
                _categories.Remove(category);
            }
        }

        public void ClearCategories()
        {
            if (_categories != null)
            {
                _categories.Clear();
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildGroup Clone()
        {
            ConceptualGroup group = new ConceptualGroup(this);

            return group;
        }

        #endregion
    }
}
