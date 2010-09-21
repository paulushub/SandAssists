using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepWebViewerStart : BuildStep
    {
        #region Private Fields

        private string _webHelpFile;

        #endregion

        #region Constructors and Destructor

        public StepWebViewerStart()
        {
            this.ContinueOnError = true;
        }

        public StepWebViewerStart(string workingDir, string webHelpFile)
            : base(workingDir)
        {
            _webHelpFile         = webHelpFile;
            this.ContinueOnError = true;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            if (String.IsNullOrEmpty(_webHelpFile) || !File.Exists(_webHelpFile))
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
                logger.WriteLine("Opening: " + _webHelpFile, BuildLoggerLevel.Info);
            }

            try
            {
                Process process = Process.Start(_webHelpFile);
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

                return true;
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
