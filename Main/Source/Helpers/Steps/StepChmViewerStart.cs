using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepChmViewerStart : BuildStep
    {
        #region Private Fields

        private string _htmlHelpFile;

        #endregion

        #region Constructors and Destructor

        public StepChmViewerStart()
        {
            this.LogTitle         = "Opening compiled Help 1.x file.";
            this.ContinueOnError = true;
        }

        public StepChmViewerStart(string workingDir, string htmlHelpFile)
            : base(workingDir)
        {
            _htmlHelpFile        = htmlHelpFile;
            this.LogTitle         = "Opening compiled Help 1.x file.";
            this.ContinueOnError = true;
        }

        #endregion

        #region Public Properties

        public string HtmlHelpFile
        {
            get
            {
                return _htmlHelpFile;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            if (String.IsNullOrEmpty(_htmlHelpFile) || !File.Exists(_htmlHelpFile))
            {
                if (logger != null)
                {
                    logger.WriteLine("The help file is not available.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (logger != null)
            {
                logger.WriteLine("Opening: " + _htmlHelpFile, BuildLoggerLevel.Info);
            }

            try
            {
                Process process = Process.Start(_htmlHelpFile);

                // The return could be null, if no process resource is started 
                // (for example, if an existing process is reused as in browsers).
                if (process != null)
                {
                    process.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
