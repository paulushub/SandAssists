using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public abstract class BuildConfig : BuildObject<BuildConfig>
    {
        #region Constructors and Destructor

        public BuildConfig()
        {
        }

        public BuildConfig(BuildConfig source)
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
    }
}
