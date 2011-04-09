using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Loggers;
using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BuildEngine : BuildObject, IDisposable
    {
        #region Private Fields

        private bool          _isInitialized;
        private BuildLogger   _logger;
        private BuildContext  _context;
        private BuildSettings _settings;

        private IncludeContent   _includeContent;
        private ConfiguratorContent _configContent;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildEngine"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEngine"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildEngine()
        {
            _configContent = new ConfiguratorContent();
        }

        /// <summary>
        /// This allows the <see cref="BuildEngine"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildEngine"/> instance is reclaimed by garbage collection.
        /// </summary>
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

        public ConfiguratorItem this[int itemIndex]
        {
            get
            {
                return _configContent[itemIndex];
            }
        }

        public ConfiguratorContent Handlers
        {
            get
            {
                return _configContent;
            }
        }

        public IncludeContent IncludeContent
        {
            get
            {                
                return _includeContent;
            }
        }

        /// <summary>
        /// Gets the list of the folders created or are use by this build engine.
        /// </summary>
        public virtual IList<string> Folders
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the current settings of the build process.
        /// </summary>
        /// <value>
        /// A <see cref="BuildSettings"/> specifying the current settings of the 
        /// build process. This is <see langword="null"/> if the
        /// configuration process is not initiated.
        /// </value>
        protected BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        protected BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        protected BuildLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        #endregion

        #region Public Methods

        #region CreateSteps Method

        public abstract BuildStep CreateInitialSteps(BuildGroup group);
        public abstract BuildStep CreateFinalSteps(BuildGroup group);

        #endregion

        #region Initialize Method

        public virtual void Initialize(BuildContext context)
        {
            if (_isInitialized)
            {
                return;
            }

            BuildExceptions.NotNull(context, "context");

            _context  = context;
            _settings = context.Settings;
            _logger   = context.Logger;

            if (_logger.IsInitialized == false)
            {
                _logger.Initialize(_context.BaseDirectory, _settings.HelpTitle);
            }

            //string workingDir = _settings.WorkingDirectory;
            //if (String.IsNullOrEmpty(workingDir))
            //{
            //    workingDir = Environment.CurrentDirectory;
            //}
            //else
            //{
            //    workingDir = Environment.ExpandEnvironmentVariables(workingDir);
            //    workingDir = Path.GetFullPath(workingDir);
            //}
            //if (String.IsNullOrEmpty(workingDir))
            //{
            //    return _isInitialized;
            //}
            //if (!Directory.Exists(workingDir))
            //{
            //    Directory.CreateDirectory(workingDir);
            //}
            //_settings.WorkingDirectory = workingDir;

            BuildEngineSettings engineSettings = 
                _settings.EngineSettings[this.EngineType];
            Debug.Assert(engineSettings != null);
            if (engineSettings == null)
            {
                return;
            }
            _includeContent = engineSettings.IncludeContent;

            _isInitialized = true;
        }

        #endregion

        #region Build Method

        public abstract bool Build();

        #endregion

        #region Uninitialize Method
                                 
        public virtual void Uninitialize()
        {
            _context  = null;
            _settings = null;
            _logger   = null;

            _isInitialized = false;
        }

        #endregion

        #region ConfigurationItem Methods

        public void AddHandler(string keyword, Action<string, XPathNavigator> handler)
        {
            ConfiguratorItem item = new ConfiguratorItem(keyword, handler);
            _configContent.Add(item);
        }

        public void AddHandler(ConfiguratorItem item)
        {
            _configContent.Add(item);
        }

        public void AddHandlers(IList<ConfiguratorItem> items)
        {
            _configContent.Add(items);
        }

        public void RemoveHandler(int index)
        {
            _configContent.Remove(index);
        }

        public void RemoveHandler(ConfiguratorItem item)
        {
            _configContent.Remove(item);
        }

        public bool ContainsHandler(ConfiguratorItem item)
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
                Environment.CurrentDirectory = _context.WorkingDirectory;

                buildResult = true;

                // 1. Initialize all the build steps, and set them ready for build...
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = listSteps[i];
                    if (buildStep != null)
                    {
                        buildStep.Initialize(_context);
                        if (!buildStep.IsInitialized)
                        {
                            _logger.WriteLine(
                                "An error occurred when initializing the step = " + i.ToString(),
                                BuildLoggerLevel.Error);

                            buildResult = false;
                            break;
                        }
                    }
                }

                // If the initialization fails, we need not continue...
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
                        //_logger.WriteLine(
                        //    "An error occurred in the step = " + i.ToString(),
                        //    BuildLoggerLevel.Error);

                        _context.StepError(buildStep);

                        _logger.WriteLine();

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
                        buildStep.Uninitialize();
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
                if (!String.IsNullOrEmpty(currentDir))
                {
                    Environment.CurrentDirectory = currentDir;
                }
            }

            return buildResult;
        }

        #endregion

        #region GetOutputFolders Method

        protected IDictionary<string, bool> GetOutputFolders(
            IList<string> listFolders)
        {
            Dictionary<string, bool> dicFolders = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);

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

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
