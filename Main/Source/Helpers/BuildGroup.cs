using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class BuildGroup : BuildObject<BuildGroup>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "documentGroup";

        #endregion

        #region Private Fields

        private bool   _isLoaded;  
        private bool   _isExcluded;
        private bool   _isTocExcluded;
        private bool   _isInitialized;  
        private bool   _syntaxUsage;

        private Version _version;

        private string _groupId;
        private string _groupName;
        private string _groupDescription;
        private string _runningTitle;

        private BuildSyntaxType _syntaxType;
        private BuildProperties _properties;

        private BuildFilePath      _contentFile;
        private BuildDirectoryPath _groupDir;

        private BuildList<TokenContent> _listTokens;
        private BuildList<MediaContent> _listMedia;
        private BuildList<SharedContent> _listShared;
        private BuildList<ResourceContent> _listResources;
        private BuildList<CodeSnippetContent> _listSnippets;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildGroup"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroup"/> class to the
        /// default parameters.
        /// </summary>
        protected BuildGroup()
            : this(Guid.NewGuid().ToString())
        {
        }

        protected BuildGroup(string groupName)
            : this(groupName, Guid.NewGuid().ToString())
        {
        }

        protected BuildGroup(string groupName, string groupId)
        {
            BuildExceptions.NotNullNotEmpty(groupName, "groupName");
            BuildExceptions.NotNullNotEmpty(groupId,   "groupId");

            _version       = new Version(1, 0, 0, 0);
            _groupName     = groupName;
            _groupId       = groupId;
            _listMedia     = new BuildList<MediaContent>();
            _listTokens    = new BuildList<TokenContent>();
            _listShared    = new BuildList<SharedContent>();
            _listSnippets  = new BuildList<CodeSnippetContent>();
            _listResources = new BuildList<ResourceContent>();

            _properties    = new BuildProperties();

            _syntaxType    = BuildSyntaxType.Standard;
        }

        protected BuildGroup(string groupName, string groupId, string contentFile)
            : this(groupName, groupId, contentFile, String.Empty)
        {
        }

        protected BuildGroup(string groupName, string groupId,
            string contentFile, string contentDir)
            : this(groupName, groupId)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            if (String.IsNullOrEmpty(contentDir))
            {
                contentDir = Path.GetDirectoryName(contentFile);
            }

            _contentFile = new BuildFilePath(contentFile);
            _groupDir  = new BuildDirectoryPath(contentDir);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroup"/> class with
        /// parameters copied from the specified argument, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildGroup"/> class specifying the initial 
        /// properties and states for this newly created instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildGroup(BuildGroup source)
            : base(source)
        {
            _isLoaded      = source._isLoaded;
            _isExcluded    = source._isExcluded;
            _isTocExcluded = source._isTocExcluded;
            _isInitialized = source._isInitialized;

            _syntaxUsage   = source._syntaxUsage;
            _syntaxType    = source._syntaxType;

            _contentFile = source._contentFile;
            _groupDir = source._groupDir;

            _groupId       = source._groupId;
            _groupName     = source._groupName;
            _groupDescription = source._groupDescription;
            _runningTitle  = source._runningTitle;
            _properties    = source._properties;

            _listMedia     = source._listMedia;
            _listTokens    = source._listTokens;
            _listShared    = source._listShared;
            _listSnippets  = source._listSnippets;
            _listResources = source._listResources;
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
        /// <remarks>
        /// An empty group will normally not contain any contents, but the state and
        /// control of this property will depend on the implementation of the various
        /// groups.
        /// </remarks>
        public abstract bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying the type of this group.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildGroupType"/> specifying the
        /// type of this group.
        /// </value>
        public abstract BuildGroupType GroupType
        {
            get;
        }

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        /// <summary>
        /// Gets the unique identifier of this help group.
        /// </summary>
        /// <value>
        /// A <see cref="System.Guid"/> specifying the unique identifier of this group.
        /// </value>
        public string Id
        {
            get
            {
                return _groupId;
            }
        }

        /// <summary>
        /// Gets or sets the name of this group.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of this group.
        /// </value>
        public string Name
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _groupName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the title of this group.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the title of this group. The 
        /// default is <see langword="null"/> or empty string.
        /// </value>
        public string Description
        {
            get
            {
                return _groupDescription;
            }
            set
            {
                _groupDescription = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string RunningHeaderText
        {
            get
            {
                return _runningTitle;
            }
            set
            {
                _runningTitle = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Exclude
        {
            get
            {
                return _isExcluded;
            }
            set
            {
                _isExcluded = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool ExcludeToc
        {
            get
            {
                return _isTocExcluded;
            }
            set
            {
                _isTocExcluded = value;
            }
        }

        public BuildSyntaxType SyntaxType
        {
            get
            {
                return _syntaxType;
            }
            set
            {
                _syntaxType = value;
            }
        }

        public bool SyntaxUsage
        {
            get
            {
                return _syntaxUsage;
            }
            set
            {
                _syntaxUsage = value;
            }
        }

        public BuildFilePath ContentFile
        {
            get
            {
                return _contentFile;
            }
            set
            {
                if (value != null)
                {
                    _contentFile = value;

                    if (_groupDir == null)
                    {
                        _groupDir = new BuildDirectoryPath(
                            Path.GetDirectoryName(value.Path));
                    }
                }
            }
        }

        public BuildDirectoryPath ContentDir
        {
            get
            {
                return _groupDir;
            }
            set
            {
                if (value != null)
                {
                    _groupDir = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<SharedContent> SharedContents
        {
            get
            {
                return _listShared;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<TokenContent> TokenContents
        {
            get
            {
                return _listTokens;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<MediaContent> MediaContents
        {
            get
            {
                return _listMedia;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<CodeSnippetContent> SnippetContents
        {
            get
            {
                return _listSnippets;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<ResourceContent> ResourceContents
        {
            get
            {
                return _listResources;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
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
                return _properties[key];
            }
            set
            {
                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BuildGroup"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="BuildGroup"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BuildGroup"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="BuildGroup"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                return _properties.Keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="BuildGroup"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildGroup"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                return _properties.Values;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IDictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildGroup"/> class instance, 
        /// this property is <see cref="BuildGroup.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Private Properties

        private bool isEmbedded
        {
            get
            {
                BuildFilePath filePath = this.ContentFile;  
                return (filePath != null && filePath.IsValid);
            }
        }

        #endregion

        #region Public Methods

        #region Load Method

        public virtual void Load()
        {
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_groupDir == null)
            {
                _groupDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _groupId);

            this.OnLoad(resolver);
        }

        public virtual void Load(BuildFilePath contentFile)
        {
            BuildExceptions.NotNull(contentFile, "contentFile");
            if (!contentFile.Exists)
            {
                throw new BuildException(
                    "The specified group file does not exist.");
            }

            this.ContentFile = contentFile;

            this.Load();
        }

        public virtual void Reload()
        {
            _isLoaded = false;

            this.Load();
        }

        #endregion

        #region Save Method

        public virtual void Save()
        {
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_groupDir == null)
            {
                _groupDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _groupId);

            this.OnSave(resolver);
        }

        public virtual void Save(BuildFilePath contentFile)
        {
            BuildExceptions.NotNull(contentFile, "contentFile");

            this.ContentFile = contentFile;

            this.Save();
        }

        #endregion

        #region Initialization Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _isInitialized = false;
        }

        public virtual void BeginSources(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");
        }

        public virtual void EndSources()
        {
        }

        #endregion

        #region Resources Methods

        public virtual void AddResourceItem(string source, string destination)
        {
            if (String.IsNullOrEmpty(source) ||
                String.IsNullOrEmpty(destination))
            {
                return;
            }

            if (_listResources == null)
            {
                _listResources = new BuildList<ResourceContent>();
            }
            ResourceContent defaultContent = null;
            if (_listResources.Count == 0)
            {
                defaultContent = new ResourceContent();
                _listResources.Add(defaultContent);
            }
            else
            {
                defaultContent = _listResources[0];
            }

            defaultContent.Add(new ResourceItem(source, destination));
        }

        public virtual void AddResource(ResourceContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listResources == null)
            {
                _listResources = new BuildList<ResourceContent>();
            }

            _listResources.Add(content);
        }

        #endregion

        #region Media Methods

        public virtual void AddMedia(MediaContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listMedia == null)
            {
                _listMedia = new BuildList<MediaContent>();
            }

            _listMedia.Add(content);
        }

        #endregion

        #region Tokens Methods

        public virtual void AddToken(TokenContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listTokens == null)
            {
                _listTokens = new BuildList<TokenContent>();
            }

            _listTokens.Add(content);
        }

        #endregion

        #region Shared Methods

        public virtual void AddShared(SharedContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listShared == null)
            {
                _listShared = new BuildList<SharedContent>();
            }

            _listShared.Add(content);
        }

        public virtual IList<SharedItem> PrepareShared(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            List<SharedItem> listShared = new List<SharedItem>();

            if (!String.IsNullOrEmpty(_runningTitle))
            {
                listShared.Add(new SharedItem("runningHeaderText",
                    _runningTitle));
            }

            return listShared;
        }

        public virtual IList<RuleItem> PrepareSharedRule(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            return null;
        }

        #endregion

        #region Snippets Methods

        public virtual void AddSnippet(CodeSnippetContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listSnippets == null)
            {
                _listSnippets = new BuildList<CodeSnippetContent>();
            }

            _listSnippets.Add(content);
        }

        #endregion

        #region Property Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="BuildGroup"/>.
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
            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildGroup"/>.
        /// </summary>
        public void ClearProperties()
        {
            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="BuildGroup"/>.
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
        /// An element with the same key already exists in the <see cref="BuildGroup"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="BuildGroup.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="BuildGroup"/>. However, if the specified key already 
        /// exists in the <see cref="BuildGroup"/>, setting the 
        /// <see cref="BuildGroup.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="BuildGroup.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildGroup"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="BuildGroup"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildGroup"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildGroup"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="BuildGroup"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildGroup"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion

        #endregion

        #region Protected Methods

        protected abstract void OnReadPropertyGroupXml(XmlReader reader);

        protected abstract void OnWritePropertyGroupXml(XmlWriter writer);

        protected abstract void OnReadContentXml(XmlReader reader);

        protected abstract void OnWriteContentXml(XmlWriter writer);

        protected abstract void OnReadXml(XmlReader reader);

        protected abstract void OnWriteXml(XmlWriter writer);

        protected virtual void OnLoad(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_contentFile) ||
                File.Exists(_contentFile) == false)
            {
                return;
            }

            if (_groupDir == null)
            {
                _groupDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_contentFile, settings);

                reader.MoveToContent();

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    this.ReadXml(reader);

                    BuildPathResolver.Pop(resolverId);
                }   

                _isLoaded = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        protected virtual void OnSave(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            // If this is not yet located, and the contents is empty, we
            // will simply not continue from here...
            if (_contentFile != null && _contentFile.Exists)
            {
                if (!this._isLoaded && this.IsEmpty)
                {
                    return;
                }
            }

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(_contentFile, settings);

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    writer.WriteStartDocument();

                    this.WriteXml(writer);

                    writer.WriteEndDocument();

                    BuildPathResolver.Pop(resolverId);
                }

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded = true;
            }
            finally
            {  
            	if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #region Private Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "id":
                                _groupId = reader.ReadString();
                                break;
                            case "name":
                                _groupName = reader.ReadString();
                                break;
                            case "exclude":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _isExcluded = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "excludetoc":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _isTocExcluded = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "syntaxusage":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _syntaxUsage = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "description":
                                _groupDescription = reader.ReadString();
                                break;
                            case "runningheadertext":
                                _runningTitle = reader.ReadString();
                                break;
                            case "syntaxtype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _syntaxType = (BuildSyntaxType)Enum.Parse(
                                        typeof(BuildSyntaxType), tempText, true);
                                }
                                break;
                           default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlContents Method

        private void ReadXmlContents(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startElement = reader.Name;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("type");
                        if (String.Equals(tempText, "Resource",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (!reader.IsEmptyElement && 
                                reader.ReadToDescendant(ResourceContent.TagName))
                            {
                                ResourceContent resource = new ResourceContent();
                                resource.ReadXml(reader);

                                _listResources.Add(resource);
                            }
                        }
                        else if (String.Equals(tempText, "Tokens", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            TokenContent content = new TokenContent();

                            if (reader.IsEmptyElement)
                            {
                                string sourceFile = reader.GetAttribute("source");
                                if (!String.IsNullOrEmpty(sourceFile))
                                {
                                    content.ContentFile = new BuildFilePath(sourceFile);
                                    content.Load();
                                }
                            }
                            else
                            {
                                if (reader.ReadToDescendant(TokenContent.TagName))
                                {
                                    content.ReadXml(reader);
                                }
                            }

                            _listTokens.Add(content);
                        }
                        else if (String.Equals(tempText, "Media",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            MediaContent content = new MediaContent();

                            if (reader.IsEmptyElement)
                            {
                                string sourceFile = reader.GetAttribute("source");
                                if (!String.IsNullOrEmpty(sourceFile))
                                {
                                    content.ContentFile = new BuildFilePath(sourceFile);
                                    content.Load();
                                }
                            }
                            else
                            {
                                if (reader.ReadToDescendant(MediaContent.TagName))
                                {
                                    content.ReadXml(reader);
                                }
                            }

                            _listMedia.Add(content);
                        }
                        else if (String.Equals(tempText, "Shared",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            SharedContent content = new SharedContent();

                            if (reader.IsEmptyElement)
                            {
                                string sourceFile = reader.GetAttribute("source");
                                if (!String.IsNullOrEmpty(sourceFile))
                                {
                                    content.ContentFile = new BuildFilePath(sourceFile);
                                    content.Load();
                                }
                            }
                            else
                            {
                                if (reader.ReadToDescendant(SharedContent.TagName))
                                {
                                    content.ReadXml(reader);
                                }
                            }

                            _listShared.Add(content);
                        }
                        else if (String.Equals(tempText, "CodeSnippets",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            CodeSnippetContent content = new CodeSnippetContent();

                            if (reader.IsEmptyElement)
                            {
                                string sourceFile = reader.GetAttribute("source");
                                if (!String.IsNullOrEmpty(sourceFile))
                                {
                                    content.ContentFile = new BuildFilePath(sourceFile);
                                    content.Load();
                                }
                            }
                            else
                            {
                                if (reader.ReadToDescendant(CodeSnippetContent.TagName))
                                {
                                    content.ReadXml(reader);
                                }
                            }

                            _listSnippets.Add(content);
                        }
                        else
                        {
                            this.OnReadContentXml(reader);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, "The processing of the group ReadXml failed.");
                return;
            }
            string tempText = reader.GetAttribute("type");
            if (String.IsNullOrEmpty(tempText))
            {
                throw new BuildException(
                    "ReadXml: The group type is not specified and it is unknown.");
            }

            BuildGroupType groupType = (BuildGroupType)Enum.Parse(
                typeof(BuildGroupType), tempText, true);
            if (groupType != this.GroupType)
            {
                throw new BuildException(String.Format(
                    "ReadXml: The group type '{0}' does not match the current type '{1}'.",
                    groupType, this.GroupType));
            }

            // Read the group identifier...
            tempText = reader.GetAttribute("id");
            if (!String.IsNullOrEmpty(tempText))
            {
                _groupId = tempText;
            }

            // Read the version information of the file group...
            tempText = reader.GetAttribute("version");
            if (!String.IsNullOrEmpty(tempText))
            {
                _version = new Version(tempText);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listTokens == null)
            {
                _listTokens = new BuildList<TokenContent>();
            }
            if (_listMedia == null)
            {
                _listMedia = new BuildList<MediaContent>();
            }
            if (_listShared == null)
            {
                _listShared = new BuildList<SharedContent>();
            }
            if (_listResources == null)
            {
                _listResources = new BuildList<ResourceContent>();
            }
            if (_listSnippets == null)
            {
                _listSnippets = new BuildList<CodeSnippetContent>();
            }                                                       

            // Sample format:
            //
            //<documentGroup type="Reference" id="82840faa-0481-4399-91a5-33a011bb513e" version="1.0">
            //    <location />
            //    <propertyGroup name="General">
            //        <property name="Exclude">False</property>
            //        <property name="ExcludeToc">True</property>
            //    </propertyGroup>
            //    <propertyGroup name="Reference">
            //        <property name="XmlnsForXaml">True</property>
            //    </propertyGroup>
            //    <propertyBag />
            //    <contents>
            //        <content type="Reference" source=".\Embedding.Reference.xml" />
            //    </contents>
            //</documentGroup>

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!reader.IsEmptyElement)
                        {
                            _groupDir = BuildDirectoryPath.ReadLocation(reader);
                        }
                    }
                    else if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "general":
                                this.ReadXmlGeneral(reader);
                                break;
                            default:
                                this.OnReadPropertyGroupXml(reader);
                                break;
                        }
                    }
                    else if (String.Equals(reader.Name, BuildProperties.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (_properties == null)
                        {
                            _properties = new BuildProperties();
                        }
                        _properties.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "contents",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlContents(reader);
                    }
                    else
                    {
                        this.OnReadXml(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            BuildPathResolver resolver = BuildPathResolver.Resolver;
            Debug.Assert(resolver != null && 
                this.isEmbedded ? resolver.Id == _groupId : true);
            if (resolver == null)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("type",    this.GroupType.ToString());
            writer.WriteAttributeString("version", _version.ToString(2));

            writer.WriteStartElement("location"); // location
            if (_groupDir != null &&
                !_groupDir.IsDirectoryOf(_contentFile))
            {
                _groupDir.WriteXml(writer);
            }
            writer.WriteEndElement();             // location

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");

            writer.WritePropertyElement("Id",                _groupId);
            writer.WritePropertyElement("Name",              _groupName);
            writer.WritePropertyElement("Exclude",           _isExcluded);
            writer.WritePropertyElement("ExcludeToc",        _isTocExcluded);
            writer.WritePropertyElement("SyntaxUsage",       _syntaxUsage);
            writer.WritePropertyElement("Description",       _groupDescription);
            writer.WritePropertyElement("RunningHeaderText", _runningTitle);
            writer.WritePropertyElement("SyntaxType",        _syntaxType.ToString());
            
            writer.WriteEndElement();                   // end - propertyGroup

            // Write the group specific property groups...
            this.OnWritePropertyGroupXml(writer);

            if (_properties != null)
            {
                _properties.WriteXml(writer);
            }

            writer.WriteStartElement("contents");  // start - contents
            if (_listTokens != null)
            {
                for (int i = 0; i < _listTokens.Count; i++)
                {
                    TokenContent content = _listTokens[i];

                    BuildFilePath filePath = content.ContentFile;
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", "Tokens");
                    if (filePath != null && filePath.IsValid)
                    {
                        writer.WriteAttributeString("source",
                            resolver.ResolveRelative(filePath));
                        content.Save();
                    }
                    else
                    {
                        content.WriteXml(writer);
                    }
                    writer.WriteEndElement();
                }
            }
            if (_listMedia != null)
            {
                for (int i = 0; i < _listMedia.Count; i++)
                {
                    MediaContent content = _listMedia[i];

                    BuildFilePath filePath = content.ContentFile;
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", "Media");
                    if (filePath != null && filePath.IsValid)
                    {
                        writer.WriteAttributeString("source",
                            resolver.ResolveRelative(filePath));
                        content.Save();
                    }
                    else
                    {
                        content.WriteXml(writer);
                    }
                    writer.WriteEndElement();
                }
            }
            if (_listShared != null)
            {
                for (int i = 0; i < _listShared.Count; i++)
                {
                    SharedContent content = _listShared[i];

                    BuildFilePath filePath = content.ContentFile;
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", "Shared");
                    if (filePath != null && filePath.IsValid)
                    {
                        writer.WriteAttributeString("source",
                            resolver.ResolveRelative(filePath));
                        content.Save();
                    }
                    else
                    {
                        content.WriteXml(writer);
                    }
                    writer.WriteEndElement();
                }
            }
            if (_listSnippets != null)
            {
                for (int i = 0; i < _listSnippets.Count; i++)
                {
                    CodeSnippetContent content = _listSnippets[i];

                    BuildFilePath filePath = content.ContentFile;
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", "CodeSnippets");
                    if (filePath != null && filePath.IsValid)
                    {
                        writer.WriteAttributeString("source",
                            resolver.ResolveRelative(filePath));
                        content.Save();
                    }
                    else
                    {
                        content.WriteXml(writer);
                    }
                    writer.WriteEndElement();
                }
            }
            if (_listResources != null)
            {
                for (int i = 0; i < _listResources.Count; i++)
                {
                    ResourceContent content = _listResources[i];

                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", "Resource");
                    content.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }

            // Write the group specific contents, if any...
            this.OnWriteContentXml(writer);
            writer.WriteEndElement();              // end - contents

            // Write other group specific options
            this.OnWriteXml(writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _groupId; 
            }
        }

        #endregion

        #region ICloneable Members

        protected virtual BuildGroup Clone(BuildGroup clonedGroup)
        {
            if (clonedGroup == null)
            {
                clonedGroup = (BuildGroup)this.MemberwiseClone();
            }

            if (_groupId != null)
            {
                clonedGroup._groupId = String.Copy(_groupId);
            }
            if (_groupName != null)
            {
                clonedGroup._groupName = String.Copy(_groupName);
            }
            if (_groupDescription != null)
            {
                clonedGroup._groupDescription = String.Copy(_groupDescription);
            }
            if (_runningTitle != null)
            {
                clonedGroup._runningTitle = String.Copy(_runningTitle);
            }

            if (_properties != null)
            {
                clonedGroup._properties = _properties.Clone();
            }

            if (_contentFile != null)
            {
                clonedGroup._contentFile = _contentFile.Clone();
            }
            if (_groupDir != null)
            {
                clonedGroup._groupDir = _groupDir.Clone();
            }  

            if (_listTokens != null)
            {
                clonedGroup._listTokens = _listTokens.Clone();
            }
            if (_listMedia != null)
            {
                clonedGroup._listMedia = _listMedia.Clone();
            }
            if (_listShared != null)
            {
                clonedGroup._listShared = _listShared.Clone();
            }
            if (_listResources != null)
            {
                clonedGroup._listResources = _listResources.Clone();
            }
            if (_listSnippets != null)
            {
                clonedGroup._listSnippets = _listSnippets.Clone();
            }

            return clonedGroup;
        }

        #endregion
    }
}
