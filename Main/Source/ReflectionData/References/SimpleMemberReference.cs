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

using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData.References
{
    [Serializable]
    public sealed class SimpleMemberReference : MemberReference
    {
        #region Private Fields

        private string memberId;

        #endregion

        #region Constructors and Destructor

        internal SimpleMemberReference()
        {
            this.memberId = String.Empty;
        }

        public SimpleMemberReference(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.memberId = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SimpleMember;
            }
        }

        public string Id
        {
            get
            {
                return (memberId);
            }
        }

        public Target Resolve(TargetCollection targets)
        {
            return (targets[memberId]);
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

            if (String.Equals(reader.Name, "SimpleMemberReference", StringComparison.OrdinalIgnoreCase))
            {
                memberId = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "SimpleMemberReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            memberId = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "SimpleMemberReference",
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

            writer.WriteStartElement("SimpleMemberReference");
            writer.WriteAttributeString("id", memberId);
            writer.WriteEndElement();
        }

        #endregion
    }
}
