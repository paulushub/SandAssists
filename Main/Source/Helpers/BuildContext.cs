using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle
{
    /// <summary>
    /// This represents the build context or environment during the execution of the
    /// build process, and can be used to store object or data that is used by various
    /// objects in the build process.
    /// </summary>
    public class BuildContext : BuildObject, IDisposable
    {
        #region Public Fields

        public const string WorkingFolder = "_HelpBuild";
        public static readonly Version SandcastleVersion = new Version("2.6.10621.1");

        #endregion

        #region Private Fields

        private bool            _isInitialized;
        private bool            _isBuildSuccess;

        private string          _outputDir;
        private string          _workingDir;
        private string          _baseWorkingDir;
        private string          _sandcastleDir;
        private string          _sandcastleToolsDir;

        private BuildType       _buildType;
        private BuildState      _buildState;
        private BuildLogger     _logger;
        private BuildSystem     _buildSystem;
        private BuildSettings   _settings;

        private BuildTocContext _tocContext;

        private IBuildNamedList<BuildGroup> _listGroups;
        private IBuildNamedList<BuildGroupContext> _groupContexts;

        private EventWaitHandle _waitHandle;
        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        public BuildContext()
            : this(BuildSystem.Console, BuildType.Testing)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="system"></param>
        public BuildContext(BuildSystem system, BuildType type)
        {
            _buildType   = type;
            _buildState  = BuildState.None;
            _buildSystem = system;
            _waitHandle  = new ManualResetEvent(false);
            _properties  = new Dictionary<string, string>(
               StringComparer.OrdinalIgnoreCase);

            _tocContext  = new BuildTocContext();

            // Reset to the default properties
            this.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        ~BuildContext()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current build process is cancelled.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if current build process is 
        /// cancelled; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool IsCancelled
        {
            get
            {
                return (_buildState == BuildState.Cancelled);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this context is initialized and ready for
        /// the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this context is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildContext.Initialize(BuildContext)"/>
        /// <seealso cref="BuildContext.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public BuildState State
        {
            get
            {
                return _buildState;
            }
        }

        public BuildType BuildType
        {
            get 
            { 
                return _buildType; 
            }
        }

        public BuildLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public BuildSystem System
        {
            get 
            { 
                return _buildSystem; 
            }
        }

        public BuildTocContext TocContext
        {
            get
            {
                return _tocContext;
            }
        }

        public WaitHandle BuildWait
        {
            get
            {
                return _waitHandle;
            }
        }

        /// <summary>
        /// Gets or sets the fully qualified path of the current working directory.
        /// </summary>
        /// <value>
        /// A string containing a directory path.
        /// </value>
        public string WorkingDirectory
        {
            get
            {
                return _workingDir;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                    value = Path.GetFullPath(value);
                }
                _workingDir = value;
            }
        }

        public string OutputDirectory
        {
            get
            {
                return _outputDir;
            }
            set
            {
                _outputDir = value;
            }
        }

        public string BaseDirectory
        {
            get
            {
                return _baseWorkingDir;
            }
        }

        public string SandcastleDirectory
        {
            get
            {
                return _sandcastleDir;
            }
        }

        public string SandcastleToolsDirectory
        {
            get
            {
                return _sandcastleToolsDir;
            }
        }


        public bool BuildResult
        {
            get 
            { 
                return _isBuildSuccess; 
            }
            set
            {
                _isBuildSuccess = value;
            }
        }

        public IBuildNamedList<BuildGroup> Groups
        {
            get
            {
                return _listGroups;
            }
        }

        public IBuildNamedList<BuildGroupContext> GroupContexts
        {
            get
            {
                return _groupContexts;
            }
        }

        #endregion

        #region Public Methods

        public void BeginDirectories(BuildSettings settings)
        {
            // Prepare the working directory...
            string workingDir = settings.WorkingDirectory;
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
                return;
            }
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            _baseWorkingDir = workingDir;
            workingDir = Path.Combine(workingDir, BuildContext.WorkingFolder);
            if (Directory.Exists(workingDir))
            {
                try
                {                       
                    DirectoryUtils.DeleteDirectory(workingDir, true);
                }
                catch
                {  
                    // Mostly, the usual Windows - another process is using the
                    // folder issue...
                    // If the folder is empty, we just ignore the error
                    // and continue...
                	if (!DirectoryUtils.IsDirectoryEmpty(workingDir))
                    {
                        throw;
                    }
                }
            }
            Directory.CreateDirectory(workingDir);
            _workingDir = workingDir;

            // Prepare the output directory...
            string outputDir = settings.OutputDirectory;
            if (String.IsNullOrEmpty(outputDir))
            {
                outputDir = _baseWorkingDir;
            }
            else
            {
                outputDir = Environment.ExpandEnvironmentVariables(outputDir);
                outputDir = Path.GetFullPath(outputDir);
            }
            if (String.IsNullOrEmpty(outputDir))
            {
                return;
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            this.OutputDirectory = outputDir;
        }

        public void EndDirectories(BuildSettings settings)
        {
            if (_isBuildSuccess &&
                settings != null && settings.CleanIntermediate)
            {
                if (!String.IsNullOrEmpty(_workingDir) &&
                    Directory.Exists(_workingDir))
                {
                    try
                    {
                        DirectoryUtils.DeleteDirectory(_workingDir, true);
                    }
                    catch //(Exception ex)
                    {
                        //if (_logger != null)
                        //{
                        //    _logger.WriteLine(ex);
                        //    foreach (DictionaryEntry de in ex.Data)
                        //    {
                        //        String message = String.Format(
                        //            "The key is '{0}' and the value is: {1}",
                        //            de.Key, de.Value);
                        //        _logger.WriteLine(message, BuildLoggerLevel.Info);
                        //    }
                        //}
                    }
                }
            }
        }

        public virtual void BeginGroups(IBuildNamedList<BuildGroup> groups,
            IBuildNamedList<BuildGroupContext> contexts)
        {
            BuildExceptions.NotNull(groups,   "groups");
            BuildExceptions.NotNull(contexts, "contexts");

            _listGroups    = groups;
            _groupContexts = contexts;
        }

        public virtual void EndGroups()
        {
            _listGroups    = null;
            _groupContexts = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <seealso cref="BuildContext.IsInitialized"/>
        /// <seealso cref="BuildContext.Uninitialize()"/>
        public virtual void Initialize(BuildSettings settings, BuildLogger logger)
        {
            if (_isInitialized)
            {
                return;
            }

            BuildExceptions.NotNull(logger,   "logger");
            BuildExceptions.NotNull(settings, "settings");

            Debug.Assert(_listGroups != null && _listGroups.Count != 0);
            Debug.Assert(_groupContexts != null && _groupContexts.Count != 0);

            _logger             = logger;
            _settings           = settings;

            _isBuildSuccess     = false;

            _sandcastleDir      = String.Empty;
            _sandcastleToolsDir = String.Empty;

            // Get the Sandcastle installed directory and tools path information....
            string sandPath = _settings.SandcastleDirectory;
            if (!String.IsNullOrEmpty(sandPath))
            {
                sandPath = Environment.ExpandEnvironmentVariables(sandPath);
                if (!Directory.Exists(sandPath))
                {
                    sandPath = Environment.ExpandEnvironmentVariables("%DXROOT%");
                }
            }
            else
            {
                sandPath = Environment.ExpandEnvironmentVariables("%DXROOT%");
            }
            if (!Directory.Exists(sandPath))
            {
                _logger.WriteLine(
                    "The sandcastle installed directory could not be found.",
                    BuildLoggerLevel.Error);

                return;
            }
            else
            {
                _sandcastleDir      = sandPath;
                _sandcastleToolsDir = Path.Combine(sandPath, "ProductionTools");
                if (!Directory.Exists(_sandcastleToolsDir))
                {
                    _sandcastleDir      = String.Empty;
                    _sandcastleToolsDir = String.Empty;

                    _logger.WriteLine(
                        "The sandcastle installed directory found, but ProductionTools sub-directory is not found.",
                        BuildLoggerLevel.Error);

                    return;
                }
                _isInitialized = this.ValidateSandcastle();
            }

            _tocContext.Initialize(this);

            _isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="BuildContext.IsInitialized"/>
        /// <seealso cref="BuildContext.Initialize(BuildContext)"/>
        public virtual void Uninitialize()
        {
            _logger        = null;
            _settings      = null;
            _isInitialized = false;

            _tocContext.Uninitialize();
        }

        public virtual void SetState(BuildState state)
        {
            _buildState = state;
            if (state == BuildState.Cancelled)
            {
                if (_waitHandle != null)
                {
                    _waitHandle.Set();
                }
            }
        }

        public virtual bool StepCreated(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            if (_buildState == BuildState.Cancelled ||
                _buildState == BuildState.Error)
            {
                return false;
            }

            return true;
        }

        public virtual bool StepStarts(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            if (_buildState == BuildState.Cancelled || 
                _buildState == BuildState.Error)
            {
                return false;
            }

            if (_waitHandle != null)
            {
                return _waitHandle.Reset();
            }

            return true;
        }

        public virtual bool StepEnds(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            return true;
        }

        public virtual bool StepError(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            return true;
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
        }

        private bool ValidateSandcastle()
        {
            if (String.IsNullOrEmpty(_sandcastleDir) ||
                String.IsNullOrEmpty(_sandcastleToolsDir))
            {
                throw new InvalidOperationException("The validation requires Sandcastle installed paths.");
            }
            _logger.WriteLine("Sandcastle Directory: " + _sandcastleDir,
                BuildLoggerLevel.Info);

            string sampleTool = Path.Combine(_sandcastleToolsDir,
                "BuildAssembler.exe");
            if (!File.Exists(sampleTool))
            {
                return false;
            }
            FileVersionInfo sandcastleVersionInfo =
                FileVersionInfo.GetVersionInfo(sampleTool);

            _logger.WriteLine("Sandcastle Version: " +
                sandcastleVersionInfo.FileVersion, BuildLoggerLevel.Info);

            return true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_waitHandle != null)
            {
                _waitHandle.Close();
                _waitHandle = null;
            }
        }

        #endregion
    }
}
