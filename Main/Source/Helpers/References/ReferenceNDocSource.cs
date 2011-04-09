using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceNDocSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceNDocSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceNDocSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceNDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceNDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceNDocSource(ReferenceNDocSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceNDocSource.SourceName;
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
            ReferenceNDocSource source = new ReferenceNDocSource(this);

            return source;
        }

        #endregion
    }
}
