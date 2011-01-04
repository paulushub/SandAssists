using System;

namespace Sandcastle
{   
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the build process.
    /// </summary>
    /// <remarks>
    /// In general, this settings are options that are preserved between build sessions.
    /// </remarks>
    [Serializable]
    public abstract class BuildOptions<T> : BuildObject<T>
                where T : BuildOptions<T>
    {
        #region Private Fields

        private bool _isInitialized;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildOptions"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildOptions"/> class
        /// to the default values.
        /// </summary>
        protected BuildOptions()
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildOptions{T}"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildOptions{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildOptions{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildOptions(BuildOptions<T> source)
            : base(source)
        {
            _isInitialized = source._isInitialized;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this options is initialized 
        /// and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this options is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildOptions.Initialize(BuildContext)"/>
        /// <seealso cref="BuildOptions.Uninitialize()"/>
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

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _isInitialized = false;
        }

        /// <summary>
        /// This sets the options defined by this object to the default values
        /// or states. 
        /// </summary>
        public virtual void Reset()
        {
        }

        #endregion
    }
}
