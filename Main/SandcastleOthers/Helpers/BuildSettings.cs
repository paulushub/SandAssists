using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Formats;

namespace Sandcastle
{
    [Serializable]
    public class BuildSettings : BuildObject<BuildSettings>
    {
        #region Private Fields

        // Logging information or settings...
        private bool   _useLogFile;
        private bool   _keepLogFile;
        private string _logFile;

        private bool   _cleanIntermediate;
        private bool   _showUpdated;
        private bool   _showPreliminary;
        private bool   _syntaxUsage;

        private bool   _isHelpApi;
        private bool   _isHelpTopics;

        private string _helpName;
        private string _helpTitle;

        private string _copyrightHref;
        private string _copyrightText;

        private string _outputDir;
        private string _configDir;
        private string _workingDir;
        private string _contentDir;
        private string _sandcastleDir;
        private string _sandassistDir;

        private CultureInfo       _outputCulture;
        private BuildStyle        _outputStyle;
        private BuildFramework    _outputFramework;
        private BuildSyntaxType   _syntaxType;

        private List<string>               _outputFolders;
        private List<BuildFormat>          _listFormats;
        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

        public BuildSettings()
        {
            _useLogFile  = true;
            _keepLogFile = true;
            _logFile     = "HelpBuild.log";

            _copyrightHref = String.Empty;
            _copyrightText = String.Empty;

            _helpName    = "Documentation";
            _helpTitle   = "Sandcastle Documentation";
            _outputStyle = new BuildStyle();
            _listFormats = new List<BuildFormat>();

            _outputCulture   = new CultureInfo(1033, false);
            _outputFramework = BuildFramework.Default;

            // Check and set the Sandcastle installed directory...
            string tempDir = Environment.ExpandEnvironmentVariables("%DXROOT%");
            if (Directory.Exists(tempDir))
            {
                _sandcastleDir = "%DXROOT%";
            }
            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            _configDir     = Path.Combine(assemblyPath, "Configurations");
            _contentDir    = Path.Combine(assemblyPath, "Contents");
            _sandassistDir = String.Copy(assemblyPath);

            _syntaxType    = BuildSyntaxType.Standard;

            // We create all the current three file formats, and reset to set
            // the initial values...
            // NOTE: The OutputFormat.Reset() is virtual so we are not calling it
            //       from the constructors.
            FormatChm chmFormat = new FormatChm();
            chmFormat.Reset();
            chmFormat.Enabled = true; // enable only this format...
            _listFormats.Add(chmFormat);
            FormatHxs hxsFormat = new FormatHxs();
            hxsFormat.Reset();
            _listFormats.Add(hxsFormat);
            FormatHtm htmFormat = new FormatHtm();
            htmFormat.Reset();
            _listFormats.Add(htmFormat);

            _properties = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);

            // Add all the "standard" Sandcastle/Assist folders....
            _outputFolders = new List<string>();
            _outputFolders.Add("icons");
            _outputFolders.Add("scripts");
            _outputFolders.Add("styles");
            _outputFolders.Add("media");
            _outputFolders.Add("images");
            _outputFolders.Add("math");
        }

        public BuildSettings(BuildSettings source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public CultureInfo CultureInfo
        {
            get
            {
                return _outputCulture;
            }
            set
            {
                if (value != null)
                {
                    _outputCulture = value;
                }
            }
        }

        public string HelpName
        {
            get
            {
                return _helpName;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                value = value.Trim();

                if (!String.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(' ') >= 0)
                    {
                        throw new BuildException("The help file name cannot contain space.");
                    }
                    _helpName = value;
                }
            }
        }

