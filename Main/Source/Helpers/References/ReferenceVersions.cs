using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.References
{
    public sealed class ReferenceVersions : BuildObject, IBuildNamedItem
    {
        #region Private Fields

        private static StringBuilder _parameterTypes = new StringBuilder();
        private static StringBuilder _parameterNames = new StringBuilder();

        private static XPathExpression _apiParameterTypeNameExpression
            = XPathExpression.Compile("string(.//type/@api)");

        private static XPathExpression _apiElementsExpression
            = XPathExpression.Compile("elements/element");

        private static XPathExpression _apiParametersExpression
            = XPathExpression.Compile("parameters/parameter");

        private static XPathExpression _apiParameterTemplateNameExpression
            = XPathExpression.Compile("string(.//template/@name)");

        private static Dictionary<string, string> _standardPlatforms;

        private int    _groupIndex;
        private int    _sourceIndex;
        private string _sourceId;
        private string _platformId;
        private string _platformDir;
        private string _platformFile;
        private string _platformTitle;

        private BuildList<string> _versionDirs;

        private BuildKeyedList<ReferenceVersionSource> _listSources;

        #endregion

        #region Constructors and Destrutor

        public ReferenceVersions()
        {
            _groupIndex  = -1;
            _sourceIndex = -1;
            _sourceId    = String.Format("ApiPlatform{0:x}", Guid.NewGuid().ToString().GetHashCode());
            _platformId  = String.Copy(_sourceId); 
            _versionDirs = new BuildList<string>();
            _listSources = new BuildKeyedList<ReferenceVersionSource>();
        }

        public ReferenceVersions(string platformId)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(platformId, "platformId");

            _platformId    = platformId;
            _platformTitle = String.Empty;
        }

        public ReferenceVersions(string platformId, string platformTitle)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(platformId,    "platformId");
            BuildExceptions.NotNullNotEmpty(platformTitle, "platformTitle");

            _platformId    = platformId;
            _platformTitle = platformTitle;
        }

        #endregion

        #region Public Properties

        public bool IsStandard
        {
            get
            {
                return IsStandardPlatform(_platformId);
            }
        }

        public int GroupIndex
        {
            get
            {
                return _groupIndex;
            }
            set
            {
                _groupIndex = value;
            }
        }

        public string SourceId
        {
            get
            {
                return _sourceId;
            }
        }

        public int SourceIndex
        {
            get
            {
                return _sourceIndex;
            }
            set
            {
                _sourceIndex = value;
            }
        }

        public string PlatformId
        {
            get
            {
                return _platformId;
            }
        }

        public string PlatformTitle
        {
            get
            {
                return _platformTitle;
            }
            set
            {
                _platformTitle = value;
            }
        }

        public string PlatformDir
        {
            get
            {
                return _platformDir;
            }
            set
            {
                _platformDir = value;
            }
        }

        public string PlatformFile
        {
            get
            {
                return _platformFile;
            }
        }

        public IList<string> VersionDirs
        {
            get
            {
                return _versionDirs;
            }
        }

        public int Count
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources.Count;
                }

                return 0;
            }
        }

        public ReferenceVersionSource this[int index]
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources[index];
                }

                return null;
            }
        }

        public ReferenceVersionSource this[string sourceId]
        {
            get
            {
                if (String.IsNullOrEmpty(sourceId))
                {
                    return null;
                }

                if (_listSources != null)
                {
                    return _listSources[sourceId];
                }

                return null;
            }
        }

        public IBuildNamedList<ReferenceVersionSource> Sources
        {
            get
            {
                return _listSources;
            }
        }

        #endregion

        #region Public Methods

        public static bool IsStandardPlatform(string platformId)
        {
            if (_standardPlatforms == null)
            {
                _standardPlatforms = new Dictionary<string, string>();
                _standardPlatforms.Add("netfw", ".NET Framework");
                _standardPlatforms.Add("netcfw", ".NET Compact Framework");
                _standardPlatforms.Add("xnafw", "XNA Framework");
                _standardPlatforms.Add("silverlight", "Silverlight");
                _standardPlatforms.Add("silverlight_mobile", "Silverlight for Windows Phone");

                // The definitions for the following are still not known...
                //_standardPlatforms.Add("netfwcp", ".NET Framework Client Profile");
                //_standardPlatforms.Add("netpcl", "Portable Class Library");
            }
            if (String.IsNullOrEmpty(platformId))
            {
                return false;
            }

            return _standardPlatforms.ContainsKey(platformId);
        }

        public void Add(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }

            _listSources.Add(source);
        }

        public void Add(IList<ReferenceVersionSource> sources)
        {
            BuildExceptions.NotNull(sources, "sources");

            int sourceCount = sources.Count;
            if (sourceCount == 0)
            {
                return;
            }

            for (int i = 0; i < sourceCount; i++)
            {
                this.Add(sources[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.RemoveAt(index);
        }

        public void Remove(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Remove(source);
        }

        public bool Contains(ReferenceVersionSource source)
        {
            if (source == null || _listSources == null || _listSources.Count == 0)
            {
                return false;
            }

            return _listSources.Contains(source);
        }

        public void Clear()
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Clear();
        }

        public bool WritePlatformFile(BuildContext context, string apiVersionsDir)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.PathMustExist(apiVersionsDir, "apiVersionsDir");
            Debug.Assert(_sourceIndex >= 0, "The platform source index is not set");
            //Debug.Assert(_groupIndex >= 0, "The platform group index is not set");
            Debug.Assert(_listSources != null && _listSources.Count != 0, 
                "The platform does not contain any version source.");

            BuildLogger logger = context.Logger;

            if (_sourceIndex < 0)
            {
                if (logger != null)
                {
                    logger.WriteLine("The platform source index is not set.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (_listSources == null || _listSources.Count == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine("The platform does not contain any version source.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            bool isSuccessful = true;

            string platformFile = null;
            // If there is a group index...
            if (_groupIndex >= 0)
            {   
                // We write platform filter file name like ApiPlatform1a.xml,
                // ApiPlatform1b.xml, ApiPlatform1c.xml etc
                // "97" is the integer value of the letter "a".
                platformFile = Path.Combine(context.WorkingDirectory,
                    String.Format("ApiPlatform{0}{1}.xml", _groupIndex,
                    Convert.ToChar(97 + _sourceIndex)));
            }
            else
            {   
                // We write platform filter file name like ApiPlatformA.xml,
                // ApiPlatformB.xml, ApiPlatformC.xml etc
                // "65" is the integer value of the letter "A".
                platformFile = Path.Combine(context.WorkingDirectory,
                    String.Format("ApiPlatform{0}.xml", Convert.ToChar(65 + _sourceIndex)));
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(platformFile, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("platforms");

                for (int i = 0; i < _listSources.Count; i++)
                {
                    ReferenceVersionSource versionSource = _listSources[i];

                    isSuccessful = this.CreatePlatformFilter(writer,
                        versionSource, apiVersionsDir);
                    if (!isSuccessful)
                    {
                        break;
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            if (isSuccessful)
            {
                _platformFile = platformFile;
            }

            return isSuccessful;
        }

        #endregion

        #region Private Methods

        private bool CreatePlatformFilter(XmlWriter writer,
            ReferenceVersionSource versionSource, string apiVersionsDir)
        {
            string reflectionFile = Path.Combine(apiVersionsDir,
                versionSource.SourceId + ".xml");

            if (!File.Exists(reflectionFile))
            {
                return false;
            }

            XPathDocument document = new XPathDocument(reflectionFile);
            XPathNavigator documentNavigator = document.CreateNavigator();
            XPathNavigator rootNavigator = documentNavigator.SelectSingleNode(
                "reflection/apis");

            if (rootNavigator == null)
            {
                return false;
            }

            // Select the namespaces...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[starts-with(@id, 'N:')]");

            if (iterator == null || iterator.Count == 0)
            {
                return false;
            }

            foreach (XPathNavigator navigator in iterator)
            {
                writer.WriteStartElement("platform");

                // NOTE: Handling of the platform id for a successful build
                // is the most confusing part of this write process.
                // With the exception of the .NET Framework platform, all others
                // must be marked with a defined term, which indicates its use...
                switch (_platformId)
                {
                    case "netcfw":             // .NET Compact Framework
                        writer.WriteAttributeString("name",
                            "WindowsCE," + _platformId);
                        break;
                    case "xnafw":              // XNA Framework
                        writer.WriteAttributeString("name",
                            "Xbox360," + _platformId);
                        break;
                    case "silverlight":        // Silverlight
                    case "silverlight_mobile": // Silverlight for Windows Phone
                        writer.WriteAttributeString("name",
                            "SilverlightPlatforms," + _platformId);
                        break;
                    case "netfw":              // .NET Framework
                    default:
                        writer.WriteAttributeString("name", _platformId);
                        break;
                }
                writer.WriteAttributeString("version", versionSource.VersionId);

                CreateNamespaceFilter(writer, navigator, rootNavigator);

                writer.WriteEndElement();
            }

            return true;
        }

        private static bool CreateNamespaceFilter(XmlWriter writer,
            XPathNavigator navigator, XPathNavigator rootNode)
        {
            if (navigator.IsEmptyElement)
            {
                return false;
            }
            string namespaceId = navigator.GetAttribute("id", String.Empty);
            if (String.IsNullOrEmpty(namespaceId))
            {
                return false;
            }

            writer.WriteStartElement("namespace");
            writer.WriteAttributeString("name", namespaceId);
            writer.WriteAttributeString("include", "true");

            XPathNodeIterator elements = navigator.Select(_apiElementsExpression);
            if (elements != null && elements.Count != 0)
            {
                foreach (XPathNavigator element in elements)
                {
                    string typeId = element.GetAttribute("api", String.Empty);
                    if (!String.IsNullOrEmpty(typeId))
                    {
                        XPathNavigator typeNode = rootNode.SelectSingleNode(
                            String.Format("api[@id='{0}']", typeId));

                        if (typeNode != null)
                        {
                            CreateTypeFilter(writer, typeNode, rootNode);
                        }
                    }
                }
            }

            writer.WriteEndElement();

            return true;
        }

        private static bool CreateTypeFilter(XmlWriter writer,
            XPathNavigator typeNode, XPathNavigator rootNode)
        {
            if (typeNode.IsEmptyElement)
            {
                return false;
            }
            string typeId = typeNode.GetAttribute("id", String.Empty);
            if (String.IsNullOrEmpty(typeId))
            {
                return false;
            }
            // Determine whether the class is static; it is sealed abstract.
            // We will test for extension methods in static classes...
            bool isStatic = false;
            XPathNavigator typedataNode = typeNode.SelectSingleNode("typedata");
            if (typedataNode != null)
            {
                string tempText = typedataNode.GetAttribute("abstract", String.Empty);
                if (!String.IsNullOrEmpty(tempText) && tempText.Equals(
                    "true", StringComparison.OrdinalIgnoreCase))
                {
                    tempText = typedataNode.GetAttribute("sealed", String.Empty);
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        isStatic = tempText.Equals("true",
                            StringComparison.OrdinalIgnoreCase);
                    }
                }
            }

            XPathNavigator allMembersNode = rootNode.SelectSingleNode(
                String.Format("api[@id='{0}']", "AllMembers." + typeId));
            if (allMembersNode == null)
            {
                return false;
            }

            XPathNodeIterator elements = allMembersNode.Select(
                _apiElementsExpression);

            writer.WriteStartElement("type");
            writer.WriteAttributeString("name", typeId);
            writer.WriteAttributeString("include", "true");

            if (elements != null && elements.Count != 0)
            {
                foreach (XPathNavigator element in elements)
                {
                    string memberId = element.GetAttribute("api", String.Empty);
                    if (element.IsEmptyElement)
                    {
                        XPathNavigator memberNode = rootNode.SelectSingleNode(
                            String.Format("api[@id='{0}']", memberId));
                        if (memberNode != null)
                        {
                            CreateMemberFilter(writer, memberNode, rootNode,
                                false, isStatic);
                        }
                    }
                    else
                    {
                        if (memberId.StartsWith("Overload:"))
                        {
                            // For overloaded methods...
                            XPathNavigator overloadsNode = rootNode.SelectSingleNode(
                                String.Format("api[@id='{0}']", memberId));
                            if (overloadsNode != null)
                            {
                                CreateOverloadFilter(writer, overloadsNode,
                                    rootNode, isStatic);
                            }
                        }
                        else
                        {
                            // For inherited members/elements...
                            CreateMemberFilter(writer, element, rootNode,
                                true, isStatic);
                        }
                    }
                }
            }

            writer.WriteEndElement();

            return true;
        }

        private static bool CreateMemberFilter(XmlWriter writer,
            XPathNavigator memberNode, XPathNavigator rootNode,
            bool isElement, bool isStaticType)
        {
            if (memberNode.IsEmptyElement)
            {
                return false;
            }
            string memberId = null;
            if (isElement)
            {
                memberId = memberNode.GetAttribute("api", String.Empty);
            }
            else
            {
                memberId = memberNode.GetAttribute("id", String.Empty);
            }
            if (String.IsNullOrEmpty(memberId))
            {
                return false;
            }

            XPathNavigator apidataNode = memberNode.SelectSingleNode("apidata");
            if (apidataNode == null)
            {
                return false;
            }
            writer.WriteStartElement("member");
            writer.WriteAttributeString("name", apidataNode.GetAttribute(
                "name", String.Empty));
            writer.WriteAttributeString("include", "true");

            writer.WriteEndElement();

            return true;
        }

        private static bool CreateOverloadFilter(XmlWriter writer,
            XPathNavigator memberNode, XPathNavigator rootNode,
            bool isStaticType)
        {
            if (memberNode.IsEmptyElement)
            {
                return false;
            }
            string memberId = memberNode.GetAttribute("id", String.Empty);
            if (String.IsNullOrEmpty(memberId))
            {
                return false;
            }
            XPathNavigator apidataNode = memberNode.SelectSingleNode("apidata");
            if (apidataNode == null)
            {
                return false;
            }
            string baseName = memberId.Substring(9);

            writer.WriteStartElement("member");
            writer.WriteAttributeString("name", apidataNode.GetAttribute(
                "name", String.Empty));
            writer.WriteAttributeString("include", "true");

            XPathNodeIterator elements = memberNode.Select(
                _apiElementsExpression);
            foreach (XPathNavigator element in elements)
            {
                string overloadApi = element.GetAttribute("api", String.Empty);
                if (String.IsNullOrEmpty(overloadApi))
                {
                    continue;
                }

                XPathNavigator overloadNode = rootNode.SelectSingleNode(
                    String.Format("api[@id='{0}']", overloadApi));
                if (overloadNode == null)
                {
                    continue;
                }

                // <overload api="api" types="" names="" include="boolean"/>
                writer.WriteStartElement("overload");
                writer.WriteAttributeString("api", overloadApi);

                XPathNodeIterator parameters = overloadNode.Select(
                    _apiParametersExpression);

                _parameterTypes.Length = 0;
                _parameterNames.Length = 0;

                if (parameters != null && parameters.Count != 0)
                {
                    int i = 0;
                    foreach (XPathNavigator parameter in parameters)
                    {
                        i++;
                        _parameterNames.Append(parameter.GetAttribute(
                            "name", String.Empty));
                        if (i < parameters.Count)
                            _parameterNames.Append(",");

                        string arrayOf = (parameter.SelectSingleNode("arrayOf") == null) ? "" : "[]";
                        string typeName = (string)parameter.Evaluate(_apiParameterTypeNameExpression);
                        if (string.IsNullOrEmpty(typeName))
                            typeName = (string)parameter.Evaluate(_apiParameterTemplateNameExpression);

                        int basenameStart = typeName.LastIndexOf(':') + 1;
                        if (basenameStart > 0 && basenameStart < typeName.Length)
                            typeName = typeName.Substring(basenameStart);

                        _parameterTypes.Append(typeName + arrayOf);
                        if (i < parameters.Count)
                            _parameterTypes.Append(",");
                    }
                }
                writer.WriteAttributeString("types", _parameterTypes.ToString());
                writer.WriteAttributeString("names", _parameterNames.ToString());
                writer.WriteAttributeString("include", "true");

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            return true;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _platformId; 
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return null;
        }

        #endregion
    }
}
