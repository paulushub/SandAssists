using System;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the conceptual build process.
    /// </summary>
    [Serializable]
    public abstract class BuildConfiguration
        : BuildOptions<BuildConfiguration>, IBuildNamedItem
    {
        #region Private Fields

        private bool   _isEnabled;
        private bool   _continueOnError;
        private string _name;
        private BuildEngineType _engineType;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// to the default values.
        /// </summary>
        protected BuildConfiguration()
            : this(Guid.NewGuid().ToString(), BuildEngineType.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <param name="engineType">
        /// Specifies the build engine type, which is targeted by this configuration.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        protected BuildConfiguration(string optionsName, 
            BuildEngineType engineType)
        {
            BuildExceptions.NotNullNotEmpty(optionsName, "optionsName");

            _name            = optionsName;
            _isEnabled       = true;
            _continueOnError = true;
            _engineType      = engineType;
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
        protected BuildConfiguration(BuildConfiguration source)
            : base(source)
        {
            _name            = source._name;
            _isEnabled       = source._isEnabled;
            _continueOnError = source._continueOnError;
            _engineType      = source._engineType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the category of options.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the unique name of this 
        /// category of options.
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
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
        /// Gets or sets a value specifying whether the build process can continue when
        /// an error occur while processing the behavior defined by this object.
        /// The default is <see langword="true"/>.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if build process can continue when an error
        /// occur in the processing of the behavior defined by this options; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool ContinueOnError
        {
            get
            {
                return _continueOnError;
            }
            set
            {
                _continueOnError = value;
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
        public BuildEngineType EngineType
        {
            get
            {
                return _engineType;
            }
        }

        #endregion
    }

    /// <summary>
    /// A strongly-typed collection of <see cref="BuildConfiguration"/> objects.
    /// </summary>
    [Serializable]
    public sealed class BuildConfigurationList : BuildKeyedList<BuildConfiguration>
    {
        #region Private Fields

        private BuildEngineType _engineType;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildConfigurationList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationList"/> 
        /// class with the default parameters.
        /// </summary>
        public BuildConfigurationList()
            : this(BuildEngineType.None)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationList"/> 
        /// class with the specified engine type.
        /// </summary>
        /// <param name="engineType"></param>
        public BuildConfigurationList(BuildEngineType engineType)
        {
            _engineType = engineType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationList"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildConfigurationList"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildConfigurationList"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildConfigurationList(BuildConfigurationList source)
            : base(source)
        {
            _engineType = source._engineType;
        }   

        #endregion

        #region Public Properties

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

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="BuildConfigurationList">collection</see> 
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
        protected override void InsertItem(int index, BuildConfiguration newItem)
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
        protected override void SetItem(int index, BuildConfiguration newItem)
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
    }
}
