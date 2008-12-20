using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Properties;

namespace Sandcastle
{
    public abstract class HelpLogger : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool   _isAppend;
        private bool   _isInitialize;
        private bool   _isLogging;
        private bool   _keepLogFile;
        private string _logFile;

        private string _logStarted;
        private string _logInfo;
        private string _logWarn;
        private string _logError;
        private string _logEnded;

        private Encoding _encoding;

        private StreamWriter _baseWriter;

        private HelpLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        protected HelpLogger()
            : this(null)
        {
            _isLogging = true;
        }

        protected HelpLogger(string logFile)
        {
            _isLogging   = true;
            _keepLogFile = true;
            _logFile     = logFile;
            _encoding    = new UTF8Encoding();

            _verbosity   = HelpLoggerVerbosity.Normal;

            _logStarted  = Resources.LogStarted + ": ";
            _logInfo     = Resources.LogInfo + ": ";
            _logWarn     = Resources.LogWarn + ": ";
            _logError    = Resources.LogError + ": ";
            _logEnded    = Resources.LogEnded + ": ";        
        }

        protected HelpLogger(string logFile, bool append, Encoding encoding)
            : this(logFile)
        {
            _isLogging = true;

            _isAppend    = append;
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }
            _encoding = encoding;
        }

        ~HelpLogger()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsInitialize
        {
            get
            {
                return _isInitialize;
            }

            protected set
            {
                _isInitialize = value;
            }
        }

        public virtual bool Logging
        {
            get
            {
                return _isLogging;
            }

            set
            {
                _isLogging = value;
            }
        }

        public virtual string LogFile
        {
            get
            {
                return _logFile;
            }

            set
            {
                _logFile = value;
            }
        }

        public virtual bool KeepLog
        {
            get
            {
                return _keepLogFile;
            }

            set
            {
                _keepLogFile = value;
            }
        }

        public virtual bool Append
        {
            get
            {
                return _isAppend;
            }

            set
            {
                _isAppend = value;
            }
        }

        public virtual Encoding Encoding
        {
            get
            {
                return _encoding;
            }

            set
            {
                if (value != null)
                {
                    _encoding = value;
                }
            }
        }

        public virtual HelpLoggerVerbosity Verbosity 
        { 
            get
            {
                return _verbosity;
            }

            set
            {
                _verbosity = value;
            }
        }

        #endregion

        #region Protected Properties

        protected virtual TextWriter BaseWriter
        {
            get
            {
                return _baseWriter;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize()
        {
            if (_isInitialize)
            {
                return;
            }

            if (!String.IsNullOrEmpty(_logFile))
            {
                // if the log directory does not exits, create it...
                string logDir = Path.GetDirectoryName(_logFile);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                _baseWriter = new StreamWriter(_logFile, _isAppend, _encoding);
            }

            _isInitialize = true;
        }

        public virtual void Uninitialize()
        {
            if (_baseWriter != null)
            {
                _baseWriter.Close();

                _baseWriter.Dispose();
                _baseWriter = null;
            }

            _isInitialize = false;
        }

        public virtual void Write(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), HelpLoggerLevel.Error);
        }

        public virtual void WriteLine(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            this.WriteLine(ex.ToString(), HelpLoggerLevel.Error);
        }

        public virtual void Write(Exception ex, HelpLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public abstract void WriteLine();

        public virtual void WriteLine(Exception ex, HelpLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.WriteLine(ex.ToString(), level);
        }

        public virtual void Write(string outputText, HelpLoggerLevel level)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            this.Write(this.FormatText(outputText, level));
        }

        public virtual void WriteLine(string outputText, HelpLoggerLevel level)
        {   
            if (String.IsNullOrEmpty(outputText))
            {
                this.WriteLine();
            }
            else
            {
                this.WriteLine(this.FormatText(outputText, level));
            }
        }

        public virtual void Close()
        {
            this.Dispose(true);
        }

        #endregion

        #region Protected Methods

        protected abstract void Write(string outputText);
        protected abstract void WriteLine(string outputText);

        protected virtual string FormatText(string logText, HelpLoggerLevel level)
        {
            string finalText = null;
            if (level == HelpLoggerLevel.None)
            {
                finalText = logText;
            }
            else if (level == HelpLoggerLevel.Started)
            {
                finalText = _logStarted + logText;
            }
            else if (level == HelpLoggerLevel.Info)
            {
                finalText = _logInfo + logText;
            }
            else if (level == HelpLoggerLevel.Warn)
            {
                finalText = _logWarn + logText;
            }
            else if (level == HelpLoggerLevel.Error)
            {
                finalText = _logError + logText;
            }
            else if (level == HelpLoggerLevel.Ended)
            {
                finalText = _logEnded + logText;
            }

            return finalText;
        }

        protected virtual void Log(string logText)
        {
            if (_isLogging == false || logText == null || _baseWriter == null)
            {
                return;
            }

            _baseWriter.Write(logText);
        }

        protected virtual void LogLine()
        {
            if (_isLogging == false || _baseWriter == null)
            {
                return;
            }

            _baseWriter.WriteLine();
        }

        protected virtual void LogLine(string logText)
        {
            if (_isLogging == false || logText == null || _baseWriter == null)
            {
                return;
            }

            _baseWriter.WriteLine(logText);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_baseWriter != null)
            {
                _baseWriter.Dispose();
                _baseWriter = null;
            }
        }

        #endregion
    }
}
