using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Sandcastle.Formats;
using Sandcastle.Utilities;
using Sandcastle.Configurators;

namespace Sandcastle.Steps
{
    public sealed class StepChmBuilder : StepProcess
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
                
        private int    _tocWindowStyle;
        private bool   _optimizeStyle;

        private string _tocStyle;
        private string _helpName;
        private string _helpFolder;
        private string _helpSource;
        private string _helpDirectory;

        private FormatChm        _format;
        private FormatChmOptions _options;

        #endregion

        #region Constructors and Destructor

        public StepChmBuilder()
        {
            this.ConstructorDefaults();
        }

        public StepChmBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.ConstructorDefaults();
        }

        public StepChmBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.ConstructorDefaults();
        }

        private void ConstructorDefaults()
        {
            _optimizeStyle = true;
            _helpFolder    = "HtmlHelp";
            this.LogTitle  = "Building the HtmlHelp 1.x help format.";
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

        public string HelpDirectory
        {
            get
            {
                return _helpDirectory;
            }
            set
            {
                _helpDirectory = value;
            }
        }

        /// <summary>
        /// Gets or set a value indicating whether the selected build style 
        /// build for be optimized for this format. 
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the style can be optimized for
        /// this format; otherwise, the stoke style or presentation is not
        /// overwritten or modified. The default is <see langword="true"/>.
        /// </value>
        public bool OptimizeStyle
        {
            get
            {
                return _optimizeStyle;
            }
            set
            {
                _optimizeStyle = value;
            }
        }

        #endregion

        #region Internal Properties

        internal FormatChm Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        internal FormatChmOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            bool buildResult = false;
            try
            {
                buildResult = PreProcess(context, logger);

                if (buildResult)
                {
                    if (_options != null)
                    {
                        _options.TocStyle = _tocStyle;
                        FormatChmHelper chmHelper = new FormatChmHelper(_options);
                        chmHelper.Run(context);
                    }
                    else
                    {
                        buildResult = base.Run(logger);

                        if (buildResult)
                        {
                            buildResult = PostProcess(context, logger);
                        }
                    }

                    this.OptimizeStyles(context);
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

        #region PreProcess Method

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

            BuildSettings settings = context.Settings;

            if (_format == null || !_format.Enabled)
            {
                return buildResult;
            }

            string workingDir  = context.WorkingDirectory;
            string configDir   = settings.ConfigurationDirectory;
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
            _helpSource = Path.Combine(workingDir, _helpFolder);
            if (!Directory.Exists(_helpSource))
            {
                throw new BuildException("The help file sources directory does not exist.");
            }

            string chmBuilder      = String.Empty;
            string finalChmBuilder = String.Empty;
            if (String.IsNullOrEmpty(configDir) == false &&
                Directory.Exists(configDir))
            {
                chmBuilder      = Path.Combine(configDir,  "ChmBuilder.config");
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
            if (!_format.UseFullTextSearch)
            {
                configItem = "Full-text search=No";
            }
            configurator.AddContentItem("fullTextSearch", configItem);

            // 3. For fullTextStopWords: Full text search stop list file=StopWordList.stp
            string fileSource = Path.Combine(contentsDir, "StopWordList.stp");
            if (File.Exists(fileSource) && _format.IncludeStopWords)
            {
                string fileDest = Path.Combine(workingDir, _helpFolder + @"\StopWordList.stp");
                File.Copy(fileSource, fileDest);
                File.SetAttributes(fileDest, FileAttributes.Normal);
                configItem = "Full text search stop list file=StopWordList.stp";
                configurator.AddContentItem("fullTextStopWords", configItem);
            }

            // 4. For autoIndex: Auto Index=Yes
            configItem = "Auto Index=Yes";
            if (!_format.UseAutoIndex)
            {
                configItem = "Auto Index=No";
            }
            configurator.AddContentItem("autoIndex", configItem);

            // 5. For binaryTOC: Binary TOC=Yes
            configItem = "Binary TOC=Yes";
            if (!_format.UseBinaryToc)
            {
                configItem = "Binary TOC=No";
            }
            configurator.AddContentItem("binaryTOC", configItem);

            // 6. For binaryIndex: Binary Index=Yes
            configItem = "Binary Index=Yes";
            if (!_format.UseBinaryIndex)
            {
                configItem = "Binary Index=No";
            }
            configurator.AddContentItem("binaryIndex", configItem);

            // 7. For mainFrame:
            // MainFrame="{3}","{0}.hhc","{0}.hhk","{1}","{1}",,,,,0x43520,,0x387e,[50,50,1050,900],,,,,,,0
            configItem = _format["HelpWindow"];
            // if the use defined the help window, we use it...
            if (String.IsNullOrEmpty(configItem))
            {
                configItem = "MainFrame=\"{3}\",\"{0}.hhc\",\"{0}.hhk\",\"{1}\",\"{1}\",,,,,";
                if (_format.IncludeFavorites)
                {
                    configItem += _format.IncludeAdvancedSearch ? "0x63520,,0x387e" : "0x43520,,0x387e";
                }
                else
                {
                    configItem += _format.IncludeAdvancedSearch ? "0x62520,,0x387e" : "0x42520,,0x387e";
                }
                configItem += ",[50,50,1050,900],,,,,,,0";
            }
            configurator.AddContentItem("mainFrame", configItem);

            configurator.Configure(chmBuilder, finalChmBuilder);

            configurator.Uninitialize(); 

            // 2. We modify the windows styles of the table of contents
            _tocWindowStyle = 0x800000;
            if (_format.TocHasButtons)
            {
                _tocWindowStyle |= TocHasButtons;
            }
            if (_format.TocHasLines)
            {
                _tocWindowStyle |= TocHasLines;
            }
            if (_format.TocLinesAtRoot)
            {
                _tocWindowStyle |= TocLinesAtRoot;
            }
            if (_format.TocShowSelectionAlways)
            {
                _tocWindowStyle |= TocShowSelAlways;
            }
            if (_format.TocTrackSelect)
            {
                _tocWindowStyle |= TocTrackSelect;
            }
            if (_format.TocSingleExpand)
            {
                _tocWindowStyle |= TocSingleExpand;
            }
            if (_format.TocFullrowSelect)
            {
                _tocWindowStyle |= TocFullrowSelect;
            }
            _tocStyle = String.Format("0x{0:x}", _tocWindowStyle);

            buildResult = true;

            return buildResult;
        }

        #endregion

        #region PostProcess Method

        private bool PostProcess(BuildContext context, BuildLogger logger)
        {
            bool buildResult = false;

            // We modify the encoding of the hhp file to fix a bug...
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

           // //<object type="text/site properties">
           // //    <param name="Window Styles" value="0x801627"/>
           // //</object>
           // filePath = Path.Combine(this.WorkingDirectory,
           //     String.Format(@"{0}\{1}.hhc", _helpFolder, _helpName));

           // XmlDocument document = null;
           // try
           // {
           //     document = new XmlDocument();
           //     document.Load(filePath);

           //     XPathNavigator navigator = document.CreateNavigator();

           //     navigator.MoveToChild("body", String.Empty);

           //     XmlWriter xmlWriter = navigator.PrependChild();
           //     xmlWriter.WriteStartElement("object");
           //     xmlWriter.WriteAttributeString("type", "text/site properties");

           //     xmlWriter.WriteStartElement("param");
           //     xmlWriter.WriteAttributeString("name", "Window Styles");
           //     xmlWriter.WriteAttributeString("value", windowStyle);
           //     xmlWriter.WriteEndElement();

           //     xmlWriter.WriteEndElement();

           //     xmlWriter.Close();

           //     XmlWriterSettings xmlSettings = new XmlWriterSettings();

           //     xmlSettings.Indent = true;
           //     xmlSettings.Encoding = new UTF8Encoding(false);
           //     xmlSettings.OmitXmlDeclaration = false;

           //     xmlWriter = XmlWriter.Create(filePath, xmlSettings);

           //     document.Save(xmlWriter);

           //     xmlWriter.Close();

           //     buildResult = true;
           //}
           // catch (Exception ex)
           // {
           //     if (logger != null)
           //     {
           //         logger.WriteLine(ex, BuildLoggerLevel.Error);
           //     }

           //     buildResult = false;
           // }

            return buildResult;
        }

        #endregion

        #region OptimizeStyle Methods

        private void OptimizeStyles(BuildContext context)
        {
            if (!_optimizeStyle)
            {
                return;
            }

            BuildSettings settings   = context.Settings;
            BuildStyleType styleType = settings.Style.StyleType;

            // Currently, only the classic style is available and optimized...
            if (styleType == BuildStyleType.ClassicWhite ||
                styleType == BuildStyleType.ClassicBlue)
            {
                this.OptimizeClassicStyles(context);
            }
        }

        private void OptimizeClassicStyles(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            string startMessage = "Started style optimization for Classic Style";
            string endMessage   = "Completed style optimization for Classic Style";

            logger.WriteLine(startMessage, BuildLoggerLevel.Info);

            BuildSettings settings = context.Settings;
            string sandassistDir = settings.SandAssistDirectory;
            if (!Directory.Exists(sandassistDir))
            {
                logger.WriteLine("Sandcastle Assist directory does not exists.",
                    BuildLoggerLevel.Warn);

                logger.WriteLine(endMessage, BuildLoggerLevel.Info);
                return;
            }

            string formatDir = Path.Combine(sandassistDir,
                @"Optimizations\Vs2005\Chm");
            if (!Directory.Exists(formatDir))
            {
                logger.WriteLine("The format directory does not exists.",
                    BuildLoggerLevel.Warn);

                logger.WriteLine(endMessage, BuildLoggerLevel.Info);
                return;
            }

            // 1. For the icons: the directory must exist and not empty...
            string iconsDir = Path.Combine(formatDir, @"Icons");
            string targetDir = Path.Combine(_helpSource, "icons");
            if (Directory.Exists(iconsDir) && Directory.Exists(targetDir) &&
                !DirectoryUtils.IsDirectoryEmpty(iconsDir))
            {
                try
                {
                    BuildDirCopier dirCopier = new BuildDirCopier();
                    dirCopier.Overwrite = true;
                    dirCopier.Recursive = false;

                    if (logger != null)
                    {
                        logger.WriteLine("Replacing stock icons with: " + iconsDir,
                            BuildLoggerLevel.Info);
                    }

                    int fileCopies = dirCopier.Copy(iconsDir, targetDir);

                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "Total of {0} icons or images replaced.", fileCopies),
                            BuildLoggerLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(ex, BuildLoggerLevel.Error);
                    }
                }
            }

            // 2. For the style-sheets: the directory must exist and not empty...
            string stylesDir = Path.Combine(formatDir, @"Styles");
            targetDir = Path.Combine(_helpSource, "styles");
            if (Directory.Exists(stylesDir) && Directory.Exists(targetDir) &&
                !DirectoryUtils.IsDirectoryEmpty(stylesDir))
            {
                try
                {
                    BuildDirCopier dirCopier = new BuildDirCopier();
                    dirCopier.Overwrite = true;
                    dirCopier.Recursive = false;

                    if (logger != null)
                    {
                        logger.WriteLine("Replacing stock styles with: " + stylesDir,
                            BuildLoggerLevel.Info);
                    }

                    int fileCopies = dirCopier.Copy(stylesDir, targetDir);

                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "Total of {0} styles replaced.", fileCopies),
                            BuildLoggerLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(ex, BuildLoggerLevel.Error);
                    }
                }
            }

            // 3. For the scripts: the directory must exist and not empty...
            string scriptsDir = Path.Combine(formatDir, @"Scripts");
            targetDir = Path.Combine(_helpSource, "scripts");
            if (Directory.Exists(scriptsDir) && Directory.Exists(targetDir) &&
                !DirectoryUtils.IsDirectoryEmpty(scriptsDir))
            {
                try
                {
                    BuildDirCopier dirCopier = new BuildDirCopier();
                    dirCopier.Overwrite = true;
                    dirCopier.Recursive = false;

                    if (logger != null)
                    {
                        logger.WriteLine("Replacing stock scripts with: " + scriptsDir,
                            BuildLoggerLevel.Info);
                    }

                    int fileCopies = dirCopier.Copy(scriptsDir, targetDir);

                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "Total of {0} scripts replaced.", fileCopies),
                            BuildLoggerLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(ex, BuildLoggerLevel.Error);
                    }
                }
            }

            logger.WriteLine(endMessage, BuildLoggerLevel.Info);
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
