// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public sealed class EnumerationTarget : TypeTarget
    {
        #region Private Fields

        internal IList<MemberTarget> elements;

        #endregion

        #region Constructors and Destructor

        public EnumerationTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Enumeration;
            }
        }

        public IList<MemberTarget> Elements
        {
            get
            {
                return elements;
            }
        }

        #endregion

        #region Public Methods

        public override void Add(TargetStorage storage)
        {
            base.Add(storage);

            foreach (MemberTarget element in elements)
            {
                element.Add(storage);
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "EnumerationTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            elements = new List<MemberTarget>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "TypeTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Elements",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.IndexOf("Target", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    MemberTarget target = TargetsReader.ReadMemberTarget(reader);
                                    if (target != null)
                                    {
                                        elements.Add(target);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Elements",
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
                    if (String.Equals(reader.Name, "EnumerationTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("EnumerationTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (elements != null && elements.Count != 0)
            {
                writer.WriteStartElement("Elements");

                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].WriteXml(writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "EnumerationTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "EnumerationTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "EnumerationTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "EnumerationTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }
}
