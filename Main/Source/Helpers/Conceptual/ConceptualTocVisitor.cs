using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public abstract class ConceptualTocVisitor : ConceptualVisitor
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        protected ConceptualTocVisitor()
        {
        }

        protected ConceptualTocVisitor(string name)
            : base(name)
        {
        }

        protected ConceptualTocVisitor(string name,
            ConceptualEngineSettings engineSettings)
            : base(name, engineSettings)
        {
        }

        protected ConceptualTocVisitor(ConceptualTocVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        #endregion
    }
}
