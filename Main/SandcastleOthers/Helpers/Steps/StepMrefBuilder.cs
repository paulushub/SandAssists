using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepMrefBuilder : StepProcess
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepMrefBuilder()
        {
        }

        public StepMrefBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepMrefBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        public StepMrefBuilder(StepMrefBuilder source)
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
            StepMrefBuilder buildStep = new StepMrefBuilder(this);
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
