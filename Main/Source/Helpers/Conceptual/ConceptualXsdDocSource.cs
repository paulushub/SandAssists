using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualXsdDocSource : ConceptualSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.Conceptual.ConceptualXsdDocSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualXsdDocSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualXsdDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualXsdDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualXsdDocSource(ConceptualXsdDocSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ConceptualXsdDocSource.SourceName;
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
            ConceptualXsdDocSource source = new ConceptualXsdDocSource(this);

            return source;
        }

        #endregion
    }
}
