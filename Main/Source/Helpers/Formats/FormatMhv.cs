using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Steps;
using Sandcastle.Utilities;

namespace Sandcastle.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class FormatMhv : BuildFormat
    {
        #region Private Fields

        private int  _tocParent;
        private int  _topicVersion;
        private int  _tocParentVersion;
        private bool _selfbranded;

        #endregion

        #region Constructors and Destructor

        public FormatMhv()
        {  
            this.AddProperty("SharedContentSuffix", "Mshc");
        }

        public FormatMhv(FormatMhv source)
            : base(source)
        {
            _tocParent        = source._tocParent;
            _topicVersion     = source._topicVersion;
            _tocParentVersion = source._tocParentVersion;
            _selfbranded      = source._selfbranded;
        }

        #endregion

        #region Public Properties

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.HtmlHelp3;
            }
        }

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

        public override string TocFileName
        {
            get
            {
                return "HtmlHelpToc3.xml";
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
        /// Gets or sets a value indicating whether to write an <c>XML</c> declaration. 
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> to omit the <c>XML</c> declaration; otherwise, it is
        /// <see langword="false"/>. The default is <see langword="false"/>, an <c>XML</c> 
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

            // Case 1: Closing the HtmlHelp 3.x viewer...
            if (stage == BuildStage.CloseViewer)
            {
                StepMhvViewerClose mhvClose = new StepMhvViewerClose(workingDir);

                return mhvClose;
            }

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

            base.CloseViewerBeforeBuild = true;

            base.IntegrationTarget  = BuildIntegrationTarget.VS2010;
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

            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatMhv-General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "tocparent":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocParent = Convert.ToInt32(tempText);
                                }
                                break;
                            case "topicversion":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _topicVersion = Convert.ToInt32(tempText);
                                }
                                break;
                            case "tocparentversion":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _tocParentVersion = Convert.ToInt32(tempText);
                                }
                                break;
                            case "selfbranded":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _selfbranded = Convert.ToBoolean(tempText);
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
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWritePropertyGroupXml(XmlWriter writer)
        {
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "FormatMhv-General");
            writer.WritePropertyElement("TocParent",        _tocParent);
            writer.WritePropertyElement("TopicVersion",     _topicVersion);
            writer.WritePropertyElement("TocParentVersion", _tocParentVersion);
            writer.WritePropertyElement("Selfbranded",      _selfbranded);
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
            // May check the validity of the parsing process...
            throw new NotImplementedException();
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatMhv format = new FormatMhv(this);

            base.Clone(format);

            return format;
        }

        #endregion
    }
}
