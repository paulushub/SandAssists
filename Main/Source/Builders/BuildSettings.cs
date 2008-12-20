using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public class BuildSettings : HelpSettings
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public BuildSettings()
        {
        }

        public BuildSettings(BuildSettings source)
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

        public override HelpSettings Clone()
        {
            BuildSettings style = new BuildSettings(this);

            return style;
        }

        #endregion
    }
}
