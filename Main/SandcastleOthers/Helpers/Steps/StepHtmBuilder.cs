using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepHtmBuilder : BuildStep
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepHtmBuilder()
        {
        }

        public StepHtmBuilder(StepHtmBuilder source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool MainExecute(BuildEngine engine)
        {
            return true;
        }

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepHtmBuilder buildStep = new StepHtmBuilder(this);
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
