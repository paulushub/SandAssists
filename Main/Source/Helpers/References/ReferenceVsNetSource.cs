using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVsNetSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceVsNetSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceVsNetSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceVsNetSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVsNetSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVsNetSource(ReferenceVsNetSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceVsNetSource.SourceName;
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
            ReferenceVsNetSource source = new ReferenceVsNetSource(this);

            return source;
        }

        #endregion
    }
}
