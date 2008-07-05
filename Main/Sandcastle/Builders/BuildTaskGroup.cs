using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    public abstract class BuildTaskGroup : BuildTask
    {
        #region Private Fields

        private List<BuildTask> _listTasks;

        #endregion

        #region Constructors and Destructor

        protected BuildTaskGroup()
        {
            _listTasks = new List<BuildTask>();
        }

        protected BuildTaskGroup(BuildEngine engine)
            : base(engine)
        {
            _listTasks = new List<BuildTask>();
        }

        #endregion

        #region Public Properties

        public virtual IList<BuildTask> Tasks
        {
            get
            {
                return _listTasks;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
