using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

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
    public sealed class BuildSettings : BuildOptions<BuildSettings>
    {
        #region Private Fields

        private bool   _cleanIntermediate;
        private bool   _showUpdated;
        private bool   _showPreliminary;

        private bool   _isHelpApi;
        private bool   _isHelpTopics;

        private string _helpName;
        private string _helpTitle;

        private string _headerText;
        private string _footerText;

        private BuildDirectoryPath _outputDir;
        private BuildDirectoryPath _configDir;
        private BuildDirectoryPath _workingDir;
        private BuildDirectoryPath _contentDir;
        private BuildDirectoryPath _sandcastleDir;
        private BuildDirectoryPath _sandassistDir;

        private SharedContent     _sharedContent;
        private IncludeContent    _includeContent;
        private AttributeContent  _attributeContent;

        private CultureInfo       _outputCulture;

        private BuildToc          _outputToc;
        private BuildStyle        _outputStyle;
        private BuildLogging      _outputLogging;
        private BuildFeedback     _outputFeedback;

        private BuildList<string> _outputFolders;
        private BuildFormatList   _listFormats;
        private BuildProperties   _properties;

        private BuildEngineSettingsList _listEngineSettings;

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
            _helpName        = "Documentation";
            _helpTitle       = "Sandcastle Documentation";
     
            _outputToc       = new BuildToc();
            _outputStyle     = new BuildStyle();
            _outputLogging   = new BuildLogging();
            _listFormats     = new BuildFormatList();

            _outputCulture   = new CultureInfo(1033, false);
            _outputFeedback  = new BuildFeedback();

            _sharedContent      = new SharedContent("Default");
            _attributeContent   = new AttributeContent();
            _includeContent     = new IncludeContent("Default");

            _listEngineSettings = new BuildEngineSettingsList();
            _listEngineSettings.Add(new ReferenceEngineSettings());
            _listEngineSettings.Add(new ConceptualEngineSettings());

            // Check and set the Sandcastle installed directory...
            string tempDir = Environment.ExpandEnvironmentVariables("%DXROOT%");
            if (Directory.Exists(tempDir))
            {
                _sandcastleDir          = new BuildDirectoryPath(tempDir);
                _sandcastleDir.HintPath = "%DXROOT%";
            }
            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            _configDir     = new BuildDirectoryPath(Path.Combine(assemblyPath, "Configurations"));
            _contentDir    = new BuildDirectoryPath(Path.Combine(assemblyPath, "Contents"));
            _sandassistDir = new BuildDirectoryPath(String.Copy(assemblyPath));

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

            // Add all the "standard" Sandcastle/Assist folders....
            _outputFolders = new BuildList<string>();
            _outputFolders.Add("icons");
            _outputFolders.Add("scripts");
            _outputFolders.Add("styles");
            _outputFolders.Add("media");
            _outputFolders.Add("images");
            _outputFolders.Add("maths");

            _properties = new BuildProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public BuildSettings(BuildSettings source)
            : base(source)
        {
            _properties         = source._properties;
            _listFormats        = source._listFormats;
            _outputFolders      = source._outputFolders;
            _listEngineSettings = source._listEngineSettings;
            _sharedContent      = source._sharedContent;
            _includeContent     = source._includeContent;
        }

        #endregion

        #region Public Properties

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

        public BuildFormatList Formats
        {
            get
            {
                return _listFormats;
            }
        }   

        /// <summary>
        /// Gets or sets the fully qualified path of the current working directory.
        /// </summary>
        /// <value>
        /// A string containing a directory path.
        /// </value>
        public BuildDirectoryPath WorkingDirectory
        {
            get
            {
                return _workingDir;
            }
            set
            {
                if (value != null)
                {
                    _workingDir = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public BuildDirectoryPath ConfigurationDirectory
        {
            get
            {
                return _configDir;
            }
            set
            {
                if (value != null)
                {
                    _configDir = value;
                }
            }
        }

        public BuildDirectoryPath ContentsDirectory
        {
            get
            {
                return _contentDir;
            }
            set
            {
                if (value != null)
                {
                    _contentDir = value;
                }
            }
        }

        public BuildDirectoryPath SandcastleDirectory
        {
            get 
            { 
                return _sandcastleDir; 
            }
            set 
            {
                if (value != null)
                {
                    _sandcastleDir = value; 
                }
            }
        }

        public BuildDirectoryPath SandAssistDirectory
        {
            get 
            { 
                return _sandassistDir; 
            }
            set 
            {
                if (value != null)
                {
                    _sandassistDir = value; 
                }
            }
        }

        public BuildDirectoryPath OutputDirectory
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

        public bool ShowUpdatedDate
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

        public BuildToc Toc
        {
            get
            {
                return _outputToc;
            }
            set
            {
                if (value != null)
                {
                    _outputToc = value;
                }
            }
        }

        public BuildLogging Logging
        {
            get
            {
                return _outputLogging;
            }
            set
            {
                if (value != null)
                {
                    _outputLogging = value;
                }
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

        public BuildEngineSettingsList EngineSettings
        {
            get
            {
                return _listEngineSettings;
            }
        }

        public SharedContent SharedContent
        {
            get
            {
                return _sharedContent;
            }
        }

        public IncludeContent IncludeContent
        {
            get
            {
                return _includeContent;
            }
        }

        public AttributeContent Attributes
        {
            get
            {
                return _attributeContent;
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
                return _properties[key];
            }
            set
            {
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
                return _properties.Keys;
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
                return _properties.Values;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (!this.IsInitialized)
            {
                return;
            }

            _outputToc.Initialize(context);
            _outputStyle.Initialize(context);
            _outputLogging.Initialize(context);
            _outputFeedback.Initialize(context);

            for (int i = 0; i < _listEngineSettings.Count; i++)
            {
                _listEngineSettings[i].Initialize(context);
            }

            // 2. Process the locale
            CultureInfo culture = this.CultureInfo;
            string tempText = culture.Name.ToLower(culture);
            _sharedContent.Add(new SharedItem("helpLocale", tempText));

            // For the header...
            // 1. Process the preliminary text...
            if (this.ShowPreliminary)
            {
                _sharedContent.Add(new SharedItem("preliminaryStatement",
                    "<include item=\"preliminaryText\"/>"));
            }
            // 2. Process the running header text...  
            tempText = this.HelpTitle;
            _sharedContent.Add(new SharedItem("runningHeaderText", tempText));
            // 3. Process the header text...
            tempText = this.HeaderText;
            if (!String.IsNullOrEmpty(tempText))
            {
                _sharedContent.Add(new SharedItem("headerStatement",
                    "<include item=\"headerText\"/>"));
                _sharedContent.Add(new SharedItem("headerText", tempText));
            }

            // For the footer...
            tempText = this.FooterText;
            if (!String.IsNullOrEmpty(tempText))
            {
                _sharedContent.Add(new SharedItem("footerStatement",
                    "<include item=\"footerText\"/>"));
                _sharedContent.Add(new SharedItem("footerText", tempText));
            }

            // For the feedback...
            // 1. Process the email...
            _sharedContent.Add(new SharedItem("feedbackEmail", _outputFeedback.EmailAddress));
            // 2. Process the product...
            _sharedContent.Add(new SharedItem("feedbackProduct", _outputFeedback.Product));
            // 3. Process the product...
            _sharedContent.Add(new SharedItem("feedbackCompany", _outputFeedback.Company));
            // 4. Process the copyright...
            tempText = _outputFeedback.Copyright;
            if (!String.IsNullOrEmpty(tempText))
            {
                _sharedContent.Add(new SharedItem("copyrightStatement",
                    "<include item=\"copyrightText\"/>"));
                string copyUri = _outputFeedback.CopyrightLink;
                if (!String.IsNullOrEmpty(copyUri))
                {
                    tempText = String.Format("<a href=\"{0}\">{1}</a>",
                        copyUri, tempText);
                }
                _sharedContent.Add(new SharedItem("copyrightText", tempText));
            }
        }

        public override void Uninitialize()
        {
            for (int i = 0; i < _listEngineSettings.Count; i++)
            {
                _listEngineSettings[i].Uninitialize();
            }

            _outputToc.Uninitialize();
            _outputStyle.Uninitialize();
            _outputLogging.Uninitialize();
            _outputFeedback.Uninitialize();
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
            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void Clear()
        {
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

            if (_includeContent != null)
            {
                settings._includeContent = _includeContent.Clone();
            }

            return settings;
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        #endregion
    }
}
