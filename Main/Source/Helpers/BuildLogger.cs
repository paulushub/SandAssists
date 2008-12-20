using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Properties;

namespace Sandcastle
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all build loggers.
    /// <para>
    /// The output logger helps to monitor the progress of a Sandcastle build process. 
    /// </para>
    /// </summary>
    /// <remarks>
    /// The logger records all the relevant build events, information, warnings, 
    /// and errors to various streams or data storages.
    /// <para>
    /// The build step derives from <see cref="MarshalByRefObject"/> so that any of its 
    /// derivative can be instantiated in its own app domain.
    /// </para>
    /// </remarks>
    /// <seealso cref="BuildLoggers"/>
    /// <seealso cref="BuildLoggerLevel"/>
    /// <seealso cref="BuildLoggerVerbosity"/>
    public abstract class BuildLogger : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool   _isAppend;
        private bool   _isInitialize;
        private bool   _isLogging;
        private bool   _keepLogFile;

        private bool   _logStarted;
        private bool   _logInfo;
        private bool   _logWarn;
        private bool   _logError;
        private bool   _logEnded;
        private bool   _logCopyright;

        private string _logFile;

        private string _prefixStarted;
        private string _prefixInfo;
        private string _prefixWarn;
        private string _prefixError;
        private string _prefixEnded;
        private string _prefixRights;

        private Encoding _encoding;

        private StreamWriter _baseWriter;

        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        protected BuildLogger()
            : this(null)
        {
            _isLogging     = true;
        }

        protected BuildLogger(string logFile)
        {
            _isLogging     = true;
            _keepLogFile   = true;
            _logFile       = logFile;
            _encoding      = new UTF8Encoding();

            _verbosity     = BuildLoggerVerbosity.Normal;

            _logStarted    = true;
            _logInfo       = true;
            _logWarn       = true;
            _logError      = true;
            _logEnded      = true;
            _logCopyright  = false;

            _prefixStarted = Resources.LogStarted + ": ";
            _prefixInfo    = Resources.LogInfo + ": ";
            _prefixWarn    = Resources.LogWarn + ": ";
            _prefixError   = Resources.LogError + ": ";
            _prefixEnded   = Resources.LogEnded + ": ";
            _prefixRights  = Resources.LogCopyright + ": ";        
        }

        protected BuildLogger(string logFile, bool append, Encoding encoding)
            : this(logFile)
        {
            _isLogging   = true;

            _isAppend    = append;
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }
            _encoding    = encoding;
        }

        ~BuildLogger()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        /// <seealso cref="BuildLogger.Initialize(BuildSettings)"/>
        /// <seealso cref="BuildLogger.Uninitialize()"/>
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Logging
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string LogFile
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool KeepLog
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Append
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Encoding Encoding
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public BuildLoggerVerbosity Verbosity 
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogStarted
        {
            get 
            { 
                return _logStarted; 
            }
            set 
            { 
                _logStarted = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogInfo
        {
            get 
            { 
                return _logInfo; 
            }
            set 
            { 
                _logInfo = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogWarn
        {
            get 
            { 
                return _logWarn; 
            }
            set 
            { 
                _logWarn = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogError
        {
            get 
            { 
                return _logError; 
            }
            set 
            { 
                _logError = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogEnded
        {
            get 
            { 
                return _logEnded; 
            }
            set 
            { 
                _logEnded = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool LogCopyright
        {
            get 
            { 
                return _logCopyright; 
            }
            set 
            { 
                _logCopyright = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public virtual bool IsFormatting
        {
            get
            {
                return false;
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

        public virtual void Initialize(BuildSettings settings)
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
            this.Write(ex.ToString(), BuildLoggerLevel.Error);
        }

        public virtual void WriteLine(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            this.WriteLine(ex.ToString(), BuildLoggerLevel.Error);
        }

        public virtual void Write(Exception ex, BuildLoggerLevel level)
        {
            if (this.LogLevel(level) == false)
            {
                return;
            }

            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public abstract void WriteLine();

        public virtual void WriteLine(Exception ex, BuildLoggerLevel level)
        {
            if (this.LogLevel(level) == false)
            {
                return;
            }

            if (ex == null)
            {
                return;
            }
            this.WriteLine(ex.ToString(), level);
        }

        public virtual void Write(string outputText, BuildLoggerLevel level)
        {
            if (this.LogLevel(level) == false)
            {
                return;
            }

            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            this.Write(this.FormatText(outputText, level));
        }

        public virtual void WriteLine(string outputText, BuildLoggerLevel level)
        {   
            if (this.LogLevel(level) == false)
            {
                return;
            }

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

        protected virtual string FormatText(string logText, BuildLoggerLevel level)
        {
            string finalText = null;
            if (level == BuildLoggerLevel.None)
            {
                finalText = logText;
            }
            else if (level == BuildLoggerLevel.Started)
            {
                finalText = _prefixStarted + logText;
            }
            else if (level == BuildLoggerLevel.Info)
            {
                finalText = _prefixInfo + logText;
            }
            else if (level == BuildLoggerLevel.Warn)
            {
                finalText = _prefixWarn + logText;
            }
            else if (level == BuildLoggerLevel.Error)
            {
                finalText = _prefixError + logText;
            }
            else if (level == BuildLoggerLevel.Ended)
            {
                finalText = _prefixEnded + logText;
            }
            else if (level == BuildLoggerLevel.Copyright)
            {
                finalText = _prefixRights + logText;
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

        protected virtual bool LogLevel(BuildLoggerLevel level)
        {
            if (level == BuildLoggerLevel.None)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Started)
            {
                return _logStarted;
            }
            else if (level == BuildLoggerLevel.Info)
            {
                return _logInfo;
            }
            else if (level == BuildLoggerLevel.Warn)
            {
                return _logWarn;
            }
            else if (level == BuildLoggerLevel.Error)
            {
                return _logError;
            }
            else if (level == BuildLoggerLevel.Ended)
            {
                return _logEnded;
            }
            else if (level == BuildLoggerLevel.Copyright)
            {
                return _logCopyright;
            }

            return false;
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
