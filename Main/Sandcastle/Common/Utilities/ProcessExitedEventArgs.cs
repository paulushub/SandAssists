using System;

namespace Sandcastle.Utilities
{
    [Serializable]
    public sealed class ProcessExitedEventArgs : EventArgs
    {
        #region Private Fields

        private int      _exitCode;
        private string   _processName;
        private DateTime _startTime;
        private DateTime _exitTime;
        private TimeSpan _totalProcessorTime;
        private TimeSpan _userProcessorTime;

        #endregion

        #region Constructors and Destructor

        public ProcessExitedEventArgs()
        {
            _exitCode = Int32.MaxValue;
        }

        public ProcessExitedEventArgs(int exitCode, string processName,
            DateTime startTime, DateTime exitTime, TimeSpan totalProcessorTime,
            TimeSpan userProcessorTime)
        {
            _exitCode           = exitCode;
            _processName        = processName;
            _startTime          = startTime;
            _exitTime           = exitTime;
            _totalProcessorTime = totalProcessorTime;
            _userProcessorTime  = userProcessorTime;
        }

        #endregion

        #region Public Properties

        //
        // Summary:
        //     Gets the name of the process.
        //
        // Returns:
        //     The name that the system uses to identify the process to the user.
        public string ProcessName 
        { 
            get
            {
                return _processName;
            }
        }

        //
        // Summary:
        //     Gets the time that the associated process was started.
        //
        // Returns:
        //     A System.DateTime that indicates when the process started. This only has
        //     meaning for started processes.
        public DateTime StartTime 
        { 
            get
            {
                return _startTime;
            }
        }
   
        //
        // Summary:
        //     Gets the value that the associated process specified when it terminated.
        //
        // Returns:
        //     The code that the associated process specified when it terminated.
        public int ExitCode 
        { 
            get
            {
                return _exitCode;
            }
        }

        //
        // Summary:
        //     Gets the time that the associated process exited.
        //
        // Returns:
        //     A System.DateTime that indicates when the associated process was terminated.
        public DateTime ExitTime 
        { 
            get
            {
                return _exitTime;
            }
        }

        //
        // Summary:
        //     Gets the total processor time for this process.
        //
        // Returns:
        //     A System.TimeSpan that indicates the amount of time that the associated process
        //     has spent utilizing the CPU. This value is the sum of the System.Diagnostics.Process.UserProcessorTime
        //     and the System.Diagnostics.Process.PrivilegedProcessorTime.
        public TimeSpan TotalProcessorTime 
        { 
            get
            {
                return _totalProcessorTime;
            }
        }
        
        //
        // Summary:
        //     Gets the user processor time for this process.
        //
        // Returns:
        //     A System.TimeSpan that indicates the amount of time that the associated process
        //     has spent running code inside the application portion of the process (not
        //     inside the operating system core).
        public TimeSpan UserProcessorTime 
        { 
            get
            {
                return _userProcessorTime;
            }
        }

        #endregion
    }
}
