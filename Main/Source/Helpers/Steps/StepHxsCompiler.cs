using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Utilities;
using Sandcastle.MSHelpCompiler;

namespace Sandcastle.Steps
{
    public sealed class StepHxsCompiler : StepProcess
    {
        #region Private Fields

        private string       _logFile;
        private string       _projectFile;

        private int          _langId;

        private bool         _keepSources;
        private bool         _sampleInfo;
        private bool         _indexSort;
        private bool         _indexMerge;
        private bool         _indexAutoInclude;

        private string       _helpOutputDir;
        private string       _helpDirectory;

        private string       _helpToc;
        private string       _helpName;
        private string       _helpFolder;
        private string       _helpTitleId;
        private Version      _helpFileVersion;
        private CultureInfo  _helpCulture;

        private string       _collectionPrefix;

        [NonSerialized]
        private BuildSettings _settings;
        [NonSerialized]
        private FormatHxs _buildFormat;
        [NonSerialized]
        private XmlWriterSettings _xmlSettings;

        #endregion

        #region Constructors and Destructor

        public StepHxsCompiler()
        {
            Reset();
        }

        public StepHxsCompiler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            Reset();
        }

        public StepHxsCompiler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            Reset();
        }
                        
        #endregion

        #region Public Properties
        
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

        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }
            set
            {
                _projectFile = value;
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

        /// <summary>
        /// Gets or sets a value indicating whether to keep the source files used
        /// to compile the help.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the help file sources are
        /// to be kept after the compilation; otherwise, it is <see langword="false"/>. 
        /// <para>
        /// The default value is <see langword="false"/>, and the help file sources 
        /// will be deleted after the compilation if the 
        /// <see cref="BuildSettings.CleanIntermediate"/> property is <see langword="true"/>.
        /// </para>
        /// </value>
        public bool KeepSources
        {
            get
            {
                return _keepSources;
            }
            set
            {
                _keepSources = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            bool processResult = false;

            if (String.IsNullOrEmpty(this.Application) &&
                String.IsNullOrEmpty(this.Arguments))
            {
                if (String.IsNullOrEmpty(_projectFile) ||
                    File.Exists(_projectFile) == false)
                {
                    return processResult;
                }
            }

            BuildLogger logger = context.Logger;

            try
            {
                if (String.IsNullOrEmpty(_projectFile) ||
                    File.Exists(_projectFile) == false)
                {
                    processResult = this.Run(logger);
                }
                else
                {
                    HxComp hxsCompiler = new HxComp();
                    hxsCompiler.Initialize();

                    HxCompError compilerError = new HxCompError(logger, this.Verbosity);

                    compilerError.Attach(hxsCompiler);

                    hxsCompiler.Compile(_projectFile, Path.GetDirectoryName(_projectFile), 
                        null, 0);

                    compilerError.Detach(hxsCompiler);
                    hxsCompiler = null;

                    processResult = compilerError.IsSuccess;
                }

                if (!processResult)
                {
                    return processResult;
                }
            }
            catch (ArgumentException)
            {
                // If the COM server compiler fails, run the command line version...
                processResult = this.Run(logger);

                if (!processResult)
                {
                    return processResult;
                }
            }
            catch (Exception ex)
            {   
            	if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }

            // If the help is successfully compiled, then...
            // 1. Copy the compiled help files to the output folder...
            string workingDir = this.WorkingDirectory;
            if (!Directory.Exists(_helpDirectory))
            {
                Directory.CreateDirectory(_helpDirectory);
            }
            // If we need to keep the help sources, then just copy everything to
            // the output folder...
            if (_keepSources)
            {
                string compiledDir = Path.Combine(workingDir, _helpFolder);
                if (!Directory.Exists(compiledDir))
                {
                    return processResult;
                }
                string outputDir = Path.Combine(_helpDirectory, _helpFolder);
                if (Directory.Exists(outputDir))
                {
                    DirectoryUtils.DeleteDirectory(outputDir, true);
                }
                Directory.Move(compiledDir, outputDir);
            }
            else  //...otherwise, just copy the essentials...
            {
                string helpOutput = Path.Combine(_helpDirectory, _helpFolder);
                if (!Directory.Exists(helpOutput))
                {
                    Directory.CreateDirectory(helpOutput);
                }
                string sourceHelpPath = Path.Combine(workingDir,
                    String.Format(@"{0}\{1}.HxS", _helpFolder, _helpName));
                string destHelpPath = Path.Combine(helpOutput, _helpName + ".HxS");
                if (File.Exists(sourceHelpPath))
                {
                    File.Copy(sourceHelpPath, destHelpPath, true);
                    File.SetAttributes(destHelpPath, FileAttributes.Normal);
                }
                // Copy the log file, if available...
                sourceHelpPath = Path.ChangeExtension(sourceHelpPath, ".log");
                if (File.Exists(sourceHelpPath))
                {
                    destHelpPath = Path.ChangeExtension(destHelpPath, ".log");
                    File.Copy(sourceHelpPath, destHelpPath, true);
                    File.SetAttributes(destHelpPath, FileAttributes.Normal);
                }
                // Copy the separated index file, if available...
                sourceHelpPath = Path.ChangeExtension(sourceHelpPath, ".HxI");
                if (File.Exists(sourceHelpPath))
                {
                    destHelpPath = Path.ChangeExtension(destHelpPath, ".HxI");
                    File.Copy(sourceHelpPath, destHelpPath, true);
                    File.SetAttributes(destHelpPath, FileAttributes.Normal);
                }
            }

            // 2. Create the help collection files for registration...
            processResult = CreateCollection(context);

            return processResult;
        }

        #endregion

        #region Run Method

        protected override bool Run(BuildLogger logger)
        {
            bool processResult = false;

            try
            {
                Process process = new Process();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName = this.Application;
                startInfo.Arguments = this.Arguments;
                startInfo.UseShellExecute = this.UseShellExecute;
                startInfo.WorkingDirectory = this.WorkingDirectory;

                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = true;
                process.StandardInput.Close();

                // Now, start the process - there will still not be output till...
                process.Start();

                // We must wait for the process to complete...
                process.WaitForExit();

                process.Close();

                if (File.Exists(_logFile))
                {
                    using (StreamReader reader = new StreamReader(_logFile))
                    {
                        while (!reader.EndOfStream)
                        {
                            logger.WriteLine(reader.ReadLine(), BuildLoggerLevel.None);
                        }
                    }
                }

                processResult = true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                processResult = false;
            }

            return processResult;
        }

        #endregion

        #endregion

        #region Private Methods

        #region CreateCollection Method

        private bool CreateCollection(BuildContext context)
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
            _xmlSettings.Encoding = Encoding.UTF8;
            _xmlSettings.Indent = true;
            _xmlSettings.IndentChars = "\t";
            _xmlSettings.OmitXmlDeclaration = false;
            _xmlSettings.CloseOutput = true;

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
                tmpText = _buildFormat["CollectionPrefix"];
                if (String.IsNullOrEmpty(tmpText) == false)
                {
                    _collectionPrefix = tmpText;
                }

                if (String.IsNullOrEmpty(_helpDirectory))
                {
                    _helpOutputDir = Path.Combine(workingDir, _helpFolder);
                }
                else
                {
                    _helpOutputDir = Path.Combine(_helpDirectory, _helpFolder);
                }
                if (Directory.Exists(_helpOutputDir) == false)
                {
                    Directory.CreateDirectory(_helpOutputDir);
                }

                string langText = _langId.ToString();

                bool lineOnAttribute = _xmlSettings.NewLineOnAttributes;
                _xmlSettings.NewLineOnAttributes = true;

                IList<string> listFiles = CreateCollectionIndex(context, langText);

                _xmlSettings.NewLineOnAttributes = lineOnAttribute;

                if (listFiles != null && listFiles.Count != 0)
                {
                    if (CreateCollectionToc(context, langText))
                    {
                        buildResult = CreateCollectionProject(context, listFiles, langText);
                    }
                }

                return buildResult;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region CreateCollectionToc Method

        private bool CreateCollectionToc(BuildContext context, 
            string langText)
        {
            BuildExceptions.NotNullNotEmpty(langText, "langText");

            string fileHxT = _collectionPrefix + _helpName + ".HxT";
            string filePath = Path.Combine(_helpOutputDir, fileHxT);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {
                string tocId = Path.GetFileNameWithoutExtension(fileHxT) + "TOC";
                xmlWriter.WriteDocType("HelpTOC", null, null, null);
                xmlWriter.WriteStartElement("HelpTOC");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id", tocId);
                if (_buildFormat.PluginTocFlat)
                {   
                    xmlWriter.WriteAttributeString("PluginStyle", "Flat"); 
                }
                else
                {   
                    xmlWriter.WriteAttributeString("PluginStyle", "Hierarchical");
                }
                string pluginTitle = _buildFormat.PluginTitle;
                if (String.IsNullOrEmpty(pluginTitle))
                {
                    pluginTitle = _settings.HelpTitle;
                }
                xmlWriter.WriteAttributeString("PluginTitle", pluginTitle);
                xmlWriter.WriteAttributeString("FileVersion",
                    _helpFileVersion.ToString());
                xmlWriter.WriteAttributeString("LangId", langText);

                // <HelpTOCNode NodeType="TOC" Url="..." />
                // For now, we support only the TOC NodeType.
                // TODO: Examine the possibilities of supporting Regular NodeType
                xmlWriter.WriteStartElement("HelpTOCNode");
                xmlWriter.WriteAttributeString("NodeType", "TOC");
                xmlWriter.WriteAttributeString("Url", _helpTitleId);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            return true;
        }

        #endregion

        #region CreateCollectionProject Method

        private bool CreateCollectionProject(BuildContext context, 
            IList<string> listFiles, string langText)
        {
            BuildExceptions.NotNull(listFiles, "listFiles");
            BuildExceptions.NotNullNotEmpty(langText, "langText");

            string fileHxC = _collectionPrefix + _helpName + ".HxC";
            string fileHxT = _collectionPrefix + _helpName + ".HxT";
            string filePath = Path.Combine(_helpOutputDir, fileHxC);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {
                xmlWriter.WriteDocType("HelpCollection", null, null, null);
                xmlWriter.WriteStartElement("HelpCollection");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileHxC));
                xmlWriter.WriteAttributeString("FileVersion", 
                    _helpFileVersion.ToString());
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteAttributeString("Title", _settings.HelpTitle);
                xmlWriter.WriteAttributeString("Copyright", _settings.Feedback.Copyright);

                // <TOCDef File="HelpFile.HxT" />
                xmlWriter.WriteStartElement("TOCDef");
                xmlWriter.WriteAttributeString("File", fileHxT);
                xmlWriter.WriteEndElement();

                // <KeywordIndexDef File="HelpFile_*.HxK" />
                int itemCount = listFiles.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    xmlWriter.WriteStartElement("KeywordIndexDef");
                    xmlWriter.WriteAttributeString("File", listFiles[i]);
                    xmlWriter.WriteEndElement();
                }

                // <ItemMoniker Name="!..." ProgId="..." InitData="" />
                string[,] itemMonikers = CreateItemMonikers();
                itemCount = itemMonikers.GetUpperBound(0);
                if (_sampleInfo)
                {
                    itemCount++;
                }

                for (int i = 0; i < itemCount; i++)
                {
                    xmlWriter.WriteStartElement("ItemMoniker");
                    xmlWriter.WriteAttributeString("Name", itemMonikers[i, 0]);
                    xmlWriter.WriteAttributeString("ProgId", itemMonikers[i, 1]);
                    xmlWriter.WriteAttributeString("InitData", itemMonikers[i, 2]);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            return true;
        }

        #endregion

        #region CreateCollectionIndex Method

        private IList<string> CreateCollectionIndex(BuildContext context, 
            string langText)
        {
            List<string> listFiles = new List<string>(6);
            // 1. For the A...
            string fileName = _collectionPrefix + _helpName + "_A.HxK";
            listFiles.Add(fileName);
            string filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "A");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge", _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort", _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "HxLink Index");
                xmlWriter.WriteAttributeString("Visible", "No");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();
            }

            // 2. For the B...
            fileName = _collectionPrefix + _helpName + "_B.HxK";
            listFiles.Add(fileName);
            filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "B");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge", _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort", _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "Dynamic Link Index");
                xmlWriter.WriteAttributeString("Visible", "No");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();
            }

            // 3. For the F...
            fileName = _collectionPrefix + _helpName + "_F.HxK";
            listFiles.Add(fileName);
            filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "F");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge", _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort", _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "Dynamic Help and F1 Index");
                xmlWriter.WriteAttributeString("Visible", "No");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();
            }

            // 4. For the K...
            fileName = _collectionPrefix + _helpName + "_K.HxK";
            listFiles.Add(fileName);
            filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "K");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("FileVersion", 
                    _helpFileVersion.ToString());
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge", _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort", _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "Keyword Index");
                xmlWriter.WriteAttributeString("Visible", "Yes");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();
            }

            // 5. For the S...
            fileName = _collectionPrefix + _helpName + "_S.HxK";
            listFiles.Add(fileName);
            filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");
                xmlWriter.WriteAttributeString("Name", "S");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge",
                    _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort",
                    _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "Search Enhancement Index");
                xmlWriter.WriteAttributeString("Visible", "No");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();
            }

            // 6. For the N...
            fileName = _collectionPrefix + _helpName + "_N.HxK";
            listFiles.Add(fileName);
            filePath = Path.Combine(_helpOutputDir, fileName);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, _xmlSettings))
            {   
                xmlWriter.WriteDocType("HelpIndex", null, null, null);
                xmlWriter.WriteStartElement("HelpIndex");  // start - HelpIndex
                xmlWriter.WriteAttributeString("Name", "NamedUrl");
                xmlWriter.WriteAttributeString("DTDVersion", "1.0");
                xmlWriter.WriteAttributeString("Id",
                    Path.GetFileNameWithoutExtension(fileName));
                xmlWriter.WriteAttributeString("AutoInclude",
                    _indexAutoInclude ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Merge",
                    _indexMerge ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Sort",
                    _indexSort ? "Yes" : "No");
                xmlWriter.WriteAttributeString("Title", "Named Url Index");
                xmlWriter.WriteAttributeString("Visible", "No");
                xmlWriter.WriteAttributeString("LangId", langText);
                xmlWriter.WriteWhitespace(_xmlSettings.NewLineChars); // force a new line
                xmlWriter.WriteFullEndElement();           // end - HelpIndex
            }

            return listFiles;
        }

        #endregion

        #region CreateItemMonikers Method

        private string[,] CreateItemMonikers()
        {
            string[,] itemMonikers = new string[,] 
            { 
                { "!DefaultToc", "HxDs.HxHierarchy", "" }, 
                { "!DefaultFullTextSearch", "HxDs.HxFullTextSearch", "" }, 
                { "!DefaultAssociativeIndex", "HxDs.HxIndex", "A" }, 
                { "!DefaultKeywordIndex", "HxDs.HxIndex", "K" }, 
                { "!DefaultContextWindowIndex", "HxDs.HxIndex", "F" }, 
                { "!DefaultNamedUrlIndex", "HxDs.HxIndex", "NamedUrl" }, 
                { "!DefaultSearchWindowIndex", "HxDs.HxIndex", "S" }, 
                { "!DefaultDynamicLinkIndex", "HxDs.HxIndex", "B" }, 
                { "!SampleInfo", "HxDs.HxSampleCollection", _sampleInfo ? "" : "" } 
            };

            return itemMonikers;
        }

        #endregion

        #region Reset Method

        private void Reset()
        {
            _langId           = 1033;
            _helpFolder       = "MsdnHelp";
            _helpFileVersion  = new Version(1, 0, 0, 0);

            _indexSort        = true;
            _indexMerge       = true;
            _indexAutoInclude = true;
            _collectionPrefix = "Coll";

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
