using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    /// <summary>
    /// This build step simply displays or writes a message for logging.
    /// </summary>
    public sealed class StepMessage : BuildStep
    {
        #region Private Fields

        private string           _loggerText;
        private BuildLoggerLevel _loggerLevel;

        #endregion

        #region Constructors and Destructor

        public StepMessage()
        {
            _loggerLevel         = BuildLoggerLevel.Info;
            this.LogTimeSpan     = false;
            this.ContinueOnError = true;
        }

        public StepMessage(string workingDir)
            : base(workingDir)
        {
            _loggerLevel         = BuildLoggerLevel.Info;
            this.LogTimeSpan     = false;
            this.ContinueOnError = true;
        }

        #endregion

        #region Public Properties

        public string LoggerText
        {
            get
            {
                return _loggerText;
            }
            set
            {
                _loggerText = value;
            }
        }

        public BuildLoggerLevel LoggerLevel
        {
            get
            {
                return _loggerLevel;
            }
            set
            {
                _loggerLevel = value;
            }
        }

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;
            if (logger != null && !String.IsNullOrEmpty(_loggerText))
            {
                logger.WriteLine(_loggerText, _loggerLevel);

                return true;
            }

            return false;
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
