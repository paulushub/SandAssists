using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ResourceContent : BuildContent<ResourceItem, ResourceContent>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public ResourceContent()
        {
        }

        public ResourceContent(ResourceContent source)
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

        public override ResourceContent Clone()
        {
            ResourceContent content = new ResourceContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
