using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceExcludeTocVisitor : ReferenceTocVisitor
    {
        #region Constructors and Destructor

        public ReferenceExcludeTocVisitor()
        {
        }

        public ReferenceExcludeTocVisitor(ReferenceExcludeTocVisitor source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceTocVisitor Clone()
        {
            ReferenceExcludeTocVisitor filter = new ReferenceExcludeTocVisitor(this);

            return filter;
        }

        #endregion
    }
}
