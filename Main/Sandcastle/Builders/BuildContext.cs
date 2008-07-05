using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public abstract class BuildContext : BuildObject<BuildContext>
    {
        #region Constructors and Destructor

        protected BuildContext()
        {
        }

        protected BuildContext(BuildContext source)
            : base(source)
        {
        }

        #endregion

        #region ICloneable Members

        public override BuildContext Clone()
        {
            return null;
        }

        #endregion
    }
}