        public string HelpTitle
        {
            get
            {
                return _helpTitle;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _helpTitle = value;
                }
            }
        }

        public string CopyrightHref
        {
            get 
            { 
                return _copyrightHref; 
            }
            set 
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _copyrightHref = value; 
            }
        }

        public string CopyrightText
        {
            get 
            { 
                return _copyrightText; 
            }
            set 
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _copyrightText = value; 
            }
        }
       
        public bool UseLogFile
        {
            get
            {
                return _useLogFile;
            }
            set
            {
                _useLogFile = value;
            }
        }
        
        public bool KeepLogFile
        {
            get
            {
                return _keepLogFile;
            }
            set
            {
                _keepLogFile = value;
            }
        }
        
        public string LogFile
        {
            get
            {
                return _logFile;
            }
            set
            {
                _logFile = value;
            }
        }

        public bool BuildReferences
        {
            get 
            {
                return _isHelpApi; 
            }
            set 
            { 
                _isHelpApi = value; 
            }
        }

        public bool BuildConceptual
        {
            get 
            { 
                return _isHelpTopics; 
            }
            set 
            { 
                _isHelpTopics = value; 
            }
        }

        public bool IsCombinedBuild
        {
            get
            {
                return (_isHelpTopics && _isHelpApi);
            }
        }

        public BuildStyle Style
        {
            get
            {
                return _outputStyle;
            }
            set
            {
                if (value != null)
                {
                    _outputStyle = value;
                }
            }
        }

        public BuildSyntaxType SyntaxType
        {
            get 
            { 
                return _syntaxType; 
            }
            set 
            { 
                _syntaxType = value; 
            }
        }

        public bool SyntaxUsage
        {
            get 
            { 
                return _syntaxUsage; 
            }
            set 
            {
                _syntaxUsage = value; 
            }
        }

        public IList<string> OutputFolders
        {
            get
            {
                return _outputFolders;
            }
        }

        public IList<BuildFormat> OutputFormats
        {
            get
            {
                return _listFormats;
            }
        }

        public string StylesDirectory
        {
            get
            {
                string sandcastleDir = this.SandcastleDirectory;
                sandcastleDir = Environment.ExpandEnvironmentVariables(sandcastleDir);
                sandcastleDir = Path.GetFullPath(sandcastleDir);

                BuildStyle outputStyle = this.Style;
                string stylesDir = outputStyle.Directory;
                if (String.IsNullOrEmpty(stylesDir) == false)
                {
                    stylesDir = Environment.ExpandEnvironmentVariables(stylesDir);
                    stylesDir = Path.GetFullPath(stylesDir);
                    if (Directory.Exists(stylesDir))
                    {
                        sandcastleDir = stylesDir;
                    }
                }

                return sandcastleDir;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _workingDir;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                    value = Path.GetFullPath(value);
                }
                _workingDir = value;
            }
        }

        public string ConfigurationDirectory
        {
            get
            {
                return _configDir;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _configDir = value;
                }
            }
        }

        public string ContentsDirectory
        {
            get
            {
                return _contentDir;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _contentDir = value;
                }
            }
        }

        public string SandcastleDirectory
        {
            get 
            { 
                return _sandcastleDir; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _sandcastleDir = value; 
                }
            }
        }

        public string SandAssistDirectory
        {
            get 
            { 
                return _sandassistDir; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _sandassistDir = value; 
                }
            }
        }

        public string OutputDirectory
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

        public bool CleanIntermediate
        {
            get
            {
                return _cleanIntermediate;
            }
            set
            {
                _cleanIntermediate = value;
            }
        }

        public bool ShowUpdated
        {
            get 
            { 
                return _showUpdated; 
            }
            set 
            { 
                _showUpdated = value; 
            }
        }
        
        public bool ShowPreliminary
        {
            get 
            { 
                return _showPreliminary; 
            }
            set 
            { 
                _showPreliminary = value; 
            }
        }

        public BuildFramework Framework
        {
            get
            {
                return _outputFramework;
            }
            set
            {
                if (value != null)
                {
                    _outputFramework = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _properties.Keys;

                    return keyColl;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _properties.Values;

                    return valueColl;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        #region GetSharedContents Method

        public virtual IList<string> GetSharedContents(BuildEngineType engineType)
        {
            //TODO: Must be reviewed later for a more optimized code...
            List<string> sharedContents = new List<string>();
            if (_outputStyle == null)
            {
                return sharedContents;
            }

            BuildStyleType styleType = _outputStyle.StyleType;

            string path = _contentDir;
            if (String.IsNullOrEmpty(path) ||
                System.IO.Directory.Exists(path) == false)
            {
                return sharedContents;
            }

            string contentFile = String.Empty;
            if (engineType == BuildEngineType.Conceptual)
            {
                if (styleType == BuildStyleType.Vs2005 ||
                    styleType == BuildStyleType.Whidbey)
                {
                    contentFile = Path.Combine(path, "Vs2005.xml");
                }
                else
                {
                    contentFile = Path.Combine(path, styleType.ToString() + ".xml");
                }
            }
            else if (engineType == BuildEngineType.Reference)
            {
                if (styleType == BuildStyleType.Vs2005 ||
                    styleType == BuildStyleType.Whidbey)
                {
                    contentFile = Path.Combine(path, "Vs2005.xml");
                }
                else
                {
                    contentFile = Path.Combine(path, styleType.ToString() + ".xml");
                }
            }

            if (String.IsNullOrEmpty(contentFile) == false && File.Exists(contentFile))
            {
                sharedContents.Add(contentFile);
            }

            return sharedContents;
        }

        #endregion

        #region Properties Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public void RemoveProperty(string key)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void ClearProperties()
        {
            if (_properties.Count == 0)
            {
                return;
            }

            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The string key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// <para>-or-</para>
        /// An element with the same key already exists in the <see cref="BuildSettings"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="BuildSettings.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="BuildSettings"/>. However, if the specified key already 
        /// exists in the <see cref="BuildSettings"/>, setting the 
        /// <see cref="BuildSettings.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="BuildSettings.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="BuildSettings"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="BuildSettings"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildSettings Clone()
        {
            BuildSettings settings = new BuildSettings(this);

            return settings;
        }

        #endregion
    }
}
