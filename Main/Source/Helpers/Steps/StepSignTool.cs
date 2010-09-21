using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepSignTool : BuildStep
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepSignTool()
        {
            this.ConstructorDefaults();
        }

        public StepSignTool(string workingDir)
            : base(workingDir)
        {
            this.ConstructorDefaults();
        }

        private void ConstructorDefaults()
        {   
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            return true;
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
