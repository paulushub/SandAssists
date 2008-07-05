using System;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    public abstract class BuildTask : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private BuildEngine _buildEngine;
        private HelpLogger  _buildLogger;

        #endregion

        #region Constructors and Destructor

        protected BuildTask()
        {   
        }

        protected BuildTask(BuildEngine engine)
        {
            _buildEngine = engine;
            if (engine != null)
            {
                _buildLogger = engine.Logger;
            }
        }

        ~BuildTask()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public abstract string Name
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract bool IsInitialized
        {
            get;
        }

        public virtual BuildEngine Engine
        {
            get
            {
                return _buildEngine;
            }

            set
            {
                if (value != null)
                {
                    _buildEngine = value;
                    this.Logger  = value.Logger;
                }
            }
        }

        public virtual HelpLogger Logger
        {
            get
            {
                return _buildLogger;
            }

            set
            {
                if (value != null)
                {
                    _buildLogger = value;
                }
            }
        }

        public abstract IList<BuildTaskItem> Items
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Initialize(IList<BuildTaskItem> taskItems);
        public abstract void Uninitialize();

        public abstract bool Execute();

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
