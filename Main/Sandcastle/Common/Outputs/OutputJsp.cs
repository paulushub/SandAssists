using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Outputs
{
    [Serializable]
    public class OutputJsp : HelpOutput
    {
        #region Constructors and Destructor

        public OutputJsp()
        {
        }

        public OutputJsp(OutputJsp source)
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
                return HelpOutputType.None;
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
            OutputJsp output = new OutputJsp(this);

            return output;
        }

        #endregion
    }
}
