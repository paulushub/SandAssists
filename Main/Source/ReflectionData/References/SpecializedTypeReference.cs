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
    public sealed class SpecializedTypeReference : TypeReference
    {
        #region Private Fields

        private IList<Specialization> specializations;

        #endregion

        #region Constructors and Destructor

        internal SpecializedTypeReference()
        {
        }

        public SpecializedTypeReference(IList<Specialization> specializations)
        {
            if (specializations == null)
                throw new ArgumentNullException("specializations");

            this.specializations = specializations;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedType;
            }
        }

        public IList<Specialization> Specializations
        {
            get
            {
                return (specializations);
            }
        }

        #endregion

        #region Public Methods

        public IDictionary<IndexedTemplateTypeReference, TypeReference> GetSpecializationDictionary()
        {
            Dictionary<IndexedTemplateTypeReference, TypeReference> dictionary =
                new Dictionary<IndexedTemplateTypeReference, TypeReference>();
            foreach (Specialization specialization in specializations)
            {
                IList<TypeReference> arguments = specialization.arguments;
                for (int index = 0; index < arguments.Count; index++)
                {
                    IndexedTemplateTypeReference template =
                        new IndexedTemplateTypeReference(specialization.TemplateType.Id, index);
                    dictionary.Add(template, arguments[index]);
                }
            }

            return (dictionary);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            specializations = new List<Specialization>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "Specialization",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Specialization item = new Specialization();
                        item.ReadXml(reader);

                        specializations.Add(item);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "SpecializedTypeReference",
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

            writer.WriteStartElement("SpecializedTypeReference");
            if (specializations != null && specializations.Count != 0)
            {
                for (int i = 0; i < specializations.Count; i++)
                {
                    specializations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
