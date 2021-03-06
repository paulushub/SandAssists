﻿using System;
using System.Diagnostics;

namespace Sandcastle.References
{
    /// <summary>
    /// These are build visitors used to process reference document sources;
    /// comments and reflections, for the building the documentations.
    /// </summary>
    [Serializable]
    public abstract class ReferenceVisitor : BuildVisitor<ReferenceVisitor>, 
        IBuildNamedItem, IDisposable
    {
        #region Private Fields

        private bool   _isInitialized;
        private string _name;

        [NonSerialized]
        private BuildContext _context;
        [NonSerialized]
        private ReferenceGroup _group;
        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;

        #endregion

        #region Constructors and Destructor

        protected ReferenceVisitor()
        {
            _name = Guid.NewGuid().ToString();
        }

        protected ReferenceVisitor(string name)
            : this()
        {
            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }
        }

        protected ReferenceVisitor(string name,
            ReferenceConfiguration configuration)
            : this()
        {
            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }
        }

        protected ReferenceVisitor(ReferenceVisitor source)
            : base(source)
        {
            _name = source._name;

            if (_name != null)
            {
                _name = String.Copy(_name);
            }

            _engineSettings = source._engineSettings;
        }

        /// <summary>
        /// This allows the <see cref="ReferenceVisitor"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="ReferenceVisitor"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~ReferenceVisitor()
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

        protected ReferenceGroup Group
        {
            get
            {
                return _group;
            }
        }

        protected ReferenceEngineSettings EngineSettings
        {
            get
            {
                return _engineSettings;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context, ReferenceGroup group)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(group, "group");

            if (_isInitialized)
            {
                return;
            }

            _context = context;
            _group   = group;

            if (_engineSettings == null)
            {
                BuildSettings settings = context.Settings;
                Debug.Assert(settings != null,
                    "The settings is not associated with the context.");
                if (settings == null)
                {
                    return;
                }
                _engineSettings = (ReferenceEngineSettings)settings.EngineSettings[
                    BuildEngineType.Reference];
                Debug.Assert(_engineSettings != null,
                    "The settings does not include the reference engine settings.");
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

        public abstract void Visit(ReferenceDocument referenceDocument);

        #endregion
                                 
        #region IDisposable Members

        /// <overloads>
        /// This releases all resources used by the <see cref="ReferenceVisitor"/> object.
        /// </overloads>
        /// <summary>
        /// This releases all resources used by the <see cref="ReferenceVisitor"/> object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This releases the unmanaged resources used by the <see cref="ReferenceVisitor"/> 
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
