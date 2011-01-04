using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class LinkContent : BuildContent<LinkItem, LinkContent>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public LinkContent()
        {
        }

        public LinkContent(LinkContent source)
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

        public override LinkContent Clone()
        {
            LinkContent content = new LinkContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
