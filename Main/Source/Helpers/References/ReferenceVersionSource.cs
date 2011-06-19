using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionSource : ReferenceSource, IBuildNamedItem
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceVersionSource";

        #endregion

        #region Private Fields

        private string           _label;
        private string           _sourceId;
        private ReferenceContent _content;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceVersionSource()
            : this("Ver" + Guid.NewGuid().ToString().Replace("-", String.Empty))
        {
        }

        public ReferenceVersionSource(string sourceId)
        {
            if (String.IsNullOrEmpty(sourceId))
            {
                _sourceId = "Ver" + Guid.NewGuid().ToString().Replace("-", String.Empty);
            }
            else
            {
                _sourceId = sourceId;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceVersionSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVersionSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVersionSource(ReferenceVersionSource source)
            : base(source)
        {
            _label    = source._label;
            _sourceId = source._sourceId;
            _content  = source._content;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceVersionSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_label) || 
                    _content == null || _content.IsEmpty)
                {
                    return false;
                }

                return true;
            }
        }

        public string Id
        {
            get
            {
                return _sourceId;
            }
        }

        public string VersionLabel
        {
            get
            {
                return _label;
            }
            set
            {
                if (value != null)
                {
                    _label = value.Trim();
                }
                else
                {
                    _label = String.Empty;
                }
            }
        }

        public ReferenceContent Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        #endregion

        #region Public Methods

        public override ReferenceContent Create()
        {
            return _content;
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

        #region ICloneable Members

        public override BuildSource Clone()
        {
            ReferenceVersionSource source = new ReferenceVersionSource(this);

            if (_label != null)
            {
                source._label = String.Copy(_label);
            }
            if (_sourceId != null)
            {
                source._sourceId = String.Copy(_sourceId);
            }
            if (_content != null)
            {
                source._content = _content.Clone();
            }

            return source;
        }

        #endregion
    }
}
