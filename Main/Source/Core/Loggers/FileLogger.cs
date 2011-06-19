using System;
using System.IO;

namespace Sandcastle.Loggers
{
    /// <summary>
    /// This logger records all the relevant build events, information, warnings, 
    /// and errors in plain text to a file.
    /// </summary>
    public class FileLogger : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.FileLogger";

        public const string LoggerFileName = "FileLogFile.log";

        #endregion

        #region Constructors and Destructor

        public FileLogger()
            : base(LoggerFileName)
        {
        }

        public FileLogger(string outputFile)
            : base(outputFile)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return 
        /// <c>Sandcastle.Loggers.FileLogger</c>.
        /// </value>
        public override string Name
        {
            get
            {
                return LoggerName;
            }
        }

        #endregion

        #region Protected Properties

        protected override bool IsFileLogging
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
            this.LogLine();
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
            this.Log(outputText);
        }

        protected override void WriteLine(string outputText)
        {
            this.LogLine(outputText);
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
