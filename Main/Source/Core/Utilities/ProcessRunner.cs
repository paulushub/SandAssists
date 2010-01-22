using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Utilities
{
    public class ProcessRunner : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private DateTime    _startTime;
        private DateTime    _exitTime;

        private Process     _process;
        private BuildLogger _processLogger;
        private ProcessInfo _processInfo;

        #endregion

        #region Constructors and Destructor

        public ProcessRunner()
        {   
        }

        ~ProcessRunner()
        {
            Dispose(false);
        }

        #endregion

        #region Public Events

        public event EventHandler<ProcessEventArgs> ProcessError;
        public event EventHandler<ProcessEventArgs> ProcessOutput;
        public event EventHandler<ProcessStartedEventArgs> ProcessStarted;
        public event EventHandler<ProcessExitedEventArgs>  ProcessExited;

        #endregion

        #region Public Properties

        public bool IsRunning
        {
            get
            {
                if (_process != null)
                {
                    return !_process.HasExited;
                }

                return false;
            }
        }

        public ProcessInfo Info
        {
            get
            {
                return _processInfo;
            }
        }

        public BuildLogger Logger
        {
            get
            {
                return _processLogger;
            }

            set
            {
                _processLogger = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Start(ProcessInfo info)
        {
            return this.Start(info, null);
        }

        public virtual bool Start(ProcessInfo info, BuildLogger buildLogger)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info",
                    "The process information cannot be null (Nothing).");
            }

            if (_process != null)
            {
                if (_process.HasExited == false)
                {
                    return false;
                }

                _process.Dispose();
                _process = null;
            }

            try
            {
                _process = new Process();

                ProcessStartInfo startInfo = _process.StartInfo;

                startInfo.CreateNoWindow         = info.CreateNoWindow;
                startInfo.FileName               = info.FileName;
                startInfo.WorkingDirectory       = info.WorkingDirectory;
                startInfo.RedirectStandardOutput = info.RedirectOutput;
                startInfo.RedirectStandardError  = info.RedirectError;
                startInfo.UseShellExecute        = info.UseShellExecute;
                startInfo.Arguments              = info.Arguments;

                _process.EnableRaisingEvents = true;

                _process.ErrorDataReceived  += new DataReceivedEventHandler(
                    OnProcessErrorReceived);
                _process.OutputDataReceived += new DataReceivedEventHandler(
                    OnProcessOutputReceived);
                _process.Exited += new EventHandler(OnProcessExited);

                _processInfo   = info;
                _processLogger = buildLogger;

                // Now, start the process - there will still not be output till...
                _startTime = DateTime.Now;

                _process.Start();

                // if we get here, send an event to the caller
                if (this.ProcessStarted != null)
                {
                    ProcessStartedEventArgs args = new 
                        ProcessStartedEventArgs(_startTime, info);
                    this.ProcessStarted(this, args);
                }

                // Start the asynchronous read of the output stream
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();

                // We must wait for the process to complete, if required...
                if (info.WaitForExit)
                {
                    _process.WaitForExit();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (_process != null)
                {
                    _process.Dispose();
                    _process = null;
                }
                if (buildLogger != null)
                {
                    buildLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        public virtual bool Stop()
        {
            if (_process == null)
            {
                return false;
            }

            try
            {
                if (_process.HasExited == false) 
                {
                    _process.Kill();
                    _process.Close();
                    _process.Dispose();
                    _process = null;
				}

                return true;
           }
            catch (Exception ex)
            {
                if (_processLogger != null)
                {
                    _processLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnProcessExited(object sender, EventArgs e)
        {
            _exitTime = DateTime.Now;

            if (this.ProcessExited != null)
            {                
                int exitCode       = Int32.MinValue;
                string processName = String.Empty;
                DateTime startTime = _startTime;
                DateTime exitTime  = _exitTime;
                TimeSpan totalProcessorTime = TimeSpan.MinValue;
                TimeSpan userProcessorTime  = TimeSpan.MinValue;

                try
                {
                    // really, this should not be null at this stage!!!
                    if (_process != null) 
                    {   
                        exitCode           = _process.ExitCode;
                        processName        = _process.ProcessName;
                        startTime          = _process.StartTime;
                        exitTime           = _process.ExitTime;
                        totalProcessorTime = _process.TotalProcessorTime;
                        userProcessorTime  = _process.UserProcessorTime;
                    }
                }
                catch
                {   
                }

                ProcessExitedEventArgs args = new ProcessExitedEventArgs(
                    exitCode, processName, startTime, exitTime, totalProcessorTime,
                    userProcessorTime);

                this.ProcessExited(this, args);
            }

            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }
        }

        protected virtual void OnProcessOutputReceived(object sender, 
            DataReceivedEventArgs e)
        {
            string dataReceived = e.Data;
            if (_processLogger != null)
            {
                _processLogger.WriteLine(dataReceived, BuildLoggerLevel.None);
            }

            if (this.ProcessOutput != null)
            {
                this.ProcessOutput(this, new ProcessEventArgs(dataReceived));
            }
        }

        protected virtual void OnProcessErrorReceived(object sender, 
            DataReceivedEventArgs e)
        {
            string dataReceived = e.Data;
            if (_processLogger != null)
            {
                _processLogger.WriteLine(dataReceived, BuildLoggerLevel.Error);
            }

            if (this.ProcessError != null)
            {
                this.ProcessError(this, new ProcessEventArgs(dataReceived));
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
            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }
        }

        #endregion
    }
}
