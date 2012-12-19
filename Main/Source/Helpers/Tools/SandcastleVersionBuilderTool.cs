using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Tools
{
    public sealed class SandcastleVersionBuilderTool : SandcastleTool
    {
        #region Private Fields

        private bool   _ripOldApis;
        private string _outputFile;
        private string _configurationFile;

        private static XPathExpression _apiIdSelector    = 
            XPathExpression.Compile("string(/api/@id)");
        private static XPathExpression _assemblySelector = 
            XPathExpression.Compile(
            "string(/api/containers/library/@assembly)");
        private static XPathExpression _elementSelectorEx  = 
            XPathExpression.Compile(
            "/api/elements/element");
        private static XPathExpression _elementSelector  = 
            XPathExpression.Compile(
            "elements/element");
        private static XPathExpression _obsoleteSelector = 
            XPathExpression.Compile(
            "/api/attributes/attribute[type[@api='T:System.ObsoleteAttribute']]");
        private static XPathExpression _boolArgsSelector = 
            XPathExpression.Compile(
            "boolean(argument[type[@api='T:System.Boolean'] and value[.='True']])");

        private static XPathExpression _versionsSelector = 
            XPathExpression.Compile("string(ancestor::versions/@name)");

        #endregion

        #region Constructors and Destructor

        public SandcastleVersionBuilderTool()
        {
            _ripOldApis = true;
        }

        #endregion

        #region Public Properties

        public bool RipOldApis
        {
            get
            {
                return _ripOldApis;
            }
            set
            {
                _ripOldApis = value;
            }
        }

        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            set
            {
                _outputFile = value;
            }
        }

        public string ConfigurationFile
        {
            get
            {
                return _configurationFile;
            }
            set
            {
                _configurationFile = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnRun(BuildLogger logger)
        {
            bool isSuccessful = false;
                                   
            if (String.IsNullOrEmpty(_configurationFile))
            {
                logger.WriteLine("You must specify a version catalog file.",
                    BuildLoggerLevel.Error);

                return isSuccessful;
            }
            if (String.IsNullOrEmpty(_outputFile))
            {
                logger.WriteLine("An output file is not specified. The result will be displaced on console.",
                    BuildLoggerLevel.Warn);
            }
            _configurationFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(_configurationFile));

            XPathDocument xPathDocument   = null;
            XPathNavigator xPathNavigator = null;
            try
            {
                xPathDocument  = new XPathDocument(_configurationFile);
                xPathNavigator = xPathDocument.CreateNavigator();
            }
            catch (IOException ex)
            {                         
                logger.WriteLine(String.Format(
                    "An error occurred while accessing the version catalog file '{0}'. The error message is: {1}",
                    _configurationFile, ex.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }
            catch (XmlException ex)
            {
                logger.WriteLine(String.Format(
                    "The version catalog file '{0}' is not well-formed. The error message is: {1}",
                    _configurationFile, ex.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }

            XPathNavigator versionsNode = xPathNavigator.SelectSingleNode("versions");
            List<string> latestVersions = new List<string>();
            List<VersionInfo> listVersions = new List<VersionInfo>();
            XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("versions//version[@file]");

            foreach (XPathNavigator navigator in xPathNodeIterator)
            {
                string group     = (string)navigator.Evaluate(_versionsSelector);
                string versionId = navigator.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(versionId))
                {
                    logger.WriteLine("Every version element must have a name attribute.",
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                string versionFile = navigator.GetAttribute("file", String.Empty);
                if (String.IsNullOrEmpty(versionId))
                {
                    logger.WriteLine("Every version element must have a file attribute.",
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }

                versionFile = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(versionFile));
                VersionInfo item = new VersionInfo(versionId, group, versionFile);
                listVersions.Add(item);
            }    
         
            string b = String.Empty;
            foreach (VersionInfo version in listVersions)
            {
                if (version.Group != b)
                {
                    latestVersions.Add(version.Name);
                    b = version.Group;
                }
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreWhitespace = true;
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;

            Dictionary<string, List<KeyValuePair<string, string>>> versionIndex = 
                new Dictionary<string, List<KeyValuePair<string, string>>>();
            Dictionary<string, Dictionary<string, ElementInfo>> versionElements = 
                new Dictionary<string, Dictionary<string, ElementInfo>>();
            
            foreach (VersionInfo versionInfo in listVersions)
            {
                logger.WriteLine(String.Format(
                    "Indexing version '{0}' using file '{1}'.",
                    versionInfo.Name, versionInfo.File), BuildLoggerLevel.Info);

                XmlReader apiReader = null;
                try
                {
                    apiReader = XmlReader.Create(versionInfo.File, xmlReaderSettings);
                    apiReader.MoveToContent();

                    while (apiReader.Read())
                    {
                        if (apiReader.NodeType == XmlNodeType.Element && 
                            apiReader.LocalName == "api")
                        {
                            string key = String.Empty;
                            List<KeyValuePair<string, string>> versionValue = null;
                            string value = String.Empty;
                            Dictionary<string, ElementInfo> dictElements = null;
                            XmlReader nodeReader = apiReader.ReadSubtree();
                            XPathDocument nodeDocument = new XPathDocument(nodeReader);
                            XPathNavigator nodeNavigator = nodeDocument.CreateNavigator();
                            key = (string)nodeNavigator.Evaluate(_apiIdSelector);
                            string assembly = (string)nodeNavigator.Evaluate(_assemblySelector);
                            if (!versionIndex.TryGetValue(key, out versionValue))
                            {
                                versionValue = new List<KeyValuePair<string, string>>();
                                versionIndex.Add(key, versionValue);
                            }
                            if (!versionElements.TryGetValue(key, out dictElements))
                            {
                                dictElements = new Dictionary<string, ElementInfo>();
                                versionElements.Add(key, dictElements);
                            }

                            XPathNodeIterator nodeIterator = 
                                nodeNavigator.Select(_elementSelectorEx);
                            foreach (XPathNavigator navigator in nodeIterator)
                            {
                                string elementName = navigator.GetAttribute("api", String.Empty);
                                ElementInfo elementInfo;
                                if (!dictElements.TryGetValue(elementName, out elementInfo))
                                {
                                    XPathNavigator elementNode = null;
                                    if (navigator.SelectSingleNode("*") != null ||
                                        navigator.SelectChildren(XPathNodeType.Attribute).Count > 1)
                                    {
                                        elementNode = navigator;
                                    }
                                    elementInfo = new ElementInfo(versionInfo.Group, 
                                        versionInfo.Name, elementNode);
                                    dictElements.Add(elementName, elementInfo);
                                }
                                else
                                {
                                    if (!elementInfo.Versions.ContainsKey(versionInfo.Group))
                                    {
                                        elementInfo.Versions.Add(
                                            versionInfo.Group, versionInfo.Name);
                                    }
                                }
                            }
                            XPathNavigator obsoleteNode = 
                                nodeNavigator.SelectSingleNode(_obsoleteSelector);
                            if (obsoleteNode != null)
                            {
                                value = (((bool)obsoleteNode.Evaluate(_boolArgsSelector)) 
                                    ? "error" : "warning");
                            }
                            versionValue.Add(
                                new KeyValuePair<string, string>(versionInfo.Name, value));
                            value = String.Empty;
                            nodeReader.Close();
                        }
                    }
                }
                catch (IOException ex)
                {
                    logger.WriteLine(String.Format(
                        "An error occurred while accessing the input file '{0}'. The error message is: {1}",
                        versionInfo.File, ex.Message), BuildLoggerLevel.Error);
                    
                    return isSuccessful;
                }
                catch (XmlException ex)
                {
                    logger.WriteLine(String.Format(
                        "The input file '{0}' is not well-formed. The error message is: {1}",
                        versionInfo.File, ex.Message), BuildLoggerLevel.Error);
                    
                    return isSuccessful;
                }
                finally
                {
                    apiReader.Close();
                }
            }

            if (_ripOldApis)
            {
                RemoveOldApis(versionIndex, latestVersions);
            }
            
            logger.WriteLine(String.Format(
                "Indexed {0} entities in {1} versions.",
                versionIndex.Count, listVersions.Count), BuildLoggerLevel.Info);

            XmlWriter apiWriter = null;
            try
            {
                if (!String.IsNullOrEmpty(_outputFile))
                {
                    apiWriter = XmlWriter.Create(_outputFile, xmlWriterSettings);
                }
                else
                {
                    apiWriter = XmlWriter.Create(Console.Out, xmlWriterSettings);
                }

                apiWriter.WriteStartDocument();
                apiWriter.WriteStartElement("reflection");
                apiWriter.WriteStartElement("assemblies");
                Dictionary<string, object> dictionary4 = 
                    new Dictionary<string, object>();
                foreach (VersionInfo versionInfo in listVersions)
                {
                    XmlReader apiReader = XmlReader.Create(
                        versionInfo.File, xmlReaderSettings);
                    apiReader.MoveToContent();
                    while (apiReader.Read())
                    {
                        if (apiReader.NodeType == XmlNodeType.Element && 
                            apiReader.LocalName == "assembly")
                        {
                            string attribute3 = apiReader.GetAttribute("name");
                            if (!dictionary4.ContainsKey(attribute3))
                            {
                                XmlReader nodeReader = apiReader.ReadSubtree();
                                apiWriter.WriteNode(nodeReader, false);
                                nodeReader.Close();
                                dictionary4.Add(attribute3, null);
                            }
                        }
                    }
                }
                apiWriter.WriteEndElement();
                apiWriter.WriteStartElement("apis");

                foreach (VersionInfo versionInfo in listVersions)
                {
                    XmlReader apiReader = XmlReader.Create(
                        versionInfo.File, xmlReaderSettings);
                    apiReader.MoveToContent();
                    while (apiReader.Read())
                    {
                        if (apiReader.NodeType == XmlNodeType.Element && 
                            apiReader.LocalName == "api")
                        {
                            string apiId = apiReader.GetAttribute("id");
                            if (versionIndex.ContainsKey(apiId))
                            {
                                List<KeyValuePair<string, string>> list4 = versionIndex[apiId];
                                if (!(versionInfo.Name != list4[0].Key))
                                {
                                    apiWriter.WriteStartElement("api");
                                    apiWriter.WriteAttributeString("id", apiId);
                                    
                                    XmlReader nodeReader = apiReader.ReadSubtree();
                                    nodeReader.MoveToContent();
                                    nodeReader.ReadStartElement();

                                    this.WriteElements(apiId, apiWriter, nodeReader, 
                                        latestVersions, versionElements);
                                    nodeReader.Close();

                                    apiWriter.WriteStartElement("versions");

                                    foreach (XPathNavigator branch in
                                        versionsNode.SelectChildren(XPathNodeType.Element))
                                    {
                                        WriteVersionTree(list4, branch, apiWriter);
                                    }

                                    apiWriter.WriteEndElement();
                                    apiWriter.WriteEndElement();
                                }
                            }
                        }
                    }
                    apiReader.Close();
                }
                apiWriter.WriteEndElement();
                apiWriter.WriteEndElement();
                apiWriter.WriteEndDocument();

                isSuccessful = true;
            }
            catch (IOException ex)
            {
                logger.WriteLine(ex, BuildLoggerLevel.Error);

                return isSuccessful;
            }
            finally
            {
                apiWriter.Close();
            }

            return isSuccessful;
        }

        #endregion

        #region Private Methods

        private void WriteElements(string apiId, XmlWriter apiWriter, 
            XmlReader nodeReader, List<string> latestVersions,
            Dictionary<string, Dictionary<string, ElementInfo>> versionElements)
        {
            while (!nodeReader.EOF)
            {
                if (nodeReader.NodeType == XmlNodeType.Element && 
                    nodeReader.LocalName == "elements")
                {
                    Dictionary<string, ElementInfo> dictionary5 = 
                        versionElements[apiId];
                    Dictionary<string, object> elementIdSet = 
                        new Dictionary<string, object>();
                    apiWriter.WriteStartElement("elements");
                    XmlReader elementNodeReader = nodeReader.ReadSubtree();
                    XPathNavigator elementNodeNavigator = 
                        new XPathDocument(elementNodeReader).CreateNavigator();
                    foreach (XPathNavigator elementNode in
                        elementNodeNavigator.Select(_elementSelector))
                    {
                        string elementId = elementNode.GetAttribute("api", String.Empty);
                        elementIdSet[elementId] = null;
                        apiWriter.WriteStartElement("element");
                        apiWriter.WriteAttributeString("api", elementId);
                        foreach (string current5 in dictionary5[elementId].Versions.Keys)
                        {
                            apiWriter.WriteAttributeString(current5, 
                                dictionary5[elementId].Versions[current5]);
                        }

                        foreach (XPathNavigator navigator in elementNode.Select("@*"))
                        {
                            if (navigator.LocalName != "api")
                            {
                                apiWriter.WriteAttributeString(
                                    navigator.LocalName, navigator.Value);
                            }
                        }

                        foreach (XPathNavigator navigator in elementNode.Select("*"))
                        {
                            apiWriter.WriteNode(navigator, false);
                        }

                        apiWriter.WriteEndElement();
                    }

                    elementNodeReader.Close();

                    if (elementIdSet.Count != dictionary5.Count)
                    {
                        foreach (string apiName in dictionary5.Keys)
                        {
                            if (!elementIdSet.ContainsKey(apiName) && 
                                (!_ripOldApis || IsLatestElement(
                                dictionary5[apiName].Versions.Values, latestVersions)))
                            {
                                apiWriter.WriteStartElement("element");
                                apiWriter.WriteAttributeString("api", apiName);
                                foreach (string current7 in dictionary5[apiName].Versions.Keys)
                                {
                                    apiWriter.WriteAttributeString(current7, 
                                        dictionary5[apiName].Versions[current7]);
                                }
                                if (dictionary5[apiName].ElementNode != null)
                                {
                                    foreach (XPathNavigator navigator in
                                        dictionary5[apiName].ElementNode.Select("@*"))
                                    {
                                        if (navigator.LocalName != "api")
                                        {
                                            apiWriter.WriteAttributeString(
                                                navigator.LocalName, navigator.Value);
                                        }
                                    }

                                    foreach (XPathNavigator navigator in
                                        dictionary5[apiName].ElementNode.Select("*"))
                                    {
                                        apiWriter.WriteNode(navigator, false);
                                    }
                                }
                                apiWriter.WriteEndElement();
                            }
                        }
                    }
                    apiWriter.WriteEndElement();
                    nodeReader.Read();
                }
                else
                {
                    if (nodeReader.NodeType == XmlNodeType.Element)
                    {
                        apiWriter.WriteNode(nodeReader, false);
                    }
                    else
                    {
                        nodeReader.Read();
                    }
                }
            }
        }

        private static void WriteVersionTree(
            List<KeyValuePair<string, string>> versions, 
            XPathNavigator branch, XmlWriter writer)
        {
            string localName = branch.LocalName;
            string attribute = branch.GetAttribute("name", String.Empty);
            if (localName.Equals("versions", StringComparison.OrdinalIgnoreCase))
            {
                writer.WriteStartElement("versions");
                if (!String.IsNullOrEmpty(attribute))
                {
                    writer.WriteAttributeString("name", attribute);
                }
                XPathNodeIterator iterator = branch.SelectChildren(XPathNodeType.Element);
                foreach (XPathNavigator navigator in iterator)
                {
                    WriteVersionTree(versions, navigator, writer);
                }
                writer.WriteEndElement();
                return;
            }
            else if (localName.Equals("version", StringComparison.OrdinalIgnoreCase))
            {
                foreach (KeyValuePair<string, string> current in versions)
                {
                    if (current.Key == attribute)
                    {
                        writer.WriteStartElement("version");
                        writer.WriteAttributeString("name", attribute);
                        if (!String.IsNullOrEmpty(current.Value))
                        {
                            writer.WriteAttributeString("obsolete", current.Value);
                        }
                        writer.WriteEndElement();
                    }
                }
            }
        }

        private static void RemoveOldApis(
            Dictionary<string, List<KeyValuePair<string, string>>> versionIndex, 
            List<string> latestVersions)
        {
            string[] array = new string[versionIndex.Count];
            versionIndex.Keys.CopyTo(array, 0);
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string key = array2[i];
                List<KeyValuePair<string, string>> list = versionIndex[key];
                bool flag = true;
                foreach (KeyValuePair<string, string> current in list)
                {
                    if (latestVersions.Contains(current.Key))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    versionIndex.Remove(key);
                }
            }
        }

        private static bool IsLatestElement(ICollection<string> versions, 
            IList<string> latestVersions)
        {
            foreach (string current in versions)
            {
                if (latestVersions.Contains(current))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region VersionInfo Class

        private sealed class VersionInfo
        {
            public string Name;
            public string Group;
            public string File;

            public VersionInfo(string name, string group, string file)
            {
                this.Name  = name;
                this.Group = group;
                this.File  = file;
            }
        }

        #endregion

        #region ElementInfo Class

        private sealed class ElementInfo
        {
            public Dictionary<string, string> Versions 
                = new Dictionary<string, string>();

            public XPathNavigator ElementNode;

            public ElementInfo(string versionGroup, string version, 
                XPathNavigator elementNode)
            {
                this.Versions[versionGroup] = version;

                if (elementNode != null && this.ElementNode == null)
                {
                    this.ElementNode = elementNode;
                }
            }
        }

        #endregion
    }
}
