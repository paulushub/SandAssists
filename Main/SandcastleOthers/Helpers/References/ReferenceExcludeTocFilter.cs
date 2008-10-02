using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceExcludeTocFilter : ReferenceTocFilter
    {
        #region Constructors and Destructor

        public ReferenceExcludeTocFilter()
        {
        }

        public ReferenceExcludeTocFilter(ReferenceExcludeTocFilter source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceTocFilter Clone()
        {
            ReferenceExcludeTocFilter filter = new ReferenceExcludeTocFilter(this);

            return filter;
        }

        #endregion
    }
}
