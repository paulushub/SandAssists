using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceApiFilter : ReferenceFilter
    {
        #region Constructors and Destructor

        public ReferenceApiFilter()
        {   
        }

        public ReferenceApiFilter(ReferenceApiFilter source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceApiFilter filter = new ReferenceApiFilter(this);

            return filter;
        }

        #endregion
    }
}
