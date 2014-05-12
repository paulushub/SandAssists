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
using System.Text.RegularExpressions;

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public class Target : IXmlSerializable
    {
        #region Private Fields

        internal string    id;
        internal string    container;
        internal string    file;

        internal ReferenceLinkType type;

        #endregion

        #region Constructors and Destructor

        public Target()
        {
            id        = String.Empty;
            container = String.Empty;
            file      = String.Empty;
            type      = ReferenceLinkType.None;
       }

        #endregion

        #region Public Properties

        public virtual TargetType TargetType
        {
            get
            {
                return TargetType.None;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public string Container
        {
            get
            {
                return container;
            }
        }

        public string File
        {
            get
            {
                return file;
            }
        }

        public ReferenceLinkType LinkType
        {
            get
            {
                return type;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Add(TargetStorage targets)
        {
            targets.Add(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnReadXml(XmlReader reader)
        {
            // This reads only the target node...
            if (!String.Equals(reader.Name, "Target",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            id        = reader.GetAttribute("id");
            container = reader.GetAttribute("container");
            file      = reader.GetAttribute("file");

            string typeText = reader.GetAttribute("type");
            if (!String.IsNullOrEmpty(typeText))
            {
                switch (typeText.ToLower())
                {
                    case "none":
                        type = ReferenceLinkType.None;
                        break;
                    case "self":
                        type = ReferenceLinkType.Self;
                        break;
                    case "local":
                        type = ReferenceLinkType.Local;
                        break;
                    case "index":
                        type = ReferenceLinkType.Index;
                        break;
                    case "localorindex":
                        type = ReferenceLinkType.LocalOrIndex;
                        break;
                    case "msdn":
                        type = ReferenceLinkType.Msdn;
                        break;
                    case "id":
                        type = ReferenceLinkType.Id;
                        break;
               }
            }
        }

        protected virtual void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Target");

            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("container", container);
            writer.WriteAttributeString("file", file);
            writer.WriteAttributeString("type", type.ToString());
            
            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "Target", 
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
                        if (String.Equals(reader.Name, "Target",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Target",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "Target",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
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
