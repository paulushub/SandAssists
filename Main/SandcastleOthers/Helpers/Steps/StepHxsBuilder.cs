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

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepHxsBuilder : BuildStep
    {
        #region Private Fields

        private int          _formatIndex;
        private int          _copyright;
        private int          _messageCount;
        private string       _helpDir;
        private string       _helpToc;
        private string       _helpName;
        private string       _helpFolder;
        private Version      _helpFileVersion;
        private CultureInfo  _helpCulture;

        private string       _defaultPage;

        [NonSerialized]
        private BuildLogger _logger;

        #endregion

        #region Constructors and Destructor

        public StepHxsBuilder()
        {
            _formatIndex     = -1;
            _helpFolder      = "MsdnHelp";
            _helpFileVersion = new Version(1, 0, 0, 0);
        }

        public StepHxsBuilder(string workingDir)
            : base(workingDir)
        {
            _formatIndex     = -1;
            _helpFolder      = "MsdnHelp";
            _helpFileVersion = new Version(1, 0, 0, 0);
        }

        public StepHxsBuilder(StepHxsBuilder source)
            : base(source)
        {
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

        protected override bool MainExecute(BuildEngine engine)
        {
            if (String.IsNullOrEmpty(_helpName) || String.IsNullOrEmpty(_helpToc))
            {
                throw new InvalidOperationException(
                    "The required property values are set.");
            }

            BuildLogger logger = engine.Logger;
            _logger = logger;

            bool buildResult = false;
            FormatHxs hxsFormat = null;

            BuildSettings settings = engine.Settings;
            IList<BuildFormat> formats = settings.OutputFormats;
            if (formats == null || formats.Count == 0)
            {
                return buildResult;
            }

            _formatIndex = -1;
            int itemCount = formats.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildFormat format = formats[i];
                if (format != null && format.FormatType == BuildFormatType.Hxs)
                {
                    _formatIndex = i;
                    hxsFormat = (FormatHxs)format;
                    break;
                }
            }

            if (hxsFormat == null || hxsFormat.Enabled == false)
            {
                return buildResult;
            }

            buildResult = true;
            try
            {
                _helpDir = Path.Combine(this.WorkingDirectory, _helpFolder);
                if (Directory.Exists(_helpDir) == false)
                {
                    Directory.CreateDirectory(_helpDir);
                }

                buildResult = CreateContents();
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateCollection(settings, hxsFormat);
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateTableOfContent(settings, hxsFormat);
                if (buildResult == false)
                {
                    return buildResult;
                }

                buildResult = CreateIndex(settings, hxsFormat);

                return buildResult;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region CreateIndex Method

        protected virtual bool CreateIndex(BuildSettings settings,
            FormatHxs hxsFormat)
        {
            if (_defaultPage == null)
            {
                _defaultPage = String.Empty;
            }

            //TODO-Move the repeated write pattern/code to a separate method...
            int langId = 1033;
            if (_helpCulture != null)
            {
                langId = _helpCulture.LCID;
            }

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = "\t";
            xmlSettings.OmitXmlDeclaration = false;

            // 1. For the A...
            string filePath = Path.Combine(_helpDir, _helpName + "_A.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "A");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langId.ToString());
                xmlWriter.WriteFullEndElement();
            }

            // 2. For the B...
            filePath = Path.Combine(_helpDir, _helpName + "_B.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "B");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langId.ToString());
                xmlWriter.WriteFullEndElement();
            }

            // 3. For the F...
            filePath = Path.Combine(_helpDir, _helpName + "_F.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "F");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langId.ToString());
                xmlWriter.WriteFullEndElement();
            }

            // 4. For the K...
            filePath = Path.Combine(_helpDir, _helpName + "_K.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "K");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("FileVersion", 
                    _helpFileVersion.ToString());
                xmlWriter.WriteAttributeString("LangId", langId.ToString());
                xmlWriter.WriteAttributeString("Visible", "Yes");
                xmlWriter.WriteFullEndElement();
            }

            // 5. For the S...
            filePath = Path.Combine(_helpDir, _helpName + "_S.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "S");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langId.ToString());
                xmlWriter.WriteFullEndElement();
            }

            // 6. For the N...
            filePath = Path.Combine(_helpDir, _helpName + "_N.HxK");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, 
                    "MS-Help://Hx/Resources/HelpIndex.dtd", null);
                xmlWriter.WriteStartElement("HelpIndex");  // start - HelpIndex
                xmlWriter.WriteAttributeString("Name", "NamedUrl");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("LangId", langId.ToString());


                //<Keyword Term="HomePage">
                //    <Jump Url="" />
                //</Keyword>
                IList<PropertyItem> items = CreateNamedUrl(hxsFormat);
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    PropertyItem item = items[i];
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

        protected virtual bool CreateContents()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = false;

            IList<string> listSources = new List<string>();
            listSources.Add(@"icons\*.gif");
            listSources.Add(@"scripts\*.js");
            listSources.Add(@"styles\*.css");
            listSources.Add(@"images\*.*");
            listSources.Add(@"math\*.*");
            listSources.Add(@"media\*.*");
            listSources.Add(@"html\*.htm");
            //listSources.Add();

            int itemCount = listSources.Count;

            string filePath = Path.Combine(_helpDir, _helpName + ".HxF");
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, settings))
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

        #region CreateCollection Method

        protected virtual bool CreateCollection(BuildSettings settings, 
            FormatHxs hxsFormat)
        {
            string workingDir = settings.WorkingDirectory;
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

            string sandcastleDir = settings.StylesDirectory;
            string tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\CreateHxc.xsl");
            string arguments = String.Format(
                "/xsl:\"{0}\" {1} /arg:fileNamePrefix={2} /out:{3}\\{4}.HxC",
                tempText, _helpToc, _helpName, _helpFolder, _helpName);
            startInfo.FileName         = "XslTransform.exe";
            startInfo.Arguments        = arguments;
            startInfo.UseShellExecute  = false;
            startInfo.WorkingDirectory = settings.WorkingDirectory;
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

            // 2. We try to clean up the generated file, adding necessary options...
            XmlDocument xmlDoc = new XmlDocument();
            // the document contains DTD, which cannot be resolve...
            xmlDoc.XmlResolver = null;
            string hxcFile = Path.Combine(settings.WorkingDirectory,
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
                    CultureInfo culture = settings.CultureInfo;
                    if (culture != null)
                    {
                        lcid = culture.LCID;
                    }
                    attribute.Value = lcid.ToString();
                }
                attribute = mainNode.Attributes["Title"];
                if (attribute != null)
                {
                    attribute.Value = settings.HelpTitle;
                }
                attribute = mainNode.Attributes["Copyright"];
                if (attribute != null)
                {
                    attribute.Value = settings.CopyrightText;
                }

                string contentsDir = settings.ContentsDirectory;
                if (String.IsNullOrEmpty(contentsDir) == false && 
                    Directory.Exists(contentsDir))
                {
                    XmlNode optionNode = mainNode.SelectSingleNode("CompilerOptions");
                    string fileSource = Path.Combine(contentsDir, "StopWordList.stp");
                    if (optionNode != null && File.Exists(fileSource) && hxsFormat.IncludeStopWords)
                    {
                        if (hxsFormat.SeparateIndexFile) // the default is combined single Hxs.
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

        #region CreateTableOfContent Method

        protected virtual bool CreateTableOfContent(BuildSettings settings, 
            FormatHxs hxsFormat)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            _copyright    = 2;

            _messageCount = 0;

            // We create the table of contents file; HxT...
            process = new Process();

            startInfo = process.StartInfo;

            string sandcastleDir = settings.StylesDirectory;
            string tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\TocToHxSContents.xsl");
            string arguments = String.Format("/xsl:\"{0}\" {1} /out:{2}\\{3}.HxT",
                tempText, _helpToc, _helpFolder, _helpName);
            startInfo.FileName = "XslTransform.exe";
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
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

                string hxtFile = Path.Combine(settings.WorkingDirectory,
                    String.Format("{0}\\{1}.HxT", _helpFolder, _helpName));

                using (XmlReader xmlReader = XmlReader.Create(hxtFile, xmlSettings))
                {
                    xmlReader.MoveToContent();
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Equals(xmlReader.Name, "HelpTOCNode",
                                StringComparison.CurrentCultureIgnoreCase))
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
        /// <param name="hxsFormat"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/bb164963(VS.80).aspx">
        /// Implementing Named URL Indexes
        /// </seealso>
        private IList<PropertyItem> CreateNamedUrl(FormatHxs hxsFormat)
        {
            List<PropertyItem> items = new List<PropertyItem>(10);
            string tempText = hxsFormat.HomePage;
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = _defaultPage;
            }
            items.Add(new PropertyItem("HomePage", tempText));
            tempText = hxsFormat.DefaultPage;
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = _defaultPage;
            }
            items.Add(new PropertyItem("DefaultPage",    tempText));
            items.Add(new PropertyItem("NavFailPage",    hxsFormat.NavFailPage));
            items.Add(new PropertyItem("AboutPageInfo",  hxsFormat.AboutPageInfo));
            items.Add(new PropertyItem("AboutPageIcon",  hxsFormat.AboutPageIcon));
            items.Add(new PropertyItem("FilterEditPage", hxsFormat.FilterEditPage));
            items.Add(new PropertyItem("HelpPage",       hxsFormat.HelpPage));
            items.Add(new PropertyItem("SupportPage",    hxsFormat.SupportPage));
            items.Add(new PropertyItem("SampleDirPage",  hxsFormat.SampleDirPage));
            items.Add(new PropertyItem("SearchHelpPage", hxsFormat.SearchHelpPage));
            
            return items;
        }

        #endregion

        #region OnDataReceived Method

        protected virtual void OnDataReceived(object sender, 
            DataReceivedEventArgs e)
        {
            _messageCount++;
            if (_logger != null)
            {
                if (_messageCount > _copyright)
                {
                    _logger.WriteLine(e.Data, BuildLoggerLevel.None);
                }
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepHxsBuilder buildStep = new StepHxsBuilder(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
