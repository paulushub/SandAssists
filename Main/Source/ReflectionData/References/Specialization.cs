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
    public sealed class Specialization : IXmlSerializable
    {
        #region Private Fields

        private SimpleTypeReference template;
        public IList<TypeReference> arguments;

        #endregion

        #region Constructors and Destructor

        internal Specialization()
        {
        }

        public Specialization(SimpleTypeReference template, IList<TypeReference> arguments)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            this.template  = template;
            this.arguments = arguments;
        }

        #endregion

        #region Public Properties

        public SimpleTypeReference TemplateType
        {
            get
            {
                return template;
            }
        }

        public IList<TypeReference> Arguments
        {
            get
            {
                return arguments;
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
            if (reader == null || reader.IsEmptyElement)
            {
                return;
            }

            arguments = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        template = new SimpleTypeReference();
                        template.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Arguments",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.EndsWith("TypeReference",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    TypeReference typeRef = ReferencesReader.ReadTypeReference(reader);
                                    if (typeRef != null)
                                    {
                                        arguments.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Arguments",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Specialization",
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

            writer.WriteStartElement("Specialization");
            if (template != null)
            {
                template.WriteXml(writer);
            }
            if (arguments != null && arguments.Count != 0)
            {
                writer.WriteStartElement("Arguments");
                for (int i = 0; i < arguments.Count; i++)
                {
                    arguments[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
