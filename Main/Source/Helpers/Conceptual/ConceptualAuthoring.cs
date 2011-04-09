using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public abstract class ConceptualAuthoring : BuildObject<ConceptualAuthoring>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualAuthoring"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualAuthoring"/> class
        /// to the default values.
        /// </summary>
        protected ConceptualAuthoring()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualAuthoring"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualAuthoring"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ConceptualAuthoring(ConceptualAuthoring source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
