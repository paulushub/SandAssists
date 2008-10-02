using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
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
        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

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
            _properties    = new Dictionary<string, string>();
        }

        public ConceptualGroup(ConceptualGroup source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Conceptual;
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

        public Guid ProjectID
        {
            get
            {
                return _projectID;
            }
        }

        public Guid RepositoryID
        {
            get
            {
                return _repositoryID;
            }
        }

        public Guid DocID
        {
            get
            {
                return _docID;
            }
        }

        public string DocWriter
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

        public string DocEditor
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

        public string DocManager
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

        public IList<ConceptualItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        public ConceptualCategoryContent Categories
        {
            get
            {
                return _categories;
            }
        }

        public ConceptualMetadataContent Metadata
        {
            get
            {
                return _metadata;
            }
        }

        public IDictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }
        }

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

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="ConceptualGroup"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="ConceptualGroup"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="ConceptualGroup"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="ConceptualGroup"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _properties.Keys;

                    return keyColl;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="ConceptualGroup"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="ConceptualGroup"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _properties.Values;

                    return valueColl;
                }

                return null;
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

        public override bool Initialize(BuildSettings settings)
        {
            base.Initialize(settings);

            string workingDir = this.WorkingDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = settings.WorkingDirectory;
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
            tableOfContents.Write(this);

            // 2. Write the project settings
            ConceptualSettingsLoc projectSettings = new ConceptualSettingsLoc();
            projectSettings.Write(this);

            // 3. Write the project content metadata
            if (_metadata == null)
            {
                _metadata = new ConceptualMetadataContent();
            }
            _metadata.Write(this);

            // 4. Write the project build manifest
            ConceptualBuildManifest buildManifest = new ConceptualBuildManifest();
            buildManifest.Write(this);

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

        #region Property Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="ConceptualGroup"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public void RemoveProperty(string key)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="ConceptualGroup"/>.
        /// </summary>
        public void ClearProperties()
        {
            if (_properties.Count == 0)
            {
                return;
            }

            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="ConceptualGroup"/>.
        /// </summary>
        /// <param name="key">The string key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// <para>-or-</para>
        /// An element with the same key already exists in the <see cref="ConceptualGroup"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="ConceptualGroup.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="ConceptualGroup"/>. However, if the specified key already 
        /// exists in the <see cref="ConceptualGroup"/>, setting the 
        /// <see cref="ConceptualGroup.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="ConceptualGroup.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="ConceptualGroup"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="ConceptualGroup"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="ConceptualGroup"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="ConceptualGroup"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="ConceptualGroup"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="ConceptualGroup"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
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
