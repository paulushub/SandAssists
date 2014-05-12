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
    public sealed class SpecializedMemberReference : MemberReference
    {
        #region Private Fields

        private SimpleMemberReference member;
        private SpecializedTypeReference type;

        #endregion

        #region Constructors and Destructor

        internal SpecializedMemberReference()
        {
        }

        public SpecializedMemberReference(SimpleMemberReference member, SpecializedTypeReference type)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            if (type == null)
                throw new ArgumentNullException("type");

            this.member = member;
            this.type = type;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedMember;
            }
        }

        public SimpleMemberReference TemplateMember
        {
            get
            {
                return (member);
            }
        }

        public SpecializedTypeReference SpecializedType
        {
            get
            {
                return (type);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null || reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SimpleMemberReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        member = new SimpleMemberReference();
                        member.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SpecializedTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = new SpecializedTypeReference();
                        type.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "SpecializedMemberReference",
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

            writer.WriteStartElement("SpecializedMemberReference");

            if (member != null)
            {
                member.WriteXml(writer);
            }
            if (type != null)
            {
                type.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
