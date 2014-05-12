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
    public sealed class SpecializedMemberWithParametersReference : MemberReference
    {
        #region Private Fields

        private string prefix;
        private string member;
        private SpecializedTypeReference type;
        private IList<TypeReference> parameters;

        #endregion

        #region Constructors and Destructor

        internal SpecializedMemberWithParametersReference()
        {
            this.prefix = String.Empty;
            this.member = String.Empty;
        }

        public SpecializedMemberWithParametersReference(string prefix, SpecializedTypeReference type,
            string member, IList<TypeReference> parameters)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            this.prefix = prefix;
            this.type = type;
            this.member = member;
            this.parameters = parameters;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedMemberWithParameters;
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }
        }

        public SpecializedTypeReference SpecializedType
        {
            get
            {
                return type;
            }
        }

        public string MemberName
        {
            get
            {
                return member;
            }
        }

        public IList<TypeReference> ParameterTypes
        {
            get
            {
                return parameters;
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

            if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
                StringComparison.OrdinalIgnoreCase))
            {
                prefix = reader.GetAttribute("prefix");
                member = reader.GetAttribute("member");
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            parameters = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        prefix = reader.GetAttribute("prefix");
                        member = reader.GetAttribute("member");
                    }
                    else if (String.Equals(reader.Name, "SpecializedTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = new SpecializedTypeReference();
                        type.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Parameters",
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
                                        parameters.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Parameters",
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
                    if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
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

            writer.WriteStartElement("SpecializedMemberWithParametersReference");
            writer.WriteAttributeString("prefix", prefix);
            writer.WriteAttributeString("member", member);

            if (type != null)
            {
                type.WriteXml(writer);
            }
            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartElement("Parameters");
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
