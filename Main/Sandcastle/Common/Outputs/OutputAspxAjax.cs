using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Outputs
{
    [Serializable]
    public class OutputAspxAjax : OutputAspx
    {
        #region Constructors and Destructor

        public OutputAspxAjax()
        {
        }

        public OutputAspxAjax(OutputAspxAjax source)
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
            OutputAspxAjax output = new OutputAspxAjax(this);

            return output;
        }

        #endregion
    }
}
