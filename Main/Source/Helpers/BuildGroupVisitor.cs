using System;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildGroupVisitor<T> : BuildObject<T>, IBuildNamedItem
                where T : BuildGroupVisitor<T>
    {
        #region Private Fields

        private bool         _isInitialized;
        private bool         _isEnabled;
        private bool         _continueOnError;
        private string       _name;

        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildGroupVisitor"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroupVisitor"/> class
        /// to the default values.
        /// </summary>
        protected BuildGroupVisitor()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// with the specified group visitor name.
        /// </summary>
        /// <param name="visitorName">
        /// A <see cref="System.String"/> specifying the name of this group visitor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="visitorName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="visitorName"/> is empty.
        /// </exception>
        protected BuildGroupVisitor(string visitorName)
        {   
            BuildExceptions.NotNullNotEmpty(visitorName, "visitorName");

            _name            = visitorName;
            _isEnabled       = true;
            _continueOnError = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroupVisitor{T}"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildGroupVisitor{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildGroupVisitor{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildGroupVisitor(BuildGroupVisitor<T> source)
            : base(source)
        {
            _isInitialized   = source._isInitialized;
            _name            = source._name;
            _isEnabled       = source._isEnabled;
            _continueOnError = source._continueOnError;
        }

        #endregion

        #region Public Properties

        public abstract BuildGroupType GroupType
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this visitor is initialized 
        /// and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this visitor is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildGroupVisitor.Initialize(BuildContext)"/>
        /// <seealso cref="BuildGroupVisitor.Uninitialize()"/>
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
        /// Gets the unique name of the group visitor.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the unique name of this 
        /// group visitor.
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

        #endregion

        #region Protected Properties

        protected BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            _context       = context;
            _isInitialized = true;
        }

        public abstract void Visit(BuildGroup group);

        public virtual void Uninitialize()
        {
            _context       = null;
            _isInitialized = false;
        }

        #endregion
    }
}
