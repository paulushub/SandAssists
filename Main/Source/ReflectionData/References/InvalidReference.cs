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
    public sealed class InvalidReference : Reference
    {
        #region Private Fields

        private string id;

        #endregion

        #region Constructors and Destructor

        internal InvalidReference()
        {
            this.id = String.Empty;
        }

        public InvalidReference(string id)
        {
            this.id = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.Invalid;
            }
        }

        public String Id
        {
            get
            {
                return (id);
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

            if (String.Equals(reader.Name, "InvalidReference", StringComparison.OrdinalIgnoreCase))
            {
                id = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "InvalidReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            id = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "InvalidReference",
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

            writer.WriteStartElement("InvalidReference");
            writer.WriteAttributeString("id", id);
            writer.WriteEndElement();
        }

        #endregion
    }
}
