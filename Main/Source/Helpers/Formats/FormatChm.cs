using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;

using Sandcastle.Steps;
using Sandcastle.Utilities;

namespace Sandcastle.Formats
{
    [Serializable]
    public sealed class FormatChm : BuildFormat
    {
        #region Private Fields

        private bool _keepSources;
        private bool _useAutoIndex;
        private bool _useBinaryToc;
        private bool _useBinaryIndex;
        private bool _useFullTextSearch;
        private bool _includeFavorites;
        private bool _includeStopWords;
        private bool _includeAdvancedSearch;

        private bool _tocHasButtons;
        private bool _tocHasLines;
        private bool _tocLinesAtRoot;
        private bool _tocShowSelAlways;
        private bool _tocTrackSelect;
        private bool _tocSingleExpand;
        private bool _tocFullrowSelect;

        private BuildFilePath _compilerFile;

        #endregion

        #region Constructors and Destructor

        public FormatChm()
        {
            _useAutoIndex          = true;
            _useBinaryToc          = true;
            _useBinaryIndex        = true;
            _useFullTextSearch     = true;
            _includeFavorites      = true;
            _includeStopWords      = true;
            _includeAdvancedSearch = true;

            _tocHasButtons         = true;
            _tocHasLines           = true;
            _tocLinesAtRoot        = true;
            //_tocShowSelAlways = false;
            //_tocTrackSelect = false;
            //_tocSingleExpand = false;
            //_tocFullrowSelect = false;

            this.AddProperty("SharedContentSuffix", "Chm");
        }

        public FormatChm(FormatChm source)
            : base(source)
        {
            _useAutoIndex          = source._useAutoIndex;
            _useBinaryToc          = source._useBinaryToc;
            _useBinaryIndex        = source._useBinaryIndex;
            _useFullTextSearch     = source._useFullTextSearch;
            _includeFavorites      = source._includeFavorites;
            _includeStopWords      = source._includeStopWords;
            _includeAdvancedSearch = source._includeAdvancedSearch;

            _compilerFile          = source._compilerFile;

            _tocHasButtons         = source._tocHasButtons;
            _tocHasLines           = source._tocHasLines;
            _tocLinesAtRoot        = source._tocLinesAtRoot;
            _tocShowSelAlways      = source._tocShowSelAlways;
            _tocTrackSelect        = source._tocTrackSelect;
            _tocSingleExpand       = source._tocSingleExpand;
            _tocFullrowSelect      = source._tocFullrowSelect;
        }

        #endregion

