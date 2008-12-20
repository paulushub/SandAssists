﻿using System;
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
        #region Private Static Fields

        private static int _groupCount = 0;

        #endregion

        #region Private Fields

        private bool   _isExcluded;
        private bool   _isTocExcluded;

        private Guid   _groupId;

        private string _groupName;
        private string _groupTitle;
        private string _workingDir;
        private string _runningTitle;

        private List<LinkContent>     _listLinks;
        private List<TokenContent>    _listTokens;
        private List<MediaContent>    _listMedia;
        private List<SharedContent>   _listShared;
        private List<SnippetContent>  _listSnippets;
        private List<ResourceContent> _listResources;
        private Dictionary<string, string> _properties;

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
        {
            _groupCount++;

            _groupName     = "Group" + _groupCount.ToString();
            _groupId       = Guid.NewGuid();
            _listLinks     = new List<LinkContent>();
            _listMedia     = new List<MediaContent>();
            _listTokens    = new List<TokenContent>();
            _listShared    = new List<SharedContent>();
            _listSnippets  = new List<SnippetContent>();
            _listResources = new List<ResourceContent>();
            _properties    = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);
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
            _listLinks     = source._listLinks;
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

        /// <summary>
        /// Gets the unique identifier of this help group.
        /// </summary>
        /// <value>
        /// A <see cref="System.Guid"/> specifying the unique identifier of this group.
        /// </value>
        public Guid Identifier
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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
                    value = Environment.ExpandEnvironmentVariables(value);
                    value = Path.GetFullPath(value);
                }
                _workingDir = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<LinkContent> LinkContents
        {
            get
            {
                return _listLinks;
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
        public IList<SnippetContent> SnippetContents
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
        /// Gets a collection containing the values in the <see cref="BuildGroup"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildGroup"/>.
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

        public virtual bool Initialize(BuildSettings settings)
        {
            BuildExceptions.NotNull(settings, "settings");

            return true;
        }

        public virtual void Uninitialize()
        {
        }

        #endregion

        #region Links Methods

        public virtual void AddLinkItem(string linkFile)
        {
            if (String.IsNullOrEmpty(linkFile))
            {
                return;
            }

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }
            LinkContent defaultContent = null;
            if (_listLinks.Count == 0)
            {
                defaultContent = new LinkContent();
                _listLinks.Add(defaultContent);
            }
            else
            {
                defaultContent = _listLinks[0];
            }

            defaultContent.Add(new LinkItem(linkFile));
        }

        public virtual void AddLinkItem(string linkDir, bool isRecursive)
        {
            if (String.IsNullOrEmpty(linkDir))
            {
                return;
            }

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }
            LinkContent defaultContent = null;
            if (_listLinks.Count == 0)
            {
                defaultContent = new LinkContent();
                _listLinks.Add(defaultContent);
            }
            else
            {
                defaultContent = _listLinks[0];
            }

            defaultContent.Add(new LinkItem(linkDir, isRecursive));
        }

        public virtual void AddLink(LinkContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }

            _listLinks.Add(content);
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
                _listResources = new List<ResourceContent>();
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
                _listResources = new List<ResourceContent>();
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
                _listMedia = new List<MediaContent>();
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
                _listTokens = new List<TokenContent>();
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
                _listShared = new List<SharedContent>();
            }

            _listShared.Add(content);
        }

        public virtual IList<SharedItem> PrepareShared()
        {
            List<SharedItem> listShared = new List<SharedItem>();

            if (!String.IsNullOrEmpty(_runningTitle))
            {
                listShared.Add(new SharedItem("runningHeaderText",
                    _runningTitle));
            }

            return listShared;
        }

        #endregion

        #region Snippets Methods

        public virtual void AddSnippet(SnippetContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listSnippets == null)
            {
                _listSnippets = new List<SnippetContent>();
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
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildGroup"/>.
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
            BuildExceptions.NotNullNotEmpty(key, "key");

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
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

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
    }
}