using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Utilities;
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
        #region Public Fields

        public const string TagName = "settings";

        #endregion

        #region Private Fields

        private bool   _cleanIntermediate;
        private bool   _showUpdatedDate;
        private bool   _showPreliminary;

        private bool   _buildReferences;
        private bool   _buildConceptual;

        private string _helpName;
        private string _helpTitle;

        private string _headerText;
        private string _footerText;

        private CultureInfo _cultureInfo;

        private BuildDirectoryPath _outputDir;
        private BuildDirectoryPath _configDir;
        private BuildDirectoryPath _workingDir;
        private BuildDirectoryPath _contentDir;
        private BuildDirectoryPath _sandcastleDir;
        private BuildDirectoryPath _sandassistDir;

        private SharedContent     _sharedContent;
        private IncludeContent    _includeContent;
        private AttributeContent  _attributeContent;

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

            _cultureInfo   = new CultureInfo(1033, false);
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
                return _buildReferences; 
            }
            set 
            { 
                _buildReferences = value; 
            }
        }

        public bool BuildConceptual
        {
            get 
            { 
                return _buildConceptual; 
            }
            set 
            { 
                _buildConceptual = value; 
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return _cultureInfo;
            }
            set
            {
                if (value != null)
                {
                    _cultureInfo = value;
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
                return _showUpdatedDate; 
            }
            set 
            { 
                _showUpdatedDate = value; 
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
            _sharedContent.Add(new SharedItem("feedbackProduct", _outputFeedback.ProductName));
            // 3. Process the product...
            _sharedContent.Add(new SharedItem("feedbackCompany", _outputFeedback.CompanyName));
            // 4. Process the copyright...
            tempText = _outputFeedback.CopyrightText;
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

        #region Private Methods

        #region Read Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "cleanintermediate":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _cleanIntermediate = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "showupdateddate":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _showUpdatedDate = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "showpreliminary":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _showPreliminary = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "buildreferences":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _buildReferences = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "buildconceptual":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _buildConceptual = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "helpname":
                                _helpName = reader.ReadString();
                                break;
                            case "helptitle":
                                _helpTitle = reader.ReadString();
                                break;
                            case "headertext":
                                _headerText = reader.ReadString();
                                break;
                            case "footertext":
                                _footerText = reader.ReadString();
                                break;
                            case "cultureinfo":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _cultureInfo = new CultureInfo(tempText, false);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlDirectories Method

        private void ReadXmlDirectories(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "directories"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.IsEmptyElement && String.Equals(reader.Name,
                        "directory", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("type");
                        BuildDirectoryPath dirPath = BuildDirectoryPath.ReadLocation(reader);

                        switch (tempText.ToLower())
                        {
                            case "output":
                                _outputDir = dirPath;
                                break;
                            case "configuration":
                                _configDir = dirPath;
                                break;
                            case "working":
                                _workingDir = dirPath;
                                break;
                            case "content":
                                _contentDir = dirPath;
                                break;
                            case "sandcastle":
                                _sandcastleDir = dirPath;
                                break;
                            case "sandcastleassist":
                                _sandassistDir = dirPath;
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlContents Method

        private void ReadXmlContents(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "contents"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.IsEmptyElement && String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "shared":
                                if (_sharedContent == null)
                                {
                                    _sharedContent = new SharedContent();
                                }
                                if (reader.ReadToDescendant(SharedContent.TagName))
                                {
                                    _sharedContent.ReadXml(reader);
                                }
                                break;
                            case "include":
                                if (_includeContent == null)
                                {
                                    _includeContent = new IncludeContent();
                                }
                                if (reader.ReadToDescendant(IncludeContent.TagName))
                                {
                                    _includeContent.ReadXml(reader);
                                }
                                break;
                            case "attribute":
                                if (_attributeContent == null)
                                {
                                    _attributeContent = new AttributeContent();
                                }
                                if (reader.ReadToDescendant(AttributeContent.TagName))
                                {
                                    _attributeContent.ReadXml(reader);
                                }
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlFormats Method

        private void ReadXmlFormats(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "formats"));

            if (_listFormats == null || _listFormats.Count == 0)
            {
                _listFormats = new BuildFormatList();

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
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "format",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildFormatType formatType = BuildFormatType.Parse(
                            reader.GetAttribute("type"));

                        BuildFormat format = _listFormats[formatType];

                        if (format == null)
                        {
                            // If not found in the list, we attempt to create
                            // it based on the known types and extensions...
                            if (formatType == BuildFormatType.WebHelp)
                            {
                                format = new FormatWeb();
                            }
                            else if (formatType == BuildFormatType.HtmlHelp1)
                            {
                                format = new FormatChm();
                            }
                            else if (formatType == BuildFormatType.HtmlHelp2)
                            {
                                format = new FormatHxs();
                            }
                            else if (formatType == BuildFormatType.HtmlHelp3)
                            {
                                format = new FormatMhv();
                            }
                            else if (formatType >= BuildFormatType.Custom)
                            {
                                throw new NotImplementedException();
                            }

                            if (format != null)
                            {
                                format.Reset();
                                _listFormats.Add(format);
                            }
                        }

                        if (format != null)
                        {
                            format.ReadXml(reader);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlOptions Method

        private void ReadXmlOptions(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "options"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "option",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "toc":
                                if (_outputToc == null)
                                {
                                    _outputToc = new BuildToc();
                                }
                                _outputToc.ReadXml(reader);
                                break;
                            case "style":
                                if (_outputStyle == null)
                                {
                                    _outputStyle = new BuildStyle();
                                }
                                _outputStyle.ReadXml(reader);
                                break;
                            case "logging":
                                if (_outputLogging == null)
                                {
                                    _outputLogging = new BuildLogging();
                                }
                                _outputLogging.ReadXml(reader);
                                break;
                            case "feedback":
                                if (_outputFeedback == null)
                                {
                                    _outputFeedback = new BuildFeedback();
                                }
                                _outputFeedback.ReadXml(reader);
                                break;
                            default:
                                throw new NotImplementedException(
                                    reader.GetAttribute("type"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlEngineSettings Method

        private void ReadXmlEngineSettings(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "engineSettings"));

            if (_listEngineSettings == null || _listEngineSettings.Count != 2)
            {
                _listEngineSettings = new BuildEngineSettingsList();
                _listEngineSettings.Add(new ReferenceEngineSettings());
                _listEngineSettings.Add(new ConceptualEngineSettings());
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "engineSetting", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("type");
                        if (String.Equals(tempText, "Reference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            ReferenceEngineSettings engineSettings =
                                _listEngineSettings[BuildEngineType.Reference] as ReferenceEngineSettings;
                            if (engineSettings == null)
                            {
                                throw new BuildException(
                                    "The engine-settings section of the settings is invalid.");
                            }

                            engineSettings.ReadXml(reader);
                        }
                        else if (String.Equals(tempText, "Conceptual",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            ConceptualEngineSettings engineSettings =
                                _listEngineSettings[BuildEngineType.Conceptual] as ConceptualEngineSettings;
                            if (engineSettings == null)
                            {
                                throw new BuildException(
                                    "The engine-settings section of the settings is invalid.");
                            }

                            engineSettings.ReadXml(reader);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Write Methods

        private static void WriteDirectory(XmlWriter writer, string type,
            BuildDirectoryPath directoryPath)
        {
            if (directoryPath == null || !directoryPath.IsValid)
            {
                return;
            }
            writer.WriteStartElement("directory");
            writer.WriteAttributeString("type", String.IsNullOrEmpty(type) ? String.Empty : type);
            directoryPath.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, "The processing of the settings ReadXml is not valid");
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "propertygroup":
                            this.ReadXmlGeneral(reader);
                            break;
                        case "directories":
                            this.ReadXmlDirectories(reader);
                            break;
                        case "outputfolders":
                            if (_outputFolders == null || _outputFolders.Count != 0)
                            {
                                _outputFolders = new BuildList<string>();
                            }
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (String.Equals(reader.Name, "outputFolder",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        string tempText = reader.GetAttribute("name");
                                        if (!String.IsNullOrEmpty(tempText))
                                        {
                                            _outputFolders.Add(tempText);
                                        }
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (String.Equals(reader.Name, "outputFolders",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case "contents":
                            this.ReadXmlContents(reader);
                            break;
                        case "formats":
                            this.ReadXmlFormats(reader);
                            break;
                        case "options":
                            this.ReadXmlOptions(reader);
                            break;
                        case "enginesettings":
                            this.ReadXmlEngineSettings(reader);
                            break;
                        case "propertybag":
                            if (_properties == null)
                            {
                                _properties = new BuildProperties();
                            }
                            _properties.ReadXml(reader);
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.Name);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - settings
                
            // A. Settings: General settings
            writer.WriteComment(" A. Settings: General settings ");
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("CleanIntermediate", _cleanIntermediate.ToString());
            writer.WritePropertyElement("ShowUpdatedDate", _showUpdatedDate.ToString());
            writer.WritePropertyElement("ShowPreliminary", _showPreliminary.ToString());
            writer.WritePropertyElement("BuildReferences", _buildReferences.ToString());
            writer.WritePropertyElement("BuildConceptual", _buildConceptual.ToString());
            writer.WritePropertyElement("HelpName", _helpName);
            writer.WritePropertyElement("HelpTitle", _helpTitle);
            writer.WritePropertyElement("HeaderText", _headerText);
            writer.WritePropertyElement("FooterText", _footerText);
            writer.WritePropertyElement("CultureInfo", _cultureInfo.ToString());
            writer.WriteEndElement();             // end - propertyGroup

            // B. Settings: User properties
            writer.WriteComment(" B. Settings: User properties ");
            if (_properties != null)
            {
                _properties.WriteXml(writer);
            }

            // C. Settings: Various directories in the settings
            writer.WriteComment(" C. Settings: Various directories in the settings ");
            writer.WriteStartElement("directories");  // start - directories
            WriteDirectory(writer, "Output", _outputDir);
            WriteDirectory(writer, "Configuration", _configDir);
            WriteDirectory(writer, "Working", _workingDir);
            WriteDirectory(writer, "Content", _contentDir);
            WriteDirectory(writer, "Sandcastle", _sandcastleDir);
            WriteDirectory(writer, "SandcastleAssist", _sandassistDir);
            writer.WriteEndElement();                 // end - directories

            // D. Settings: List of the output folders
            writer.WriteComment(" D. Settings: List of the output folders ");
            writer.WriteStartElement("outputFolders");  // start - outputFolders
            if (_outputFolders != null && _outputFolders.Count != 0)
            {
                for (int i = 0; i < _outputFolders.Count; i++)
                {
                    string outputFolder = _outputFolders[i];
                    if (!String.IsNullOrEmpty(outputFolder))
                    {
                        writer.WriteStartElement("outputFolder");
                        writer.WriteAttributeString("name", outputFolder);
                        writer.WriteEndElement();
                    }
                }
            }
            writer.WriteEndElement();                   // end - outputFolders

            // E. Settings: The common contents
            writer.WriteComment(" E. Settings: The common contents ");
            writer.WriteStartElement("contents");  // start - contents
            if (_sharedContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Shared");
                _sharedContent.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_includeContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Include");
                _includeContent.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_attributeContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Attribute");
                _attributeContent.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();              // end - contents

            // F. Settings: All available formats or output targets
            writer.WriteComment(" F. Settings: All available formats or output targets ");
            writer.WriteStartElement("formats");  // start - formats
            if (_listFormats != null && _listFormats.Count != 0)
            {
                for (int i = 0; i < _listFormats.Count; i++)
                {
                    BuildFormat format = _listFormats[i];
                    // We save all, even if not enabled, since there may be customization
                    if (format != null)
                    {
                        writer.WriteComment(String.Format(
                            " For the output/target: {0} ", format.Name));
                        format.WriteXml(writer);
                    }
                }
            }
            writer.WriteEndElement();             // end - formats

            // G. Settings: The various options
            writer.WriteComment(" G. Settings: The various options ");
            writer.WriteStartElement("options");  // start - options
            if (_outputToc != null)
            {
                writer.WriteComment(" Options for custom table of contents ");
               _outputToc.WriteXml(writer);
            }
            if (_outputStyle != null)
            {
                writer.WriteComment(" Options for currently selected style ");
                _outputStyle.WriteXml(writer);
            }
            if (_outputLogging != null)
            {
                writer.WriteComment(" Options for logging progress messages ");
                _outputLogging.WriteXml(writer);
            }
            if (_outputFeedback != null)
            {
                writer.WriteComment(" Options for product information used for feedback ");
                _outputFeedback.WriteXml(writer);
            }
            writer.WriteEndElement();             // end - options

            // H. Settings: Build engine specific settings
            writer.WriteComment(" H. Settings: Build engine specific settings ");
            writer.WriteStartElement("engineSettings");  // start - engineSettings
            if (_listEngineSettings != null && _listEngineSettings.Count != 0)
            {
                for (int i = 0; i < _listEngineSettings.Count; i++)
                {
                    BuildEngineSettings engineSettings = _listEngineSettings[i];
                    if (engineSettings != null)
                    {
                        BuildEngineType engineType = engineSettings.EngineType;
                        if (engineType == BuildEngineType.Reference)
                        {
                            writer.WriteComment(" Reference build engine specific settings ");
                        }
                        else if (engineType == BuildEngineType.Conceptual)
                        {
                            writer.WriteComment(" Conceptual build engine specific settings ");
                        }
                        else if (engineType == BuildEngineType.Custom)
                        {
                            writer.WriteComment(" Custom build engine specific settings ");
                        }

                        // finally, write the engine settings...
                        engineSettings.WriteXml(writer);
                    }
                }

            }
            writer.WriteEndElement();                    // end - engineSettings

            writer.WriteEndElement();           // end - settings
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
