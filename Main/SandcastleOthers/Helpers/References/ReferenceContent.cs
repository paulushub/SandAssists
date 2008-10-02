using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceContent : BuildContent<ReferenceItem, ReferenceContent>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public ReferenceContent()
        {
        }

        public ReferenceContent(ReferenceContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

        public override ReferenceContent Clone()
        {
            ReferenceContent content = new ReferenceContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
