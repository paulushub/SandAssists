using System;
using System.Diagnostics;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceTocVisitor : ReferenceVisitor
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        protected ReferenceTocVisitor()
        {
        }

        protected ReferenceTocVisitor(string name)
            : base(name)
        {
        }

        protected ReferenceTocVisitor(string name, 
            ReferenceConfiguration configuration)
            : base(name, configuration)
        {
        }

        protected ReferenceTocVisitor(ReferenceTocVisitor source)
            : base(source)
        {
        }

        #endregion
    }
}