        #region Public Properties

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.HtmlHelp1;
            }
        }

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// This will always return "HtmlHelp 1.x".
        /// </value>
        public override string Name
        {
            get
            {
                return "HtmlHelp 1.x";
            }
        }

        public override string Extension
        {
            get
            {
                return ".chm";
            }
        }

        public override string OutputExtension
        {
            get
            {
                return ".htm";
            }
        }

        public override bool IsCompilable
        {
            get
            {
                return true;
            }
        }

        public override string TocFileName
        {
            get
            {
                return "HtmlHelpToc1.xml";
            }
        }

        public BuildFilePath CompilerFile
        {
            get
            {
                if (_compilerFile == null || !_compilerFile.Exists)
                {
                    FindHtmlHelpCompiler();
                }

                return _compilerFile;
            }
            set
            {
                _compilerFile = value;
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

        public bool UseAutoIndex
        {
            get
            {
                return _useAutoIndex;
            }
            set
            {
                _useAutoIndex = value;
            }
        }

        public bool UseBinaryToc
        {
            get 
            { 
                return _useBinaryToc; 
            }
            set 
            { 
                _useBinaryToc = value; 
            }
        }

        public bool UseBinaryIndex
        {
            get 
            { 
                return _useBinaryIndex; 
            }
            set 
            { 
                _useBinaryIndex = value; 
            }
        }

        public bool UseFullTextSearch
        {
            get 
            { 
                return _useFullTextSearch; 
            }
            set 
            { 
                _useFullTextSearch = value; 
            }
        }

        public bool IncludeFavorites
        {
            get 
            { 
                return _includeFavorites; 
            }
            set 
            { 
                _includeFavorites = value; 
            }
        }

        public bool IncludeStopWords
        {
            get 
            { 
                return _includeStopWords; 
            }
            set 
            { 
                _includeStopWords = value; 
            }
        }

        public bool IncludeAdvancedSearch
        {
            get 
            { 
                return _includeAdvancedSearch; 
            }
            set 
            { 
                _includeAdvancedSearch = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the table of contents displays plus 
        /// and minus squares.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to display plus and minus squares; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// If <see langword="true"/>, this adds a button to the left side of each 
        /// parent item. The user can click the button to expand or collapse the child 
        /// items as an alternative to double-clicking the parent item.
        /// </remarks>
        public bool TocHasButtons
        {
            get 
            { 
                return _tocHasButtons; 
            }
            set 
            { 
                _tocHasButtons = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the table of contents displays lines
        /// between items.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to display the lines between the items;
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// This style enhances the graphic representation of a tree control's hierarchy 
        /// by drawing lines that link child items to their corresponding parent item.
        /// <para>
        /// This style does not link items at the root of the hierarchy. To do so, 
        /// you need to combine this with the <see cref="FormatChm.TocLinesAtRoot"/>
        /// property.
        /// </para>
        /// <note type="important">
        /// The lines are only drawn if the <see cref="FormatChm.UseBinaryToc"/> property
        /// is <see langword="false"/>.
        /// </note>
        /// </remarks>
        public bool TocHasLines
        {
            get 
            { 
                return _tocHasLines; 
            }
            set 
            { 
                _tocHasLines = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to start the lines between the items
        /// in the table of contents from the root.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to start the lines from the root; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <note type="important">
        /// The lines are only drawn if the <see cref="FormatChm.UseBinaryToc"/> property
        /// is <see langword="false"/>.
        /// </note>
        /// </remarks>
        public bool TocLinesAtRoot
        {
            get 
            { 
                return _tocLinesAtRoot; 
            }
            set 
            { 
                _tocLinesAtRoot = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show selection of item in the table
        /// of contents when focus is lost.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to show selection when focus is lost; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool TocShowSelectionAlways
        {
            get 
            { 
                return _tocShowSelAlways; 
            }
            set 
            { 
                _tocShowSelAlways = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically track selection
        /// in the table of contents.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to automatically track selection; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool TocTrackSelect
        {
            get 
            { 
                return _tocTrackSelect; 
            }
            set 
            { 
                _tocTrackSelect = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether only expand a single heading in the
        /// table of contents.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to expand only a single heading; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The user can expand or collapse a parent item's list of child items by 
        /// double-clicking the parent item. A table of contents that has this property
        /// set to <see langword="true"/> causes the item being selected to expand and 
        /// the item being unselected to collapse. 
        /// <para>
        /// If the mouse is used to single-click the selected item and that item is 
        /// closed, it will be expanded. If the selected item is single-clicked when 
        /// it is open, it will be collapsed.
        /// </para>
        /// </remarks>
        public bool TocSingleExpand
        {
            get 
            { 
                return _tocSingleExpand; 
            }
            set 
            { 
                _tocSingleExpand = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to select an entire row in the table
        /// of contents.
        /// </summary>
        /// <value>
        /// The property is <see langword="true"/> to select an entire row; otherwise, 
        /// it is <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool TocFullrowSelect
        {
            get 
            { 
                return _tocFullrowSelect; 
            }
            set 
            { 
                _tocFullrowSelect = value; 
            }
        }

        #endregion

        #region Public Methods

        public override BuildStep CreateStep(BuildContext context,
            BuildStage stage, string workingDir)
        {
            if (context == null || context.Settings == null)
            {
                return null;
            }

            // Accessing the property will force a lookup for the compiler...
            BuildFilePath helpCompiler = this.CompilerFile;
            if (helpCompiler == null || !helpCompiler.Exists)
            {
                throw new BuildException(
                    "The CHM format compiler cannot be found. If installed, manually set its path.");
            }

            BuildSettings settings = context.Settings;

            string helpDirectory = context.OutputDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = context.WorkingDirectory;
            }

            string helpName = settings.HelpName;
            if (String.IsNullOrEmpty(helpName))
            {
                helpName = "Documentation";
            }
            string helpTitle = settings.HelpTitle;
            if (String.IsNullOrEmpty(helpTitle))
            {
                helpTitle = helpName;
            }
            string helpFolder = this.OutputFolder;
            string helpPath = Path.Combine(helpDirectory,
                String.Format(@"{0}\{1}.chm", helpFolder, helpName));

            // Case 1: Closing the HtmlHelp 1.x viewer...
            if (stage == BuildStage.CloseViewer)
            {
                StepChmViewerClose chmClose = new StepChmViewerClose(
                    helpDirectory, helpPath, helpTitle);

                return chmClose;
            }
            
            // Case 2: Starting the HtmlHelp 1.x viewer...
            if (stage == BuildStage.StartViewer)
            {
                StepChmViewerStart chmStart = new StepChmViewerStart(
                    helpDirectory, helpPath);

                return chmStart;
            }

            // Case 3: Compiling the HtmlHelp 1.x help file...
            if (stage == BuildStage.Compilation)
            {
                string sandassistDir = settings.SandAssistDirectory;
                CultureInfo culture  = settings.CultureInfo;
                int lcid = 1033;
                if (culture != null)
                {
                    lcid = culture.LCID;
                }
                string appLocale = null;
                if (lcid != 1033)
                {
                    string toolsDir = Path.Combine(sandassistDir, "Tools");
                    if (Directory.Exists(toolsDir))
                    {
                        appLocale = Path.Combine(toolsDir, "SBAppLocale.exe");
                    }
                }

                // If there is a customized or format specific TOC use it,
                // otherwise, use the default...
                string tocFile = context["$HelpTocFile"];

                FormatChmOptions options = new FormatChmOptions();
                options.ConfigFile       = Path.Combine(workingDir, "ChmBuilder.config");
                options.HtmlDirectory    = Path.Combine(workingDir, @"Output\html");
                options.LangID           = lcid;
                options.Metadata         = false;
                options.WorkingDirectory = workingDir;
                options.OutputDirectory  = Path.Combine(workingDir, helpFolder);
                options.ProjectName      = helpName;
                options.TocFile          = Path.Combine(workingDir, tocFile);
                
                BuildMultiStep listSteps = new BuildMultiStep();
                listSteps.LogTitle    = "Building document output format - " + this.Name;
                listSteps.LogTimeSpan = true;

                // 1. Prepare the help html files, and create the html project
                // ChmBuilder.exe 
                // /project:Manual /html:Output\html 
                //   /lcid:1041 /toc:Toc.xml /out:Help
                string application = Path.Combine(context.SandcastleToolsDirectory, 
                    "ChmBuilder.exe");
                string arguments = String.Format(
                    "/project:{0} /html:Output\\html /lcid:{1} /toc:{2} /out:{3} /config:ChmBuilder.config",
                    helpName, lcid, tocFile, helpFolder);
                StepChmBuilder chmProcess = new StepChmBuilder(workingDir,
                    application, arguments);
                chmProcess.LogTitle        = String.Empty;
                chmProcess.Message         = "Creating project and HTML files for the compiler";
                chmProcess.CopyrightNotice = 2;
                chmProcess.HelpName      = helpName;
                chmProcess.HelpFolder    = helpFolder;
                chmProcess.HelpDirectory = helpDirectory;  
                chmProcess.Options       = options;
                chmProcess.OptimizeStyle = this.OptimizeStyle;
                chmProcess.Format        = this;

                listSteps.Add(chmProcess);

                // 2. Fix the file encoding: DBCSFix.exe /d:Help /l:1033  
                application = Path.Combine(context.SandcastleToolsDirectory, 
                    "DBCSFix.exe");
                arguments = String.Format("/d:{0} /l:{1}", helpFolder + @"\html", lcid);
                StepChmDbcsFix dbcsFixProcess = new StepChmDbcsFix(workingDir,
                    application, arguments);
                dbcsFixProcess.LogTitle        = String.Empty;
                dbcsFixProcess.Message         = "Fixing the DBCS for the non-Unicode compiler";
                dbcsFixProcess.CopyrightNotice = 2;   
                dbcsFixProcess.Options         = options;

                listSteps.Add(dbcsFixProcess);

                // 3. Compile the Html help files: hhc Help\Manual.hhp
                application = helpCompiler.Path;
                arguments   = String.Format(@"{0}\{1}.hhp", helpFolder, helpName);
                if (String.IsNullOrEmpty(appLocale) == false && 
                    File.Exists(appLocale))
                {
                    arguments = String.Format("$({0}) \"{1}\" {2}", lcid, application,
                        arguments);
                    application = appLocale;
                }
                StepChmCompiler hhcProcess = new StepChmCompiler(workingDir,
                    application, arguments);
                hhcProcess.LogTitle        = String.Empty;
                hhcProcess.Message         = "Compiling HtmlHelp 1.x help file";
                hhcProcess.CopyrightNotice = 2;
                hhcProcess.KeepSources     = _keepSources;
                hhcProcess.HelpName        = helpName;
                hhcProcess.HelpFolder      = helpFolder;
                hhcProcess.HelpDirectory   = helpDirectory;

                listSteps.Add(hhcProcess);

                return listSteps;
            }

            return null;
        }

        public override void WriteAssembler(BuildContext context, 
            BuildGroup group, XmlWriter xmlWriter)
        {
            base.WriteAssembler(context, group, xmlWriter);
        }

        public override void Reset()
        {
            base.Reset();

            this.FormatFolder     = "html";
            this.OutputFolder     = "HtmlHelp";
            this.ExternalLinkType = BuildLinkType.Msdn;
        }

        #endregion

        #region Protected Methods

        protected override void OnReadPropertyGroupXml(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "propertyGroup",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "OnReadPropertyGroupXml: The current element is '{0}' not the expected 'propertyGroup'.",
                    startElement));
            }

            string groupName = reader.GetAttribute("name");
            Debug.Assert(String.Equals(groupName, "FormatChm-General") ||
                String.Equals(groupName, "FormatChm-TreeView"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (String.Equals(groupName, "FormatChm-General",
                StringComparison.OrdinalIgnoreCase))
            {
                this.ReadXmlGeneral(reader);
            }
            else if (String.Equals(groupName, "FormatChm-TreeView",
                StringComparison.OrdinalIgnoreCase))
            {
                this.ReadXmlTreeView(reader);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnWritePropertyGroupXml(XmlWriter writer)
        {
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "FormatChm-General");
            writer.WritePropertyElement("KeepSources",           _keepSources);
            writer.WritePropertyElement("UseAutoIndex",          _useAutoIndex);
            writer.WritePropertyElement("UseBinaryToc",          _useBinaryToc);
            writer.WritePropertyElement("UseBinaryIndex",        _useBinaryIndex);
            writer.WritePropertyElement("UseFullTextSearch",     _useFullTextSearch);
            writer.WritePropertyElement("IncludeFavorites",      _includeFavorites);
            writer.WritePropertyElement("IncludeStopWords",      _includeStopWords);
            writer.WritePropertyElement("IncludeAdvancedSearch", _includeAdvancedSearch);
            writer.WriteEndElement();                   // end - propertyGroup
            
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "FormatChm-TreeView");
            writer.WritePropertyElement("HasButtons",    _tocHasButtons);
            writer.WritePropertyElement("HasLines",      _tocHasLines);
            writer.WritePropertyElement("LinesAtRoot",   _tocLinesAtRoot);
            writer.WritePropertyElement("ShowSelAlways", _tocShowSelAlways);
            writer.WritePropertyElement("TrackSelect",   _tocTrackSelect);
            writer.WritePropertyElement("SingleExpand",  _tocSingleExpand);
            writer.WritePropertyElement("FullrowSelect", _tocFullrowSelect);
            writer.WriteEndElement();                   // end - propertyGroup
        }

        protected override void OnReadContentXml(XmlReader reader)
        {
            // May check the validity of the parsing process...
            throw new NotImplementedException();
        }

        protected override void OnWriteContentXml(XmlWriter writer)
        {
        }

        protected override void OnReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            if (String.Equals(reader.Name, "compilerFile", 
                StringComparison.OrdinalIgnoreCase))
            {
                _compilerFile = BuildFilePath.ReadLocation(reader);
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            BuildFilePath.WriteLocation(_compilerFile,
                "compilerFile", writer);
        }

        #endregion

        #region Private Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatChm-General"));

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
                            case "keepsources":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _keepSources = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "useautoindex":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useAutoIndex = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "usebinarytoc":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useBinaryToc = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "usebinaryindex":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useBinaryIndex = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "usefulltextsearch":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useFullTextSearch = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includefavorites":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeFavorites = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includestopwords":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeStopWords = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includeadvancedsearch":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeAdvancedSearch = Convert.ToBoolean(tempText);
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

        #region ReadXmlTreeView Method

        private void ReadXmlTreeView(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatChm-TreeView"));

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
                            case "hasbuttons":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocHasButtons = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "haslines":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocHasLines = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "linesatroot":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocLinesAtRoot = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "showselalways":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocShowSelAlways = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "trackselect":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocTrackSelect = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "singleexpand":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocSingleExpand = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "fullrowselect":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocFullrowSelect = Convert.ToBoolean(tempText);
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

        #region FindHtmlHelpCompiler Method

        private void FindHtmlHelpCompiler()
        {
            if (!String.IsNullOrEmpty(_compilerFile) && File.Exists(_compilerFile))
            {
                return;
            }

            // 1. Search the default installed directory, the "Programs Files" folder...
            _compilerFile = new BuildFilePath(Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                @"HTML Help Workshop\hhc.exe"));
            if (_compilerFile.Exists)
            {
                return;
            }
            _compilerFile = null;

            // 2. Search the MS installed programs, the Help Workshop is one of them...
            string key = @"HKEY_CURRENT_USER\Software\Microsoft\HTML Help Workshop";

            string installDir = (string)Registry.GetValue(key, "InstallDir", null);

            if (String.IsNullOrEmpty(installDir) == false &&
                Directory.Exists(installDir))
            {
                _compilerFile = new BuildFilePath(Path.Combine(installDir, "hhc.exe"));
                if (_compilerFile.Exists)
                {
                    return;
                }
            }
            _compilerFile = null;

            // 3. Finally, search using the registered file types...
            RegistryKey hhwKey = Registry.ClassesRoot.OpenSubKey("hhc.file");

            if (hhwKey == null)
            {
                // Keep searching, one will exists...
                hhwKey = Registry.ClassesRoot.OpenSubKey("hhk.file");
                if (hhwKey == null)
                {
                    hhwKey = Registry.ClassesRoot.OpenSubKey("hhp.file");
                }
            }
            if (hhwKey != null)
            {
                hhwKey = hhwKey.OpenSubKey("DefaultIcon");
                if (hhwKey != null)
                {
                    object hhwValue = hhwKey.GetValue(null);
                    if (hhwValue != null)
                    {
                        string hhwPath = Convert.ToString(hhwValue);
                        if (String.IsNullOrEmpty(hhwPath) == false)
                        {
                            int splitIndex = hhwPath.IndexOf(',');
                            if (splitIndex > 0)
                            {
                                hhwPath = hhwPath.Substring(0, splitIndex);
                                _compilerFile = new BuildFilePath(Path.Combine(
                                    Path.GetDirectoryName(hhwPath), "hhc.exe"));
                            }
                        }
                    }
                }
            }

            if (_compilerFile.Exists)
            {
                return;
            }
            _compilerFile = null;
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatChm format = new FormatChm(this);

            base.Clone(format);

            if (_compilerFile != null)
            {
                format._compilerFile = _compilerFile.Clone();
            }

            return format;
        }

        #endregion
    }
}
