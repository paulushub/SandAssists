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
    /// <summary>
    /// This presents the options available for the help build system for customization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The options presented by this class applies to all help groups, and may be
    /// overwritten by further options in the various groups or related objects.
    /// </para>
    /// <para>
    /// In the build process, most objects are initialized, and this options are made
    /// available to all such objects.
    /// </para>
    /// <para>
    /// It further provides string-string properties object for user customization.
    /// </para>
    /// <para>
    /// Also, it provides access to the following help objects:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="BuildSettings.Style"/>: A <see cref="BuildStyle"/> object specifying
    /// style options of the help, 
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="BuildSettings.Formats"/>: A list <see cref="IList{T}"/> of 
    /// <see cref="BuildFormat"/> object specifying output format options of the help, 
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="BuildSettings.Feedback"/>: A <see cref="BuildFeedback"/> object specifying
    /// the product, the company and other information required for a help feedback system, 
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="BuildSettings.Framework"/>: A <see cref="BuildFramework"/> object specifying
    /// the .NET framework version for the help system. 
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class BuildSettings : BuildObject<BuildSettings>
    {
        #region Private Fields

        // Logging information or settings...
        private bool   _useLogFile;
        private bool   _keepLogFile;
        private string _logFileName;

        private bool   _isInitialized;
        private bool   _cleanIntermediate;
        private bool   _showUpdated;
        private bool   _showPreliminary;
        private bool   _syntaxUsage;

        private bool   _isHelpApi;
        private bool   _isHelpTopics;

        private bool   _rootContainer;
        private string _rootTitle;

        private string _helpName;
        private string _helpTitle;

        private string _headerText;
        private string _footerText;

        private string _outputDir;
        private string _configDir;
        private string _workingDir;
        private string _contentDir;
        private string _sandcastleDir;
        private string _sandassistDir;

        private CultureInfo       _outputCulture;
        private BuildStyle        _outputStyle;
        private BuildFeedback     _outputFeedback;
        private BuildFramework    _outputFramework;
        private BuildSyntaxType   _syntaxType;

        private BuildLoggerVerbosity _verbosity;

        private List<string>               _outputFolders;
        private List<BuildFormat>          _listFormats;
        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// 
        /// </overloads>
        /// <summary>
        /// 
        /// </summary>
        public BuildSettings()
        {
            _useLogFile      = true;
            _keepLogFile     = true;
            _logFileName     = "HelpBuild.log";

            _rootContainer   = true;
            _rootTitle       = "Programmer's Reference";

            _helpName        = "Documentation";
            _helpTitle       = "Sandcastle Documentation";
            _outputStyle     = new BuildStyle();
            _listFormats     = new List<BuildFormat>();

            _outputCulture   = new CultureInfo(1033, false);
            _outputFeedback  = new BuildFeedback();
            _outputFramework = BuildFramework.Default;

            _verbosity       = BuildLoggerVerbosity.Minimal;

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
            
            FormatMhv mhvFormat = new FormatMhv();
            mhvFormat.Reset();
            _listFormats.Add(mhvFormat);

            FormatWeb htmFormat = new FormatWeb();
            htmFormat.Reset();
            _listFormats.Add(htmFormat);
            
            _properties = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            // Add all the "standard" Sandcastle/Assist folders....
            _outputFolders = new List<string>();
            _outputFolders.Add("icons");
            _outputFolders.Add("scripts");
            _outputFolders.Add("styles");
            _outputFolders.Add("media");
            _outputFolders.Add("images");
            _outputFolders.Add("maths");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public BuildSettings(BuildSettings source)
            : base(source)
        {
            _rootTitle     = source._rootTitle;
            _rootContainer = source._rootContainer;

            _syntaxType    = source._syntaxType;
            _properties    = source._properties;
            _listFormats   = source._listFormats;
            _outputFolders = source._outputFolders;
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
                    // If the title is not yet set, give it this value...
                    if (String.IsNullOrEmpty(_helpTitle))
                    {
                        _helpTitle = value;
                    }

                    if (value.IndexOf(' ') >= 0)
                    {
                        throw new BuildException(
                            "The help file name cannot contain space.");
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
        
        public string LogFileName
        {
            get
            {
                return _logFileName;
            }
            set
            {
                _logFileName = value;
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

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
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

        public IList<BuildFormat> Formats
        {
            get
            {
                return _listFormats;
            }
        }   

        /// <summary>
        /// Gets or sets the level of detail to show in the build log.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLoggerVerbosity"/> specifying 
        /// the level of detail. The default is <see cref="BuildLoggerVerbosity.Minimal"/>.
        /// </value>
        public BuildLoggerVerbosity Verbosity
        {
            get 
            { 
                return _verbosity; 
            }
            set 
            { 
                _verbosity = value; 
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

        /// <summary>
        /// Gets or sets the fully qualified path of the current working directory.
        /// </summary>
        /// <value>
        /// A string containing a directory path.
        /// </value>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
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

        public IList<string> OutputFolders
        {
            get
            {
                return _outputFolders;
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

        public bool RootNamespaceContainer
        {
            get
            {
                return _rootContainer;
            }
            set
            {
                _rootContainer = value;
            }
        }

        public string RootNamespaceTitle
        {
            get
            {
                return _rootTitle;
            }
            set
            {
                _rootTitle = value;
            }
        }
        
        public string HeaderText
        {
            get 
            { 
                return _headerText; 
            }
            set 
            { 
                _headerText = value; 
            }
        }
        
        public string FooterText
        {
            get 
            { 
                return _footerText; 
            }
            set 
            { 
                _footerText = value; 
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

        public BuildFeedback Feedback
        {
            get
            {
                return _outputFeedback;
            }
            set
            {
                if (value != null)
                {
                    _outputFeedback = value;
                }
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

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _outputStyle.Initialize(this);

            _isInitialized = true;
        }

        public void Uninitialize()
        {
            _outputStyle.Uninitialize();

            _isInitialized = false;
        }

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
        public void Remove(string key)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void Clear()
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
        public void Add(string key, string value)
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
        public bool ContainsKey(string key)
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
        public bool ContainsValue(string value)
        {
            return _properties.ContainsValue(value);
        }

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
