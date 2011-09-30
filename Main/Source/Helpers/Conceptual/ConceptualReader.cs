using System;

namespace Sandcastle.Conceptual
{
    public abstract class ConceptualReader : BuildObject
    {
        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualReader"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReader"/> class
        /// with the default parameters.
        /// </summary>
        protected ConceptualReader()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public abstract ConceptualContent Read(string contentFile);

        #endregion
    }
}
