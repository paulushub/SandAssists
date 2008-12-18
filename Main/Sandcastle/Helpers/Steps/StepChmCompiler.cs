using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepChmCompiler : StepProcess
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepChmCompiler()
        {
        }

        public StepChmCompiler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepChmCompiler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
            string strApp = Path.GetFileName(this.Application);

            if (String.Equals(strApp, "hhc.exe", 
                StringComparison.CurrentCultureIgnoreCase))
            {   
                // hhc.exe is different, returns 0 if an error occurs
                this.ExpectedExitCode = 1;
            }
            else
            {
                // Unlike the hhc.exe tool,  SbAppLocale.exe will return 0 on success.
                this.ExpectedExitCode = 0;
            }

            return base.MainExecute(context);
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
