using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Tasks
{
    public class TaskMsdnHelpXslHxC : BuildTask
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public TaskMsdnHelpXslHxC()
        {
        }

        public TaskMsdnHelpXslHxC(BuildEngine engine)
            : base(engine)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return null;
            }
        }

        public override string Description
        {
            get
            {
                return null;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return false;
            }
        }

        public override IList<BuildTaskItem> Items
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {   
        }

        public override void Uninitialize()
        {   
        }

        public override bool Execute()
        {
            return false;
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
