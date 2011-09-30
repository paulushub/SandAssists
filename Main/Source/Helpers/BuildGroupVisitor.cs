using System;

namespace Sandcastle
{
    /// <summary>
    /// <para>
    /// This is an <see cref="abstract"/> base class for build group visitors,
    /// which prepare the groups for the build process.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Group visitors provide group specific processing by extracting or 
    /// customizing the group properties prior to the build process. This is 
    /// done during the initialization or preparation stage of the build process.
    /// </remarks>
    public abstract class BuildGroupVisitor : BuildObject, IDisposable
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
        /// This allows the <see cref="BuildObject{T}"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildObject{T}"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildGroupVisitor()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the category or type of this group.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildGroupType"/> specifying
        /// the category or type of the group.
        /// </value>
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

        /// <summary>
        /// Gets the build context for this group visitor during the build
        /// process.
        /// </summary>
        /// <value>
        /// An instance of the type, <see cref="BuildContext"/>, if this 
        /// visitor is initialized; otherwise, this is <see langword="null"/>.
        /// </value>
        protected BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepares or initializes this group visitor for processing of its
        /// operations.
        /// </summary>
        /// <param name="context">
        /// An instance of the <see cref="BuildContext"/> specifying the
        /// build context for this visitor.
        /// </param>
        /// <remarks>
        /// This will not be initialized, if already in the initialized state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="BuildGroupVisitor.Uninitialize()"/>
        /// <seealso cref="BuildGroupVisitor.IsInitialized"/>
        public virtual void Initialize(BuildContext context)
        {                  
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                BuildLogger logger = context.Logger;
                if (logger != null)
                {
                    logger.WriteLine("This visitor is already initialized.",
                        BuildLoggerLevel.Warn);
                }

                return;
            }

            _context       = context;
            _isInitialized = true;
        }

        /// <summary>
        /// Applies the processing operations defined by this visitor to the
        /// specified build group.
        /// </summary>
        /// <param name="group">
        /// The <see cref="BuildGroup">build group</see> to which the processing
        /// operations defined by this visitor will be applied.
        /// </param>
        /// <remarks>
        /// The visitor must be initialized before any call this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="group"/> is <see langword="null"/>.
        /// </exception>
        public abstract void Visit(BuildGroup group);

        /// <summary>
        /// Provides the un-initialization by this group visitor, any object
        /// created for the processing is disposed.
        /// <para>
        /// The group visitor returns to an uninitialized state.
        /// </para>
        /// </summary>
        /// <seealso cref="BuildGroupVisitor.Initialize(BuildContext)"/>
        /// <seealso cref="BuildGroupVisitor.IsInitialized"/>
        public virtual void Uninitialize()
        {
            _context       = null;
            _isInitialized = false;
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            _context = null;
        }

        #endregion
    }
}
