using System;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildSource : BuildObject<BuildSource>
    {
        #region Private Fields

        private bool               _isInitialized;

        [NonSerialized]
        private BuildSourceContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildSource(BuildSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique identifier of this content source.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this content source. This must
        /// not be <see langword="null"/> or empty.
        /// </value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this build source is 
        /// initialized and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build source is 
        /// initialized; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildSource.Initialize(BuildSourceContext)"/>
        /// <seealso cref="BuildSource.Uninitialize()"/>
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
        /// Gets a value indicating whether this content source is a content
        /// generator.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this source is a content generator;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        public abstract bool IsGenerator
        {
            get;
        }

        public abstract bool IsValid
        {
            get;
        }

        public abstract BuildEngineType EngineType
        {
            get;
        }

        #endregion

        #region Protected Properties

        protected BuildSourceContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildSourceContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            _context       = context;
            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _context       = null;
            _isInitialized = false;
        }

        #endregion
    }
}
