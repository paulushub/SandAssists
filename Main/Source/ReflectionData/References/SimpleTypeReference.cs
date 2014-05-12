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
    public sealed class SimpleTypeReference : TypeReference
    {
        #region Private Fields

        private string typeId;

        #endregion

        #region Constructors and Destructor

        internal SimpleTypeReference()
        {
            this.typeId = String.Empty;
        }

        public SimpleTypeReference(string id)
        {
            this.typeId = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SimpleType;
            }
        }

        public string Id
        {
            get
            {
                return (typeId);
            }
        }

        #endregion

        #region Public Methods

        public Target Resolve(TargetCollection targets)
        {
            return (targets[typeId]);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "SimpleTypeReference", StringComparison.OrdinalIgnoreCase))
            {
                typeId = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "SimpleTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            typeId = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "SimpleTypeReference",
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

            writer.WriteStartElement("SimpleTypeReference");
            writer.WriteAttributeString("id", typeId);
            writer.WriteEndElement();
        }

        #endregion
    }
}
