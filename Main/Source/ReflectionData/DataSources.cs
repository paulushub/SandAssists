using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DataSources
    {
        #region Public Fields

        public const string XmlFileName      = "PersistentDictionary.xml";
        public const string DatabaseFileName = "PersistentDictionary.edb";

        #endregion

        #region Private Fields

        private bool   _isSystem;
        private bool   _isBuilding;
        private bool   _isDatabase;
        private bool   _isSilverlight;
        private string _language;
        private string _inputDir;
        private string _outputDir;

        private string _baseInput;
        private string _baseOutput;

        private Version _version;

        private HashSet<string> _sources;

        private DataSourceType  _sourceType;

        #endregion

        #region Constructors and Destructor

        public DataSources(bool isBuilding)
        {   
            _isBuilding    = isBuilding;
            _sourceType    = DataSourceType.None;
            _baseInput     = String.Empty;
            _baseOutput    = String.Empty;
            _language      = String.Empty;
            _sources       = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public DataSources(bool isBuilding, bool isSystem, bool isDatabase,
            bool isSilverlight, DataSourceType sourceType)
            : this(isBuilding)
        {
            _isSystem      = isSystem;
            _isDatabase    = isDatabase;
            _isSilverlight = isSilverlight;
            _sourceType    = sourceType;
        }

        public DataSources(bool isBuilding, XPathNavigator navigator)
            : this(isBuilding)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException("navigator");
            }
            if (!String.Equals(navigator.LocalName, "sources", 
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("navigator");  
            }

            string tempText = navigator.GetAttribute("system", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _isSystem = Convert.ToBoolean(tempText);
            }

            tempText = navigator.GetAttribute("name", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _sourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType),
                    tempText, true);
            }

            tempText = navigator.GetAttribute("platform", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _isSilverlight = tempText.Equals("silverlight",
                    StringComparison.OrdinalIgnoreCase);
            }

            tempText = navigator.GetAttribute("version", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _version = new Version(tempText);
            }

            _language = navigator.GetAttribute("lang", String.Empty);

            tempText = navigator.GetAttribute("storage", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _isDatabase = tempText.Equals("database", 
                    StringComparison.OrdinalIgnoreCase);
            }

            if (!navigator.IsEmptyElement)
            {
                XPathNavigator nodePaths = navigator.SelectSingleNode("paths");

                if (nodePaths != null)
                {
                    string baseInput = nodePaths.GetAttribute("baseInput", 
                        String.Empty);
                    string baseOutput = nodePaths.GetAttribute("baseOutput",
                        String.Empty);

                    this.SetDirectories(baseInput, baseOutput);
                }
            }

            XPathNodeIterator iterator = navigator.Select("source");
            if (iterator != null && iterator.Count != 0)
            {
                foreach (XPathNavigator sourceNode in iterator)
                {
                    string sourcePath = sourceNode.GetAttribute(
                        "path", String.Empty);

                    this.AddSource(sourcePath);
                }
            }
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (_isBuilding)
                {
                    if (_sources == null || _sources.Count == 0)
                    {
                        if (String.IsNullOrEmpty(_inputDir) ||
                            !Directory.Exists(_inputDir))
                        {
                            return false;
                        }
                    }
                    if (String.IsNullOrEmpty(_outputDir))
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    if (String.IsNullOrEmpty(_outputDir) ||
                        !Directory.Exists(_outputDir))
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public bool Exists
        {
            get
            {
                if (this.IsValid)
                {
                    return PersistentDictionaryFile.Exists(_outputDir);
                }

                return false;
            }
        }

        public DataSourceType SourceType
        {
            get
            {
                return _sourceType;
            }
        }

        public bool IsSystem
        {
            get
            {
                return _isSystem;
            }
        }

        public bool IsDatabase
        {
            get
            {
                return _isDatabase;
            }
        }

        public bool IsBuilding
        {
            get
            {
                return _isBuilding;
            }
        }

        public bool IsSilverlight
        {
            get
            {
                return _isSilverlight;
            }
        }

        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        public string InputDir
        {
            get
            {
                return _inputDir;
            }
            set
            {
                _inputDir = value;
            }
        }

        public string OutputDir
        {
            get
            {
                return _outputDir;
            }
            set
            {
                _outputDir = value;
            }
        }

        public IEnumerable<string> Sources
        {
            get
            {
                return _sources;
            }
        }

        public int SourceCount
        {
            get
            {
                if (_sources != null)
                {
                    return _sources.Count;
                }

                return 0;
            }
        }

        #endregion

        #region Public Methods

        public bool ContainsSource(string sourcePath)
        {
            if (String.IsNullOrEmpty(sourcePath) || _sources == null ||
                _sources.Count == 0)
            {
                return false;
            }

            string finalDir = sourcePath;
            if (!finalDir.EndsWith("\\"))
            {
                finalDir += "\\";
            }

            return _sources.Contains(finalDir);
        }

        public bool AddSource(string sourcePath)
        {
            if (String.IsNullOrEmpty(sourcePath))
            {
                return false;
            }
            if (_sources == null)
            {
                _sources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            string finalDir = sourcePath;
            if (!finalDir.EndsWith("\\"))
            {
                finalDir += "\\";
            }

            return _sources.Add(finalDir);
        }

        public bool RemoveSource(string sourcePath)
        {
            if (String.IsNullOrEmpty(sourcePath) || _sources == null ||
                _sources.Count == 0)
            {
                return false;
            }

            string finalDir = sourcePath;
            if (!finalDir.EndsWith("\\"))
            {
                finalDir += "\\";
            }

            return _sources.Remove(finalDir);
        }

        public void SetDirectories(string baseInput, string baseOutput)
        {
            _baseInput = String.Empty;
            _baseOutput = String.Empty;

            if (!String.IsNullOrEmpty(baseInput))
            {
                string inputDir = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(baseInput));

                if (Directory.Exists(inputDir))
                {
                    if (!String.IsNullOrEmpty(_language))
                    {
                        string tempDir = Path.Combine(inputDir, _language);
                        if (Directory.Exists(tempDir))
                        {
                            inputDir = tempDir;
                        }
                    }

                    _inputDir  = inputDir;
                    _baseInput = baseInput;
                }
            }  

            if (!String.IsNullOrEmpty(baseOutput))
            {
                // We create the following format:
                // (BaseOutput)\(Source)\vVersion\Language
                // Eg: A:\Data\Comments\Framework\v4.0\en
                string outputDir = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(baseOutput));

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (_sourceType != DataSourceType.None)
                {
                    outputDir = Path.Combine(outputDir, _sourceType.ToString());
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    // Blend SDK is separately available for Silverlight and WPF...
                    if (_sourceType == DataSourceType.Blend)
                    {
                        if (_isSilverlight)
                        {
                            outputDir = Path.Combine(outputDir, "Silverlight");
                        }
                        else
                        {
                            outputDir = Path.Combine(outputDir, "Wpf");
                        }

                        if (!Directory.Exists(outputDir))
                        {
                            Directory.CreateDirectory(outputDir);
                        }
                    }
                }

                if (_version != null)
                {
                    outputDir = Path.Combine(outputDir, "v" + _version.ToString(2));
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                }

                if (!String.IsNullOrEmpty(_language))
                {
                    outputDir = Path.Combine(outputDir, _language);
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                }

                _outputDir  = outputDir;
                _baseOutput = baseOutput;
            }
        }

        #endregion

        #region IXmlSerializable Members

        public void ReadXml(XmlReader reader)
        {
            XmlNodeType nodeType = reader.NodeType;
            string nodeName = reader.Name;

            string tempText = null;
            if (nodeType == XmlNodeType.Element && String.Equals(
                nodeName, "sources", StringComparison.OrdinalIgnoreCase))
            {
                tempText = reader.GetAttribute("system");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _isSystem = Convert.ToBoolean(tempText);
                }

                tempText = reader.GetAttribute("name");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _sourceType = (DataSourceType)Enum.Parse(
                        typeof(DataSourceType), tempText, true);
                }

                tempText = reader.GetAttribute("platform");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _isSilverlight = tempText.Equals("silverlight",
                        StringComparison.OrdinalIgnoreCase);
                }

                tempText = reader.GetAttribute("version");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _version = new Version(tempText);
                }

                _language = reader.GetAttribute("lang");

                tempText = reader.GetAttribute("storage");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _isDatabase = tempText.Equals("database",
                        StringComparison.OrdinalIgnoreCase);
                }
            }

            while (reader.Read())
            {
                nodeType = reader.NodeType;
                nodeName = reader.Name;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(nodeName, "sources",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        tempText = reader.GetAttribute("system");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _isSystem = Convert.ToBoolean(tempText);
                        }

                        tempText = reader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _sourceType = (DataSourceType)Enum.Parse(
                                typeof(DataSourceType), tempText, true);
                        }

                        tempText = reader.GetAttribute("platform");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _isSilverlight = tempText.Equals("silverlight",
                                StringComparison.OrdinalIgnoreCase);
                        }

                        tempText = reader.GetAttribute("version");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _version = new Version(tempText);
                        }

                        _language = reader.GetAttribute("lang");

                        tempText = reader.GetAttribute("storage");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _isDatabase = tempText.Equals("database",
                                StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    else if (String.Equals(nodeName, "paths",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string baseInput  = reader.GetAttribute("baseInput");
                        string baseOutput = reader.GetAttribute("baseOutput");

                        this.SetDirectories(baseInput, baseOutput);
                    }
                    else if (String.Equals(nodeName, "source",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string sourcePath = reader.GetAttribute("path");

                        this.AddSource(sourcePath);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(nodeName, "sources", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (_language == null)
            {
                _language = String.Empty;
            }

            writer.WriteStartElement("sources");    // start: sources
            writer.WriteAttributeString("system", "true");
            writer.WriteAttributeString("name", _sourceType.ToString());
            writer.WriteAttributeString("platform", _isSilverlight ?
                "Silverlight" : "Framework");
            writer.WriteAttributeString("version", _version != null ?
                _version.ToString(2) : "");
            writer.WriteAttributeString("lang", _language);
            writer.WriteAttributeString("storage",
                _isDatabase ? "database" : "memory");

            writer.WriteStartElement("paths"); // start: paths
            if (String.IsNullOrEmpty(_baseInput))
            {
                writer.WriteAttributeString("baseInput", "");
            }
            else
            {
                writer.WriteAttributeString("baseInput", Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(_baseInput)));
            }
            writer.WriteAttributeString("baseOutput", _baseOutput);
            writer.WriteEndElement();          // end: paths 

            if (_sources != null)
            {
                foreach (string commentDir in _sources)
                {
                    if (!Directory.Exists(commentDir))
                    {
                        continue;
                    }
                    string finalDir = null;
                    string langDir  = String.IsNullOrEmpty(_language) ? 
                        commentDir : Path.Combine(commentDir, _language);

                    if (!String.IsNullOrEmpty(_language) && 
                        Directory.Exists(langDir))
                    {
                        finalDir = langDir;
                    }
                    else
                    {
                        finalDir = String.Copy(commentDir);
                    }
                    if (!finalDir.EndsWith("\\"))
                    {
                        finalDir += "\\";
                    }

                    writer.WriteStartElement("source");  // start - source                  
                    writer.WriteAttributeString("path", finalDir);
                    writer.WriteEndElement();            // end - source 
                }
            }

            writer.WriteEndElement();              // end: sources
        }

        #endregion
    }
}
