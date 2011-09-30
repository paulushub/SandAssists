using System;
using System.Xml;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the conceptual build process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A custom build component implementation must provide this configuration
    /// for users to access the options available.
    /// </para>
    /// <para>
    /// This is the base class for writing a custom build component plugin for
    /// the Sandcastle Assist documentation system.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class BuildComponentConfiguration
        : BuildOptions<BuildComponentConfiguration>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "componentConfiguration";

        #endregion

        #region Private Fields

        private bool _isEnabled;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// to the default values.
        /// </summary>
        protected BuildComponentConfiguration()
        {
            _isEnabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualOptions"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualOptions"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildComponentConfiguration(BuildComponentConfiguration source)
            : base(source)
        {
            _isEnabled = source._isEnabled;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the category of options.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value specifying whether the set of selections defined
        /// by this object can be applied in the build process.
        /// </summary>
        /// <value>
        /// This <see langword="true"/> if the selection set can be applied; otherwise, 
        /// <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and usable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        public virtual bool IsActive
        {
            get
            {
                return this.Enabled;
            }
        }

        /// <summary>
        /// Gets the build engine type, which is targeted by this configuration.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildEngineType"/>, specifying
        /// the build engine type targeted by this configuration.
        /// </value>
        public abstract BuildEngineType EngineType
        {
            get;
        }

        /// <summary>
        /// Gets the source of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildComponentType"/>,
        /// specifying the source of this build component.
        /// </value>
        public abstract BuildComponentType ComponentType
        {
            get;
        }

        /// <summary>
        /// Gets the unique name of the build component supported by this configuration. 
        /// </summary>
        /// <value>
        /// A string containing the unique name of the build component, this 
        /// should normally include the namespace.
        /// </value>
        public abstract string ComponentName
        {
            get;
        }

        /// <summary>
        /// Gets the path of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// A string containing the path to the assembly in which the build
        /// component is defined.
        /// </value>
        public abstract string ComponentPath
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying whether this configuration is displayed or 
        /// visible to the user.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this configuration is visible
        /// and accessible to the user; otherwise it is <see langword="false"/>.
        /// </value>
        public abstract bool IsBrowsable
        {
            get;
        }

        /// <summary>
        /// Gets a copyright and license notification for the component targeted 
        /// by this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the copyright and license of the component.
        /// </value>
        public abstract string Copyright
        {
            get;
        }

        /// <summary>
        /// Gets the description of the component targeted by this configuration.
        /// </summary>
        /// <value>
        /// A string providing a description of the component.
        /// </value>
        /// <remarks>
        /// This must be a plain text, brief and informative.
        /// </remarks>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Gets the file name of the documentation explaining the features and
        /// how to use the component.
        /// </summary>
        /// <value>
        /// A string containing the file name of the documentation.
        /// </value>
        /// <remarks>
        /// <para>
        /// This should be either a file name (with file extension, but without
        /// the path) or include a path relative to the assembly containing this
        /// object implementation.
        /// </para>
        /// <para>
        /// The expected file format is HTML, PDF, XPS, CHM and plain text.
        /// </para>
        /// </remarks>
        public abstract string HelpFileName
        {
            get;
        }

        /// <summary>
        /// Gets the version of the target component.
        /// </summary>
        /// <value>
        /// An instance of <see cref="System.Version"/> specifying the version
        /// of the target component.
        /// </value>
        public abstract Version Version
        {
            get;
        }

        /// <summary>
        /// Gets the location marker component name of the target component in 
        /// configuration file.
        /// </summary>
        /// <value>
        /// <para>
        /// A string containing the unique name, which should normally include t
        /// he namespace, of the component relative to which the target 
        /// component of this configuration is placed. 
        /// </para>
        /// <para>
        /// For components already placed in the configuration file this is 
        /// <see langword="null"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// This must be one of the well-known build components. For components
        /// already placed in the configuration file, this property is not needed.
        /// </remarks>
        public abstract string InsertLocationComponentName
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying the location of the target component relative
        /// to the location marker component.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildInsertType"/> specifying
        /// the relative position for a custom build component not already defined
        /// in the configuration file; otherwise, this is 
        /// <see cref="BuildInsertType.None"/>.
        /// </value>
        public abstract BuildInsertType InsertType
        {
            get; 
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceEngineSettings"/> class instance, 
        /// this property is <see cref="TagName"/>.
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

        #region Public Methods

        /// <summary>
        /// The creates the configuration information or settings required by the
        /// target component for the build process.
        /// </summary>
        /// <param name="group">
        /// A build group, <see cref="BuildGroup"/>, representing the documentation
        /// target for configuration.
        /// </param>
        /// <param name="writer">
        /// An <see cref="XmlWriter"/> object used to create one or more new 
        /// child nodes at the end of the list of child nodes of the current node. 
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> for a successful configuration;
        /// otherwise, it returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="XmlWriter"/> writer passed to this configuration object
        /// may be passed on to other configuration objects, so do not close or 
        /// dispose it.
        /// </remarks>
        public abstract bool Configure(BuildGroup group, XmlWriter writer);

        #endregion
    }

    /// <summary>
    /// A strongly-typed collection of <see cref="BuildComponentConfiguration"/> objects.
    /// </summary>
    [Serializable]
    public sealed class BuildComponentConfigurationList : BuildKeyedList<BuildComponentConfiguration>
    {
        #region Private Fields

        private bool            _isInitialized;
        private BuildEngineType _engineType;

        private BuildMultiSet<string, BuildComponentConfiguration> _multiMap;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildComponentConfigurationList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildComponentConfigurationList"/> 
        /// class with the default parameters.
        /// </summary>
        public BuildComponentConfigurationList()
            : this(BuildEngineType.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildComponentConfigurationList"/> 
        /// class with the specified engine type.
        /// </summary>
        /// <param name="engineType">
        /// The build engine type, which is targeted by this set of configurations.
        /// </param>
        public BuildComponentConfigurationList(BuildEngineType engineType)
        {
            _engineType = engineType;
            _multiMap   = new BuildMultiSet<string, BuildComponentConfiguration>();

            this.Changed += new EventHandler<
                BuildListEventArgs<BuildComponentConfiguration>>(OnComponentConfigurationListChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildComponentConfigurationList"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildComponentConfigurationList"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildComponentConfigurationList"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildComponentConfigurationList(BuildComponentConfigurationList source)
            : base(source)
        {
            _engineType = source._engineType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this options is initialized 
        /// and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this options is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildComponentConfigurationList.Initialize(BuildContext)"/>
        /// <seealso cref="BuildComponentConfigurationList.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// Gets the build engine type, which is targeted by this set of
        /// configurations.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildEngineType"/>, specifying
        /// the build engine type targeted by this set of configurations.
        /// </value>
        public BuildEngineType EngineType
        {
            get
            {
                return _engineType;
            }
        }

        #endregion
        
        #region Public Methods

        public void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            for (int i = 0; i < this.Count; i++)
            {
                BuildComponentConfiguration config = this[i];
                config.Initialize(context);
            }

            _isInitialized = true;
        }

        public void Uninitialize()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].Uninitialize();
            }

            _isInitialized = false;
        }

        public IList<BuildComponentConfiguration> GetConfigurations(
            string componentName)
        {
            if (String.IsNullOrEmpty(componentName))
            {
                return null;
            }

            if (_multiMap == null || _multiMap.Count == 0)
            {
                return null;
            }           

            HashSet<BuildComponentConfiguration> componentSet = 
                _multiMap[componentName];

            if (componentSet != null && componentSet.Count != 0)
            {
                return new List<BuildComponentConfiguration>(componentSet);
            }

            return null;
        }

        public bool ContainsComponent(string componentName)
        {
            if (_multiMap != null && _multiMap.Count != 0)
            {
                return _multiMap.ContainsKey(componentName);
            }

            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="BuildComponentConfigurationList">collection</see> 
        /// at the specified index. 
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="newItem">
        /// The object to insert. The value cannot be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the build engine type the <paramref name="newItem"/> does not
        /// match that of the list.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="newItem"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than zero
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is greater than the total count.
        /// </exception>
        protected override void InsertItem(int index, BuildComponentConfiguration newItem)
        {
            if (newItem != null)
            {
                if (newItem.EngineType != _engineType)
                {
                    throw new ArgumentException(
                        "The build engine type of the item does not match the type of this list.",
                        "newItem");
                }
            }

            base.InsertItem(index, newItem);
        }

        /// <summary>
        /// This replaces the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="newItem">
        /// The new value for the element at the specified index. 
        /// The value cannot be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the build engine type the <paramref name="newItem"/> does not
        /// match that of the list.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="newItem"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than zero
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is greater than the total count.
        /// </exception>
        protected override void SetItem(int index, BuildComponentConfiguration newItem)
        {
            if (newItem != null)
            {
                if (newItem.EngineType != _engineType)
                {
                    throw new ArgumentException(
                        "The build engine type of the item does not match the type of this list.",
                        "newItem");
                }
            }

            base.SetItem(index, newItem);
        }

        #endregion

        #region Private Methods

        private void OnComponentConfigurationListChanged(object sender,
            BuildListEventArgs<BuildComponentConfiguration> e)
        {    
            if (_multiMap == null)
            {
                _multiMap = 
                    new BuildMultiSet<string, BuildComponentConfiguration>();
            }

            switch (e.ChangeType)
            {
                case BuildListChangeType.Added:
                    BuildComponentConfiguration addedItem = e.ChangedItem;
                    _multiMap.Add(addedItem.ComponentName, addedItem);
                    break;
                case BuildListChangeType.Removed:
                    BuildComponentConfiguration removedItem = e.ChangedItem;
                    _multiMap.Remove(removedItem.ComponentName, removedItem);
                    break;
                case BuildListChangeType.Replaced:
                    BuildComponentConfiguration changedItem = e.ChangedItem;
                    BuildComponentConfiguration replacedWithItem = e.ReplacedWith;
                    _multiMap.Remove(changedItem.ComponentName, changedItem);
                    _multiMap.Add(replacedWithItem.ComponentName, replacedWithItem);
                    break;
                case BuildListChangeType.Cleared:
                    _multiMap.Clear();
                    break;
            }
        }

        #endregion

        #region ICloneable Members

        public override BuildKeyedList<BuildComponentConfiguration> Clone()
        {
            BuildComponentConfigurationList clonedList = 
                new BuildComponentConfigurationList(this);

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                clonedList.Add(this[i].Clone());
            }

            return clonedList;
        }

        #endregion
    }
}
