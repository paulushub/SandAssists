using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all build loggers.
    /// <para>
    /// The output logger helps to monitor the progress of a Sandcastle build process. 
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The logger records all the relevant build events, information, warnings, 
    /// and errors to various streams or data storages.
    /// </para>
    /// <para>
    /// Each build logger is uniquely named to make it easy to identify and script.
    /// </para>
    /// <para>
    /// The default or system build loggers will have names in the format:
    /// </para>
    /// <para>
    /// <bold>Sandcastle.{X}.Logger</bold>
    /// where {X} represents the format or output destination of the logger.
    /// </para>
    /// <para>
    /// The available loggers and their names are listed below:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <bold>Sandcastle.NoneLogger</bold>, the output logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.ConsoleLogger</bold>, the console logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.FileLogger</bold>, the file logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.HtmlLogger</bold>, the HTML format logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.XmlLogger</bold>, the XML format logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.XamlLogger</bold>, the XAML flow document logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.BuildLoggers</bold>, the logger container, with no output
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BuildLoggers"/>
    /// <seealso cref="BuildLoggerLevel"/>
    /// <seealso cref="BuildLoggerVerbosity"/>
    public abstract class BuildLogger : BuildObject, IDisposable
    {
        #region Private Fields

        private bool   _isAppend;
        private bool   _includeSummary;
        private bool   _isInitialize;
        private bool   _isLogging;
        private bool   _includeMetadata;
        private bool   _keepLogFile;

        private bool   _logStarted;
        private bool   _logInfo;
        private bool   _logWarn;
        private bool   _logError;
        private bool   _logEnded;
        private bool   _logCopyright;

        private string _logTitle;
        private string _logFileName;
        private string _logFullPath;

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
            _includeSummary  = true;
            _includeMetadata = true;
            _isLogging       = true;
        }

        protected BuildLogger(string logFile)
        {
            if (logFile != null)
            {
                logFile = logFile.Trim();
            }
            if (!String.IsNullOrEmpty(logFile) && Path.IsPathRooted(logFile))
            {
                logFile = Path.GetFileName(logFile);
            }

            _isLogging     = true;
            _keepLogFile   = true;
            _logTitle      = String.Empty;
            _logFileName   = logFile;
            _encoding      = new UTF8Encoding();

            _verbosity     = BuildLoggerVerbosity.Normal;

            _logStarted    = true;
            _logInfo       = true;
            _logWarn       = true;
            _logError      = true;
            _logEnded      = true;
            _logCopyright  = false;

            //_prefixStarted = ResourcesEx.LogStarted + ": ";
            //_prefixInfo    = ResourcesEx.LogInfo + ": ";
            //_prefixWarn    = ResourcesEx.LogWarn + ": ";
            //_prefixError   = ResourcesEx.LogError + ": ";
            //_prefixEnded   = ResourcesEx.LogEnded + ": ";
            //_prefixRights  = ResourcesEx.LogCopyright + ": ";        

            _prefixStarted = "Started: ";
            _prefixInfo    = "Info: ";
            _prefixWarn    = "Warn: ";
            _prefixError   = "Error: ";
            _prefixEnded   = "Ended: ";
            _prefixRights  = "Copyright: ";        
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
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation.
        /// </value>
        /// <remarks>
        /// The name makes it possible to uniquely identify the logger, which may be
        /// required by other build operations such as the build completion 
        /// notification process. Since the build helpers library makes it possible
        /// to use different types of logger, user can specify which build logger
        /// output to include in, say the mail, notifications. 
        /// </remarks>
        public abstract string Name
        {
            get;
        }

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
        public bool IncludeSummary
        {
            get
            {
                return _includeSummary;
            }

            set
            {
                _includeSummary = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool IncludeMetadata
        {
            get
            {
                return _includeMetadata;
            }

            set
            {
                _includeMetadata = value;
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
        public string LogTitle
        {
            get
            {
                return _logTitle;
            }
        }

        /// <summary>
        /// Gets the fully qualified path of the log file.
        /// </summary>
        /// <value>
        /// A fully qualified name of the log file, if initialized; otherwise,
        /// the file name of the log file.
        /// </value>
        public string LogFullPath
        {
            get
            {
                if (String.IsNullOrEmpty(_logFullPath))
                {
                    return _logFileName;
                }

                return _logFullPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string LogFileName
        {
            get
            {
                return _logFileName;
            }

            set
            {
                // If this logger is initialized, setting this value has no effect...
                if (_isInitialize)
                {
                    return;
                }
                // Trim any non-empty/null string to avoid any preventable problem...
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value) && Path.IsPathRooted(value))
                {
                    value = Path.GetFileName(value);
                }
                _logFileName = value;
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
                if (!_isInitialize && value != null)
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

        protected string PrefixStarted
        {
            get
            {
                return _prefixStarted;
            }
        }

        protected string PrefixInfo
        {
            get
            {
                return _prefixInfo;
            }
        }

        protected string PrefixWarn
        {
            get
            {
                return _prefixWarn;
            }
        }

        protected string PrefixError
        {
            get
            {
                return _prefixError;
            }
        }

        protected string PrefixEnded
        {
            get
            {
                return _prefixEnded;
            }
        }

        protected string PrefixRights
        {
            get
            {
                return _prefixRights;
            }
        }

        protected TextWriter BaseWriter
        {
            get
            {
                return _baseWriter;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(string logWorkingDir, string logTitle)
        {
            if (_isInitialize)
            {
                return;
            }
            _logTitle    = logTitle;
            _logFullPath = null;

            if (!String.IsNullOrEmpty(_logFileName))
            {
                if (!String.IsNullOrEmpty(logWorkingDir))
                {   
                    if (Path.IsPathRooted(_logFileName))
                    {
                        _logFileName = Path.GetFileName(_logFileName);
                    }
                    // if the log directory does not exits, create it...
                    logWorkingDir = Path.GetFullPath(logWorkingDir);
                    if (!Directory.Exists(logWorkingDir))
                    {
                        Directory.CreateDirectory(logWorkingDir);
                    }

                    _logFullPath = Path.Combine(logWorkingDir, _logFileName);
                }
                else
                {
                    _logFullPath = Path.GetFullPath(_logFileName);
                }

                _baseWriter = new StreamWriter(_logFullPath, _isAppend, _encoding);
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
            if (!this.LogLevel(level))
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
            if (!this.LogLevel(level))
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
            if (!this.LogLevel(level))
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
            if (!this.LogLevel(level))
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
