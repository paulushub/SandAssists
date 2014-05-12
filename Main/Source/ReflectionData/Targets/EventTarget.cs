// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public sealed class EventTarget : ProcedureTarget
    {
        #region Constructors and Destructor

        public EventTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Event;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "EventTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "EventTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("EventTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

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

            if (String.Equals(reader.Name, "EventTarget",
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
                        if (String.Equals(reader.Name, "EventTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "EventTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "EventTarget",
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
