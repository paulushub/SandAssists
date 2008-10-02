﻿using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    public class HtmlLogger : BuildLogger
    {
        #region Constructors and Destructor

        public HtmlLogger()
        {
        }

        public HtmlLogger(string logFile)
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
