using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Win32;

using Ionic.Zip;

using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    public sealed class StepMhvBuilder : BuildStep
    {
        #region Public Fields

        public const string HelpContentSetup = "HelpContentSetup.msha";

        #endregion

        #region Private Fields

        private int    _totalFileEntries;
        private int    _totalFileSaved;

        private bool   _optimizeStyle;
        private string _helpName;
        private string _helpFolder;
        private string _helpDirectory;
        private string _helpSource;
        private string _helpOutputDir;

        private XmlWriter    _metadataWriter;
        private Dictionary<string, string> _contentTypes;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="StepMhvBuilder"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="StepMhvBuilder"/> class
        /// to the default properties or values.
        /// </summary>
        public StepMhvBuilder()
        {
            this.LogTitle = "Creating Microsoft Help Viewer Files";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepMhvBuilder"/> class with the
        /// specified working directory.
        /// </summary>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory.
        /// </param>
        public StepMhvBuilder(string workingDir)
            : base(workingDir)
        {
            this.LogTitle = "Creating Microsoft Help Viewer Files";
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            _contentTypes = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            if (String.IsNullOrEmpty(_helpName)   || 
                String.IsNullOrEmpty(_helpFolder) ||
                String.IsNullOrEmpty(_helpDirectory))
            {
                throw new BuildException("The required property values are set.");
            }
            _helpSource = Path.Combine(this.WorkingDirectory, _helpFolder);
            if (!Directory.Exists(_helpSource))
            {
                throw new BuildException("The help file sources directory does not exist.");
            }

            _helpOutputDir = Path.Combine(_helpDirectory, _helpFolder);
            if (!Directory.Exists(_helpDirectory))
            {
                Directory.CreateDirectory(_helpDirectory);
            }
            if (!Directory.Exists(_helpOutputDir))
            {
                Directory.CreateDirectory(_helpOutputDir);
            }

            this.OptimizeStyles(context);

            this.CreateMsha(context);
            this.CreateMshc(context);

            return true;
        }

        #endregion

        #region Private Methods

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

            string startMessage = "Started style optimization for Classic Style.";
            string endMessage   = "Completed style optimization for Classic Style.";

            logger.WriteLine(startMessage, BuildLoggerLevel.Info);

            BuildSettings settings = context.Settings;
            string sandassistDir   = settings.SandAssistDirectory;
            if (!Directory.Exists(sandassistDir))
            {
                logger.WriteLine("Sandcastle Assist directory does not exists.", 
                    BuildLoggerLevel.Warn);

                logger.WriteLine(endMessage, BuildLoggerLevel.Info);
                return;
            }

            string formatDir       = Path.Combine(sandassistDir, 
                @"Optimizations\Vs2005\Mhv");
            if (!Directory.Exists(formatDir))
            {
                logger.WriteLine("The format directory does not exists.", 
                    BuildLoggerLevel.Warn);

                logger.WriteLine(endMessage, BuildLoggerLevel.Info);
                return;
            }

            // 1. For the icons: the directory must exist and not empty...
            string iconsDir  = Path.Combine(formatDir, @"Icons");
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

        #region CreateMsha Method

        private void CreateMsha(BuildContext context)
        {
            string helpMshaPath = Path.Combine(_helpOutputDir, HelpContentSetup);
            BuildSettings settings = context.Settings;
            BuildFeedback feedback = settings.Feedback;
            string companyName     = feedback.Company;
            string productName     = feedback.Product;
            string displayedName   = settings.HelpTitle;

            XmlWriterSettings readerSettings = new XmlWriterSettings();

            readerSettings.OmitXmlDeclaration = true;
            readerSettings.Indent             = true;
            readerSettings.IndentChars        = "    ";
            readerSettings.Encoding           = Encoding.UTF8;
            using (XmlWriter xmlWriter = XmlWriter.Create(helpMshaPath, readerSettings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("html", "http://www.w3.org/1999/xhtml"); // html
                xmlWriter.WriteStartElement("head"); //head
                xmlWriter.WriteStartElement("title"); // title
                xmlWriter.WriteString("Optional: may be useful for tools used to generate these files.");
                xmlWriter.WriteEndElement(); //title
                xmlWriter.WriteEndElement(); // head

                xmlWriter.WriteStartElement("body");
                xmlWriter.WriteAttributeString("class", "vendor-book");

                xmlWriter.WriteStartElement("div"); //div - details
                xmlWriter.WriteAttributeString("class", "details");
                xmlWriter.WriteStartElement("span"); // span
                xmlWriter.WriteAttributeString("class", "vendor");
                xmlWriter.WriteString(companyName); //must be unique, will be installed dir-name.
                xmlWriter.WriteEndElement(); // span

                xmlWriter.WriteStartElement("span"); // span
                xmlWriter.WriteAttributeString("class", "locale");
                xmlWriter.WriteString(settings.CultureInfo.Name.ToLower());
                xmlWriter.WriteEndElement(); // span

                xmlWriter.WriteStartElement("span"); // span
                xmlWriter.WriteAttributeString("class", "product");
                xmlWriter.WriteString(productName);  
                xmlWriter.WriteEndElement(); // span

                xmlWriter.WriteStartElement("span"); // span
                xmlWriter.WriteAttributeString("class", "name");
                xmlWriter.WriteString(displayedName);
                xmlWriter.WriteEndElement(); // span
                xmlWriter.WriteEndElement(); // div - details

                xmlWriter.WriteStartElement("div"); //div - package-list
                xmlWriter.WriteAttributeString("class", "package-list");

                xmlWriter.WriteStartElement("div"); //div - package
                xmlWriter.WriteAttributeString("class", "package");

                // List out all the packages - only one in this case...
                xmlWriter.WriteStartElement("span"); // span
                xmlWriter.WriteAttributeString("class", "name");
                xmlWriter.WriteString(_helpName);
                xmlWriter.WriteEndElement(); // span

                xmlWriter.WriteStartElement("a"); // a
                xmlWriter.WriteAttributeString("class", "current-link");
                xmlWriter.WriteAttributeString("href", _helpName + ".mshc");
                xmlWriter.WriteString(_helpName + ".mshc");
                xmlWriter.WriteEndElement(); // a

                xmlWriter.WriteEndElement(); // div - package
                xmlWriter.WriteEndElement(); // div - package-list

                xmlWriter.WriteEndElement(); // body

                xmlWriter.WriteEndElement(); // html
                xmlWriter.WriteEndDocument();
            }
        }

        #endregion

        #region CreateMshc Method

        private void CreateMshc(BuildContext context)
        {
            string workingDir = _helpDirectory;

            // 1. Delete any empty folder in the source directory...
            string mediaDir = Path.Combine(_helpSource, "media");
            if (Directory.Exists(mediaDir))
            {
                if (DirectoryUtils.IsDirectoryEmpty(mediaDir))
                {
                    Directory.Delete(mediaDir);
                }
            }
            string imageDir = Path.Combine(_helpSource, "images");
            if (Directory.Exists(imageDir))
            {
                if (DirectoryUtils.IsDirectoryEmpty(imageDir))
                {
                    Directory.Delete(imageDir);
                }
            }
            string mathDir = Path.Combine(_helpSource, "maths");
            if (Directory.Exists(mathDir))
            {
                if (DirectoryUtils.IsDirectoryEmpty(mathDir))
                {
                    Directory.Delete(mathDir);
                }
            }

            string metadataFile = Path.Combine(workingDir, "metadata.xml");

            using (ZipFile zipMshc = new ZipFile())
            {
                BeginMetadata(metadataFile, _helpName);

                zipMshc.UseZip64WhenSaving = Zip64Option.AsNecessary;

                zipMshc.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                zipMshc.AddProgress += new EventHandler<AddProgressEventArgs>(OnZipAddProgress);

                zipMshc.AddDirectory(_helpSource, null);

                //zipMshc.AddProgress -= new EventHandler<AddProgressEventArgs>(OnZipAddProgress);

                EndMetadata();

                string outputMetaFile = Path.Combine(_helpSource, "metadata.xml");
                if (File.Exists(outputMetaFile))
                {
                    File.Delete(outputMetaFile);
                }
                File.Move(metadataFile, outputMetaFile);

                zipMshc.AddFile(outputMetaFile, "");

                zipMshc.SaveProgress += new EventHandler<SaveProgressEventArgs>(OnZipSaveProgress);
                zipMshc.Save(Path.Combine(_helpOutputDir, _helpName + ".mshc"));
            }
        }

        #endregion

        #region CreateMshc Event Handlers

        private void BeginMetadata(string fileName, string helpName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.Encoding = Encoding.UTF8;
            _metadataWriter = XmlWriter.Create(fileName, settings);

            //<?xml version="1.0" encoding="utf-8"?>
            //<metadata package-name="packageName" xmlns="urn:MTPS-FH:metadata">
            //  <locale ref="" />
            //  <items>

            _metadataWriter.WriteStartDocument();
            _metadataWriter.WriteStartElement("metadata", "urn:MTPS-FH:metadata");
            _metadataWriter.WriteAttributeString("package-name", helpName);
            _metadataWriter.WriteStartElement("locale");     // locale
            _metadataWriter.WriteAttributeString("ref", "");
            _metadataWriter.WriteEndElement();               // locale
            _metadataWriter.WriteStartElement("items");      // items
        }

        private void EndMetadata()
        {
            _metadataWriter.WriteEndElement();   // items
            _metadataWriter.WriteStartElement("asset-ids");
            _metadataWriter.WriteEndElement();   // asset-ids
            _metadataWriter.WriteEndElement();   // metadata
            _metadataWriter.WriteEndDocument();
            //  </items>
            //  <asset-ids />
            //</metadata>

            _metadataWriter.Close();
            _metadataWriter = null;
        }

        private string GetContentType(string fileName)
        {
            string contentType = String.Empty;

            string ext = Path.GetExtension(fileName).ToLower();

            if (_contentTypes.ContainsKey(ext))
            {
                return _contentTypes[ext];
            }

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                string mimeType = regKey.GetValue("Content Type").ToString();

                if (mimeType.StartsWith("text"))
                {
                    contentType = String.Format("{0}; charset=utf-8", mimeType);
                }
                else
                {
                    contentType = mimeType;
                }
                _contentTypes[ext] = contentType;
            }

            return contentType;
        }

        private void OnZipAddProgress(object sender, AddProgressEventArgs e)
        {
            BuildLogger logger = this.Context.Logger;
            BuildLoggerVerbosity loggerVerbosity = BuildLoggerVerbosity.Quiet;
            if (logger == null)
            {
                loggerVerbosity = logger.Verbosity;
            }

            switch (e.EventType)
            {
                case ZipProgressEventType.Adding_Started:
                    if (logger != null)
                    {
                        _totalFileEntries = 0;
                        logger.WriteLine("Started adding files for MSHC",
                            BuildLoggerLevel.Info);
                    }
                    break;

                case ZipProgressEventType.Adding_AfterAddEntry:
                    string fileEntry = e.CurrentEntry.FileName;
                    if (e.EntriesTotal > _totalFileEntries)
                    {
                        _totalFileEntries = e.EntriesTotal;
                    }
                    // <item name="R1.htm" date="Wed, 25 Nov 2009 22:43:01 GMT" 
                    //       content-type="text/html; charset=utf-8" />
                    if (_metadataWriter != null && !e.CurrentEntry.IsDirectory)
                    {
                        _metadataWriter.WriteStartElement("item");     // item
                        _metadataWriter.WriteAttributeString("name", fileEntry);
                        _metadataWriter.WriteAttributeString("date",
                            DateTime.Now.ToString("R")); //TODO: which date is this?
                        _metadataWriter.WriteAttributeString("content-type", GetContentType(fileEntry));
                        _metadataWriter.WriteEndElement();             // item
                    }
                    if (logger != null && loggerVerbosity > BuildLoggerVerbosity.Normal)
                    {
                        logger.WriteLine("Added: " + fileEntry,
                            BuildLoggerLevel.Info);
                    }
                    break;

                case ZipProgressEventType.Adding_Completed:
                    if (logger != null)
                    {
                        // The additional file is the single metadata file added later...
                        logger.WriteLine(String.Format("Total of {0} files added.",
                            _totalFileEntries + 1), BuildLoggerLevel.Info);

                        logger.WriteLine("Completed adding files for MSHC",
                            BuildLoggerLevel.Info);
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnZipSaveProgress(object sender, SaveProgressEventArgs e)
        {
            BuildLogger logger = this.Context.Logger;
            BuildLoggerVerbosity loggerVerbosity = BuildLoggerVerbosity.Quiet;
            if (logger == null)
            {
                loggerVerbosity = logger.Verbosity;
            }

            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_Started:
                    if (logger != null)
                    {
                        _totalFileSaved = 0;
                        logger.WriteLine("Started compression for MSHC",
                            BuildLoggerLevel.Info);
                    }
                    break;

                case ZipProgressEventType.Saving_AfterWriteEntry:
                    if (e.EntriesTotal > _totalFileSaved)
                    {
                        _totalFileSaved = e.EntriesTotal;
                    }
                    if (logger != null && loggerVerbosity > BuildLoggerVerbosity.Normal)
                    {
                        logger.WriteLine(String.Format("Saved {0}/{1}: {2}", e.EntriesSaved, 
                            e.EntriesTotal, e.CurrentEntry.FileName),
                            BuildLoggerLevel.Info);
                    }
                    break;

                case ZipProgressEventType.Saving_Completed:
                    if (logger != null)
                    {
                        logger.WriteLine(String.Format("Total of {0} files saved.",
                            _totalFileSaved), BuildLoggerLevel.Info);

                        logger.WriteLine("Completed compression for MSHC.",
                            BuildLoggerLevel.Info);
                    }
                    break;

                default:
                    break;
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
