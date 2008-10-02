using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepXslTransform : StepProcess
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepXslTransform()
        {
        }

        public StepXslTransform(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepXslTransform(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        public StepXslTransform(StepXslTransform source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool MainExecute(BuildEngine engine)
        {
            return base.MainExecute(engine);
        }

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepXslTransform buildStep = new StepXslTransform(this);
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
