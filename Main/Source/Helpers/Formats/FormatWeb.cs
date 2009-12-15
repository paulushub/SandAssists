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
    public class FormatWeb : FormatHtm
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
            _theme     = "Smoothness";
            _framework = "JQuery";
        }

        public FormatWeb(FormatWeb source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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
            string helpPath = Path.Combine(helpDirectory,
                String.Format(@"{0}\index.htm", helpFolder));

            // Case 2: Starting the HtmlHelp 2.x viewer...
            if (stage == BuildStage.StartViewer)
            {
            }

            // Case 3: Compiling the HtmlHelp 2.x help file...
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
                StepDirectoryCopy dirCopy = new StepDirectoryCopy();

                dirCopy.Recursive = true;
                dirCopy.Add(webStyle, Path.Combine(tempOutputDir, "webtheme"));
                listSteps.Add(dirCopy);

                FormatWebOptions options = new FormatWebOptions();
                options.HelpTitle   = helpTitle;
                options.HelpTocFile = context["$HelpTocFile"];
                options.ProjectName = helpName;
                options.WorkingDirectory = workingDir;
                options.HtmlDirectory = Path.Combine(workingDir, 
                    @"Output\" + this.FormatFolder);
                options.OutputDirectory = tempOutputDir;

                StepWebBuilder webBuilder = new StepWebBuilder(options, workingDir);
                webBuilder.HelpDirectory = webHelpDir;

                listSteps.Add(webBuilder);

                // 3. Move the output html files to the help folder for compilation...
                StepDirectoryMove dirMove = new StepDirectoryMove(workingDir);
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
        }

        #endregion

        #region Private Methods

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatWeb format = new FormatWeb(this);

            return format;
        }

        #endregion
    }
}
