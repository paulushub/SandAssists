﻿using System;
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
    [Serializable]
    public sealed class FormatWeb : BuildFormat
    {
        #region Private Fields

        private bool   _useTabView;
        private bool   _includeIndex;
        private bool   _includeSearch;
        private bool   _includeServerSide;
        private string _theme;
        private string _framework;

        #endregion

        #region Constructors and Destructor

        public FormatWeb()
        {
            _theme             = "Smoothness";
            _framework         = "JQuery";

            this.AddProperty("SharedContentSuffix", "Web");
        }

        public FormatWeb(FormatWeb source)
            : base(source)
        {
            _useTabView        = source._useTabView;
            _includeIndex      = source._includeIndex;
            _includeSearch     = source._includeSearch;
            _includeServerSide = source._includeServerSide;
            _theme             = source._theme;
            _framework         = source._framework;
        }

        #endregion

        #region Public Properties

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.WebHelp;
            }
        }

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// This will always return "WebHelp".
        /// </value>
        public override string Name
        {
            get
            {
                return "WebHelp";
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
                return "WebHelpToc.xml";
            }
        }

        public bool UseTabView
        {
            get 
            { 
                return _useTabView; 
            }
            set 
            { 
                _useTabView = value; 
            }
        }

        public bool IncludeIndex
        {
            get 
            { 
                return _includeIndex; 
            }
            set 
            { 
                _includeIndex = value; 
            }
        }

        public bool IncludeSearch
        {
            get 
            { 
                return _includeSearch; 
            }
            set 
            { 
                _includeSearch = value; 
            }
        }

        public bool ServerSide
        {
            get 
            { 
                return _includeServerSide; 
            }
            set 
            { 
                _includeServerSide = value; 
            }
        }

        public string Theme
        {
            get 
            { 
                return _theme; 
            }
            set 
            { 
                _theme = value; 
            }
        }

        public string Framework
        {
            get 
            { 
                return _framework; 
            }
            set 
            { 
                _framework = value; 
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
                String.Format(@"{0}\index.htm", helpFolder));

            // Case 2: Starting the default browser viewer...
            if (stage == BuildStage.StartViewer)
            {
                StepWebViewerStart viewerStart = new StepWebViewerStart(
                    helpDirectory, helpPath);

                return viewerStart;
            }

            // Case 3: Compiling the WebHelp help file...
            if (stage == BuildStage.Compilation)
            {
                string sandassistDir = settings.SandAssistDirectory;

                string webStyle = Path.Combine(sandassistDir,
                    String.Format(@"Web\{0}\Themes\{1}", _framework, _theme));

                if (!Directory.Exists(webStyle))
                {                      
                    return null;
                }

                string tempOutputDir = Path.Combine(workingDir, helpFolder);
                string webHelpDir = Path.Combine(helpDirectory, helpFolder);

                BuildMultiStep listSteps = new BuildMultiStep();
                listSteps.LogTitle    = "Building document output format - " + this.Name;
                listSteps.LogTimeSpan = true;

                StepDirectoryCopy dirCopy = new StepDirectoryCopy();

                dirCopy.LogTitle  = String.Empty;
                dirCopy.Message   = "Copying the format files to the help folder.";
                dirCopy.Recursive = true;
                dirCopy.Add(webStyle, Path.Combine(tempOutputDir, "webtheme"));

                listSteps.Add(dirCopy);

                string tocFile = context["$HelpTocFile"];

                FormatWebOptions options = new FormatWebOptions();
                options.HelpTitle   = helpTitle;
                options.HelpTocFile = tocFile;
                options.ProjectName = helpName;
                options.WorkingDirectory = workingDir;
                options.HtmlDirectory = Path.Combine(workingDir, 
                    @"Output\" + this.FormatFolder);
                options.OutputDirectory = tempOutputDir;

                StepWebBuilder webBuilder = new StepWebBuilder(options, workingDir);
                webBuilder.Format        = this;
                webBuilder.LogTitle      = String.Empty;
                webBuilder.Message       = "Creating the WebHelp files.";
                webBuilder.HelpDirectory = webHelpDir;

                listSteps.Add(webBuilder);

                // 3. Move the output html files to the help folder...
                StepDirectoryMove dirMove = new StepDirectoryMove(workingDir);
                dirMove.LogTitle = String.Empty;
                dirMove.Message  = "Moving the output html files to the help folder.";
                dirMove.Add(options.OutputDirectory, webHelpDir);

                listSteps.Add(dirMove);

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

            this.FormatFolder     = "html0";
            this.OutputFolder     = "WebHelp";
            this.ExternalLinkType = BuildLinkType.Msdn;

            base.CloseViewerBeforeBuild = false;
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

            Debug.Assert(String.Equals(reader.GetAttribute("name"), "FormatWeb-General"));

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
                            case "usetabview":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useTabView = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includeindex":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeIndex = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includesearch":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeSearch = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includeserverside":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeServerSide = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "theme":
                                _theme = reader.ReadString();
                                break;
                            case "framework":
                                _framework = reader.ReadString();
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
            writer.WriteAttributeString("name", "FormatWeb-General");
            writer.WritePropertyElement("UseTabView",        _useTabView);
            writer.WritePropertyElement("IncludeIndex",      _includeIndex);
            writer.WritePropertyElement("IncludeSearch",     _includeSearch);
            writer.WritePropertyElement("IncludeServerSide", _includeServerSide);
            writer.WritePropertyElement("Theme",             _theme);
            writer.WritePropertyElement("Framework",         _framework);
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

        #region Private Methods

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatWeb format = new FormatWeb(this);

            base.Clone(format);

            if (_theme != null)
            {
                format._theme = String.Copy(_theme);
            }
            if (_framework != null)
            {
                format._framework = String.Copy(_framework);
            }

            return format;
        }

        #endregion
    }
}
