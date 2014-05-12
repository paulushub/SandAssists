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
    public sealed class TypeTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private int position;
        private SimpleTypeReference template;

        #endregion

        #region Constructors and Destructor

        internal TypeTemplateTypeReference()
        {
        }

        public TypeTemplateTypeReference(SimpleTypeReference template, int position)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (position < 0)
                throw new ArgumentOutOfRangeException("position");

            this.template = template;
            this.position = position;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.TypeTemplate;
            }
        }

        public SimpleTypeReference TemplateType
        {
            get
            {
                return (template);
            }
        }

        public int Position
        {
            get
            {
                return (position);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "TypeTemplateTypeReference",
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("position");
                if (!String.IsNullOrEmpty(tempText))
                {
                    position = Int32.Parse(tempText);
                }
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "TypeTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("position");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            position = Int32.Parse(tempText);
                        }
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        template = new SimpleTypeReference();
                        template.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "TypeTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
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

            writer.WriteStartElement("TypeTemplateTypeReference");
            writer.WriteAttributeString("position", position.ToString());

            if (template != null)
            {
                template.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
