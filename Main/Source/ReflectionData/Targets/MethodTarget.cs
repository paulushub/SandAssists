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
    public sealed class MethodTarget : ProcedureTarget
    {
        #region Private Fields

        internal IList<Parameter> parameters;
        internal TypeReference returnType;
        internal IList<string> templates;

        // property to hold specialized template arguments (used with extension methods)
        internal IList<TypeReference> templateArgs;

        #endregion

        #region Constructors and Destructor

        public MethodTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Method;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public TypeReference ReturnType
        {
            get
            {
                return returnType;
            }
        }

        public IList<string> Templates
        {
            get
            {
                return templates;
            }
        }

        public IList<TypeReference> TemplateArgs
        {
            get
            {
                return templateArgs;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "MethodTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            parameters = new List<Parameter>();
            templates = new List<string>();

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
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        returnType = ReferencesReader.ReadTypeReference(reader);
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
                    else if (String.Equals(reader.Name, "TemplateArgs",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.EndsWith("TypeReference",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    TypeReference typeRef = ReferencesReader.ReadTypeReference(reader);
                                    if (typeRef != null)
                                    {
                                        if (templateArgs == null)
                                        {
                                            templateArgs = new List<TypeReference>();
                                        }
                                        templateArgs.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "TemplateArgs",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
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
                    if (String.Equals(reader.Name, "MethodTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MethodTarget");

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
            if (templateArgs != null && templateArgs.Count != 0)
            {
                writer.WriteStartElement("TemplateArgs");

                for (int i = 0; i < templateArgs.Count; i++)
                {
                    templateArgs[i].WriteXml(writer);
                }

                writer.WriteEndElement();
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
            if (returnType != null)
            {
                returnType.WriteXml(writer);
            }

            writer.WriteEndElement();
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

            if (String.Equals(reader.Name, "MethodTarget",
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
                        if (String.Equals(reader.Name, "MethodTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "MethodTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "MethodTarget",
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
