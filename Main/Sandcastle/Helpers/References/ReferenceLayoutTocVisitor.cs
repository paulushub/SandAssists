using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceLayoutTocVisitor : ReferenceTocVisitor
    {
        #region Constructors and Destructor

        public ReferenceLayoutTocVisitor()
        {
        }

        public ReferenceLayoutTocVisitor(ReferenceLayoutTocVisitor source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceTocVisitor Clone()
        {
            ReferenceLayoutTocVisitor filter = new ReferenceLayoutTocVisitor(this);

            return filter;
        }

        #endregion
    }
}
