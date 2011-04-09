using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceDocProjSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceDocProjSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceDocProjSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceDocProjSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceDocProjSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceDocProjSource(ReferenceDocProjSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceDocProjSource.SourceName;
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
            ReferenceDocProjSource source = new ReferenceDocProjSource(this);

            return source;
        }

        #endregion
    }
}
