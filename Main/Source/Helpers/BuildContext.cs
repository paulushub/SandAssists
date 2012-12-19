using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Reflection;
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
    public class BuildContext : MarshalByRefObject, IDisposable
    {
        #region Public Fields

        public const string WorkingFolder = "_HelpBuild";

        #endregion

        #region Private Fields

        private int             _processedTopics;

        private bool            _isInitialized;
        private bool            _isBuildSuccess;
        private bool            _isDirectSandcastle;

        private string          _outputDir;
        private string          _workingDir;
        private string          _baseWorkingDir;
        private string          _sandcastleDir;
        private string          _sandcastleToolsDir;

        private string          _targetPlatform;
        private string          _targetConfiguration;

        private BuildType       _buildType;
        private BuildState      _buildState;
        private BuildLogger     _logger;
        private BuildSystem     _buildSystem;
        private BuildSettings   _settings;

        private BuildProperties _properties;

        private BuildTocContext _tocContext;

        private BuildDictionary<object> _objects;

        private IBuildNamedList<BuildGroup> _listGroups;
        private IBuildNamedList<BuildGroupContext> _groupContexts;

        public List<BuildTuple<BuildFormatType, string>> _buildOutputs;

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
            _buildType    = type;
            _buildState   = BuildState.None;
            _buildSystem  = system;
            _properties   = new BuildProperties();
            _objects      = new BuildDictionary<object>();

            _targetPlatform      = String.Empty;
            _targetConfiguration = String.Empty;

            _tocContext   = new BuildTocContext();
            _buildOutputs = new List<BuildTuple<BuildFormatType, string>>();

            // Reset to the default properties
            this.Reset();
        }

        public BuildContext(BuildSystem system, BuildType type,
            string targetPlatform, string targetConfiguration)
            : this(system, type)
        {
            if (!String.IsNullOrEmpty(targetPlatform))
            {
                _targetPlatform = targetPlatform;
            }
            if (!String.IsNullOrEmpty(targetConfiguration))
            {
                _targetConfiguration = targetConfiguration;
            }
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
                return _properties[key];
            }
            set
            {
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

        /// <summary>
        /// Gets a value indicating whether the build process uses the built-in 
        /// <c>Sandcastle</c> library instead of the command-line tools.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the build system uses the build-in
        /// <c>Sandcastle</c> library; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool IsDirectSandcastle
        {
            get
            {
                return _isDirectSandcastle;
            }
        }

        public int ProcessedTopics
        {
            get
            {
                return _processedTopics;
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

        public string StylesDirectory
        {
            get
            {
                string sandcastleDir = this.SandcastleDirectory;
                sandcastleDir = Environment.ExpandEnvironmentVariables(sandcastleDir);
                sandcastleDir = Path.GetFullPath(sandcastleDir);

                if (_settings != null)
                {
                    BuildStyle outputStyle = _settings.Style;
                    string stylesDir = outputStyle.Directory;
                    if (!String.IsNullOrEmpty(stylesDir))
                    {
                        stylesDir = Environment.ExpandEnvironmentVariables(stylesDir);
                        stylesDir = Path.GetFullPath(stylesDir);
                        if (Directory.Exists(stylesDir))
                        {
                            sandcastleDir = stylesDir;
                        }
                    }
                }

                return sandcastleDir;
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

        public string DataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                if (_settings == null)
                {
                    return null;
                }

                string dataDir = Path.Combine(_settings.SandAssistDirectory, 
                    "Data");

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                return dataDir;
            }
        }

        public string ReflectionDataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                string dataDir = this.DataDirectory;
                if (String.IsNullOrEmpty(dataDir))
                {
                    return null;
                }

                string reflectionDir = Path.Combine(dataDir, "Reflections");

                if (!Directory.Exists(reflectionDir))
                {
                    Directory.CreateDirectory(reflectionDir);
                }

                return reflectionDir;
            }
        }

        public string TargetDataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                string dataDir = this.DataDirectory;
                if (String.IsNullOrEmpty(dataDir))
                {
                    return null;
                }

                string targetsDir = Path.Combine(dataDir, "Targets");

                if (!Directory.Exists(targetsDir))
                {
                    Directory.CreateDirectory(targetsDir);
                }

                return targetsDir;
            }
        }

        public string CommentDataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                string dataDir = this.DataDirectory;
                if (String.IsNullOrEmpty(dataDir))
                {
                    return null;
                }

                string commentsDir = Path.Combine(dataDir, "Comments");

                if (!Directory.Exists(commentsDir))
                {
                    Directory.CreateDirectory(commentsDir);
                }

                return commentsDir;
            }
        }

        public string IndexedDataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                string dataDir = this.DataDirectory;
                if (String.IsNullOrEmpty(dataDir))
                {
                    return null;
                }

                string indexedDataDir = Path.Combine(dataDir, "IndexedData");

                if (!Directory.Exists(indexedDataDir))
                {
                    Directory.CreateDirectory(indexedDataDir);
                }

                return indexedDataDir;
            }
        }

        public string LinksDataDirectory
        {
            get
            {
                //TODO-PAUL: From configuration file...
                string dataDir = this.DataDirectory;
                if (String.IsNullOrEmpty(dataDir))
                {
                    return null;
                }

                string linksDataDir = Path.Combine(dataDir, "Links");

                if (!Directory.Exists(linksDataDir))
                {
                    Directory.CreateDirectory(linksDataDir);
                }

                return linksDataDir;
            }
        }

        public string TargetPlatform
        {
            get
            {
                return _targetPlatform;
            }
        }

        public string TargetConfiguration
        {
            get
            {
                return _targetConfiguration;
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

        public IList<BuildTuple<BuildFormatType, string>> Outputs
        {
            get
            {
                if (_buildOutputs != null)
                {
                    return _buildOutputs.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void BeginDirectories(BuildSettings settings)
        {
            BuildExceptions.NotNull(settings, "settings");

            this.BeginDirectories(settings.WorkingDirectory,
                settings.OutputDirectory);
        }

        public void BeginDirectories(string workingDir, string outputDir)
        {
            // Prepare the working directory...
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
                    Thread.Sleep(200);

                	if (!DirectoryUtils.IsDirectoryEmpty(workingDir))
                    {
                        throw;
                    }
                }
            }
            Directory.CreateDirectory(workingDir);
            _workingDir = workingDir;

            // Prepare the output directory...
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
            BuildExceptions.NotNull(settings, "settings");

            this.EndDirectories(settings.CleanIntermediate);
        }

        public void EndDirectories(bool cleanIntermediate)
        {
            if (!_isBuildSuccess || !cleanIntermediate)
            {
                return;
            }

            if (String.IsNullOrEmpty(_workingDir) ||
                !Directory.Exists(_workingDir))
            {
                return;
            }

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

            _processedTopics = 0;

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

            if (_groupContexts != null && _groupContexts.Count != 0)
            {
                for (int i = 0; i < _groupContexts.Count; i++)
                {
                    _groupContexts[i].Initialize(this);
                }
            }

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

            if (_groupContexts != null && _groupContexts.Count != 0)
            {
                for (int i = 0; i < _groupContexts.Count; i++)
                {
                    _groupContexts[i].Uninitialize();
                }
            }
        }

        public void AddProcessedTopics(int processedTopics)
        {
            if (processedTopics > 0)
            {
                _processedTopics += processedTopics;
            }
        }

        public void AddOutput(BuildFormatType formatType, string outputFile)
        {
            BuildExceptions.PathMustExist(outputFile, "outputFile");

            if (_buildOutputs == null)
            {
                _buildOutputs = new List<BuildTuple<BuildFormatType, string>>();
            }

            _buildOutputs.Add(BuildTuple.Create(formatType, outputFile));
        }

        public virtual void SetState(BuildState state)
        {
            _buildState = state;
        }

        public virtual bool StepStarts(BuildStep buildStep)
        {
            BuildExceptions.NotNull(buildStep, "buildStep");

            if (_buildState == BuildState.Cancelled || 
                _buildState == BuildState.Error)
            {
                return false;
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

        public object GetValue(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            return _objects[key];
        }

        public void SetValue(string key, object value)
        {
            if (String.IsNullOrEmpty(key))
            {
                return;
            }

            _objects[key] = value;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
            string sandcastleAssist = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            if (File.Exists(Path.Combine(sandcastleAssist,
                "Sandcastle.Reflection.dll")) &&
                File.Exists(Path.Combine(sandcastleAssist,
                "Sandcastle.BuildAssembler.dll")))
            {
                _isDirectSandcastle = true;
            }
        }

        private bool ValidateSandcastle()
        {
            if (this.IsDirectSandcastle)
            {
                _logger.WriteLine("Sandcastle Version: " +
                    "Using the Sandcastle Assist Version.", BuildLoggerLevel.Info);

                return true;
            }

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
        }

        #endregion
    }
}
