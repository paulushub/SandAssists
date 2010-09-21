using System;
using System.Text;
using System.Threading;

namespace Sandcastle.Steps
{
    /// <summary>
    /// A build step for sleeping a specified period of time. This is useful 
    /// when a build requires an interval between steps.
    /// </summary>
    public sealed class StepSleep : BuildStep
    {
        #region Private Fields

        private TimeSpan _duration;

        #endregion

        #region Constructors and Destructor

        public StepSleep()
        {
            this.ContructorDefaults();
        }

        public StepSleep(string workingDir)
            : base(workingDir)
        {
            this.ContructorDefaults();
        }

        private void ContructorDefaults()
        {
            _duration            = TimeSpan.Zero;
            this.LogTitle        = "Sleeping for a specified period of time.";
            this.ContinueOnError = true;
        }

        #endregion

        #region Public Properties

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            if (_duration == TimeSpan.MaxValue || 
                _duration == TimeSpan.MinValue ||
                _duration == TimeSpan.Zero)
            {
                return false;
            }

            try
            {
                Thread.Sleep(_duration);

                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {      
                BuildLogger logger = context.Logger;
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

            	return false;
            }
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
