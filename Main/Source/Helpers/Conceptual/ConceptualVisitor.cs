using System;
using System.Diagnostics;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// These are build visitors used to process reference document sources;
    /// comments and reflections, for the building the documentations.
    /// </summary>
    [Serializable]
    public abstract class ConceptualVisitor : BuildVisitor<ConceptualVisitor>,
        IBuildNamedItem, IDisposable
    {
        #region Private Fields

        private bool _isInitialized;
        private string _name;

        [NonSerialized]
        private BuildContext _context;
        [NonSerialized]
        private ConceptualGroup _group;
        [NonSerialized]
        private ConceptualEngineSettings _engineSettings;

        #endregion

        #region Constructors and Destructor

        protected ConceptualVisitor()
        {
            _name = Guid.NewGuid().ToString();
        }

        protected ConceptualVisitor(string name)
            : this()
        {
            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }
        }

        protected ConceptualVisitor(string name,
            ConceptualEngineSettings engineSettings)
            : this()
        {
            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }

            _engineSettings = engineSettings;
        }

        protected ConceptualVisitor(ConceptualVisitor source)
            : base(source)
        {
            _name = source._name;

            if (_name != null)
            {
                _name = String.Copy(_name);
            }
        }

        /// <summary>
        /// This allows the <see cref="ConceptualVisitor"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="ConceptualVisitor"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~ConceptualVisitor()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this reference visitor.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this reference visitor.
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the unique name of the target build configuration or options 
        /// processed by this reference visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of the options processed by this
        /// reference visitor.
        /// </value>
        public abstract string TargetName
        {
            get;
        }

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

        #region Protected Properties

        protected BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        protected ConceptualGroup Group
        {
            get
            {
                return _group;
            }
        }

        protected ConceptualEngineSettings EngineSettings
        {
            get
            {
                return _engineSettings;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context, ConceptualGroup group)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(group, "group");

            if (_isInitialized)
            {
                return;
            }

            _context = context;
            _group = group;

            if (_engineSettings == null)
            {
                BuildSettings settings = context.Settings;
                Debug.Assert(settings != null,
                    "The settings is not associated with the context.");
                if (settings == null)
                {
                    return;
                }
                _engineSettings = (ConceptualEngineSettings)settings.EngineSettings[
                    BuildEngineType.Conceptual];
                Debug.Assert(_engineSettings != null,
                    "The settings does not include the conceptual engine settings.");
                if (_engineSettings == null)
                {
                    return;
                }
            }

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _isInitialized = false;
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This releases all resources used by the <see cref="ConceptualVisitor"/> object.
        /// </overloads>
        /// <summary>
        /// This releases all resources used by the <see cref="ConceptualVisitor"/> object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This releases the unmanaged resources used by the <see cref="ConceptualVisitor"/> 
        /// and optionally releases the managed resources. 
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
