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

using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData.References
{
    /// <summary>
    /// Contains the information to generate the display string for an extension method link
    /// </summary>
    [Serializable]
    public sealed class ExtensionMethodReference : Reference
    {
        #region Private Fields

        private string methodName;
        private IList<Parameter> parameters;
        private IList<TypeReference> templateArgs;

        #endregion

        #region Constructors and Destructor

        internal ExtensionMethodReference()
        {
        }

        public ExtensionMethodReference(string methodName, IList<Parameter> parameters,
            IList<TypeReference> templateArgs)
        {
            if (methodName == null)
                throw new ArgumentNullException("methodName");

            this.methodName = methodName;
            this.parameters = parameters;
            this.templateArgs = templateArgs;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ExtensionMethod;
            }
        }

        public string Name
        {
            get
            {
                return (methodName);
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return (parameters);
            }
        }

        public IList<TypeReference> TemplateArgs
        {
            get
            {
                return (templateArgs);
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

            if (String.Equals(reader.Name, "ExtensionMethodReference",
                StringComparison.OrdinalIgnoreCase))
            {
                methodName = reader.GetAttribute("methodName");
            }

            parameters = new List<Parameter>();
            templateArgs = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "ExtensionMethodReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        methodName = reader.GetAttribute("methodName");
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
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ExtensionMethodReference",
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

            writer.WriteStartElement("ExtensionMethodReference");
            if (methodName == null)
            {
                methodName = String.Empty;
            }
            writer.WriteAttributeString("methodName", methodName);

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

            writer.WriteEndElement();
        }

        #endregion
    }
}
