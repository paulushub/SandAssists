using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepWebViewerStart : BuildStep
    {
        #region Private Fields

        private string _htmlHelpFile;

        #endregion

        #region Constructors and Destructor

        public StepWebViewerStart()
        {
        }

        public StepWebViewerStart(string workingDir, string htmlHelpFile)
            : base(workingDir)
        {
            _htmlHelpFile = htmlHelpFile;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            if (logger != null)
            {
                logger.WriteLine("Opening compiled HtmlHelp file...",
                    BuildLoggerLevel.Started);
            }

            if (String.IsNullOrEmpty(_htmlHelpFile) ||
                File.Exists(_htmlHelpFile) == false)
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
                logger.WriteLine("Help File: " + _htmlHelpFile, BuildLoggerLevel.Info);
            }

            bool isStarted = false;
            try
            {
                Process process = Process.Start(_htmlHelpFile);

                isStarted = (process != null);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (logger != null)
            {
                logger.WriteLine("Opening compiled HtmlHelp file.",
                    BuildLoggerLevel.Ended);
            }

            return isStarted;
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
