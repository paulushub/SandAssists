using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public class HelpFileLogger : HelpLogger
    {
        #region Public Static Fields

        public const string DefaultOutputFile = "BuildLog.log";

        #endregion

        #region Constructors and Destructor

        public HelpFileLogger()
            : base(DefaultOutputFile, false, null)
        {
        }

        public HelpFileLogger(string outputFile, bool append)
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
