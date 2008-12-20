using System;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.Tasks
{
    public class TaskRunProcess : BuildTask
    {
        #region Private Fields

        private bool                 _isInitialized;
        private ProcessInfo          _processInfo; 
        private IList<BuildTaskItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public TaskRunProcess()
        {
        }

        public TaskRunProcess(BuildEngine engine, ProcessInfo info)
            : base(engine)
        {
            _processInfo = info;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "RunProcess";
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
                return _isInitialized;
            }
        }

        public override IList<BuildTaskItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        public virtual ProcessInfo ProcessInfo
        {
            get
            {
                return _processInfo;
            }

            set
            {
                _processInfo = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {
            _listItems     = taskItems;
            _isInitialized = (_processInfo != null);
        }

        public override void Uninitialize()
        {
            _isInitialized = false;
            _listItems     = null;
            _processInfo   = null;
        }

        public override bool Execute()
        {
            if (!_isInitialized)
            {
                TaskException.ThrowTaskNotInitialized("TaskRunProcess");
            }

            if (_processInfo == null)
            {
                throw new TaskException("The process information is not specified.");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskRunProcess");
            }

            try
            {
                ProcessRunner runner = new ProcessRunner();

                runner.ProcessStarted += new EventHandler<ProcessStartedEventArgs>(
                    OnProcessStarted);
                runner.ProcessExited  += new EventHandler<ProcessExitedEventArgs>(
                    OnProcessExited);

                _processInfo.WaitForExit = true; //TODO-PAUL - any better way?
                bool runResult = runner.Start(_processInfo, logger);

                return runResult;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex, HelpLoggerLevel.Error);

                return false;
            }
        }

        #endregion

        #region Private Methods

        private void OnProcessStarted(object sender, ProcessStartedEventArgs e)
        {
        }

        private void OnProcessExited(object sender, ProcessExitedEventArgs e)
        {
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
