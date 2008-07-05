using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public class ReferenceContext : ReferenceObject<ReferenceContext>
    {
        #region Constructors and Destructor

        public ReferenceContext()
        {
        }

        public ReferenceContext(ReferenceContext source)
            : base(source)
        {
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override ReferenceContext Clone()
        {
            ReferenceContext style = new ReferenceContext(this);

            return style;
        }

        #endregion
    }
}
