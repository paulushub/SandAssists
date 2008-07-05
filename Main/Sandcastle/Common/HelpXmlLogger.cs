using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public class HelpXmlLogger : HelpLogger
    {
        #region Constructors and Destructor

        public HelpXmlLogger()
        {
        }

        public HelpXmlLogger(string logFile)
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
