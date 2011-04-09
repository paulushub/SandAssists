using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceAjaxDocSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceAjaxDocSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceAjaxDocSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceAjaxDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceAjaxDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceAjaxDocSource(ReferenceAjaxDocSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceAjaxDocSource.SourceName;
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
            ReferenceAjaxDocSource source = new ReferenceAjaxDocSource(this);

            return source;
        }

        #endregion
    }
}
