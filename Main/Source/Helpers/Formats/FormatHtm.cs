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
    public class FormatHtm : BuildFormat
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public FormatHtm()
        {
        }

        public FormatHtm(FormatHtm source)
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
        /// This will always return "WebHelp".
        /// </value>
        public override string FormatName
        {
            get
            {
                return "WebHelp";
            }
        }

        public override string FormatExtension
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
                return BuildFormatType.WebHelp;
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
                CultureInfo culture = settings.CultureInfo;
                int lcid = 1033;
                if (culture != null)
                {
                    lcid = culture.LCID;
                }
                
                BuildMultiStep listSteps = new BuildMultiStep();
                // 2. Move the output html files to the help folder for compilation...
                StepDirectoryMove dirMove = new StepDirectoryMove(workingDir);
                dirMove.Add(@"Output\" + this.FormatFolder, helpFolder + @"\html");
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
        }

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatHtm format = new FormatHtm(this);

            return format;
        }

        #endregion
    }
}
