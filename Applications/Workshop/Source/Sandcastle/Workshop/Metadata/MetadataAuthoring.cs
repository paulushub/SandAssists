using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Workshop.Metadata
{
    public sealed class MetadataAuthoring : ICloneable, IXmlSerializable
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public MetadataAuthoring()
        {   
        }

        public MetadataAuthoring(MetadataAuthoring source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader",
                    "The reader parameter is required and cannot be null (or Nothing).");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer",
                    "The writer parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public MetadataAuthoring Clone()
        {
            MetadataAuthoring content = new MetadataAuthoring(this);

            return content;
        }

        #endregion
    }
}
