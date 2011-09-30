using System;
using System.Collections.Generic;

using Sandcastle.Sources;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class BuildGroupSource : BuildSource<BuildGroupSource>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "documentSource";

        #endregion

        #region Private Fields

        private bool   _isEnabled;

        private string _sourceId;
        private string _sourceTitle;

        private bool _includesSettings;
        private bool _includesReferences;
        private bool _includesConceptuals;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildGroupSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroupSource"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildGroupSource()
            : this(Guid.NewGuid().ToString())
        {
        }

        protected BuildGroupSource(string sourceTitle)
        {
            BuildExceptions.NotNullNotEmpty(sourceTitle, "sourceTitle");

            _sourceId            = Guid.NewGuid().ToString();
            _sourceTitle         = sourceTitle;
            _includesSettings    = true;
            _includesReferences  = true;
            _includesConceptuals = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildGroupSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildGroupSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildGroupSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildGroupSource(BuildGroupSource source)
            : base(source)
        {
            _sourceId            = source._sourceId;
            _sourceTitle         = source._sourceTitle;
            _includesSettings    = source._includesSettings;
            _includesReferences  = source._includesReferences;
            _includesConceptuals = source._includesConceptuals;
        }

        #endregion

        #region Public Properties

        public string Id
        {
            get
            {
                return _sourceId;
            }
            protected set
            {
                _sourceId = value;
            }
        }

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
        /// Gets a value indicating whether this content source is a content generator.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this source is a content generator;
        /// otherwise, it is <see langword="false"/>. By default, this returns 
        /// <see langword="false"/>.
        /// </value>
        public override bool IsGenerator
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the name of this group.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of this group.
        /// </value>
        public string Title
        {
            get
            {
                return _sourceTitle;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!String.IsNullOrEmpty(value))
                {
                    _sourceTitle = value;
                }
            }
        }

        public virtual bool IncludesSettings
        {
            get
            {
                return _includesSettings;
            }
            set
            {
                _includesSettings = value;
            }
        }

        public virtual bool IncludesReferences
        {
            get
            {
                return _includesReferences;
            }
            set
            {
                _includesReferences = value;
            }
        }

        public virtual bool IncludesConceptuals
        {
            get
            {
                return _includesConceptuals;
            }
            set
            {
                _includesConceptuals = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract IList<BuildGroup> Create(BuildSettings settings,
            BuildContext context);

        public static BuildGroupSource CreateSource(string sourceName)
        {
            BuildExceptions.NotNullNotEmpty(sourceName, "sourceName");

            switch (sourceName)
            {
                case ShfbGroupSource.SourceName:
                    return new ShfbGroupSource();
                case ImportGroupSource.SourceName:
                    return new ImportGroupSource();
                case DocProjectGroupSource.SourceName:
                    return new DocProjectGroupSource();
                case NDocGroupSource.SourceName:
                    return new NDocGroupSource();
            }

            return null;
        }

        #endregion

        #region Protected Methods

        protected virtual BuildGroupSource Clone(BuildGroupSource source)
        {
            if (source == null)
            {
                source = (BuildGroupSource)this.MemberwiseClone();

                source._sourceId            = this._sourceId;
                source._sourceTitle         = this._sourceTitle;
                source._includesSettings    = this._includesSettings;
                source._includesReferences  = this._includesReferences;
                source._includesConceptuals = this._includesConceptuals;
            }
            if (_sourceId != null)
            {
                source._sourceId = String.Copy(_sourceId);
            }
            if (_sourceTitle != null)
            {
                source._sourceTitle = String.Copy(_sourceTitle);
            }

            return source;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _sourceId; 
            }
        }

        #endregion
    }
}
