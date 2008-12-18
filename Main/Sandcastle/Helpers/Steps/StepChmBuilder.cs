using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Sandcastle.Formats;
using Sandcastle.Configurations;

namespace Sandcastle.Steps
{
    public class StepChmBuilder : StepProcess
    {
        #region Private Const Fields

        // TVS_* values defined in the Windows commctrl.h header file.
        private const int TocHasButtons    = 0x0001;
        private const int TocHasLines      = 0x0002;
        private const int TocLinesAtRoot   = 0x0004;
        private const int TocShowSelAlways = 0x0020;
        private const int TocTrackSelect   = 0x0200;
        private const int TocSingleExpand  = 0x0400;
        private const int TocFullrowSelect = 0x1000;

        // With the exception of the Right-to-Left reading, the following are
        // not supported for the display of the table of content tree.
        private const int TocRTLReading      = 0x0040;
        private const int TocNoTooltips      = 0x0080;
        private const int TocCheckBoxes      = 0x0100;
        private const int TocInfoTip         = 0x0800;
        private const int TocEditLabels      = 0x0008;
        private const int TocDisableDragDrop = 0x0010;

        #endregion

        #region Private Fields

        private int    _formatIndex;
        //private int    _tocWindowStyle;
        private string _helpName;
        private string _helpFolder;

        #endregion

        #region Constructors and Destructor

        public StepChmBuilder()
        {
            _helpFolder  = "HtmlHelp";
            this.Message = "ChmBuilder Tool";
        }

        public StepChmBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            _helpFolder  = "HtmlHelp";
            this.Message = "ChmBuilder Tool";
        }

        public StepChmBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            _helpFolder  = "HtmlHelp";
            this.Message = "ChmBuilder Tool";
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

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            //string message = this.Message;
            //if (String.IsNullOrEmpty(message))
            //{
            //    message = "ChmBuilder Tool";
            //}

            bool buildResult = false;
            try
            {
                buildResult = PreProcess(context, logger);

                if (buildResult)
                {
                    buildResult = base.Run(logger);
                }

                if (buildResult)
                {
                    buildResult = PostProcess(context, logger);
                }
            }
            catch (Exception ex)
            {
                buildResult = false;
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            return buildResult;
        }

        #endregion

        #region Private Methods

        private bool PreProcess(BuildContext context, BuildLogger logger)
        {
            bool buildResult = false;

            if (String.IsNullOrEmpty(_helpName) || String.IsNullOrEmpty(_helpFolder))
            {
                throw new BuildException("The required property values are set.");
            }

            if (String.IsNullOrEmpty(this.Application) &&
               String.IsNullOrEmpty(this.Arguments))
            {
                return buildResult;
            }

            FormatChm chmFormat = null;

            BuildSettings settings = context.Settings;
            IList<BuildFormat> formats = settings.Formats;
            if (formats == null || formats.Count == 0)
            {
                return buildResult;
            }

            _formatIndex  = -1;
            int itemCount = formats.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildFormat format = formats[i];
                if (format != null && format.FormatType == BuildFormatType.Chm)
                {
                    _formatIndex = i;
                    chmFormat = (FormatChm)format;
                    break;
                }
            }

            if (chmFormat == null || chmFormat.Enabled == false)
            {
                return buildResult;
            }

