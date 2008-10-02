using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepChmViewerStart : BuildStep
    {
        #region Private Fields

        private string _htmlHelpFile;

        #endregion

        #region Constructors and Destructor

        public StepChmViewerStart()
        {
        }

        public StepChmViewerStart(string workingDir, string htmlHelpFile)
            : base(workingDir)
        {
            _htmlHelpFile = htmlHelpFile;
        }

        public StepChmViewerStart(StepChmViewerStart source)
            : base(source)
        {
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

        protected override bool MainExecute(BuildEngine engine)
        {
            BuildLogger logger = engine.Logger;

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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepChmViewerStart buildStep = new StepChmViewerStart(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
