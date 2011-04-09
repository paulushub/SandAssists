using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    public sealed class StepHxsBuilder : BuildStep
    {
        #region Private Fields

        //private Regex regex = new Regex(
        //    @"<\s*Title\s*>(?<title>[\s|\S]*)<\s*/Title\s*>", 
        //    RegexOptions.IgnoreCase);
        private int          _langId;
        private int          _copyright;
        private int          _messageCount;

        private bool         _sampleInfo;
        private bool         _indexSort;
        private bool         _indexMerge;
        private bool         _indexAutoInclude;

        private string       _helpDir;
        private string       _helpToc;
        private string       _helpName;
        private string       _helpFolder;
        private string       _helpTitleId;
        private Version      _helpFileVersion;
        private CultureInfo  _helpCulture;

        private string       _defaultPage;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        [NonSerialized]
        private BuildLogger  _logger;
        [NonSerialized]
        private BuildSettings _settings;
        [NonSerialized]
        private FormatHxs _buildFormat;
        [NonSerialized]
        private XmlWriterSettings _xmlSettings;

        #endregion

        #region Constructors and Destructor

        public StepHxsBuilder()
            : this(String.Empty)
        {
            this.LogTitle = "Building Help 2.x contents";
            _lastLevel   = BuildLoggerLevel.None;
            _verbosity   = BuildLoggerVerbosity.None;
        }

        public StepHxsBuilder(string workingDir)
            : base(workingDir)
        {
            _langId           = 1033;
            _helpFolder       = "MsdnHelp";
            _helpFileVersion  = new Version(1, 0, 0, 0);

            _indexSort        = true;
            _indexMerge       = true;
            _indexAutoInclude = true;

            this.LogTitle      = "Building Help 2.x contents";
            _lastLevel        = BuildLoggerLevel.None;
            _verbosity        = BuildLoggerVerbosity.None;
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
                _helpName = value;
            }
        }

        public string HelpFolder
        {
            get
            {
                return _helpFolder;
            }
            set
            {
                _helpFolder = value;
            }
        }

        public Version HelpFileVersion
        {
            get
            {
                return _helpFileVersion;
            }
            set
            {
                if (value != null)
                {
                    _helpFileVersion = value;
                }
            }
        }

        public string HelpToc
        {
            get 
            { 
                return _helpToc; 
            }
            set 
            { 
                _helpToc = value; 
            }
        }

        public string HelpTitleId
        {
            get
            {
                return _helpTitleId;
            }
            set
            {
                if (value == null)
                {
                    _helpTitleId = value;
                    return;
                }
                value = value.Trim();

                if (!String.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(' ') >= 0)
                    {
                        throw new BuildException("The help file title ID cannot contain space.");
                    }
                }
                _helpTitleId = value;
            }
        }

        public CultureInfo HelpCultureInfo
        {
            get
            {
                return _helpCulture;
            }
            set
            {
                _helpCulture = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            string workingDir = this.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new BuildException("A working directory is required.");
            }

            if (String.IsNullOrEmpty(_helpName) || String.IsNullOrEmpty(_helpToc))
            {
                throw new BuildException("The required property values are set.");
            }

            _logger = context.Logger;
            if (_logger != null)
            {
                _verbosity = _logger.Verbosity;
            }

            bool buildResult = false;

            _settings = context.Settings;
            BuildFormatList formats = _settings.Formats;
            if (formats == null || formats.Count == 0)
            {
                return buildResult;
            }
            FormatHxs hxsFormat = 
                formats[BuildFormatType.HtmlHelp2] as FormatHxs;

            if (hxsFormat == null || hxsFormat.Enabled == false)
            {
                return buildResult;
            }
            _buildFormat = hxsFormat;
            string helpTitleId = _helpTitleId;
            if (helpTitleId != null)
            {
                helpTitleId = helpTitleId.Trim();
            }
            if (String.IsNullOrEmpty(helpTitleId))
            {
                helpTitleId = _helpName;
            }
            _helpTitleId = helpTitleId;

            // We create a common XML Settings for all the writers...
            _xmlSettings = new XmlWriterSettings();
            _xmlSettings.Encoding           = Encoding.UTF8;
            _xmlSettings.Indent             = true;
            _xmlSettings.IndentChars        = "    ";
            _xmlSettings.OmitXmlDeclaration = false;
            _xmlSettings.CloseOutput        = true;

            buildResult = true;
            try
            {
                if (_helpCulture != null)
                {
                    _langId = _helpCulture.LCID;
                }

                // For these properties, we do not mind if set or there is error.
                string tmpText = _buildFormat["IndexAutoInclude"];
                if (String.IsNullOrEmpty(tmpText) == false)
                {
                    try
                    {
                        _indexAutoInclude = Convert.ToBoolean(tmpText);
                    }
                    catch
                    {
                    }
                }
                tmpText = _buildFormat["IndexSort"];
                if (String.IsNullOrEmpty(tmpText) == false)
                {
                    try
                    {
                        _indexSort = Convert.ToBoolean(tmpText);
                    }
                    catch
                    {
                    }
                }
                tmpText = _buildFormat["IndexMerge"];
                if (String.IsNullOrEmpty(tmpText) == false)
                {
                    try
                    {
                        _indexMerge = Convert.ToBoolean(tmpText);
                    }
                    catch
                    {
                    }
                }
                tmpText = _buildFormat["SampleInfo"];
                if (String.IsNullOrEmpty(tmpText) == false)
                {
                    try
                    {
                        _sampleInfo = Convert.ToBoolean(tmpText);
                    }
                    catch
                    {
                    }
                }

                _helpDir = Path.Combine(workingDir, _helpFolder);
                if (Directory.Exists(_helpDir) == false)
                {
                    Directory.CreateDirectory(_helpDir);
                }

                buildResult = CreateContents(context);
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateProject(context);
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateToc(context);
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateIndex(context);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                buildResult = false;
            }

            return buildResult;
        }

        #endregion

        #endregion

        #region Private Methods

        #region CreateIndex Method

        private bool CreateIndex(BuildContext context)
        {
            string langText = _langId.ToString();

            // 1. For the A...
            string filePath = Path.Combine(_helpDir, _helpName + "_A.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "A");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteFullEndElement();
            }

            // 2. For the B...
            filePath = Path.Combine(_helpDir, _helpName + "_B.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "B");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteFullEndElement();
            }

            // 3. For the F...
            filePath = Path.Combine(_helpDir, _helpName + "_F.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "F");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteFullEndElement();
            }

            // 4. For the K...
            filePath = Path.Combine(_helpDir, _helpName + "_K.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "K");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("FileVersion", 
                    _helpFileVersion.ToString());
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteAttributeString("Visible", "Yes");
                xmlWriter.WriteFullEndElement();
            }

            // 5. For the S...
            filePath = Path.Combine(_helpDir, _helpName + "_S.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "S");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteFullEndElement();
            }

            // 6. For the N...
            filePath = Path.Combine(_helpDir, _helpName + "_N.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");  // start - HelpIndex
                xmlWriter.WriteAttributeString("Name", "NamedUrl");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langText);

                //<Keyword Term="HomePage">
                //    <Jump Url="" />
                //</Keyword>
                IList<AttributeItem> items = CreateNamedUrl();
                int itemCount = (items == null) ? 0 : items.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    AttributeItem item = items[i];
                    xmlWriter.WriteStartElement("Keyword");
                    xmlWriter.WriteAttributeString("Term", item.Name);

                    xmlWriter.WriteStartElement("Jump");
                    xmlWriter.WriteAttributeString("Url", item.Value);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();               // end - HelpIndex
            }

            return true;
        }

        #endregion

        #region CreateContents Method

        private bool CreateContents(BuildContext context)
        {
            IList<string> listSources = new List<string>();
            listSources.Add(@"icons\*.gif");
            listSources.Add(@"scripts\*.js");
            listSources.Add(@"styles\*.css");

            // The images, maths and media directories may be empty...
            string tempDir = Path.Combine(_helpDir, "images");
            if (Directory.Exists(tempDir) && 
                !DirectoryUtils.IsDirectoryEmpty(tempDir))
            {
                listSources.Add(@"images\*.*");
            }

            tempDir = Path.Combine(_helpDir, "maths");
            if (Directory.Exists(tempDir) &&
                !DirectoryUtils.IsDirectoryEmpty(tempDir))
            {
                listSources.Add(@"maths\*.*");
            }
            
            tempDir = Path.Combine(_helpDir, "media");
            if (Directory.Exists(tempDir) &&
                !DirectoryUtils.IsDirectoryEmpty(tempDir))
            {
                listSources.Add(@"media\*.*");
            }

            listSources.Add(@"html\*.htm");
            //listSources.Add();

            int itemCount = listSources.Count;

            string filePath = Path.Combine(_helpDir, _helpName + ".HxF");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {
                xmlWriter.WriteDocType("HelpFileList", null,
                    "MS-Help://Hx/Resources/HelpFileList.dtd", null);
                xmlWriter.WriteStartElement("HelpFileList"); // start - HelpFileList
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");

                //<File Url="icons\*.gif" />
                for (int i = 0; i < itemCount; i++)
                {
                    xmlWriter.WriteStartElement("File");
                    xmlWriter.WriteAttributeString("Url", listSources[i]);
                    xmlWriter.WriteEndElement();
                }    

                xmlWriter.WriteEndElement();                 // end - HelpFileList  
            }

            return true;
        }

        #endregion

        #region CreateProject Method

        private bool CreateProject(BuildContext context)
        {
            string workingDir = context.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (_logger != null)
                {
                    _logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            // 1. We create the Hxs project file, HxC...
            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            string sandcastleDir = _settings.StylesDirectory;
            string tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\CreateHxc.xsl");
            string arguments = String.Format(
                "/xsl:\"{0}\" {1} /arg:fileNamePrefix={2} /out:{3}\\{4}.HxC",
                tempText, _helpToc, _helpName, _helpFolder, _helpName);
            startInfo.FileName         = "XslTransform.exe";
            startInfo.Arguments        = arguments;
            startInfo.UseShellExecute  = false;
            startInfo.CreateNoWindow   = true;
            startInfo.WorkingDirectory = workingDir;
            startInfo.RedirectStandardOutput = true;

            _copyright    = 2;
            _messageCount = 0;

            // Add the event handler to receive the console output...
            process.OutputDataReceived += new DataReceivedEventHandler(OnDataReceived);

            // Now, start the process - there will still not be output till...
            process.Start();

            // Start the asynchronous read of the output stream
            process.BeginOutputReadLine();

            // We must wait for the process to complete...
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Close();
            if (exitCode != 0)
            {
                return false;
            }

            // For the unexpected case of no argument options to the
            // XslTransformer tool, the exit code is still 0...
            if (_lastLevel == BuildLoggerLevel.Error)
            {
                return false;
            }

            // 2. We try to clean up the generated file, adding necessary options...
            XmlDocument xmlDoc = new XmlDocument();
            // the document contains DTD, which cannot be resolve...
            xmlDoc.XmlResolver = null;
            string hxcFile = Path.Combine(workingDir,
                String.Format("{0}\\{1}.HxC", _helpFolder, _helpName));

            xmlDoc.Load(hxcFile);

            XmlNode mainNode = xmlDoc.SelectSingleNode("HelpCollection");
            if (mainNode != null)
            {
                // By ignore the processing of the DTD, [] is appended to it, 
                // so we replace it with another same DTD!
                XmlDocumentType docType = xmlDoc.DocumentType;
                xmlDoc.RemoveChild(docType);
                docType = xmlDoc.CreateDocumentType("HelpCollection", null,
                    "MS-Help://Hx/Resources/HelpCollection.dtd", null);
                xmlDoc.InsertBefore(docType, mainNode);

                XmlAttribute attribute = mainNode.Attributes["FileVersion"];
                if (attribute != null)
                {
                    attribute.Value = _helpFileVersion.ToString();
                }
                attribute = mainNode.Attributes["LangId"];
                if (attribute != null)
                {
                    int lcid = 1033;
                    CultureInfo culture = _settings.CultureInfo;
                    if (culture != null)
                    {
                        lcid = culture.LCID;
                    }
                    attribute.Value = lcid.ToString();
                }
                attribute = mainNode.Attributes["Title"];
                if (attribute != null)
                {
                    attribute.Value = _settings.HelpTitle;
                }
                attribute = mainNode.Attributes["Copyright"];
                if (attribute != null)
                {
                    attribute.Value = _settings.Feedback.Copyright;
                }

                string contentsDir = _settings.ContentsDirectory;
                if (String.IsNullOrEmpty(contentsDir) == false && 
                    Directory.Exists(contentsDir))
                {
                    XmlNode optionNode = mainNode.SelectSingleNode("CompilerOptions");
                    string fileSource = Path.Combine(contentsDir, "StopWordList.stp");
                    if (optionNode != null && File.Exists(fileSource) && _buildFormat.IncludeStopWords)
                    {
                        if (_buildFormat.SeparateIndexFile) // the default is combined single Hxs.
                        {
                            attribute = optionNode.Attributes["CompileResult"];
                            if (attribute != null)
                            {
                                attribute.Value = "HxiHxs";  // default value: Hxs
                            }
                        }    

                        string fileDest = Path.Combine(workingDir, _helpFolder + @"\local\StopWordList.stp");
                        string tempDir = Path.GetDirectoryName(fileDest);
                        if (Directory.Exists(tempDir) == false)
                        {
                            Directory.CreateDirectory(tempDir);
                        }
                        string fileContents = File.ReadAllText(fileSource);
                        if (String.IsNullOrEmpty(fileContents) == false)
                        {
                            // Convert the stop word list to Unicode, do not 
                            // know why it is required...
                            File.WriteAllText(fileDest, fileContents,
                                new UnicodeEncoding());

                            attribute = xmlDoc.CreateAttribute("StopWordFile");
                            attribute.Value = @"local\StopWordList.stp";
                            optionNode.Attributes.Append(attribute);
                        }
                    }
                }
            }

            xmlDoc.Save(hxcFile);

            return true;
        }

        #endregion

        #region CreateToc Method

        private bool CreateToc(BuildContext context)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            _copyright    = 2;

            _messageCount = 0;

            // We create the table of contents file; HxT...
            process = new Process();

            startInfo = process.StartInfo;

            string sandcastleDir = _settings.StylesDirectory;
            string tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\TocToHxSContents.xsl");
            string arguments = String.Format("/xsl:\"{0}\" {1} /out:{2}\\{3}.HxT",
                tempText, _helpToc, _helpFolder, _helpName);
            startInfo.FileName         = "XslTransform.exe";
            startInfo.Arguments        = arguments;
            startInfo.UseShellExecute  = false;
            startInfo.CreateNoWindow   = true;
            startInfo.WorkingDirectory = this.WorkingDirectory;
            startInfo.RedirectStandardOutput = true;

            // Add the event handler to receive the console output...
            process.OutputDataReceived += new DataReceivedEventHandler(
                OnDataReceived);

            // Now, start the process - there will still not be output till...
            process.Start();

            // Start the asynchronous read of the output stream
            process.BeginOutputReadLine();

            // We must wait for the process to complete...
            process.WaitForExit();
            int exitCode = process.ExitCode;

            process.Close();

            bool buildResult = (exitCode == 0);

            if (buildResult)
            {
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.IgnoreComments = true;
                xmlSettings.IgnoreProcessingInstructions = true;
                xmlSettings.IgnoreWhitespace = true;
                xmlSettings.ProhibitDtd = false;
                xmlSettings.XmlResolver = null;

                string hxtFile = Path.Combine(context.WorkingDirectory,
                    String.Format("{0}\\{1}.HxT", _helpFolder, _helpName));

                using (XmlReader xmlReader = XmlReader.Create(hxtFile, xmlSettings))
                {
                    xmlReader.MoveToContent();
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Equals(xmlReader.Name, "HelpTOCNode",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                string urlAttribute = xmlReader.GetAttribute("Url");
                                if (String.IsNullOrEmpty(urlAttribute) == false)
                                {
                                    _defaultPage = urlAttribute;
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            return buildResult;
        }

        #endregion

        #region CreateNamedUrl Method

        /// <summary>
        /// A named URL index makes it possible to customize some display aspects of 
        /// a Help collection, such as which page is displayed when a Help collection 
        /// is first opened.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/bb164963(VS.80).aspx">
        /// Implementing Named URL Indexes
        /// </seealso>
        private IList<AttributeItem> CreateNamedUrl()
        {
            if (_defaultPage == null)
            {
                _defaultPage = String.Empty;
            }

            List<AttributeItem> items = new List<AttributeItem>(10);
            string tempText = _buildFormat.HomePage;
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = _defaultPage;
            }
            items.Add(new AttributeItem("HomePage", tempText));
            tempText = _buildFormat.DefaultPage;
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = _defaultPage;
            }
            items.Add(new AttributeItem("DefaultPage",    tempText));

            tempText = _buildFormat.NavFailPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("NavFailPage", tempText));
            }

            tempText = _buildFormat.AboutPageInfo;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("AboutPageInfo", tempText));
            }

            tempText = _buildFormat.AboutPageIcon;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("AboutPageIcon", tempText));
            }

            tempText = _buildFormat.FilterEditPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("FilterEditPage", tempText));
            }

            tempText = _buildFormat.HelpPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("HelpPage", tempText));
            }

            tempText = _buildFormat.SupportPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("SupportPage", tempText));
            }

            tempText = _buildFormat.SampleDirPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("SampleDirPage", tempText));
            }

            tempText = _buildFormat.SearchHelpPage;
            if (String.IsNullOrEmpty(tempText) == false ||
                File.Exists(tempText))
            {
                items.Add(new AttributeItem("SearchHelpPage", tempText));
            }
            
            return items;
        }

        #endregion

        #region OnDataReceived Method

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger == null || _verbosity == BuildLoggerVerbosity.Quiet)
            {
                return;
            }
            _messageCount++;

            if (_messageCount <= _copyright)
            {
                return;
            }

            string textData = e.Data;
            if (String.IsNullOrEmpty(textData))
            {
                return;
            }

            int findPos = textData.IndexOf(':');
            if (findPos <= 0)
            {
                // 1. Check for no options/arguments...
                if (textData.StartsWith("XslTransformer", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                // 2. Check for missing or extra assembly directories...
                if (textData.StartsWith("Specify", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                _lastLevel = BuildLoggerLevel.Info;
                return;
            }

            string levelText = textData.Substring(0, findPos);
            string messageText = textData.Substring(findPos + 1).Trim();
            if (String.Equals(levelText, "Info"))
            {
                if (_verbosity != BuildLoggerVerbosity.Minimal)
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Info);
                }
                _lastLevel = BuildLoggerLevel.Info;
            }
            else if (String.Equals(levelText, "Warn"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Warn);
                _lastLevel = BuildLoggerLevel.Warn;
            }
            else if (String.Equals(levelText, "Error"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                _lastLevel = BuildLoggerLevel.Error;
            }
            else
            {
                // Check for invalid options...
                if (String.Equals(levelText, "?",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "xsl",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "arg",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "w",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Info);
                    _lastLevel = BuildLoggerLevel.None;
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
