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
    /// <bold>Sandcastle.Loggers.{X}Logger</bold>
    /// where {X} represents the format or output destination of the logger.
    /// </para>
    /// <para>
    /// The available loggers and their names are listed below:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.NoneLogger</bold>, the output logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.ConsoleLogger</bold>, the console logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.FileLogger</bold>, the file logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.HtmlLogger</bold>, the HTML format logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.XmlLogger</bold>, the <c>XML</c> format logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.XamlLogger</bold>, the XAML flow document logger
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <bold>Sandcastle.Loggers.BuildLoggers</bold>, the logger container, with no output
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BuildLoggers"/>
    /// <seealso cref="BuildLoggerLevel"/>
    /// <seealso cref="BuildLoggerVerbosity"/>
    public abstract class BuildLogger : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool   _isInitialized;
        private bool   _isEnabled;
        private bool   _keepLogFile;

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

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildLogger"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildLogger"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildLogger()
            : this(null)
        {
            _isEnabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildLogger"/> class
        /// with the specified log file name.
        /// </summary>
        /// <param name="logFileName">
        /// A string specifying the log file name.
        /// </param>
        protected BuildLogger(string logFileName)
        {
            if (logFileName != null)
            {
                logFileName = logFileName.Trim();
            }
            if (!String.IsNullOrEmpty(logFileName) && 
                Path.IsPathRooted(logFileName))
            {
                logFileName = Path.GetFileName(logFileName);
            }

            _isEnabled     = true;
            _keepLogFile   = true;
            _logTitle      = String.Empty;
            _logFileName   = logFileName;
            _encoding      = new UTF8Encoding();

            _verbosity     = BuildLoggerVerbosity.Normal;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildLogger"/> class
        /// with the specified log file name, a value indicating whether to 
        /// append this logging to existing log file and the log file encoding.
        /// </summary>
        /// <param name="logFileName">
        /// A string specifying the log file name.
        /// </param>
        /// <param name="encoding">
        /// The encoding of the log file. If <see langword="null"/>, the default
        /// encoding of UTF-8 is used.
        /// </param>
        protected BuildLogger(string logFileName, Encoding encoding)
            : this(logFileName)
        {
            _isEnabled   = true; 
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
            }
            _encoding    = encoding;
        }

        /// <summary>
        /// This allows a converter to attempt to free resources and perform 
        /// other cleanup operations before the converter is reclaimed by 
        /// garbage collection.
        /// </summary>
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
        /// Gets a value indicating whether this logger is initialized.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this logger is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildLogger.Initialize(string,string)"/>
        /// <seealso cref="BuildLogger.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this logger is enabled.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this logger is enabled; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public bool Enabled
        {
            get
            {
                return _isEnabled;
            } 
            set
            {
                _isEnabled = value;
            }
        }

        /// <summary>
        /// Gets the logging title or description.
        /// </summary>
        /// <value>
        /// A string specifying the logging title or description.
        /// </value>
        /// <remarks>
        /// The title is used by loggers, like the 
        /// <see cref="Sandcastle.Loggers.HtmlLogger"/>, which requires a title.
        /// </remarks>
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
        /// Gets or sets the logger file name.
        /// </summary>
        /// <value>
        /// A string specifying the logger file name.
        /// </value>
        /// <remarks>
        /// This cannot be changed after the logger is initialized.
        /// </remarks>
        public string LogFileName
        {
            get
            {
                return _logFileName;
            } 
            set
            {
                // If this logger is initialized, setting this value has no effect...
                if (_isInitialized)
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
        /// Gets or sets a value indicating whether to keep the log file on
        /// completion.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this logger keeps the log file
        /// after it is disposed; otherwise, it is <see langword="true"/>.
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
        /// Gets or sets the encoding of the log file.
        /// </summary>
        /// <value>
        /// An instance of the type <see cref="System.Text.Encoding"/> specifying
        /// the encoding of the log file.
        /// </value>
        /// <remarks>
        /// This cannot be <see langword="null"/>, and it is not changed after
        /// the logger is initialized.
        /// </remarks>
        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                if (!_isInitialized && value != null)
                {
                    _encoding = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the verbosity of the logger.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLoggerVerbosity"/>
        /// specifying the verbosity.
        /// </value>
        /// <remarks>
        /// This is not changed after the logger is initialized.
        /// </remarks>
        public BuildLoggerVerbosity Verbosity 
        { 
            get
            {
                return _verbosity;
            }
            set
            {
                if (!_isInitialized)
                {
                    _verbosity = value;
                }
            }
        }

        #endregion

        #region Protected Properties

        protected abstract bool IsFileLogging
        {
            get;
        }

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

        /// <summary>
        /// Initializes the logger with the specified working directory and log
        /// title.
        /// </summary>
        /// <param name="logWorkingDir">
        /// A string specifying the working directory of the logger, where the
        /// log file is created.
        /// </param>
        /// <param name="logTitle">
        /// A logging title or description, optional.
        /// </param>
        public virtual void Initialize(string logWorkingDir, string logTitle)
        {
            if (_isInitialized)
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

                if (this.IsFileLogging)
                {
                    _baseWriter = new StreamWriter(_logFullPath, false, _encoding);
                }
            }

            _isInitialized = true;
        }

        /// <summary>
        /// This provides the un-initialization operation, which cleans up any
        /// object create during the initialization operation.
        /// </summary>
        public virtual void Uninitialize()
        {
            if (_baseWriter != null)
            {
                _baseWriter.Close();
                _baseWriter = null;
            }

            // If the log file is not required, we delete it...
            if (!_keepLogFile && !String.IsNullOrEmpty(_logFullPath) &&
                File.Exists(_logFullPath))
            {   
                try
                {
                    File.SetAttributes(_logFullPath, FileAttributes.Normal);
                    File.Delete(_logFullPath);
                }            
                catch
                {                    	
                }
            }

            _isInitialized = false;
        }

        /// <summary>
        /// Writes the text representation of the specified exception with the
        /// <see cref="BuildLoggerLevel.Error"/> level.
        /// </summary>
        /// <param name="ex"></param>
        public virtual void Write(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            this.Write(ex, BuildLoggerLevel.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public virtual void WriteLine(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex, BuildLoggerLevel.Error);
        }

        /// <summary>
        /// Writes the text representation of the specified exception and logging level.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="level"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public abstract void WriteLine();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="level"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputText"></param>
        /// <param name="level"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputText"></param>
        /// <param name="level"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public virtual void Close()
        {
            this.Dispose(true);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputText"></param>
        protected abstract void Write(string outputText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputText"></param>
        protected abstract void WriteLine(string outputText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="level"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logText"></param>
        protected virtual void Log(string logText)
        {
            if (_isEnabled == false || logText == null || _baseWriter == null)
            {
                return;
            }

            _baseWriter.Write(logText);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void LogLine()
        {
            if (_isEnabled == false || _baseWriter == null)
            {
                return;
            }

            _baseWriter.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logText"></param>
        protected virtual void LogLine(string logText)
        {
            if (_isEnabled == false || logText == null || _baseWriter == null)
            {
                return;
            }

            _baseWriter.WriteLine(logText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected virtual bool LogLevel(BuildLoggerLevel level)
        {
            if (level == BuildLoggerLevel.None)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Started)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Info)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Warn)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Error)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Ended)
            {
                return true;
            }
            else if (level == BuildLoggerLevel.Copyright)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This releases all resources used by the <see cref="BuildLogger"/> object.
        /// </overloads>
        /// <summary>
        /// This releases all resources used by the <see cref="BuildLogger"/> object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true); 
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This releases the unmanaged resources used by the <see cref="BuildLogger"/> 
        /// and optionally releases the managed resources. 
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
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
