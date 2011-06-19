using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Sandcastle.Components.Versions;

using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.Components
{
    public sealed class BuildComponentController
    {
        #region Private Fields

        private static BuildComponentController _buildController;

        private bool _hasConceptualLinks;
        private IList<VersionInfo> _currentVersions;
        private Dictionary<string, VersionInfo> _versionInfo;

        #endregion

        #region Constructors and Destructor

        private BuildComponentController()
        {
            _versionInfo     = new Dictionary<string, VersionInfo>(
                StringComparer.OrdinalIgnoreCase);
            _currentVersions = new List<VersionInfo>();
        }

        static BuildComponentController()
        {
            Globals.Init();
        }

        #endregion

        #region Public Properties

        public bool HasVersions
        {
            get
            {
                return (_versionInfo != null && _versionInfo.Count != 0);
            }
        }

        public bool HasConceptualLinks
        {
            get
            {
                return _hasConceptualLinks;
            }
            set
            {
                _hasConceptualLinks = value;
            }
        }

        public IList<VersionInfo> CurrentVersions
        {
            get
            {
                return _currentVersions;
            }
        }

        public IDictionary<string, VersionInfo> Versions
        {
            get
            {
                return _versionInfo;
            }
        }

        public static BuildComponentController Controller
        {
            get
            {
                if (_buildController == null)
                {
                    _buildController = new BuildComponentController();
                }

                return _buildController;
            }
        }

        #endregion

        #region Public Methods

        public void AddVersion(VersionInfo versionInfo)
        {
            if (versionInfo == null || 
                String.IsNullOrEmpty(versionInfo.AssemblyName))
            {
                return;
            }

            _versionInfo[versionInfo.AssemblyName] = versionInfo;
        }  

        public void ClearVersions()
        {
            _currentVersions.Clear();
        }

        public bool UpdateVersion(string assemblyName)
        {
            VersionInfo curVersion = null;
            if (!String.IsNullOrEmpty(assemblyName) &&
                _versionInfo.TryGetValue(assemblyName, out curVersion))
            {
                _currentVersions.Add(curVersion);

                return true;
            }

            return false;
        }

        public void ParseVersionInfo(XPathNavigator configuration)
        {
            XPathNavigator navigator = configuration.SelectSingleNode("versionInfo");
            if (navigator == null)
            {
                return;
            }
            this.OnParseVersionInfo(navigator);
        }

        #endregion

        #region Private Methods

        #region Version Information Methods

        private void OnParseVersionInfo(XPathNavigator navigator)
        {
            string nodeText = navigator.GetAttribute("enabled", String.Empty);
            if (String.IsNullOrEmpty(nodeText))
            {
                return;
            }
            bool isEnabled = Convert.ToBoolean(nodeText);
            if (!isEnabled)
            {
                return;
            }
            nodeText = navigator.GetAttribute("sourceFile", String.Empty);
            if (String.IsNullOrEmpty(nodeText))
            {
                return;
            }
            string reflectionFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(nodeText));
            if (String.IsNullOrEmpty(reflectionFile) ||
                !File.Exists(reflectionFile))
            {
                return;
            }
            nodeText = navigator.GetAttribute("type", String.Empty);
            if (String.IsNullOrEmpty(nodeText))
            {
                return;
            }
            VersionInfoType infoType = (VersionInfoType)Enum.Parse(
                typeof(VersionInfoType), nodeText, true);
            if (infoType == VersionInfoType.None || 
                infoType == VersionInfoType.Advanced)
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel  = ConformanceLevel.Document;
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
                            string asmName = reader.GetAttribute("assemblyName");
                            string asmVersion = reader.GetAttribute("assemblyVersion");
                            string fileVersion = reader.GetAttribute("fileVersion");
                            if (!String.IsNullOrEmpty(asmName))
                            {
                                this.AddVersion(new VersionInfo(asmName,
                                    asmVersion, fileVersion, infoType));
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
                    ParseAssemblies(reader, infoType);
                }
            }
        }

        private void ParseAssemblies(XmlReader reader, VersionInfoType infoType)
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

                                    this.AddVersion(new VersionInfo(asmName,
                                        asmVersion, fileVersion, infoType));

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

        #endregion
    }
}
