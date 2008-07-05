using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Templates
{
    [Serializable]
    public class TemplateErrorMessages : TemplateObject
    {
        #region Constructors and Destructor

        public TemplateErrorMessages()
        {
        }

        public TemplateErrorMessages(TemplateErrorMessages source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return null;
            }
        }

        public override string Description
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public override void Write()
        {
        }

        public override void Reset()
        {
            base.Reset();
        }

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

        public override TemplateObject Clone()
        {
            TemplateErrorMessages template = new TemplateErrorMessages(this);

            return template;
        }

        #endregion
    }
}
