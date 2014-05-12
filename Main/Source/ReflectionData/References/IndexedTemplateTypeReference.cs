// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.References
{
    [Serializable]
    public sealed class IndexedTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private string templateId;
        private int index;

        #endregion

        #region Constructors and Destructor

        internal IndexedTemplateTypeReference()
        {
        }

        public IndexedTemplateTypeReference(string templateId, int index)
        {
            if (templateId == null)
                throw new ArgumentNullException("templateId");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            this.templateId = templateId;
            this.index = index;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.IndexedTemplate;
            }
        }

        public string TemplateId
        {
            get
            {
                return (templateId);
            }
        }

        public int Index
        {
            get
            {
                return (index);
            }
        }

        #endregion

        #region Public Methods

        public override int GetHashCode()
        {
            return (index ^ templateId.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            IndexedTemplateTypeReference other = obj as IndexedTemplateTypeReference;
            if (other == null) return (false);
            if ((this.index == other.index) && (this.templateId == other.templateId))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "IndexedTemplateTypeReference",
                StringComparison.OrdinalIgnoreCase))
            {
                templateId = reader.GetAttribute("id");
                string tempText = reader.GetAttribute("index");
                if (!String.IsNullOrEmpty(tempText))
                {
                    index = Int32.Parse(tempText);
                }
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;

                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "IndexedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            templateId = reader.GetAttribute("id");
                            string tempText = reader.GetAttribute("index");
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                index = Int32.Parse(tempText);
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "IndexedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("IndexedTemplateTypeReference");
            writer.WriteAttributeString("id", templateId);
            writer.WriteAttributeString("index", index.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }
}
