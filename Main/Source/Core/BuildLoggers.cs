using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This logger records all the relevant build events, information, 
    /// warnings, and errors to its contained members.
    /// </summary>
    /// <remarks>
    /// This is the logger used by the build engines, and it is simply a 
    /// container for other loggers.
    /// </remarks>
    public sealed class BuildLoggers : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.BuildLoggers";

        public const string LoggerFileName = "BuildLogFile.log";

        #endregion   

        #region Private Fields

        private int _totalWarnings;
        private int _totalErrors;

        private List<BuildLogger> _listLoggers;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildLoggers"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildLoggers"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildLoggers()
        {
            _listLoggers = new List<BuildLogger>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildLoggers"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildLoggers"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildLoggers"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildLoggers(string logFile)
            : base(logFile)
        {
            _listLoggers = new List<BuildLogger>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return 
        /// <c>Sandcastle.Loggers.BuildLoggers</c>.
        /// </value>
        public override string Name
        {
            get
            {
                return LoggerName;
            }
        }

        public int Count
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers.Count;
                }

                return 0;
            }
        }

        public BuildLogger this[int index]
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers[index];
                }

                return null;
            }
        }

        public IList<BuildLogger> Loggers
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers.AsReadOnly();
                }

                return null;
            }
        }

        public int TotalWarnings
        {
            get
            {
                return _totalWarnings;
            }
        }


        public int TotalErrors
        {
            get
            {
                return _totalErrors;
            }
        }

        #endregion

        #region Protected Properties

        protected override bool IsFileLogging
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(string logWorkingDir, string logTitle)
        {
            base.Initialize(logWorkingDir, logTitle);

            _totalWarnings = 0;
            _totalErrors   = 0;

            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.Initialize(logWorkingDir, logTitle);
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.Uninitialize();
                }
            }
        }

        public void Add(BuildLogger logger)
        {
            if (_listLoggers != null)
            {
                _listLoggers.Add(logger);
            }
        }

        public void Remove(BuildLogger logger)
        {
            if (_listLoggers != null)
            {
                _listLoggers.Remove(logger);
            }
        }

        public void Clear()
        {
            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    _listLoggers[i].Dispose();
                }

                _listLoggers.Clear();
            }
        }

        public override void WriteLine()
        {
            if (_listLoggers == null || this.Enabled == false)
            {
                return;
            }

            int itemCount = _listLoggers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildLogger logger = _listLoggers[i];
                logger.WriteLine();
            }
        }

        public override void Write(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public override void Write(string outputText, BuildLoggerLevel level)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            if (_listLoggers == null || this.Enabled == false)
            {
                return;
            }

            if (level == BuildLoggerLevel.Warn)
            {
                _totalWarnings++;
            }
            else if (level == BuildLoggerLevel.Error)
            {
                _totalErrors++;
            }

            int itemCount = _listLoggers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildLogger logger = _listLoggers[i];
                logger.Write(outputText, level);
            }
        }

        public override void WriteLine(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex.ToString(), level);
        }

        public override void WriteLine(string outputText, BuildLoggerLevel level)
        {
            if (_listLoggers == null || this.Enabled == false)
            {
                return;
            }

            if (level == BuildLoggerLevel.Warn)
            {
                _totalWarnings++;
            }
            else if (level == BuildLoggerLevel.Error)
            {
                _totalErrors++;
            }

            int itemCount = _listLoggers.Count;

            if (String.IsNullOrEmpty(outputText))
            {
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.WriteLine();
                }
            }
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.WriteLine(outputText, level);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
        }

        protected override void WriteLine(string outputText)
        {
        }

        #endregion
    }
}
