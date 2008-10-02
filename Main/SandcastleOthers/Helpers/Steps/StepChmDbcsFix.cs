using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
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

        public StepChmDbcsFix(StepChmDbcsFix source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildEngine engine)
        {
            return base.MainExecute(engine);
        }

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepChmDbcsFix buildStep = new StepChmDbcsFix(this);
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
