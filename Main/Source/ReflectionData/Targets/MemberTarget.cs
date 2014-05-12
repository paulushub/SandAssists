// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public class MemberTarget : Target
    {
        #region Private Fields

        internal string name;
        internal string overload;
        internal SimpleTypeReference containingType;

        #endregion

        #region Constructors and Destructor

        public MemberTarget()
        {
            name = String.Empty;
            overload = String.Empty;
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Member;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public TypeReference Type
        {
            get
            {
                return containingType;
            }
        }

        public string OverloadId
        {
            get
            {
                return overload;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "MemberTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            name = reader.GetAttribute("name");
            overload = reader.GetAttribute("overload");

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "Target",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        containingType = new SimpleTypeReference();
                        containingType.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MemberTarget");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("overload", overload);

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (containingType != null)
            {
                containingType.WriteXml(writer);
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

            if (String.Equals(reader.Name, "MemberTarget",
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
                        if (String.Equals(reader.Name, "MemberTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "MemberTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "MemberTarget",
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
