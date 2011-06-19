using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class BuildGroup : BuildObject<BuildGroup>, IBuildNamedItem
    {
        #region Private Fields

        private bool   _isExcluded;
        private bool   _isTocExcluded;
        private bool   _isInitialized;

        private bool   _syntaxUsage;

        private string  _groupId;

        private string _groupName;
        private string _groupTitle;
        private string _runningTitle;

        private BuildSyntaxType _syntaxType;
        private BuildProperties _properties;

        private BuildList<TokenContent>       _listTokens;
        private BuildList<MediaContent>       _listMedia;
        private BuildList<SharedContent>      _listShared;
        private BuildList<ResourceContent>    _listResources;
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
            _syntaxUsage   = source._syntaxUsage;
            _syntaxType    = source._syntaxType;

            _groupId       = source._groupId;
            _groupName     = source._groupName;
            _properties    = source._properties;

            _listMedia     = source._listMedia;
            _listTokens    = source._listTokens;
            _listShared    = source._listShared;
            _listSnippets  = source._listSnippets;
            _listResources = source._listResources;
            _isInitialized = source._isInitialized;
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
        public string Title
        {
            get
            {
                return _groupTitle;
            }
            set
            {
                _groupTitle = value;
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

        #endregion

        #region Public Methods

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

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _groupId; 
            }
        }

        #endregion
    }
}
