// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public sealed class Parameter : IXmlSerializable
    {
        #region Private Fields

        private string name;
        private TypeReference type;

        #endregion

        #region Constructors and Destructor

        internal Parameter()
        {
            name = String.Empty;
        }

        public Parameter(string name, TypeReference type)
        {
            this.name = name;
            this.type = type;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public TypeReference Type
        {
            get
            {
                return type;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "Parameter",
                StringComparison.OrdinalIgnoreCase))
            {
                name = reader.GetAttribute("name");
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
                    if (String.Equals(reader.Name, "Parameter",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        name = reader.GetAttribute("name");
                    }
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Parameter",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("name", name);

            if (type != null)
            {
                type.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
