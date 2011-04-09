using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceShfbSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceShfbSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceShfbSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceShfbSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceShfbSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceShfbSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceShfbSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceShfbSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceShfbSource(ReferenceShfbSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceShfbSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override ReferenceContent Create()
        {
            return null;
        }

        #endregion

        #region ICloneable Members

        public override BuildSource Clone()
        {
            ReferenceShfbSource source = new ReferenceShfbSource(this);

            return source;
        }

        #endregion
    }
}
