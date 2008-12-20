using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    public abstract class BuildEngine : MarshalByRefObject, IDisposable
    {
        #region Public Fields

        private BuildLogger   _buildLogger;
        private BuildSettings _buildSettings;

        #endregion

        #region Constructors and Destructor

        protected BuildEngine()
        {
            _buildLogger   = new BuildLogger();
            _buildSettings = new BuildSettings();
        }

        ~BuildEngine()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public abstract bool IsInitialized
        {
            get;
        }

        public abstract BuildContext Context
        {
            get;
        }

        public virtual BuildLogger Logger
        {
            get
            {
                return _buildLogger;
            }
        }

        public virtual BuildSettings Settings
        {
            get
            {
                return _buildSettings;
            }
        }

        public virtual int LoggerCount
        {
            get
            {
                if (_buildLogger != null)
                {
                    return _buildLogger.Count;
                }

                return 0;
            }
        }

        public abstract IList<BuildTask> Tasks
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Initialize(BuildContext context);
        public abstract void Uninitialize();

        public abstract bool Build();

        public void AddLogger(HelpLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger",
                    "The logger cannot be null (or Nothing).");
            }

            if (_buildLogger != null)
            {
                _buildLogger.Add(logger);
            }
        }

        public void RemoveLogger(HelpLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger",
                    "The logger cannot be null (or Nothing).");
            }

            if (_buildLogger != null)
            {
                _buildLogger.Remove(logger);
            }
        }

        public void ClearLoggers()
        {
            if (_buildLogger != null)
            {
                _buildLogger.Clear();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_buildLogger != null)
            {
                _buildLogger.Dispose();
                _buildLogger = null;
            }
        }

        #endregion
    }
}
