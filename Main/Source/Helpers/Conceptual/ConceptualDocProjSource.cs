using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualDocProjSource : ConceptualSource
    {
        #region Public Static Fields

        public const string SourceName = 
            "Sandcastle.Conceptual.ConceptualDocProjSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualDocProjSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualDocProjSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualDocProjSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualDocProjSource(ConceptualDocProjSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ConceptualDocProjSource.SourceName;
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

        public override ConceptualContent Create()
        {
            return null;
        }

        #endregion

        #region ICloneable Members

        public override BuildSource Clone()
        {
            ConceptualDocProjSource source = new ConceptualDocProjSource(this);

            return source;
        }

        #endregion
    }
}
