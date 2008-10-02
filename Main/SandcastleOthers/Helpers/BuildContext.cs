using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Sandcastle.Loggers;

namespace Sandcastle
{
    public class BuildContext : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private BuildSystem _buildSystem;
        private BuildEngine _buildEngine;
        private BuildState      _buildState;
        private BuildType _buildType;
        private EventWaitHandle _waitHandle;

        #endregion

        #region Constructors and Destructor

        public BuildContext()
        {
            _buildType   = BuildType.Development;
            _buildState  = BuildState.None;
            _buildSystem = BuildSystem.Console;
            _waitHandle  = new ManualResetEvent(false);
        }

        ~BuildContext()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsCancelled
        {
            get
            {
                return (_buildState == BuildState.Cancelled);
            }
        }

        public BuildState State
        {
            get
            {
                return _buildState;
            }
        }

        public BuildType BuildType
        {
            get 
            { 
                return _buildType; 
            }
            set 
            { 
                _buildType = value; 
            }
        }

        public BuildLogger Logger
        {
            get
            {
                if (_buildEngine != null)
                {
                    return _buildEngine.Logger;
                }

                return null;
            }
        }

        public BuildSettings Settings
        {
            get
            {
                if (_buildEngine != null)
                {
                    return _buildEngine.Settings;
                }

                return null;
            }
        }

        public BuildSystem System
        {
            get 
            { 
                return _buildSystem; 
            }
            set 
            { 
                _buildSystem = value; 
            }
        }

        public BuildEngine Engine
        {
            get 
            { 
                return _buildEngine; 
            }
        }

        public WaitHandle BuildWait
        {
            get
            {
                return _waitHandle;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Attach(BuildEngine engine)
        {
            BuildExceptions.NotNull(engine, "engine");

            if (_buildEngine != null)
            {
                return false;
            }
            _buildEngine = engine;

            return true;
        }

        public virtual void Detach()
        {
            _buildEngine = null;
        }

        public virtual void SetState(BuildState state)
        {
            _buildState = state;
            if (state == BuildState.Cancelled)
            {
                if (_waitHandle != null)
                {
                    _waitHandle.Set();
                }
            }
        }

        public virtual bool StepCreated(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            if (_buildState == BuildState.Cancelled ||
                _buildState == BuildState.Error)
            {
                return false;
            }

            return true;
        }

        public virtual bool StepStarts(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            if (_buildState == BuildState.Cancelled || 
                _buildState == BuildState.Error)
            {
                return false;
            }

            if (_waitHandle != null)
            {
                return _waitHandle.Reset();
            }

            return true;
        }

        public virtual bool StepEnds(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            return true;
        }

        public virtual bool StepError(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            return true;
        }

        public virtual BuildLogger CreateLogger(BuildSettings settings)
        {
            string workingDir = settings.WorkingDirectory;
            string logFile = Path.Combine(workingDir, settings.LogFile);

            if (settings.IsCombinedBuild == false && File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            if (String.IsNullOrEmpty(workingDir) ||
                _buildSystem == BuildSystem.Console)
            {
                if (String.IsNullOrEmpty(logFile) == false || settings.UseLogFile)
                {
                    ConsoleLogger logger = new ConsoleLogger(logFile);
                    logger.KeepLog = settings.KeepLogFile;
                    
                    return logger;
                }
                else
                {
                    return new ConsoleLogger();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(logFile) == false || settings.UseLogFile)
                {
                    FileLogger logger = new FileLogger(logFile);
                    logger.KeepLog = settings.KeepLogFile;

                    return logger;
                }
            }

            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_waitHandle != null)
            {
                _waitHandle.Close();
                _waitHandle = null;
            }
        }

        #endregion
    }
}
