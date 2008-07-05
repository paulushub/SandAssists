using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Conceptual
{
    [Serializable]
    public class ConceptualConfig : BuildConfig
    {
        #region Constructors and Destructor

        public ConceptualConfig()
        {   
        }

        public ConceptualConfig(ConceptualConfig source)
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

        public override BuildConfig Clone()
        {
            ConceptualConfig config = new ConceptualConfig(this);

            return config;
        }

        #endregion
    }
}
