using System;

namespace Sandcastle.Loggers
{
    /// <summary>
    /// This logger records all the relevant build events, information, warnings, 
    /// and errors in a plain text to the standard Console window.
    /// </summary>
    public class ConsoleLogger : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.ConsoleLogger";

        public const string LoggerFileName = "ConsoleLogFile.log";

        #endregion

        #region Private Fields

        private bool _isFileLogging;

        #endregion

        #region Constructors and Destructor

        public ConsoleLogger()
            : this(LoggerFileName, false)
        {
        }

        public ConsoleLogger(bool logToFile)
            : this(LoggerFileName, logToFile)
        {
        }

        public ConsoleLogger(string logFile)
            : this(logFile, true)
        {
        }

        public ConsoleLogger(string logFile, bool logToFile)
            : base(logFile)
        {
            _isFileLogging = logToFile;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return 
        /// <c>Sandcastle.Loggers.ConsoleLogger</c>.
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
                return _isFileLogging;
            }
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
            Console.WriteLine();
            this.LogLine();
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
            Console.Write(outputText);
            this.Log(outputText);
        }

        protected override void WriteLine(string outputText)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                Console.WriteLine();
                this.LogLine();
            }
            else
            {
                Console.WriteLine(outputText);
                this.LogLine(outputText);
            }
        }

        #endregion
    }
}
