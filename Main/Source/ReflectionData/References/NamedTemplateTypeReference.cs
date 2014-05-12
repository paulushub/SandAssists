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
    public sealed class NamedTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private string name;

        #endregion

        #region Constructors and Destructor

        internal NamedTemplateTypeReference()
        {
            this.name = String.Empty;
        }

        public NamedTemplateTypeReference(string name)
        {
            this.name = name;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.NamedTemplate;
            }
        }

        public string Name
        {
            get
            {
                return (name);
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

            if (String.Equals(reader.Name, "NamedTemplateTypeReference", StringComparison.OrdinalIgnoreCase))
            {
                name = reader.GetAttribute("name");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "NamedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            name = reader.GetAttribute("name");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "NamedTemplateTypeReference",
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

            writer.WriteStartElement("NamedTemplateTypeReference");
            writer.WriteAttributeString("name", name);
            writer.WriteEndElement();
        }

        #endregion
    }
}
