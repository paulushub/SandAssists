using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DataSource
    {
        #region Public Fields

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

        private Version _version;

        private DataSourceType _sourceType;

        #endregion

        #region Constructors and Destructor

        public DataSource(bool isBuilding)
        {   
            _isBuilding    = isBuilding;
            _sourceType    = DataSourceType.None;
        }

        public DataSource(bool isBuilding, bool isSystem, bool isDatabase,
            bool isSilverlight, DataSourceType sourceType)
            : this(isBuilding)
        {
            _isSystem      = isSystem;
            _isDatabase    = isDatabase;
            _isSilverlight = isSilverlight;
            _sourceType    = sourceType;
        }

        public DataSource(bool isBuilding, XPathNavigator navigator)
            : this(isBuilding)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException("navigator");
            }
            if (!String.Equals(navigator.LocalName, "source", 
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
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (_isBuilding)
                {
                    if (String.IsNullOrEmpty(_inputDir) || 
                        !Directory.Exists(_inputDir))
                    {
                        return false;
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

        #endregion

        #region Public Methods

        public void SetDirectories(string baseInput, string baseOutput)
        {
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

                    _inputDir = inputDir;
                }
            }  

            if (!String.IsNullOrEmpty(baseOutput))
            {
                // We create the following format:
                // (BaseOutput)\(Source)\vVersion\Language
                // Eg: A:\Data\Reflections\Framework\v4.0\en
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

                _outputDir = outputDir;
            }
        }

        #endregion
    }
}
