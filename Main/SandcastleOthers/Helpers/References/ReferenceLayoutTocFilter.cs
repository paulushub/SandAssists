using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceLayoutTocFilter : ReferenceTocFilter
    {
        #region Constructors and Destructor

        public ReferenceLayoutTocFilter()
        {
        }

        public ReferenceLayoutTocFilter(ReferenceLayoutTocFilter source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceTocFilter Clone()
        {
            ReferenceLayoutTocFilter filter = new ReferenceLayoutTocFilter(this);

            return filter;
        }

        #endregion
    }
}
