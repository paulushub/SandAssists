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
    public sealed class PointerTypeReference : TypeReference
    {
        #region Private Fields

        private TypeReference pointedToType;

        #endregion

        #region Constructors and Destructor

        internal PointerTypeReference()
        {
        }

        public PointerTypeReference(TypeReference pointedToType)
        {
            if (pointedToType == null)
                throw new ArgumentNullException("pointedToType");

            this.pointedToType = pointedToType;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.PointerType;
            }
        }

        public TypeReference PointedToType
        {
            get
            {
                return (pointedToType);
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
                        pointedToType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "PointerTypeReference",
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

            writer.WriteStartElement("PointerTypeReference");

            if (pointedToType != null)
            {
                pointedToType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
