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
    public sealed class ConstructorTarget : MemberTarget
    {
        #region Private Fields

        internal IList<Parameter> parameters;

        #endregion

        #region Constructors and Destructor

        public ConstructorTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Constructor;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return parameters;
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
            if (!String.Equals(reader.Name, "ConstructorTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            parameters = new List<Parameter>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Parameters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "Parameter",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    Parameter parameter = new Parameter();
                                    parameter.ReadXml(reader);
                                    parameters.Add(parameter);
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Parameters",
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
                    if (String.Equals(reader.Name, "ConstructorTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ConstructorTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartElement("Parameters");
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].WriteXml(writer);
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

            if (String.Equals(reader.Name, "ConstructorTarget",
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
                        if (String.Equals(reader.Name, "ConstructorTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "ConstructorTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "ConstructorTarget",
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
