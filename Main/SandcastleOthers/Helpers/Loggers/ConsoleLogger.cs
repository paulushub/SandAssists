using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    public class ConsoleLogger : BuildLogger
    {
        #region Constructors and Destructor

        public ConsoleLogger()
        {
        }

        public ConsoleLogger(string logFile)
            : base(logFile)
        {
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
