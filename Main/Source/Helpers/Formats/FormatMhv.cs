using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Steps;

namespace Sandcastle.Formats
{
    [Serializable]
    public sealed class FormatMhv : BuildFormat
    {
        #region Private Fields

        private int    _tocParent;
        private int    _topicVersion;
        private int    _tocParentVersion;
        private bool   _selfbranded;
        private string _tocFile;

        #endregion

        #region Constructors and Destructor

        public FormatMhv()
        {  
            this.AddProperty("SharedContentSuffix", "Mshc");
        }

        public FormatMhv(FormatMhv source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// This will always return "HtmlHelp 3.x".
        /// </value>
        public override string Name
        {
            get
            {
                return "HtmlHelp 3.x";
            }
        }

        public override string Extension
        {
            get
            {
                return ".htm";
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
                return false;
            }
        }

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.HtmlHelp3;
            }
        }

        /// <summary>
        /// Gets or sets the link type for this output format, used by both the
        /// conceptual and reference help for links between its own files.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLinkType"/> specifying the link
        /// type of this output format.
        /// <note type="important">
        /// The various output formats will correctly set this property, so only change
        /// it if you have to, and understands the implications.
        /// </note>
        /// </value>
        /// <remarks>
        /// Out of the available link types, only the following are currently supported
        /// for this property:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="BuildLinkType.None"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="BuildLinkType.Id"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        public override BuildLinkType LinkType
        {
            get
            {
                return base.LinkType;
            }
            set
            {
                if (value == BuildLinkType.None || value == BuildLinkType.Id)
                {
                    base.LinkType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the link type for this output format to external help files, 
        /// mostly the online MSDN documentations.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLinkType"/> specifying the e
        /// xternal link type of this output format. The default is 
        /// <see cref="BuildLinkType.Msdn"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Out of the available link types, only the following are currently supported
        /// for this property:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="BuildLinkType.None"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="BuildLinkType.Msdn"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="BuildLinkType.Id"/></description>
        /// </item>
        /// </list>
        /// <para>
        /// Setting this property to <see cref="BuildLinkType.Index"/> will be
        /// automatically converted to <see cref="BuildLinkType.Id"/>, since that
        /// is the equivalent in MS Help Viewer.
        /// </para>
        /// </remarks>
        public override BuildLinkType ExternalLinkType
        {
            get
            {
                return base.ExternalLinkType;
            }
            set
            {
                if (value == BuildLinkType.Index)
                {
                    value = BuildLinkType.Id;
                }

                if (value == BuildLinkType.None || value == BuildLinkType.Id ||
                    value == BuildLinkType.Msdn)
                {
                    base.ExternalLinkType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to write an XML declaration. 
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> to omit the XML declaration; otherwise, it is
        /// <see langword="false"/>. The default is <see langword="false"/>, an XML 
        /// declaration is not written.
        /// </value>
        public override bool OmitXmlDeclaration
        {
            get
            {
                return base.OmitXmlDeclaration;
            }
            set
            {
                if (value == false)
                {
                    base.OmitXmlDeclaration = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add the XHTML namespace
        /// to the output HTML.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> to add the XHTML namespace; otherwise, 
        /// it is <see langword="false"/>. The default is <see langword="false"/>, 
        /// an XHTML namespace is not added.
        /// </value>
        public override bool AddXhtmlNamespace
        {
            get
            {
                return base.AddXhtmlNamespace;
            }
            set
            {
                if (value)
                {
                    base.AddXhtmlNamespace = value;
                }
            }
        }

        public bool Selfbranded
        {
            get
            {
                return _selfbranded;
            }
            set
            {
                _selfbranded = value;
            }
        }

        public int TocParent
        {
            get 
            { 
                return _tocParent; 
            }
            set 
            { 
                _tocParent = value; 
            }
        }

        public int TopicVersion
        {
            get 
            { 
                return _topicVersion; 
            }
            set 
            { 
                _topicVersion = value; 
            }
        }

        public int TocParentVersion
        {
            get 
            { 
                return _tocParentVersion; 
            }
            set 
            { 
                _tocParentVersion = value; 
            }
        }

        public string TocFile
        {
            get 
            { 
                return _tocFile; 
            }
            internal set 
            { 
                _tocFile = value; 
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
            string helpPath   = Path.Combine(helpDirectory,
                String.Format(@"{0}\{1}.mshc", helpFolder, helpName));
            string helpSetup  = Path.Combine(helpDirectory,
                String.Format(@"{0}\{1}", helpFolder, 
                StepMhvBuilder.HelpContentSetup));

            // Case 2: Starting the HtmlHelp 3.x viewer...
            if (stage == BuildStage.StartViewer)
            {
                StepMhvViewerStart mhvStart = new StepMhvViewerStart(
                    helpDirectory, helpPath, helpSetup);

                return mhvStart;
            }

            // Case 3: Compiling the HtmlHelp 3.x help file...
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

                // 2. Move the output html files to the help folder for compilation...
                StepDirectoryMove dirMove = new StepDirectoryMove(workingDir);
                dirMove.LogTitle = String.Empty;
                dirMove.Message  = "Moving the output html files to the help folder for compilation";
                dirMove.Add(@"Output\" + this.FormatFolder, helpFolder + @"\html");

                listSteps.Add(dirMove);

                StepMhvBuilder mhvBuilder = new StepMhvBuilder(workingDir);

                mhvBuilder.Message       = "Compiling the help file.";
                mhvBuilder.LogTitle      = String.Empty;
                mhvBuilder.HelpName      = helpName;
                mhvBuilder.HelpFolder    = helpFolder;
                mhvBuilder.HelpDirectory = helpDirectory;
                mhvBuilder.OptimizeStyle = this.OptimizeStyle;

                listSteps.Add(mhvBuilder);

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

            base.FormatFolder       = "html3";
            base.OutputFolder       = "HelpViewer";
            base.LinkType           = BuildLinkType.Id;
            base.ExternalLinkType   = BuildLinkType.Id;

            base.OmitXmlDeclaration = false;
            base.AddXhtmlNamespace  = true;

            _tocParent              = -1;
            _topicVersion           = 100;
            _tocParentVersion       = 100; 
            _selfbranded            = true;
            _tocFile                = BuildToc.HelpToc;

            base.CloseViewerBeforeBuild = false;

            base.IntegrationTarget  = BuildIntegrationTarget.VS2010;
        }

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatMhv format = new FormatMhv(this);

            return format;
        }

        #endregion
    }
}
