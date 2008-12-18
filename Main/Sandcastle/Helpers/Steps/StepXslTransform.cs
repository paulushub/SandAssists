using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepXslTransform : StepProcess
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepXslTransform()
        {
            this.Message = "Applying Transformation";
        }                   

        public StepXslTransform(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.Message = "Applying Transformation";
        }

        public StepXslTransform(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.Message = "Applying Transformation";
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

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
