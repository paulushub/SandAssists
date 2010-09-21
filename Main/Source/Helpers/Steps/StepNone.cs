using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepNone : BuildStep
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepNone()
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
