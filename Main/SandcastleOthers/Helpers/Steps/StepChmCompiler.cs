using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
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

        public StepChmCompiler(StepChmCompiler source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildEngine engine)
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

            return base.MainExecute(engine);
        }

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepChmCompiler buildStep = new StepChmCompiler(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
