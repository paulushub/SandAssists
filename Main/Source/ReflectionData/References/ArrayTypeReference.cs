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
    public sealed class ArrayTypeReference : TypeReference
    {
        #region Private Fields

        private int rank;
        private TypeReference elementType;

        #endregion

        #region Constructors and Destructor

        internal ArrayTypeReference()
        {
        }

        public ArrayTypeReference(TypeReference elementType, int rank)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (rank <= 0)
                throw new ArgumentOutOfRangeException("rank");

            this.elementType = elementType;
            this.rank = rank;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ArrayType;
            }
        }

        public int Rank
        {
            get
            {
                return (rank);
            }
        }

        public TypeReference ElementType
        {
            get
            {
                return (elementType);
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

            bool readContent = false;
            if (String.Equals(reader.Name, "ArrayTypeReference",
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("rank");
                if (!String.IsNullOrEmpty(tempText))
                {
                    rank = Int32.Parse(tempText);
                }

                readContent = true;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "ArrayTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (readContent)
                        {
                            // It must be a nested ArrayTypeReference type...
                            elementType = ReferencesReader.ReadTypeReference(reader);
                        }
                        else
                        {
                            string tempText = reader.GetAttribute("rank");
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                rank = Int32.Parse(tempText);
                            }
                            readContent = true;
                        }
                    }
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        elementType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ArrayTypeReference",
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

            writer.WriteStartElement("ArrayTypeReference");
            writer.WriteAttributeString("rank", rank.ToString());

            if (elementType != null)
            {
                elementType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
