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

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public class TypeTarget : Target
    {
        #region Private Fields

        internal string name;
        internal IList<string> templates;
        internal SimpleTypeReference containingType;
        internal NamespaceReference containingNamespace;

        #endregion

        #region Constructors and Destructor

        public TypeTarget()
        {
            name = String.Empty;
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Type;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public NamespaceReference Namespace
        {
            get
            {
                return (containingNamespace);
            }
        }

        public SimpleTypeReference OuterType
        {
            get
            {
                return (containingType);
            }
        }

        public IList<string> Templates
        {
            get
            {
                return (templates);
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
            if (!String.Equals(reader.Name, "TypeTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            name = reader.GetAttribute("name");
            templates = new List<string>();

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
                    else if (String.Equals(reader.Name, "NamespaceReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        containingNamespace = new NamespaceReference();
                        containingNamespace.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        containingType = new SimpleTypeReference();
                        containingType.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Templates",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "Template",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    templates.Add(reader.GetAttribute("value"));
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Templates",
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
                    if (String.Equals(reader.Name, "TypeTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("TypeTarget");

            writer.WriteAttributeString("name", name);

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (containingNamespace != null)
            {
                containingNamespace.WriteXml(writer);
            }
            if (containingType != null)
            {
                containingType.WriteXml(writer);
            }
            if (templates != null && templates.Count != 0)
            {
                writer.WriteStartElement("Templates");
                for (int i = 0; i < templates.Count; i++)
                {
                    writer.WriteStartElement("Template");
                    writer.WriteAttributeString("value", templates[i]);
                    writer.WriteEndElement();
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

            if (String.Equals(reader.Name, "TypeTarget",
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
                        if (String.Equals(reader.Name, "TypeTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "TypeTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "TypeTarget",
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