            string workingDir = settings.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;
            string contentsDir = settings.ContentsDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return buildResult;
            }
            string helpDir = Path.Combine(workingDir, _helpFolder);

            string chmBuilder = String.Empty;
            string finalChmBuilder = String.Empty;
            if (String.IsNullOrEmpty(configDir) == false &&
                Directory.Exists(configDir))
            {
                chmBuilder = Path.Combine(configDir, "ChmBuilder.config");
                finalChmBuilder = Path.Combine(workingDir, "ChmBuilder.config");
            }
            if (File.Exists(chmBuilder) == false)
            {
                return buildResult;
            }
            GenericConfigurator configurator = new GenericConfigurator();
            configurator.WarnIfNotFound   = false;
            configurator.DeleteIfNotFound = true;
            configurator.Initialize(context);

            // 1. For chmTitle: <title projectName="projectName">projectTitle</title>
            string configItem = String.Format("<title projectName=\"{0}\">{1}</title>",
                settings.HelpName, settings.HelpTitle);
            configurator.AddContentItem("chmTitle", configItem);
            
            // 2. For fullTextSearch: Full-text search=Yes
            configItem = "Full-text search=Yes";
            if (!chmFormat.UseFullTextSearch)
            {
                configItem = "Full-text search=No";
            }
            configurator.AddContentItem("fullTextSearch", configItem);

            // 3. For fullTextStopWords: Full text search stop list file=StopWordList.stp
            string fileSource = Path.Combine(contentsDir, "StopWordList.stp");
            if (File.Exists(fileSource) && chmFormat.IncludeStopWords)
            {
                string fileDest = Path.Combine(workingDir, _helpFolder + @"\StopWordList.stp");
                File.Copy(fileSource, fileDest);
                configItem = "Full text search stop list file=StopWordList.stp";
                configurator.AddContentItem("fullTextStopWords", configItem);
            }

            // 4. For autoIndex: Auto Index=Yes
            configItem = "Auto Index=Yes";
            if (!chmFormat.UseAutoIndex)
            {
                configItem = "Auto Index=No";
            }
            configurator.AddContentItem("autoIndex", configItem);

            // 5. For binaryTOC: Binary TOC=Yes
            configItem = "Binary TOC=Yes";
            if (!chmFormat.UseBinaryToc)
            {
                configItem = "Binary TOC=No";
            }
            configurator.AddContentItem("binaryTOC", configItem);

            // 6. For binaryIndex: Binary Index=Yes
            configItem = "Binary Index=Yes";
            if (!chmFormat.UseBinaryIndex)
            {
                configItem = "Binary Index=No";
            }
            configurator.AddContentItem("binaryIndex", configItem);

            // 7. For mainFrame:
            // MainFrame="{3}","{0}.hhc","{0}.hhk","{1}",,,,,,0x43520,,0x387e,[50,50,1050,900],,,,,,,0
            configItem = chmFormat["HelpWindow"];
            // if the use defined the help window, we use it...
            if (String.IsNullOrEmpty(configItem))
            {
                configItem = "MainFrame=\"{3}\",\"{0}.hhc\",\"{0}.hhk\",\"{1}\",,,,,,";
                if (chmFormat.IncludeFavorites)
                {
                    configItem += chmFormat.IncludeAdvancedSearch ? "0x63520,,0x387e" : "0x43520,,0x387e";
                }
                else
                {
                    configItem += chmFormat.IncludeAdvancedSearch ? "0x62520,,0x387e" : "0x42520,,0x387e";
                }
                configItem += ",[50,50,1050,900],,,,,,,0";
            }
            configurator.AddContentItem("mainFrame", configItem);

            configurator.Configure(chmBuilder, finalChmBuilder);

            configurator.Uninitialize();

            buildResult = true;

            return buildResult;
        }

        private bool PostProcess(BuildContext context, BuildLogger logger)
        {
            bool buildResult = false;

            if (_formatIndex < 0)
            {
                return buildResult;
            }

            // 1. We modify the encoding of the hhp file to fix a bug...
            string filePath = Path.Combine(this.WorkingDirectory,
                String.Format(@"{0}\{1}.hhp", _helpFolder, _helpName));
            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                if (String.IsNullOrEmpty(fileContents) == false)
                {
                    File.WriteAllText(filePath, fileContents,
                        new UTF8Encoding(false));
                }
            }

            //TODO: The current ChmBuilder.exe's output is an invalid HTML so this will
            //      not work!
            //// 2. We modify the windows styles of the table of contents
            //BuildSettings settings = engine.Settings;
            //FormatChm chmFormat = settings.OutputFormats[_formatIndex] as FormatChm;
            //_tocWindowStyle = 0x800000;
            //if (chmFormat.TocHasButtons)
            //{
            //    _tocWindowStyle |= TocHasButtons;
            //}
            //if (chmFormat.TocHasLines)
            //{
            //    _tocWindowStyle |= TocHasLines;
            //}
            //if (chmFormat.TocLinesAtRoot)
            //{
            //    _tocWindowStyle |= TocLinesAtRoot;
            //}
            //if (chmFormat.TocShowSelectionAlways)
            //{
            //    _tocWindowStyle |= TocShowSelAlways;
            //}
            //if (chmFormat.TocTrackSelect)
            //{
            //    _tocWindowStyle |= TocTrackSelect;
            //}
            //if (chmFormat.TocSingleExpand)
            //{
            //    _tocWindowStyle |= TocSingleExpand;
            //}
            //if (chmFormat.TocFullrowSelect)
            //{
            //    _tocWindowStyle |= TocFullrowSelect;
            //}
            //string windowStyle = String.Format("0x{0:x}", _tocWindowStyle);
            ////<OBJECT type="text/site properties">
            ////    <param name="Window Styles" value="0x801627">
            ////</OBJECT>
            //filePath = Path.Combine(this.WorkingDirectory,
            //    String.Format(@"{0}\{1}.hhc", _helpFolder, _helpName));

            //XmlDocument document = new XmlDocument();
            //document.Load(filePath);

            //XPathNavigator navigator = document.CreateNavigator();

            //navigator.MoveToChild("BODY", String.Empty);

            //XmlWriter xmlWriter = navigator.PrependChild();
            //xmlWriter.WriteStartElement("OBJECT");
            //xmlWriter.WriteAttributeString("type", "text/site properties");

            //xmlWriter.WriteStartElement("param");
            //xmlWriter.WriteAttributeString("name", "Window Styles");
            //xmlWriter.WriteAttributeString("value", windowStyle);
            //xmlWriter.WriteEndElement();
            
            //xmlWriter.WriteEndElement();
 
            //xmlWriter.Close();

            //XmlWriterSettings xmlSettings = new XmlWriterSettings();

            //xmlSettings.Indent = true;
            //xmlSettings.Encoding = new UTF8Encoding(false);
            //xmlSettings.OmitXmlDeclaration = false;

            //xmlWriter = XmlWriter.Create(filePath, xmlSettings);

            //document.Save(xmlWriter);

            //xmlWriter.Close();

            buildResult = true;

            return buildResult;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
