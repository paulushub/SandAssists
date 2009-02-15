using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Others;

namespace Sandcastle.Components
{
    public class ReferencePreTransComponent : PreTransComponent
    {
        #region Private Fields

        private bool            _explicitInterface;
        private XPathExpression _explicitSelector;

        #endregion

        #region Constructors and Destructor

        public ReferencePreTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            _explicitInterface = true;
            _explicitSelector = XPathExpression.Compile(
                "//element[memberdata[@visibility='private'] and proceduredata[@virtual = 'true']]");
        }

        #endregion

        #region Public Properties

        public bool FilterExplicitInterface
        {
            get
            {
                return _explicitInterface;
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            base.Apply(document, key);

            // 1. Filter out the explicit interface documentations...
            if (_explicitInterface)
            {
                ApplyExplicitInterface(document, key);
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void ApplyExplicitInterface(XmlDocument document, 
            string key)
        {
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_explicitSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                navigator.DeleteSelf();
            }
        }

        #endregion

        #region Private Methods

        private void ParseVersionInfo(XPathNavigator configuration)
        {
            string reflectionFile = String.Empty;

            if (String.IsNullOrEmpty(reflectionFile) || 
                File.Exists(reflectionFile) == false)
            {
                return;
            }

            BuilderController controller = BuilderController.Controller;
            if (controller == null)
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            //settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(reflectionFile, settings))
            {
                reader.MoveToContent();
                if (String.Equals(reader.Name, "versions"))
                {
                    XmlNodeType nodeType = XmlNodeType.None;
                    string nodeName = String.Empty;
                    while (reader.Read())
                    {
                        nodeType = reader.NodeType;
                        if (nodeType == XmlNodeType.Element &&
                            String.Equals(reader.Name, "version"))
                        {
                            string asmName     = reader.GetAttribute("assemblyName");
                            string asmVersion  = reader.GetAttribute("assemblyVersion");
                            string fileVersion = reader.GetAttribute("fileVersion");
                            if (!String.IsNullOrEmpty(asmName))
                            {
                                controller.AddVersion(new VersionInfo(asmName,
                                    asmVersion, fileVersion));
                            }
                        }
                        else if (nodeType == XmlNodeType.EndElement)
                        {
                            if (String.Equals(reader.Name, "versions"))
                            {
                                break;
                            }
                        }
                    }
                }
                else if (String.Equals(reader.Name, "reflection"))
                {
                    ParseAssemblies(reader, controller);
                }
            }
        }

        private void ParseAssemblies(XmlReader reader, BuilderController controller)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = String.Empty;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "assembly"))
                    {
                        string asmName = reader.GetAttribute("name");
                        string asmVersion = String.Empty;
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                nodeName = reader.Name;
                                if (String.Equals(nodeName, "assemblydata"))
                                {
                                    asmVersion = reader.GetAttribute("version");
                                    reader.Skip();
                                }
                                else if (String.Equals(nodeName, "attributes"))
                                {
                                    string fileVersion = String.Empty;
                                    while (reader.Read())
                                    {
                                        nodeType = reader.NodeType;
                                        if (nodeType == XmlNodeType.Element &&
                                            String.Equals(reader.Name, "attribute"))
                                        {
                                            if (reader.ReadToDescendant("type") && String.Equals(
                                                reader.GetAttribute("api"), "T:System.Reflection.AssemblyFileVersionAttribute"))
                                            {
                                                if (reader.ReadToNextSibling("argument") &&
                                                    reader.ReadToDescendant("value"))
                                                {
                                                    fileVersion = reader.ReadString();
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                reader.Skip();
                                            }
                                        }
                                        else if (nodeType == XmlNodeType.EndElement)
                                        {
                                            if (String.Equals(reader.Name, "attributes"))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    controller.AddVersion(new VersionInfo(asmName,
                                        asmVersion, fileVersion));

                                    break;
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                nodeName = reader.Name;
                                if (String.Equals(nodeName, "assembly") ||
                                    String.Equals(nodeName, "assemblies") ||
                                    String.Equals(nodeName, "apis"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "apis"))
                    {
                        break;
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "assemblies") ||
                        String.Equals(nodeName, "apis"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
