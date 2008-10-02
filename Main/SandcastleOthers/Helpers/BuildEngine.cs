using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Loggers;
using Sandcastle.Configurations;

namespace Sandcastle
{
    public abstract class BuildEngine : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool          _isInitialized;
        private bool          _ownsLogger;
        private BuildLoggers  _logger;
        private BuildContext  _context;
        private BuildSettings _settings;

        private ConfigurationContent _configContent;

        private BuildConfiguration _configuration;

        #endregion

        #region Constructors and Destructor

        protected BuildEngine()
            : this(null, null, null, null)
        {
        }

        protected BuildEngine(BuildSettings settings)
            : this(settings, null, null, null)
        {
        }

        protected BuildEngine(BuildLoggers logger)
            : this(null, logger, null, null)
        {
        }

        protected BuildEngine(BuildSettings settings, BuildLoggers logger, 
            BuildContext context, BuildConfiguration configuration)
        {
            _settings      = settings;
            _logger        = logger;
            _context       = context;
            _configuration = configuration;

            if (_settings == null)
            {
                _settings = new BuildSettings();
            }

            if (_context == null)
            {
                _context = new BuildContext();
            }

            if (_logger == null)
            {
                _logger = new BuildLoggers();
                _ownsLogger = true;
                //CreateLogger();
            }

            if (_configuration == null)
            {
                _configuration = new BuildConfiguration();
            }

            _configContent = new ConfigurationContent();
        }

        ~BuildEngine()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public virtual bool IsInitialized
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

        public abstract BuildEngineType EngineType
        {
            get;
        }

        public abstract IList<BuildStep> Steps
        {
            get;
        }

        public abstract IList<BuildGroup> Groups
        {
            get;
        }

        /// <summary>
        /// Gets the current settings of the build process.
        /// </summary>
        /// <value>
        /// A <see cref="BuildSettings"/> specifying the current settings of the 
        /// build process. This is <see langword="null"/> if the
        /// configuration process is not initiated.
        /// </value>
        public BuildSettings Settings
        {
            get 
            { 
                return _settings; 
            }
        }

        public BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public BuildLoggers Logger
        {
            get
            {
                return _logger;
            }
        }

        public ConfigurationItem this[int itemIndex]
        {
            get
            {
                return _configContent[itemIndex];
            }
        }

        public ConfigurationContent Handlers
        {
            get
            {
                return _configContent;
            }
        }

        public BuildConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Method

        public virtual bool Initialize(BuildSettings settings)
        {
            _isInitialized = false;

            if (_logger == null)
            {
                _logger = new BuildLoggers();
                _ownsLogger = true;
                //CreateLogger();
            }

            if (_logger.IsInitialize == false)
            {
                _logger.Initialize(settings);
            }

            if (settings == null)
            {
                settings = _settings;
            }

            BuildExceptions.NotNull(settings, "settings");

            _settings         = settings;
            string workingDir = _settings.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = Environment.CurrentDirectory;
            }
            else
            {
                workingDir = Environment.ExpandEnvironmentVariables(workingDir);
                workingDir = Path.GetFullPath(workingDir);
            }
            if (String.IsNullOrEmpty(workingDir))
            {
                return _isInitialized;
            }
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            _settings.WorkingDirectory = workingDir;

            _isInitialized = true;

