using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepChmDbcsFix : StepProcess
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepChmDbcsFix()
        {
        }

        public StepChmDbcsFix(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepChmDbcsFix(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
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
