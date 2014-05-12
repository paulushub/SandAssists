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
    public sealed class ReferenceTypeReference : TypeReference
    {
        #region Private Fields

        private TypeReference referredToType;

        #endregion

        #region Constructors and Destructor

        internal ReferenceTypeReference()
        {
        }

        public ReferenceTypeReference(TypeReference referredToType)
        {
            if (referredToType == null)
                throw new ArgumentNullException("referedToType");

            this.referredToType = referredToType;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ReferenceType;
            }
        }

        public TypeReference ReferedToType
        {
            get
            {
                return (referredToType);
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

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        referredToType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ReferenceTypeReference",
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

            writer.WriteStartElement("ReferenceTypeReference");

            if (referredToType != null)
            {
                referredToType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