            return _isInitialized;
        }

        #endregion

        #region Build Method

        public abstract bool Build();

        #endregion

        #region Uninitialize Method
                                 
        public virtual void Uninitialize()
        {
            if (_ownsLogger && (_logger != null))
            {
                _logger.Uninitialize();
            }

            _isInitialized = false;
        }

        #endregion

        #region ConfigurationItem Methods

        public void AddHandler(string keyword, ConfigurationItemHandler handler)
        {
            ConfigurationItem item = new ConfigurationItem(keyword, handler);
            _configContent.Add(item);
        }

        public void AddHandler(ConfigurationItem item)
        {
            _configContent.Add(item);
        }

        public void AddHandlers(IList<ConfigurationItem> items)
        {
            _configContent.Add(items);
        }

        public void RemoveHandler(int index)
        {
            _configContent.Remove(index);
        }

        public void RemoveHandler(ConfigurationItem item)
        {
            _configContent.Remove(item);
        }

        public bool ContainsHandler(ConfigurationItem item)
        {
            return _configContent.Contains(item);
        }

        public void ClearHandlers()
        {
            if (_configContent.Count == 0)
            {
                return;
            }

            _configContent.Clear();
        }

        #endregion

        #endregion

        #region Protected Methods

        #region RunSteps Method

        protected virtual bool RunSteps(IList<BuildStep> listSteps)
        {
            if (_settings == null || _context == null ||
                _isInitialized == false)
            {
                return false;
            }

            if (listSteps == null || listSteps.Count == 0)
            {
                return false;
            }

            int stepCount = listSteps.Count;

            string currentDir = Environment.CurrentDirectory;
            bool buildResult  = false;

            try
            {
                Environment.CurrentDirectory = _settings.WorkingDirectory;

                if (_context.Engine != this)
                {
                    _context.Detach();
                    _context.Attach(this);
                }

                buildResult = true;

                // 1. Initialize all the build steps, and set them ready for build...
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = listSteps[i];
                    if (buildStep != null)
                    {
                        if (buildStep.Initialize(_context) == false)
                        {
                            _logger.WriteLine(
                                "An error occurred when initializing the step = " + i.ToString(),
                                BuildLoggerLevel.Error);

                            buildResult = false;
                            break;
                        }
                    }
                }

                // If the initialization fails, we need not continune...
                if (buildResult == false)
                {
                    return buildResult;
                }

                // 2. Now, run each build step, and monitor the results...
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = listSteps[i];

                    if (buildStep == null || buildStep.Enabled == false)
                    {
                        continue;
                    }
                    bool executeIt = _context.StepStarts(buildStep);
                    if (executeIt == false)
                    {
                        continue;
                    }

                    if (buildStep.Execute() == false)
                    {
                        _logger.WriteLine(
                            "An error occurred in the step = " + i.ToString(),
                            BuildLoggerLevel.Error);

                        _context.StepError(buildStep);

                        buildResult = false;
                        break;
                    }

                    _context.StepEnds(buildStep);

                    _logger.WriteLine();
                }

                // 3. Finally, un-initialize all the build steps, allowing each to clean up...
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = listSteps[i];
                    if (buildStep != null)
                    {
                        if (buildStep.Uninitialize(_context) == false)
                        {
                            _logger.WriteLine(
                                "An error occurred when uninitializing the step = " + i.ToString(),
                                BuildLoggerLevel.Error);

                            buildResult = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);

                buildResult = false;
            }
            finally
            {
                _context.Detach();

                if (!String.IsNullOrEmpty(currentDir))
                {
                    Environment.CurrentDirectory = currentDir;
                }
            }

            return buildResult;
        }

        #endregion

        #region ExpandPath Method

        protected string ExpandPath(string inputFile)
        {
            if (_settings == null)
            {
                return String.Empty;
            }

            string outputFile = Environment.ExpandEnvironmentVariables(inputFile);
            if (!Path.IsPathRooted(outputFile))
            {
                string workingDir = _settings.WorkingDirectory;
                if (!String.IsNullOrEmpty(workingDir))
                {
                    outputFile = Path.Combine(workingDir, outputFile);
                }
            }
            outputFile = Path.GetFullPath(outputFile);

            return outputFile;
        }

        #endregion

        #region GetOutputFolders Method

        protected IDictionary<string, bool> GetOutputFolders(
            IList<string> listFolders)
        {
            Dictionary<string, bool> dicFolders = new Dictionary<string, bool>(
                StringComparer.CurrentCultureIgnoreCase);

            if (_settings == null)
            {
                return dicFolders;
            }

            IList<string> folders = _settings.OutputFolders;

            if (folders == null || folders.Count == 0)
            {
                return dicFolders;
            }

            int folderCount = folders.Count;
            for (int i = 0; i < folderCount; i++)
            {
                string folder = folders[i];
                if (String.IsNullOrEmpty(folder) == false &&
                    dicFolders.ContainsKey(folder) == false)
                {
                    dicFolders.Add(folder, true);
                    listFolders.Add(folder);
                }
            }

            return dicFolders;
        }

        #endregion

        #endregion

        #region Private Methods

        private void CreateLogger()
        {
            if (_logger != null || _settings == null) 
            {
                return;
            }

            string workingDir = _settings.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                return;
            }
            string logFile    = Path.Combine(workingDir, _settings.LogFile);

            _ownsLogger = true;

            if (_settings.IsCombinedBuild == false)
            {
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                }
            }

            BuildLogger logger = null;
            if (String.IsNullOrEmpty(workingDir) ||
                Environment.UserInteractive == false)
            {
                if (String.IsNullOrEmpty(logFile) == false || _settings.UseLogFile)
                {
                    logger = new ConsoleLogger(logFile);
                    logger.KeepLog = _settings.KeepLogFile;
                }
                else
                {
                    logger = new ConsoleLogger();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(logFile) == false || _settings.UseLogFile)
                {
                    logger = new FileLogger(logFile);
                    logger.KeepLog = _settings.KeepLogFile;
                }
            }

            if (logger != null)
            {
                _logger.Add(logger);
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
            if (_ownsLogger && (_logger != null))
            {
                _logger.Dispose();
            }
            _logger = null;
        }

        #endregion
    }
}
