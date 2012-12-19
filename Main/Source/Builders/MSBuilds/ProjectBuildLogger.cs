using System;

using Microsoft.Build.Utilities;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectBuildLogger : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.ProjectBuildLogger";
        public const string LoggerFileName = "ProjectLogFile.log";

        #endregion

        #region Private Fields

        private int _errorCount;
        private int _warningCount;
        private TaskLoggingHelper _logHelper;

        #endregion

        #region Constructors and Destructor

        public ProjectBuildLogger(TaskLoggingHelper logHelper)
        {
            BuildExceptions.NotNull(logHelper, "logHelper");

            _logHelper    = logHelper;
            _errorCount   = 1;
            _warningCount = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return 
        /// <c>Sandcastle.Loggers.ProjectBuildLogger</c>.
        /// </value>
        public override string Name
        {
            get
            {
                return LoggerName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TaskLoggingHelper LoggingHelper
        {
            get
            {
                return _logHelper;
            }
        }

        #endregion

        #region Protected Properties

        protected override bool IsFileLogging
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
            // The LogMessage method will add a new line...
            _logHelper.LogMessage(String.Empty);
        }

        public override void Write(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public override void Write(string outputText, BuildLoggerLevel level)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            this.WriteLine(outputText, level);
        }

        public override void WriteLine(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex.ToString(), level);
        }

        public override void WriteLine(string outputText, BuildLoggerLevel level)
        {
            if (level == BuildLoggerLevel.None)
            {
                return;
            }

            if (level == BuildLoggerLevel.Started)
            {
                _logHelper.LogMessage(this.FormatText(outputText, level));
            }
            else if (level == BuildLoggerLevel.Ended)
            {
                _logHelper.LogMessage(this.FormatText(outputText, level));
            }
            else if (level == BuildLoggerLevel.Warn)
            {
                _warningCount++;
                _logHelper.LogWarning(null, _warningCount.ToString(), 
                    String.Empty, this.PrefixWarn, 0, 0, 0, 0, outputText);
            }
            else if (level == BuildLoggerLevel.Error)
            {
                _errorCount++;
                _logHelper.LogError(null, _errorCount.ToString(), 
                    String.Empty, this.PrefixError, 0, 0, 0, 0, outputText);
            }
            else
            {
                _logHelper.LogMessage(outputText);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
        }

        protected override void WriteLine(string outputText)
        {
        }

        #endregion
    }
}
