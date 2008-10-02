using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    public class MailLogger : BuildLogger
    {
        #region Constructors and Destructor

        public MailLogger()
        {
        }

        public MailLogger(string logFile)
            : base(logFile)
        {
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
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
