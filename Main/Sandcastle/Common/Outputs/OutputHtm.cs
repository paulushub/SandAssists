using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Outputs
{
    [Serializable]
    public class OutputHtm : HelpOutput
    {
        #region Constructors and Destructor

        public OutputHtm()
        {
        }

        public OutputHtm(OutputHtm source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override HelpCommand Command
        {
            get
            {
                return null;
            }
        }

        public override HelpOutputType OutputType
        {
            get
            {
                return HelpOutputType.HtmHelp;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        #endregion

        #region ICloneable Members

        public override HelpOutput Clone()
        {
            OutputHtm output = new OutputHtm(this);

            return output;
        }

        #endregion
    }
}
