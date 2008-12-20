using System;

namespace Sandcastle.Utilities
{
    [Serializable]
    public sealed class ProcessStartedEventArgs : EventArgs
    {
        #region Private Fields

        private DateTime         _startTime;
        private ProcessInfo _processInfo;

        #endregion

        #region Constructors and Destructor

        public ProcessStartedEventArgs()
        {
            _startTime = DateTime.Now;
        }

        public ProcessStartedEventArgs(DateTime startTime,
            ProcessInfo processInfo)
        {   
            if (processInfo == null)
            {
                throw new ArgumentNullException("processInfo", 
                    "The process information object cannot be null (or Nothing)");
            }

            _startTime   = startTime;
            _processInfo = processInfo;
        }

        #endregion

        #region Public Properties

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }

        public ProcessInfo ProcessInfo
        {
            get
            {
                return _processInfo;
            }
        }

        #endregion
    }
}
