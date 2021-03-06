﻿using System;
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
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following are the supported custom attributes or properties of this
    /// build format:
    /// </para>
    /// <para>
    /// CollectionPrefix (string) - "Coll"
    /// IndexAutoInclude (bool) - ...
    /// IndexSort (bool) - ...
    /// IndexMerge (bool) - ...
    /// SampleInfo (bool) - ...
    /// RegistrationLevel (Enumeration{Basic, Medium, Full}) - Basic 
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class FormatHxs : BuildFormat
    {
        #region Private Fields

        private bool   _keepSources;
        private bool   _separateIndex;
        private bool   _includeStopWords;

        private BuildFilePath      _compilerFile;
        private BuildDirectoryPath _compilerDir;

        private string _helpTitleId;

        // Plugin properties...
        private bool         _pluginTocFlat;
        private string       _pluginTitle;
        private BuildList<string> _pluginParents;
        private BuildList<string> _pluginChildren;

        // Named Url Properties...
        private string _homePage;
        private string _defaultPage;
        private string _navFailPage;
        private string _aboutPageInfo;
        private string _aboutPageIcon;
        private string _filterEditPage;
        private string _helpPage;
        private string _supportPage;
        private string _sampleDirPage;
        private string _searchHelpPage;

        #endregion

        #region Constructors and Destructor

        public FormatHxs()
        {
            _includeStopWords = true;
            _homePage         = String.Empty;
            _defaultPage      = String.Empty;
            _navFailPage      = String.Empty;
            _aboutPageInfo    = String.Empty;
            _aboutPageIcon    = String.Empty;
            _filterEditPage   = String.Empty;
            _helpPage         = String.Empty;
            _supportPage      = String.Empty;
            _sampleDirPage    = String.Empty;
            _searchHelpPage   = String.Empty;

            _pluginParents    = new BuildList<string>();
            _pluginChildren   = new BuildList<string>();

            this.AddProperty("CollectionPrefix",    "Coll");
            this.AddProperty("RegistrationLevel",   "Basic");
            this.AddProperty("SharedContentSuffix", "Hxs");
        }

        public FormatHxs(FormatHxs source)
            : base(source)
        {   
            _keepSources      = source._keepSources;
            _separateIndex    = source._separateIndex;
            _includeStopWords = source._includeStopWords;
            _compilerFile     = source._compilerFile;
            _compilerDir      = source._compilerDir;

            _helpTitleId      = source._helpTitleId;

            // Plugin properties...
            _pluginTocFlat    = source._pluginTocFlat;
            _pluginTitle      = source._pluginTitle;
            _pluginParents    = source._pluginParents;
            _pluginChildren   = source._pluginChildren;

            // Named Url Properties...
            _homePage         = source._homePage;
            _defaultPage      = source._defaultPage;
            _navFailPage      = source._navFailPage;
            _aboutPageInfo    = source._aboutPageInfo;
            _aboutPageIcon    = source._aboutPageIcon;
            _filterEditPage   = source._filterEditPage;
            _helpPage         = source._helpPage;
            _supportPage      = source._supportPage;
            _sampleDirPage    = source._sampleDirPage;
            _searchHelpPage   = source._searchHelpPage;
        }

        #endregion

        #region Public Properties

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.HtmlHelp2;
            }
        }

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// This will always return "HtmlHelp 2.x".
        /// </value>
        public override string Name
        {
            get
            {
                return "HtmlHelp 2.x";
            }
        }

        public override string Extension
        {
            get
            {
                return ".hxs";
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
                return "HtmlHelpToc2.xml";
            }
        }

        public BuildFilePath CompilerFile
        {
            get
            {
                if (String.IsNullOrEmpty(_compilerFile) || 
                    !File.Exists(_compilerFile))
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

        public BuildDirectoryPath CompilerDirectory
        {
            get 
            {
                if (String.IsNullOrEmpty(_compilerDir) || 
                    !Directory.Exists(_compilerDir))
                {
                    FindHtmlHelpCompiler();
                }

                return _compilerDir; 
            }
            set 
            { 
                _compilerDir = value; 
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

        public bool SeparateIndexFile
        {
            get
            {
                return _separateIndex;
            }
            set
            {
                _separateIndex = value;
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
                        throw new BuildException(
                            "The help file title ID cannot contain space.");
                    }
                }
                _helpTitleId = value;
            }
        }

        public string HomePage
        {
            get
            {
                return _homePage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _homePage = value;
            }
        }

        public string DefaultPage
        {
            get
            {
                return _defaultPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _defaultPage = value;
            }
        }

        public string NavFailPage
        {
            get
            {
                return _navFailPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _navFailPage = value;
            }
        }

        public string AboutPageInfo
        {
            get
            {
                return _aboutPageInfo;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _aboutPageInfo = value;
            }
        }

        public string AboutPageIcon
        {
            get
            {
                return _aboutPageIcon;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _aboutPageIcon = value;
            }
        }

        public string FilterEditPage
        {
            get
            {
                return _filterEditPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _filterEditPage = value;
            }
        }

        public string HelpPage
        {
            get
            {
                return _helpPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _helpPage = value;
            }
        }

        public string SupportPage
        {
            get
            {
                return _supportPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _supportPage = value;
            }
        }

        public string SampleDirPage
        {
            get
            {
                return _sampleDirPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _sampleDirPage = value;
            }
        }

        public string SearchHelpPage
        {
            get
            {
                return _searchHelpPage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _searchHelpPage = value;
            }
        }

        public bool PluginTocFlat
        {
            get
            {
                return _pluginTocFlat;
            }
            set
            {
                _pluginTocFlat = value;
            }
        }
        
        public string PluginTitle
        {
            get
            {
                return _pluginTitle;
            }
            set
            {
                _pluginTitle = value;
            }
        }
        
        public IList<string> PluginParents
        {
            get
            {
                return _pluginParents;
            }
        }

        public IList<string> PluginChildren
        {
            get
            {
                return _pluginChildren;
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
                String.Format(@"{0}\{1}.hxs", helpFolder, helpName));

            if (String.IsNullOrEmpty(_helpTitleId))
            {
                _helpTitleId = helpName.Trim();
            }

            // Case 1: Closing the HtmlHelp 2.x viewer...
            if (stage == BuildStage.CloseViewer)
            {
                StepHxsViewerClose hxsClose = new StepHxsViewerClose(
                   workingDir, helpPath, helpTitle);

                return hxsClose;
            }
            // Case 2: Starting the HtmlHelp 2.x viewer...
            if (stage == BuildStage.StartViewer)
            {
                string collPrefix = this["CollectionPrefix"];
                if (String.IsNullOrEmpty(collPrefix))
                {
                    collPrefix = "Coll";
                    this.AddProperty("CollectionPrefix", collPrefix);
                }
                string helpColl = Path.Combine(helpDirectory,
                    String.Format(@"{0}\{1}{2}.hxC", helpFolder, collPrefix, helpName));
                string registrar = Path.Combine(this.CompilerDirectory, "HxReg.exe");
                StepHxsViewerStart hxsStart = new StepHxsViewerStart(
                    Path.GetDirectoryName(helpPath), helpPath, helpColl);
                hxsStart.Registrar         = registrar;
                hxsStart.HelpTitleId       = _helpTitleId;
                hxsStart.SeparateIndexFile = _separateIndex;

                return hxsStart;
            }

            // Case 3: Compiling the HtmlHelp 2.x help file...
            if (stage == BuildStage.Compilation)
            {
                CultureInfo culture = settings.CultureInfo;
                int lcid = 1033;
                if (culture != null)
                {
                    lcid = culture.LCID;
                }
                
                BuildMultiStep listSteps = new BuildMultiStep();
                listSteps.LogTitle    = "Building document output format - " + this.Name;
                listSteps.LogTimeSpan = true;

                // Prepare the help html files, and create the html project
                // 1. Move the output html files to the help folder for compilation...
                StepDirectoryMove dirMove = new StepDirectoryMove(workingDir);
                dirMove.LogTitle = String.Empty;
                dirMove.Message  = "Moving the output html files to the help folder for compilation";
                dirMove.Add(@"Output\" + this.FormatFolder, helpFolder + @"\html");

                listSteps.Add(dirMove); 

                // 2. Creating the project file...
                string tocFile = context["$HelpTocFile"];

                StepHxsBuilder hxsBuilder  = new StepHxsBuilder(workingDir);
                hxsBuilder.LogTitle        = String.Empty;
                hxsBuilder.Message         = "Creating project, content and configuration files.";
                hxsBuilder.HelpFolder      = helpFolder;
                hxsBuilder.HelpToc         = tocFile;
                hxsBuilder.HelpName        = helpName;
                hxsBuilder.HelpTitleId     = _helpTitleId;
                hxsBuilder.HelpCultureInfo = culture;

                listSteps.Add(hxsBuilder); 

                // 3. Compile the Html help files hxcomp.exe -p Help\Manual.hxc
                string application = this.CompilerFile;
                string arguments   = String.Format(
                    @"-p {0}\{1}.HxC -n Output\{1}.log", helpFolder, helpName);
                StepHxsCompiler hxsCompiler = new StepHxsCompiler(workingDir, 
                    application, arguments);
                hxsCompiler.Message  = "Compiling the help file (HxComp Tool)";
                hxsCompiler.LogTitle = String.Empty;
                hxsCompiler.LogFile  = Path.Combine(workingDir,
                    String.Format(@"Output\{0}.log", helpName));
                hxsCompiler.ProjectFile = Path.Combine(workingDir,
                    String.Format(@"{0}\{1}.HxC", helpFolder, helpName));
                //hxsCompiler.CopyrightNotice = 2;
                hxsCompiler.HelpFolder      = helpFolder;
                hxsCompiler.HelpToc         = tocFile;
                hxsCompiler.HelpName        = helpName;
                hxsCompiler.HelpTitleId     = _helpTitleId;
                hxsCompiler.HelpDirectory   = helpDirectory;
                hxsCompiler.HelpCultureInfo = culture;

                listSteps.Add(hxsCompiler);

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

            this.FormatFolder      = "html2";
            this.OutputFolder      = "MsdnHelp";
            this.ExternalLinkType  = BuildLinkType.Index;

            base.IntegrationTarget = BuildIntegrationTarget.VS2008;
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
            Debug.Assert(String.Equals(groupName, "FormatHxs-General") ||
                String.Equals(groupName, "FormatHxs-NamedUrl"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (String.Equals(groupName, "FormatHxs-General",
                StringComparison.OrdinalIgnoreCase))
            {
                this.ReadXmlGeneral(reader);
            }
            else if (String.Equals(groupName, "FormatHxs-NamedUrl",
                StringComparison.OrdinalIgnoreCase))
            {
                this.ReadXmlNamedUrl(reader);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnWritePropertyGroupXml(XmlWriter writer)
        {
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "FormatHxs-General");
            writer.WritePropertyElement("KeepSources",       _keepSources);
            writer.WritePropertyElement("SeparateIndexFile", _separateIndex);
            writer.WritePropertyElement("IncludeStopWords",  _includeStopWords);
            writer.WritePropertyElement("HelpTitleId",       _helpTitleId);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "FormatHxs-NamedUrl");
            writer.WritePropertyElement("HomePage",       _homePage);
            writer.WritePropertyElement("DefaultPage",    _defaultPage);
            writer.WritePropertyElement("NavFailPage",    _navFailPage);
            writer.WritePropertyElement("AboutPageInfo",  _aboutPageInfo);
            writer.WritePropertyElement("AboutPageIcon",  _aboutPageIcon);
            writer.WritePropertyElement("FilterEditPage", _filterEditPage);
            writer.WritePropertyElement("HelpPage",       _helpPage);
            writer.WritePropertyElement("SupportPage",    _supportPage);
            writer.WritePropertyElement("SampleDirPage",  _sampleDirPage);
            writer.WritePropertyElement("SearchHelpPage", _searchHelpPage);
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
            else if (String.Equals(reader.Name, "compilerDirectory",
                StringComparison.OrdinalIgnoreCase))
            {
                _compilerDir = BuildDirectoryPath.ReadLocation(reader);
            }
            else if (String.Equals(reader.Name, "plugin",
                StringComparison.OrdinalIgnoreCase))
            {
                //<plugin flatToc="False">
                //    <title></title>
                //    <parents>
                //      <parent>ParentName</parent>
                //    </parents>
                //    <children>
                //      <child>ChildName</child>
                //    </children>
                //</plugin>  

                if (_pluginParents == null)
                {
                    _pluginParents = new BuildList<string>();
                }
                if (_pluginChildren == null)
                {
                    _pluginChildren = new BuildList<string>();
                }

                string tempText = reader.GetAttribute("flatToc");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _pluginTocFlat = Convert.ToBoolean(tempText);
                }

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name.ToLower())
                        {
                            case "title":
                                _pluginTitle = reader.ReadString();
                                break;
                            case "parent":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _pluginParents.Add(tempText);
                                }
                                break;
                            case "child":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _pluginChildren.Add(tempText);
                                }
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "plugin",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
       }

        protected override void OnWriteXml(XmlWriter writer)
        {
            BuildFilePath.WriteLocation(_compilerFile,
                "compilerFile", writer);
            BuildDirectoryPath.WriteLocation(_compilerDir,
                "compilerDirectory", writer);

            writer.WriteStartElement("plugin");  // start - plugin
            writer.WriteAttributeString("flatToc", _pluginTocFlat.ToString());
            writer.WriteTextElement("title", _pluginTitle);
            writer.WriteStartElement("parents"); // start - parents
            if (_pluginParents != null && _pluginParents.Count != 0)
            {
                for (int i = 0; i < _pluginParents.Count; i++)
                {
                    writer.WriteTextElement("parent", _pluginParents[i]);
                }
            }
            writer.WriteEndElement();            // end - parents

            writer.WriteStartElement("children"); // start - children
            if (_pluginChildren != null && _pluginChildren.Count != 0)
            {
                for (int i = 0; i < _pluginChildren.Count; i++)
                {
                    writer.WriteTextElement("child", _pluginChildren[i]);
                }
            }
            writer.WriteEndElement();            // end - children

            writer.WriteEndElement();            // end - plugin
        }

        #endregion

        #region Private Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatHxs-General"));

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
                            case "separateindexfile":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _separateIndex = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includestopwords":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeStopWords = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "helptitleid":
                                _helpTitleId = reader.ReadString();
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

        #region ReadXmlNamedUrl Method

        private void ReadXmlNamedUrl(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatHxs-NamedUrl"));

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
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "homepage":
                                _homePage = reader.ReadString();
                                break;
                            case "defaultpage":
                                _defaultPage = reader.ReadString();
                                break;
                            case "navfailpage":
                                _navFailPage = reader.ReadString();
                                break;
                            case "aboutpageinfo":
                                _aboutPageInfo = reader.ReadString();
                                break;
                            case "aboutpageicon":
                                _aboutPageIcon = reader.ReadString();
                                break;
                            case "filtereditpage":
                                _filterEditPage = reader.ReadString();
                                break;
                            case "helppage":
                                _helpPage = reader.ReadString();
                                break;
                            case "supportpage":
                                _supportPage = reader.ReadString();
                                break;
                            case "sampledirpage":
                                _sampleDirPage = reader.ReadString();
                                break;
                            case "searchhelppage":
                                _searchHelpPage = reader.ReadString();
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
            // 1. If the compiler path is set, may be by the user...
            if (_compilerFile != null && _compilerFile.Exists)
            {
                if (_compilerDir == null || !_compilerDir.Exists)
                {
                    _compilerDir = new BuildDirectoryPath(
                        Path.GetDirectoryName(_compilerFile.Path));
                }
                return;
            }

            // 2. If the directory path is set, may be by the user...
            if (_compilerDir != null && _compilerDir.Exists)
            {
                _compilerFile = new BuildFilePath(Path.Combine(
                    _compilerDir, "hxcomp.exe"));

                if (_compilerFile.Exists)
                {
                    return;
                }
            }

            // 3. If the Visual Studio .NET Help Integration Kit 2003 is installed,
            // http://www.microsoft.com/downloads/details.aspx?FamilyID=ce1b26dc-d6af-42a1-a9a4-88c4eb456d87&displaylang=en
            _compilerDir = new BuildDirectoryPath(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFiles), "Microsoft Help 2.0 SDK"));
            if (_compilerDir.Exists)
            {
                _compilerFile = new BuildFilePath(Path.Combine(_compilerDir, 
                    "hxcomp.exe"));

                if (_compilerFile.Exists)
                {
                    return;
                }
            }

            _compilerDir  = null;
            _compilerFile = null;

            // 4. If all fail, search the registry, in case any other SDK is installed... 
            //      a. Visual Studio 2005 SDK,
            // http://www.microsoft.com/downloads/details.aspx?familyid=51A5C65B-C020-4E08-8AC0-3EB9C06996F4&displaylang=en
            //      b. Microsoft Visual Studio 2008 SDK 
            // http://www.microsoft.com/downloads/details.aspx?familyid=30402623-93CA-479A-867C-04DC45164F5B&displaylang=en
            // http://www.microsoft.com/downloads/details.aspx?FamilyId=59EC6EC3-4273-48A3-BA25-DC925A45584D&displaylang=en
            // NOTE: This procedure will also find the VSHIK 2003
            //
            // We can try to get the directory of the hxcsrv.exe, which also contains
            // the compiler and other stuff we need:
            // ...\Microsoft Help 2.0 SDK\hxcsrv.exe
            // ...\VisualStudioIntegration\Archive\HelpIntegration\hxcsrv.exe
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("Hxcomp.HxComp");
            if (key == null)
            {
                return;
            }
            key = key.OpenSubKey("CLSID");
            if (key == null)
            {
                return;
            }

            string clsid = key.GetValue(null) as string;
            if (clsid != null)
            {
                key = Registry.ClassesRoot.OpenSubKey("CLSID");
                if (key != null)
                {
                    key = key.OpenSubKey(clsid);
                    if (key != null)
                    {
                        key = key.OpenSubKey("LocalServer32");
                        if (key != null)
                        {
                            string val = key.GetValue(null) as string;
                            if (val != null)
                            {
                                _compilerDir = new BuildDirectoryPath(
                                    Path.GetDirectoryName(val));
                            }
                        }
                    }
                }
            }

            if (_compilerDir.Exists)
            {
                _compilerFile = new BuildFilePath(Path.Combine(_compilerDir, 
                    "hxcomp.exe"));

                if (_compilerFile.Exists == false)
                {
                    _compilerDir = null;
                }
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatHxs format = new FormatHxs(this);

            base.Clone(format);

            if (_compilerFile != null)
            {
                format._compilerFile = _compilerFile.Clone();
            }
            if (_compilerDir != null)
            {
                format._compilerDir = _compilerDir.Clone();
            }
            if (_helpTitleId != null)
            {
                format._helpTitleId = String.Copy(_helpTitleId);
            }
            if (_pluginTitle != null)
            {
                format._pluginTitle = String.Copy(_pluginTitle);
            }
            if (_homePage != null)
            {
                format._homePage = String.Copy(_homePage);
            }
            if (_defaultPage != null)
            {
                format._defaultPage = String.Copy(_defaultPage);
            }
            if (_navFailPage != null)
            {
                format._navFailPage = String.Copy(_navFailPage);
            }
            if (_aboutPageInfo != null)
            {
                format._aboutPageInfo = String.Copy(_aboutPageInfo);
            }
            if (_aboutPageIcon != null)
            {
                format._aboutPageIcon = String.Copy(_aboutPageIcon);
            }
            if (_filterEditPage != null)
            {
                format._filterEditPage = String.Copy(_filterEditPage);
            }
            if (_helpPage != null)
            {
                format._helpPage = String.Copy(_helpPage);
            }
            if (_supportPage != null)
            {
                format._supportPage = String.Copy(_supportPage);
            }
            if (_sampleDirPage != null)
            {
                format._sampleDirPage = String.Copy(_sampleDirPage);
            }
            if (_searchHelpPage != null)
            {
                format._searchHelpPage = String.Copy(_searchHelpPage);
            }

            if (_pluginParents != null)
            {
                format._pluginParents = _pluginParents.Clone();
            }
            if (_pluginChildren != null)
            {
                format._pluginChildren = _pluginChildren.Clone();
            }

            return format;
        }

        #endregion
    }
}
