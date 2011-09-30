using System;

namespace Sandcastle
{
    /// <summary>
    /// An <see langword="abstract"/> base class for all documentation sources.
    /// <para>
    /// The documentation sources do not normally contain the documentation 
    /// contents but the information required to create the
    /// documentation contents.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The derived type of this base class.</typeparam>
    /// <remarks>
    /// <para>
    /// The documentation sources provide the bases for dynamically creating
    /// contents and for importing contents from related product formats.
    /// </para>
    /// <para>
    /// For instance, a documentation source defined by <c>XML Schema</c> 
    /// or <c>XSD</c>, may create conceptual topics by extracting the 
    /// documentation annotations in the schema.
    /// </para>
    /// <para>
    /// A derived <see langword="abstract"/> class, <see cref="BuildGroupSource"/>,
    /// provides the base class for all sources that create documentation
    /// group, <see cref="BuildGroup"/>, instead of the contents of the group.
    /// </para>
    /// <para>
    /// During the content generation, the source is initialized with an instance
    /// of the context class, <see cref="BuildSourceContext"/>, which provides
    /// the options for the content generation or creation.
    /// </para>
    /// </remarks>
    /// <seealso cref="BuildGroupSource"/>
    /// <seealso cref="BuildSourceContext"/>
    [Serializable]
    public abstract class BuildSource<T> : BuildObject<T>
        where T : BuildSource<T>
    {
        #region Private Fields

        private bool               _isInitialized;

        [NonSerialized]
        private BuildSourceContext _sourceContext;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSource{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource{T}"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource{T}"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSource{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSource{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildSource(BuildSource<T> source)
            : base(source)
        {
            _isInitialized = source._isInitialized;
            _sourceContext = source._sourceContext;
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
        /// initialized and ready for the it content generation or creation.
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

        /// <summary>
        /// Gets a value indicating whether this source is valid and can
        /// generate the documentation content.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this content source is
        /// valid and can create documentation content or group; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public abstract bool IsValid
        {
            get;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the content generation context when this source is initialized.
        /// </summary>
        /// <value>
        /// An instance of <see cref="BuildSourceContext"/> specifying the
        /// content generation context when initialized; otherwise, this is
        /// <see langword="null"/>.
        /// </value>
        protected BuildSourceContext SourceContext
        {
            get
            {
                return _sourceContext;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initiates a content generation request to this source. The actual
        /// request format and the result depends the source type, which are
        /// provided by the derived classes.
        /// </summary>
        /// <param name="context">
        /// An instance of the <see cref="BuildSourceContext"/> class defining
        /// all the information for the content generation.
        /// </param>
        public virtual void Initialize(BuildSourceContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            _sourceContext       = context;
            _isInitialized = true;
        }

        /// <summary>
        /// Signals the end of a content generation request. Any temporal
        /// disposable object created can be disposed. 
        /// </summary>
        public virtual void Uninitialize()
        {
            _sourceContext       = null;
            _isInitialized = false;
        }

        #endregion
    }
}
