using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepProcess : BuildStep
    {
        #region Private Fields

        private int _processExitCode;
        private int _expectedExitCode;
        private int _copyright;
        private int    _messageCount;
        private bool   _shellExecute;
        private bool   _ignoreExitCode;
        private bool   _ignoreWhitespace;
        private bool   _redirectOutput;
        private bool   _redirectError;
        private string _application;
        private string _arguments;
        private string _message;
        private Encoding _encoding;

        [NonSerialized]
        private BuildLogger _logger;

        #endregion

        #region Constructors and Destructor

        public StepProcess()
        {
        }

        public StepProcess(string workingDir, string arguments)
            : this(workingDir, null, arguments)
        {
        }

        public StepProcess(string workingDir, string fileName, string arguments)
            : base(workingDir)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                _arguments    = "/C " + arguments;
                _application  = "cmd";
                _shellExecute = true;
            }
            else
            {
                _arguments    = arguments;
                _application  = fileName;
            }

            _ignoreWhitespace = true;
            _redirectOutput   = true;
        }

        public StepProcess(StepProcess source)
            : base(source)
        {
            _logger           = source._logger;
            _arguments        = source._arguments;
            _application      = source._application;
            _shellExecute     = source._shellExecute;
            _ignoreExitCode   = source._ignoreExitCode;
            _processExitCode  = source._processExitCode;
            _expectedExitCode = source._expectedExitCode;
            _redirectOutput   = source._redirectOutput;
            _redirectError    = source._redirectError;
        }

        #endregion

        #region Public Properties

        public int CopyrightNotice
        {
            get 
            { 
                return _copyright; 
            }
            set 
            { 
                _copyright = value; 
            }
        }

        public string Message
        {
            get 
            { 
                return _message; 
            }
            set 
            { 
                _message = value; 
            }
        }

        public bool UseShellExecute
        {
            get
            {
                return _shellExecute;
            }
            set
            {
                _shellExecute = value;
            }
        }

        public string Application
        {
            get
            {
                return _application;
            }
            set
            {
                _application = value;
            }
        }

        public string Arguments
        {
            get
            {
                return _arguments;
            }
            set
            {
                _arguments = value;
            }
        }

        public Encoding Encoding
        {
            get 
            { 
                return _encoding; 
            }
            set 
            { 
                _encoding = value; 
            }
        }

        public int ProcessExitCode
        {
            get
            {
                return _processExitCode;
            }
            set
            {
                _processExitCode = value;
            }
        }

        public int ExpectedExitCode
        {
            get
            {
                return _expectedExitCode;
            }
            set
            {
                _expectedExitCode = value;
            }
        }

        public bool IgnoreExitCode
        {
            get
            {
                return _ignoreExitCode;
            }
            set
            {
                _ignoreExitCode = value;
            }
        }

        public bool IgnoreWhitespace
        {
            get 
            { 
                return _ignoreWhitespace; 
            }

            set 
            { 
                _ignoreWhitespace = value; 
            }
        }

        public bool RedirectOutput
        {
            get 
            { 
                return _redirectOutput; 
            }
            set 
            { 
                _redirectOutput = value; 
            }
        }

        public bool RedirectError
        {
            get 
            { 
                return _redirectError; 
            }
            set 
            { 
                _redirectError = value; 
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Methods

        protected override bool MainExecute(BuildEngine engine)
        {
            BuildLogger logger = engine.Logger;
            bool processResult = false;

            _logger = logger;

            if (String.IsNullOrEmpty(_application) &&
                String.IsNullOrEmpty(_arguments))
            {
                return processResult;
            }

            if (_logger != null && String.IsNullOrEmpty(_message) == false)
            {
                _logger.WriteLine(_message, BuildLoggerLevel.Started);
            }

            processResult = Run(logger);

            if (_logger != null && String.IsNullOrEmpty(_message) == false)
            {
                _logger.WriteLine(_message, BuildLoggerLevel.Ended);
            }

            return processResult;
        }

        #endregion

        #region Run Method

        protected virtual bool Run(BuildLogger logger)
        {
            bool processResult = false;

            _logger = logger;

            try
            {
                Process process = new Process();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName         = _application;
                startInfo.Arguments        = _arguments;
                startInfo.UseShellExecute  = _shellExecute;
                startInfo.WorkingDirectory = this.WorkingDirectory;

                startInfo.RedirectStandardInput = false;
                if (!_shellExecute)
                {
                    startInfo.RedirectStandardOutput = _redirectOutput;
                    startInfo.RedirectStandardError = _redirectError;
                }
                if (_encoding != null)
                {
                    startInfo.StandardOutputEncoding = _encoding;
                    startInfo.StandardErrorEncoding = _encoding;
                }

                // Add the event handler to receive the console output...
                if (_redirectOutput)
                {
                    process.OutputDataReceived += new DataReceivedEventHandler(
                        OnDataReceived);
                }
                if (_redirectError)
                {
                    process.ErrorDataReceived += new DataReceivedEventHandler(
                        OnErrorReceived);
                }

                // Now, start the process - there will still not be output till...
                process.Start();

                // Start the asynchronous read of the output stream   
                if (_redirectOutput)
                {
                    process.BeginOutputReadLine();
                }
                if (_redirectError)
                {
                    process.BeginErrorReadLine();
                }

                // We must wait for the process to complete...
                process.WaitForExit();

                _processExitCode = process.ExitCode;

                process.Close();

                if (_ignoreExitCode)
                {
                    processResult = true;
                }
                else
                {
                    processResult = (_expectedExitCode == _processExitCode);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                processResult = false;
            }

            return processResult;
        }

        #endregion

        #region OnDataReceived Method

        protected virtual void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger != null)
            {
                //_logger.WriteLine(e.Data, OutputLoggerLevel.None);
            }

            //_messageCount++;
            //if (_logger != null)
            //{
            //    if (_messageCount > _copyright)
            //    {
            //        _logger.WriteLine(e.Data, OutputLoggerLevel.None);
            //    }
            //}
        }

        #endregion

        #region OnDataReceived Method

        protected virtual void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger == null)
            {
                return;
            }
            _messageCount++;

            if (_messageCount > _copyright)
            {
                string textData = e.Data;
                if (_ignoreWhitespace && String.IsNullOrEmpty(textData))
                {
                    return;
                }
                _logger.WriteLine(textData, BuildLoggerLevel.None);
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepProcess buildStep = new StepProcess(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
