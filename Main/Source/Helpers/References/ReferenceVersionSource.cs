using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceVersionSource";

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
        {
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
            ReferenceVersionSource source = new ReferenceVersionSource(this);

            return source;
        }

        #endregion
    }
}
