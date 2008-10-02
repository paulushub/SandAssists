using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceAttributeFilter : ReferenceFilter
    {
        #region Constructors and Destructor

        public ReferenceAttributeFilter()
        {
        }

        public ReferenceAttributeFilter(ReferenceAttributeFilter source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceAttributeFilter filter = new ReferenceAttributeFilter(this);

            return filter;
        }

        #endregion
    }
}
