using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    /// <summary>
    /// This logger records all the relevant build events, information, warnings, 
    /// and errors in plain text to a file.
    /// </summary>
    public class FileLogger : BuildLogger
    {
        #region Public Static Fields

        public const string DefaultOutputFile = "BuildLog.log";

        #endregion

        #region Constructors and Destructor

        public FileLogger()
            : base(DefaultOutputFile, false, null)
        {
        }

        public FileLogger(string outputFile)
            : base(outputFile, false, null)
        {
        }

        public FileLogger(string outputFile, bool append)
            : base(outputFile, append, null)
        {
        }

        #endregion

        #region Public Properties

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
