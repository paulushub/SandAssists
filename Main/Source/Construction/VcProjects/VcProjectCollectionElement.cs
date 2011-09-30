using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public abstract class VcProjectCollectionElement : VcProjectContainerElement
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        protected VcProjectCollectionElement()
            : this(null, null)
        {
        }

        protected VcProjectCollectionElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Properties

        protected abstract string XmlTagName
        {
            get;
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {                    
            ProjectExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            string tagName = this.XmlTagName;

            if (!String.Equals(reader.Name, tagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (this.IsChildElement(reader.Name))
                    {
                        VcProjectElement element = this.CreateChildElement(reader.Name);
                        element.ReadXml(reader);

                        this.AppendChild(element);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(
                            "The element '{0}' is not a child element of {1}", reader.Name,
                            tagName));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, tagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            //if (this.IsEmpty)
            //{
            //    return;
            //}

            writer.WriteStartElement(this.XmlTagName);  // start - the collection

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteFullEndElement();                   // end - the collection
        }

        #endregion
    }
}
