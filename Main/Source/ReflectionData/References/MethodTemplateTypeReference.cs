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
    public sealed class MethodTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private int position;
        private MemberReference template;

        #endregion

        #region Constructors and Destructor

        internal MethodTemplateTypeReference()
        {
        }

        public MethodTemplateTypeReference(MemberReference template, int position)
        {
            this.template = template;
            this.position = position;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.MethodTemplate;
            }
        }

        public MemberReference TemplateMethod
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

            if (String.Equals(reader.Name, "MethodTemplateTypeReference",
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
                    if (String.Equals(reader.Name, "MethodTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("position");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            position = Int32.Parse(tempText);
                        }
                    }
                    else if (reader.Name.IndexOf("Member", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        template = ReferencesReader.ReadMemberReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "MethodTemplateTypeReference",
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

            writer.WriteStartElement("MethodTemplateTypeReference");
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
