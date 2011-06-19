using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Steps;
using Sandcastle.Loggers;
using Sandcastle.References;
using Sandcastle.Conceptual;
using Sandcastle.Configurators;

namespace Sandcastle
{
    public class BuildProject : BuildObject, IDisposable
    {
        #region Private Fields

        private int             _references;
        private int             _conceptuals;
        private bool            _isBuildSuccess;

        private DateTime        _startTime;
        private DateTime        _endTime;

        private bool            _isInitialized;

        private BuildType       _buildType;
        private BuildSystem     _buildSystem;  

        private BuildLoggers    _logger;
        private BuildContext    _context;
        private BuildSettings   _settings;
        private BuildDocumenter _documenter;

        private List<BuildStep>    _listSteps;
        private ReferenceEngine    _referenceEngine;
        private ConceptualEngine   _conceptualEngine;

        private IList<BuildFormat> _listFormats;
        private BuildKeyedList<BuildGroup> _listGroups;
        private BuildKeyedList<BuildGroupContext> _groupContexts;

        #endregion

        #region Constructors and Destructor

        public BuildProject()
            : this(null, BuildSystem.Console, BuildType.Development)
        {
        }

        public BuildProject(BuildSystem system, BuildType type)
            : this(null, system, type)
        {
        }

        public BuildProject(BuildDocumenter documenter, BuildSystem system, 
            BuildType type)
        {
            if (documenter == null)
            {
                _documenter = new BuildDocumenter();
            }
            else
            {
                _documenter = documenter;
            }

            _buildType   = type;
            _buildSystem = system;

            _logger      = new BuildLoggers();
            _context     = new BuildContext(system, type);
            _listSteps   = new List<BuildStep>(16);
        }

        ~BuildProject()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public BuildLoggers Logger
        {
            get
            {
                return _logger;
            }
        }

        public BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public BuildDocumenter Documenter
        {
            get
            {
                return _documenter;
            }
        }

        #endregion

        #region Protected Properties

        protected IList<BuildStep> Steps
        {
            get
            {
                return _listSteps;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Method

        public virtual void Initialize(BuildDocumenter documenter)
        {
            if (documenter != null)
            {
                _documenter = documenter;
            }

            this.Initialize();
        }

        public virtual void Initialize()
        {
            DateTime startTime = DateTime.Now;

            _startTime = startTime;

            // 1. Prepare all the build directories...
            this.OnBeginDocumentation();

            // 2. Prepare all the loggers...
            this.OnBeginLogging(); 

            try
            {
                _logger.WriteLine("Initialization of the documentation.",
                    BuildLoggerLevel.Started);
                _logger.WriteLine("Build started at: " +
                    startTime.ToString(), BuildLoggerLevel.Info);

                if (_isInitialized)
                {
                    _logger.WriteLine("The project is already initialized",
                        BuildLoggerLevel.Warn);

                    return;
                }

                this.OnInitialize();

                if (!_isInitialized)
                {
                    _logger.WriteLine("Error in reference build initialization.",
                        BuildLoggerLevel.Error);
                }
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }
            finally
            {
                if (_isInitialized)
                {
                    DateTime endTime = DateTime.Now;
                    TimeSpan timeElapsed = endTime - startTime;

                    _logger.WriteLine("Successfully completed in: "
                        + timeElapsed.ToString(), BuildLoggerLevel.Info);
                }

                _logger.WriteLine("Initialization of the documentation.",
                    BuildLoggerLevel.Ended);

                _logger.WriteLine();
            }
        }

        #endregion

        #region Build Method

        public virtual bool Build()
        {
            if (_isInitialized == false)
            {
                _logger.WriteLine("The project must be initialized before building.",
                    BuildLoggerLevel.Error);

                return false;
            }

            return this.OnBuild();
        }

        #endregion

        #region Uninitialize Method

        public virtual void Uninitialize()
        {
            _logger.WriteLine("Completion of the documentation.",
                BuildLoggerLevel.Started);

            try
            {
                this.OnEndDocumentation();

                this.OnUninitialize();

                _endTime = DateTime.Now;

                TimeSpan timeElapsed = _endTime - _startTime;

                if (_isBuildSuccess)
                {
                    IList<BuildTuple<BuildFormatType, string>> listOutputs
                        = Context.Outputs;
                    if (listOutputs != null && listOutputs.Count != 0)
                    {
                        for (int i = 0; i < listOutputs.Count; i++)
                        {
                            BuildTuple<BuildFormatType, string> outputFile
                                = listOutputs[i]; 

                            _logger.WriteLine("Output File: " + outputFile.Second,
                                BuildLoggerLevel.Info);
                        }

                        _logger.WriteLine("Total number of output Files: " + 
                            listOutputs.Count, BuildLoggerLevel.Info);
                    }

                    _logger.WriteLine("Total number of topics processed: " +
                        _context.ProcessedTopics, BuildLoggerLevel.Info);

                    _logger.WriteLine("Total number of warnings: " +
                        _logger.TotalWarnings, BuildLoggerLevel.Info);
                    _logger.WriteLine("Total number of errors: " +
                        _logger.TotalErrors, BuildLoggerLevel.Info);

                    _logger.WriteLine("Total time of completion: " +
                        timeElapsed.ToString(), BuildLoggerLevel.Info);

                    _logger.WriteLine("Build completed at: " +
                        _endTime.ToString(), BuildLoggerLevel.Info);
                }
                else
                {
                    _logger.WriteLine("Documentation failed due to error after: " +
                        timeElapsed.ToString(), BuildLoggerLevel.Info);

                    _logger.WriteLine("Build stopped at: " +
                        _endTime.ToString(), BuildLoggerLevel.Info);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }
            finally
            {
                _logger.WriteLine("Completion of the documentation.",
                    BuildLoggerLevel.Ended);
            }

            this.OnEndLogging();
        }

        #endregion

        #endregion

        #region Protected Methods

        #region OnBegin/EndDocumentation Methods

        protected virtual void OnBeginDocumentation()
        {
            _conceptuals = 0;
            _references  = 0;

            _settings    = _documenter.Settings;
            IList<BuildGroup> listGroups = _documenter.Groups;
            if (listGroups == null || listGroups.Count == 0)
            {
                throw new BuildException(
                    "There is no build group available for the documentations.");
            }

            _listGroups    = new BuildKeyedList<BuildGroup>();
            _groupContexts = new BuildKeyedList<BuildGroupContext>();
            for (int i = 0; i < listGroups.Count; i++)
            {
                BuildGroup group = listGroups[i];
                if (group != null && !group.IsEmpty && !group.Exclude)
                {
                    _listGroups.Add(group);
                }
            }
            if (_listGroups == null || _listGroups.Count == 0)
            {
                throw new BuildException(
                    "There is no valid build group available for the documentations.");
            }

            for (int i = 0; i < _listGroups.Count; i++)
            {
                BuildGroup group = _listGroups[i];
                BuildGroupType groupType = group.GroupType;
                if (groupType == BuildGroupType.Conceptual)
                {
                    _groupContexts.Add(new ConceptualGroupContext(
                        (ConceptualGroup)group));

                    _conceptuals++;
                }
                else if (groupType == BuildGroupType.Reference)
                {
                    _groupContexts.Add(new ReferenceGroupContext(
                        (ReferenceGroup)group));

                    _references++;
                }
            }

            _context.BeginDirectories(_settings);
            _context.BeginGroups(_listGroups, _groupContexts);
        }

        protected virtual void OnEndDocumentation()
        {
            _context.EndGroups();
            _context.EndDirectories(_settings);
        }

        #endregion

        #region OnBuild Method

        protected virtual bool OnBuild()
        {
            _isBuildSuccess = false;

            if (_context != null)
            {
                _context.BuildResult = _isBuildSuccess;
            }

            if (_isInitialized == false)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(
                        "The project must be initialized before building.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (!_logger.IsInitialized)
            {
                _logger.Initialize(_context.BaseDirectory, _settings.HelpTitle);
            }

            try
            {
                _isBuildSuccess = this.OnRunSteps();
            }
            catch (Exception ex)
            {
                _isBuildSuccess = false;

                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }

            if (_context != null)
            {
                _context.BuildResult = _isBuildSuccess;
            }

            return _isBuildSuccess;
        }

        #endregion

        #region OnBegin/EndLogging Method

        protected virtual void OnBeginLogging()
        {        
            BuildLogging logging = _settings.Logging;

            // 1. Create any requested logger...
            IList<string> loggerNames = logging.Loggers;
            if (loggerNames != null && loggerNames.Count != 0)
            {
                for (int i = 0; i < loggerNames.Count; i++)
                {
                    string loggerName = loggerNames[i];
                    if (String.IsNullOrEmpty(loggerName))
                    {
                        continue;
                    }

                    switch (loggerName)
                    {
                        case NoneLogger.LoggerName:
                            _logger.Add(new NoneLogger());
                            break;
                        case ConsoleLogger.LoggerName:
                            _logger.Add(new ConsoleLogger(logging.UseFile));
                            break;
                        case FileLogger.LoggerName:
                            _logger.Add(new FileLogger());
                            break;
                        case HtmlLogger.LoggerName:
                            _logger.Add(new HtmlLogger());
                            break;
                        case XmlLogger.LoggerName:
                            _logger.Add(new XmlLogger());
                            break;
                        case XamlLogger.LoggerName:
                            _logger.Add(new XamlLogger());
                            break;
                        case BuildLoggers.LoggerName:
                            _logger.Add(new BuildLoggers());
                            break;
                        default:
                            //TODO-PAUL: Add support for custom loggers...
                            throw new NotImplementedException();
                    }
                }
            }

            // 2. If there is no logger, we try creating a default logger...
            if (_logger.Count == 0)
            {
                BuildLogger defLogger = this.OnCreateLogger();
                if (defLogger != null)
                {
                    _logger.Add(defLogger);
                }
            }

            // 3. Update the logger settings...
            bool keepLogFile = logging.KeepFile;
            for (int i = 0; i < _logger.Count; i++)
            {
                BuildLogger logger = _logger[i];
                if (logger != null)
                {
                    logger.KeepLog = keepLogFile;
                }
            }

            // 4. Initialize the loggers...
            if (!_logger.IsInitialized)
            {
                string logWorkingDir = null;
                BuildDirectoryPath outputPath = logging.OutputPath;
                if (outputPath != null && outputPath.IsValid)
                {
                    logWorkingDir = outputPath.Path;
                }
                else
                {
                    logWorkingDir = Path.Combine(_context.BaseDirectory, 
                        BuildLogging.DefaultOutputDirectory);
                }

                if (!Directory.Exists(logWorkingDir))
                {
                    Directory.CreateDirectory(logWorkingDir);
                }

                _logger.Initialize(logWorkingDir, _settings.HelpTitle);
            }
        }

        protected virtual void OnEndLogging()
        {
            try
            {
                _logger.Uninitialize();
            }
            catch
            {
                return;
            }

            //if (_settings == null)
            //{
            //    return;
            //}

            // Move the log file to the output directory...
            //if (!String.IsNullOrEmpty(_logFile) && File.Exists(_logFile))
            //{
            //    try
            //    {
            //        if (_settings.KeepLogFile)
            //        {
            //            string outputDir = _context.OutputDirectory;
            //            if (String.IsNullOrEmpty(outputDir) == false)
            //            {
            //                outputDir = Environment.ExpandEnvironmentVariables(
            //                    outputDir);
            //                outputDir = Path.GetFullPath(outputDir);
            //                string logFile = Path.Combine(outputDir,
            //                    _settings.LogFile);

            //                File.SetAttributes(_logFile, FileAttributes.Normal);
            //                File.Move(_logFile, logFile);
            //            }
            //        }
            //        else
            //        {
            //            File.SetAttributes(_logFile, FileAttributes.Normal);
            //            File.Delete(_logFile);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            //_logFile = null;
        }

        protected virtual BuildLogger OnCreateLogger()
        {
            BuildLogging logging = _settings.Logging;
            string logFileName   = logging.FileName;

            if ( _buildSystem == BuildSystem.Console)
            {
                if (!String.IsNullOrEmpty(logFileName))
                {
                    return new ConsoleLogger(logFileName, logging.UseFile);
                }
                else
                {
                    return new ConsoleLogger(logging.UseFile);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(logFileName) || logging.UseFile)
                {
                    return new FileLogger(logFileName);
                }
            }

            return null;
        }

        #endregion

        #region OnInitialize Method

        protected virtual void OnInitialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = false;

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _isBuildSuccess = false;

            _settings.BeginInit();

            _context.Initialize(_settings, _logger);

            _settings.Initialize(_context);

            _settings.EndInit();

            // If the logger is not initialized, we initialize it now...
            if (!_logger.IsInitialized)
            {
                _logger.Initialize(_context.BaseDirectory, _settings.HelpTitle);
            }

            BuildFormatList listFormats = _settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                return;
            }
            int itemCount = listFormats.Count;
            _listFormats  = new BuildFormatList();
            for (int i = 0; i < itemCount; i++)
            {
                BuildFormat format = listFormats[i];
                if (format != null && format.Enabled)
                {
                    _listFormats.Add(format);
                }
            }
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return;
            }

            _referenceEngine   = new ReferenceEngine();
            _conceptualEngine  = new ConceptualEngine();

            int includedApi    = 0;
            int includedTopics = 0;
            BuildGroup group   = null;
            BuildGroupType groupType = BuildGroupType.None;
            itemCount = _listGroups.Count;
            for (int i = 0; i < itemCount; i++)
            {
                group = _listGroups[i];
                if (group == null || group.IsEmpty || group.Exclude)
                {
                    continue;
                }

                groupType = group.GroupType;
                if (groupType == BuildGroupType.Reference)
                {
                    includedApi++;
                    _referenceEngine.Add((ReferenceGroup)group);
                }
                else if (groupType == BuildGroupType.Conceptual)
                {
                    includedTopics++;
                    _conceptualEngine.Add((ConceptualGroup)group);
                }
            }

            bool buildApi = (includedApi != 0 && _references > 0 &&
                _settings.BuildReferences);
            bool buildTopics = (includedTopics != 0 && _conceptuals > 0 &&
                _settings.BuildConceptual);

            if (buildApi)
            {
                try
                {
                    _referenceEngine.Initialize(_context);
                    if (!_referenceEngine.IsInitialized)
                    {
                        _isInitialized = false;
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (buildTopics)
            {
                try
                {
                    _conceptualEngine.Initialize(_context);
                    if (!_conceptualEngine.IsInitialized)
                    {
                        _isInitialized = false;
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            _context["$HelpFormatCount"]         = _listFormats.Count.ToString();
            _context["$HelpTocFile"]             = _settings.Toc.TocFile;
            _context["$HelpTocMarkers"]          = Boolean.FalseString;
            _context["$HelpHierarchicalToc"]     = Boolean.FalseString;

            _isInitialized = this.OnCreateSteps();
        }

        #endregion

        #region OnUninitialize Method

        protected virtual void OnUninitialize()
        {
            try
            {
                if (_referenceEngine != null)
                {
                    _referenceEngine.Uninitialize();
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            try
            {
                if (_conceptualEngine != null)
                {
                    _conceptualEngine.Uninitialize();
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (_referenceEngine != null)
            {
                _referenceEngine.Dispose();
                _referenceEngine = null;
            }
            if (_conceptualEngine != null)
            {
                _conceptualEngine.Dispose();
                _conceptualEngine = null;
            }

            if (_settings != null)
            {
                _settings.Uninitialize();
            }
            if (_context != null)
            {
                _context.Uninitialize();
            }

            _isInitialized = false;
        }

        #endregion

        #region OnCreateSteps Method

        protected virtual bool OnCreateSteps()
        {
            try
            {
                if (!this.OnCreatePreBuildSteps())
                {
                    return false;
                }

                // 1. Create the initial build steps...
                if (!this.OnCreateInitialSteps())
                {
                    return false;
                }

                // 2. Create the table of contents and other steps...
                if (!this.OnCreateMergingSteps())
                {
                    return false;
                }

                // 3. Create the final build steps...
                if (!this.OnCreateFinalSteps())
                {
                    return false;
                }

                if (!this.OnCreatePostBuildSteps())
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);

                return false;
            }
        }

        #endregion

        #region OnCreateInitialSteps Method

        protected virtual bool OnCreateInitialSteps()
        {
            if (_settings.BuildReferences &&
                (_referenceEngine != null && _referenceEngine.IsInitialized))
            {
                try
                {
                    IList<ReferenceGroup> groups = _referenceEngine.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        BuildMultiStep listSteps = new BuildMultiStep();
                        listSteps.LogTitle = "Preparing references topics contents.";

                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ReferenceGroup group = groups[i];
                            if (group != null && group.IsEmpty == false)
                            {
                                BuildStep apiSteps = _referenceEngine.CreateInitialSteps(group);
                                if (apiSteps != null)
                                {
                                    listSteps.Add(apiSteps);
                                }
                            }
                        }

                        if (listSteps.Count != 0)
                        {
                            _listSteps.Add(listSteps);

                            listSteps.LogTimeSpan = (listSteps.Count > 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);

                    return false;
                }
            }

            if (_settings.BuildConceptual &&
                (_conceptualEngine != null && _conceptualEngine.IsInitialized))
            {
                try
                {
                    IList<ConceptualGroup> groups = _conceptualEngine.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        BuildMultiStep listSteps = new BuildMultiStep();
                        listSteps.LogTitle = "Preparing conceptual topics contents.";

                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ConceptualGroup group = groups[i];
                            if (group != null && group.IsEmpty == false)
                            {
                                BuildStep topicSteps = _conceptualEngine.CreateInitialSteps(group);
                                if (topicSteps != null)
                                {
                                    listSteps.Add(topicSteps);
                                }
                            }
                        }

                        if (listSteps.Count != 0)
                        {
                            _listSteps.Add(listSteps);

                            listSteps.LogTimeSpan = (listSteps.Count > 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);

                    return false;
                }
            }

            return true;
        }

        #endregion

        #region OnCreateMergingSteps Method

        protected virtual bool OnCreateMergingSteps()
        {
            BuildEngineSettingsList listSettings = _settings.EngineSettings;
            Debug.Assert(listSettings != null,
                "The settings does not include the engine settings.");
            if (listSettings == null || listSettings.Count == 0)
            {
                return false;
            }
            ReferenceEngineSettings engineSettings =
                listSettings[BuildEngineType.Reference] as ReferenceEngineSettings;

            Debug.Assert(engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (engineSettings == null)
            {
                return false;
            }

            bool isHierarchical = false;
            // If there is no hierarchical TOC, do not proceed further...
            ReferenceTocLayoutConfiguration tocConfig = engineSettings.TocLayout;
            if (tocConfig != null && tocConfig.Enabled && tocConfig.IsActive)
            {
                ReferenceTocLayoutType layoutType = tocConfig.LayoutType;
                if (layoutType == ReferenceTocLayoutType.Hierarchical ||
                    layoutType == ReferenceTocLayoutType.Custom)
                {
                    isHierarchical = true;
                }
            }

            _context["$HelpHierarchicalToc"] = isHierarchical.ToString();

            string workingDir     = _context.WorkingDirectory;

            // Merge the table of contents...
            StepTocMerge tocMerge = new StepTocMerge(workingDir, isHierarchical);
            bool createResult     = false;

            // Create the merging for the flat TOC for formats requiring it...
            try
            {
                BuildTocContext tocContext = _context.TocContext;

                IList<ConceptualGroup> topicsGroups = _conceptualEngine.Groups;

                if (topicsGroups != null && topicsGroups.Count != 0)
                {
                    int itemCount = topicsGroups.Count;

                    for (int i = 0; i < itemCount; i++)
                    {
                        ConceptualGroup group = topicsGroups[i];
                        //if (group.ExcludeToc)
                        //{
                        //    continue;
                        //}

                        BuildGroupContext groupContext = _groupContexts[group.Id];
                        if (groupContext == null)
                        {
                            throw new BuildException(
                                "The group context is not provided, and it is required by the build system.");
                        }

                        string topicsToc = groupContext["$TocFile"];
                        if (!String.IsNullOrEmpty(topicsToc))
                        {
                            tocMerge.Add(topicsToc, BuildGroupType.Conceptual,
                                group.Id, false, group.ExcludeToc);

                            if (!Path.IsPathRooted(topicsToc))
                            {
                                topicsToc = Path.Combine(workingDir, topicsToc);
                            }

                            ConceptualGroupTocInfo groupTocInfo = new ConceptualGroupTocInfo(
                                group.Id, topicsToc);
                            groupTocInfo.Exclude = group.ExcludeToc;

                            tocContext.Add(groupTocInfo);

                            ConceptualContent topicContent = group.Content;
                            IList<ConceptualRelatedTopic> relatedTopics =
                                topicContent.RelatedTopics;
                            if (relatedTopics != null && relatedTopics.Count != 0)
                            {
                                for (int j = 0; j < relatedTopics.Count; j++)
                                {
                                    ConceptualRelatedTopic relatedTopic =
                                        relatedTopics[j];
                                    BuildTopicTocInfo tocInfo = new BuildTopicTocInfo(
                                        relatedTopic.TopicId, relatedTopic.TopicId, null);

                                    tocContext.Add(tocInfo);
                                }
                            }
                        }
                    }
                }

                IList<ReferenceGroup> apiGroups = _referenceEngine.Groups;

                if (apiGroups != null && apiGroups.Count != 0)
                {
                    int itemCount = apiGroups.Count;

                    for (int i = 0; i < itemCount; i++)
                    {
                        ReferenceGroup group = apiGroups[i];
                        //if (group.ExcludeToc)
                        //{
                        //    continue;
                        //}

                        BuildGroupContext groupContext = _groupContexts[group.Id];
                        if (groupContext == null)
                        {
                            throw new BuildException(
                                "The group context is not provided, and it is required by the build system.");
                        }

                        string topicsToc = String.Empty;
                        if (isHierarchical)
                        {
                            topicsToc = groupContext["$HierarchicalTocFile"];
                        }
                        else
                        {
                            topicsToc = groupContext["$TocFile"];
                        }
                        if (!String.IsNullOrEmpty(topicsToc))
                        {
                            tocMerge.Add(topicsToc, BuildGroupType.Reference,
                                group.Id, Convert.ToBoolean(groupContext["$IsRooted"]),
                                group.ExcludeToc);

                            if (!Path.IsPathRooted(topicsToc))
                            {
                                topicsToc = Path.Combine(workingDir, topicsToc);
                            }

                            ReferenceGroupTocInfo groupTocInfo = new ReferenceGroupTocInfo(
                                group.Id, topicsToc);
                            groupTocInfo.Exclude = group.ExcludeToc;

                            tocContext.Add(groupTocInfo);
                        }
                    }
                }

                if (tocMerge != null)
                {
                    _listSteps.Add(tocMerge);
                }

                createResult = true;
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);

                createResult = false;
            }

            return createResult;
        }

        #endregion

        #region OnCreateFinalSteps Method

        protected virtual bool OnCreateFinalSteps()
        {
            HashSet<string> outputFolders = new HashSet<string>();

            if (_settings.BuildReferences &&
                (_referenceEngine != null && _referenceEngine.IsInitialized))
            {
                try
                {
                    IList<ReferenceGroup> groups = _referenceEngine.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        BuildMultiStep listSteps = new BuildMultiStep();
                        listSteps.LogTitle = "Assembling References Topics";

                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ReferenceGroup group = groups[i];
                            if (group != null && !group.IsEmpty)
                            {
                                BuildStep apiSteps = _referenceEngine.CreateFinalSteps(group);
                                if (apiSteps != null)
                                {
                                    listSteps.Add(apiSteps);
                                }
                                IList<string> listFolders = _referenceEngine.Folders;
                                if (listFolders != null && listFolders.Count != 0)
                                {
                                    outputFolders.UnionWith(listFolders);
                                }
                            }
                        }

                        if (listSteps.Count != 0)
                        {
                            _listSteps.Add(listSteps);

                            listSteps.LogTimeSpan = (listSteps.Count > 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);

                    return false;
                }
            }

            List<string> ddueHtmlDirs = new List<string>();

            if (_settings.BuildConceptual &&
                (_conceptualEngine != null && _conceptualEngine.IsInitialized))
            {
                try
                {
                    IList<ConceptualGroup> groups = _conceptualEngine.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        BuildMultiStep listSteps = new BuildMultiStep();
                        listSteps.LogTitle = "Assembling Conceptual Topics";

                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ConceptualGroup group = groups[i];

                            BuildGroupContext groupContext = _groupContexts[group.Id];
                            if (groupContext == null)
                            {
                                throw new BuildException(
                                    "The group context is not provided, and it is required by the build system.");
                            }

                            BuildStep topicSteps = _conceptualEngine.CreateFinalSteps(group);
                            if (topicSteps != null)
                            {
                                listSteps.Add(topicSteps);
                            }
                            IList<string> listFolders = _conceptualEngine.Folders;
                            if (listFolders != null && listFolders.Count != 0)
                            {
                                outputFolders.UnionWith(listFolders);
                            }

                            ddueHtmlDirs.Add(groupContext["$DdueHtmlDir"]);
                        }

                        if (listSteps.Count != 0)
                        {
                            _listSteps.Add(listSteps);

                            listSteps.LogTimeSpan = (listSteps.Count > 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);

                    return false;
                }
            }

            int formatCount = _listFormats.Count;
            BuildMultiStep copySteps = new BuildMultiStep();
            copySteps.LogTitle = "Copying the outputs to the format directories.";
            copySteps.LogTimeSpan = true;

            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format   = _listFormats[i];
                string helpOutputDir = format.OutputFolder;

                // Copy outputs to the "Help" sub-directories
                StepDirectoryCopy copyDirs = new StepDirectoryCopy(
                    _context.WorkingDirectory);
                copyDirs.LogTitle = String.Empty;

                // A message displayed before each step, for multiple formats...
                string messageText = null;

                if (formatCount != 0)
                {
                    messageText = "For the help format: " + format.Name;
                }

                string tempText = null;
                foreach (string folder in outputFolders)
                {
                    tempText = String.Format(@"{0}\{1}", helpOutputDir, folder);
                    copyDirs.Add(String.Format(@"Output\{0}\", folder), tempText);
                }

                // Copy any direct HTML topic contents...
                tempText = String.Format(@"Output\{0}", format.FormatFolder);
                for (int j = 0; j < ddueHtmlDirs.Count; j++)
                {
                    copyDirs.Add(String.Format(@"{0}\", ddueHtmlDirs[j]), tempText);
                }

                if (String.IsNullOrEmpty(messageText))
                {
                    copySteps.Add(copyDirs);
                }
                else
                {
                    copySteps.Add(copyDirs, messageText);
                }
            }

            if (copySteps != null && copySteps.Count != 0)
            {
                _listSteps.Add(copySteps);
            }

            return true;
        }

        #endregion

        #region OnCreatePreBuildSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCreatePreBuildSteps()
        {
            BuildStyle buildStyle = _settings.Style;
            string sandcastleDir  = _context.StylesDirectory;
            string sandassistDir  = _settings.SandAssistDirectory;

            if (String.IsNullOrEmpty(sandcastleDir) ||
                String.IsNullOrEmpty(sandassistDir))
            {
                return false;
            }

            BuildStyleType styleType = buildStyle.StyleType;
            string helpStyle = BuildStyleUtils.StyleFolder(styleType);
            int formatCount = _listFormats.Count;

            // Ensure that we have a valid list of folders...
            HashSet<string> listFolders = this.OnOutputFolders();

            string baseDir    = _context.BaseDirectory;
            string workingDir = _context.WorkingDirectory;

            string tempText = null;

            // In the beginning... 
            // 1. Close any current viewer of the compiled helps...
            // Where applicable, we must close all the help viewers, since an
            // opened help file cannot be deleted...
            BuildMultiStep closeViewerSteps = new BuildMultiStep();
            closeViewerSteps.LogTitle = "Closing help file viewers.";
            closeViewerSteps.LogTimeSpan = true;

            // 2. Clean up the current build output directories
            StepDirectoryDelete deleteDirs = new StepDirectoryDelete(baseDir);
            deleteDirs.LogTitle = "Deleting output directories.";
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];
                if (format != null && format.CloseViewerBeforeBuild)
                {
                    BuildStep closeViewer = format.CreateStep(_context,
                        BuildStage.CloseViewer, workingDir);

                    if (closeViewer != null)
                    {
                        closeViewer.LogTitle = String.Empty;

                        closeViewerSteps.Add(closeViewer,
                            "For the format: " + format.Name);
                    }
                }

                string formatFolder = format.OutputFolder;
                if (!String.IsNullOrEmpty(formatFolder))
                {
                    deleteDirs.Add(formatFolder);
                }
            }

            if (closeViewerSteps.Count != 0)
            {
                _listSteps.Add(closeViewerSteps);
            }

            //// Add the IntelliSense directory for deletion, may not exists...
            //deleteDirs.Add("Intellisense");
            _listSteps.Add(deleteDirs);

            // 3. Recreate the output directories
            StepDirectoryCreate createDir = new StepDirectoryCreate(workingDir);
            createDir.LogTitle = "Creating standard and formats directories.";
            //createDir.Add("Intellisense");
            foreach (string folder in listFolders)
            {
                createDir.Add(@"Output\" + folder);
            } 
            _listSteps.Add(createDir);

            // 4. Copy the resources files: icons, styles and scripts...
            StepDirectoryCopy copyOutput = new StepDirectoryCopy(workingDir);
            copyOutput.LogTitle = "Copying standard resources and contents to working directory.";
            tempText = String.Format(@"Presentation\{0}\icons\", helpStyle);
            tempText = Path.Combine(sandcastleDir, tempText);
            copyOutput.Add(tempText, @"Output\icons\");

            tempText = String.Format(@"Presentation\{0}\scripts\", helpStyle);
            tempText = Path.Combine(sandcastleDir, tempText);
            copyOutput.Add(tempText, @"Output\scripts\");

            // Copy the media extension files...
            // 1. The Adobe Flash supporting files...
            tempText = Path.Combine(sandassistDir, @"Scripts\Flash");
            if (Directory.Exists(tempText))
            {
                copyOutput.Add(tempText, @"Output\scripts\");
            }
            // 2. The Microsoft Silverlight supporting files...
            tempText = Path.Combine(sandassistDir, @"Scripts\Silverlight");
            if (Directory.Exists(tempText))
            {
                copyOutput.Add(tempText, @"Output\scripts\");
            }

            if (styleType == BuildStyleType.ClassicBlue)
            {
                tempText = String.Format(@"Presentation\{0}\styles\Whidbey", helpStyle);
            }
            else
            {
                tempText = String.Format(@"Presentation\{0}\styles\", helpStyle);
            }
            tempText = Path.Combine(sandcastleDir, tempText);
            copyOutput.Add(tempText, @"Output\styles\");

            _listSteps.Add(copyOutput);

            return true;
        }

        #endregion

        #region OnCreatePostBuildSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCreatePostBuildSteps()
        {
            string workingDir = _context.WorkingDirectory;

            // 9. Prepare the help html files, and create the html project
            // Build and/or compile the help files...
            BuildMultiStep compileSteps = new BuildMultiStep();

            int formatCount = _listFormats.Count;
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                BuildStep compileHelp = format.CreateStep(_context,
                    BuildStage.Compilation, workingDir);

                if (compileHelp != null)
                {
                    compileSteps.Add(compileHelp);
                }
            }

            if (compileSteps != null && compileSteps.Count != 0)
            {
                _listSteps.Add(compileSteps);
            }

            // In the end...
            BuildMultiStep startViewerSteps = new BuildMultiStep();
            startViewerSteps.LogTitle = "Opening help files for viewing.";
            startViewerSteps.LogTimeSpan = true;

            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                if (format != null && format.OpenViewerAfterBuild)
                {
                    BuildStep startViewer = format.CreateStep(_context,
                        BuildStage.StartViewer, workingDir);

                    if (startViewer != null)
                    {
                        startViewer.LogTitle = String.Empty;

                        startViewerSteps.Add(startViewer,
                            "For the format: " + format.Name);
                    }
                }
            }

            if (startViewerSteps != null && startViewerSteps.Count != 0)
            {
                _listSteps.Add(startViewerSteps);
            }

            return true;
        }

        #endregion

        #region OnRunSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listSteps"></param>
        /// <returns></returns>
        protected virtual bool OnRunSteps()
        {
            if (_settings == null || _context == null ||
                _isInitialized == false)
            {
                return false;
            }

            if (_listSteps == null || _listSteps.Count == 0)
            {
                return false;
            }

            int stepCount = _listSteps.Count;

            string currentDir = Environment.CurrentDirectory;
            bool buildResult = false;

            try
            {
                Environment.CurrentDirectory = _context.WorkingDirectory;

                buildResult = true;

                // 1. Initialize all the build steps, and set them ready for build...
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = _listSteps[i];
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
                    BuildStep buildStep = _listSteps[i];

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
                    BuildStep buildStep = _listSteps[i];
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

        #region OnOutputFolders Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listFolders"></param>
        /// <returns></returns>
        protected HashSet<string> OnOutputFolders()
        {
            HashSet<string> listFolders = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            if (_settings == null)
            {
                return listFolders;
            }

            IList<string> folders = _settings.OutputFolders;

            if (folders == null || folders.Count == 0)
            {
                return listFolders;
            }

            int folderCount = folders.Count;
            for (int i = 0; i < folderCount; i++)
            {
                string folder = folders[i];
                if (!String.IsNullOrEmpty(folder))
                {
                    listFolders.Add(folder);
                }
            }

            return listFolders;
        }

        #endregion

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isInitialized = false;

            if (_referenceEngine != null)
            {
                _referenceEngine.Dispose();
                _referenceEngine = null;
            }
            if (_conceptualEngine != null)
            {
                _conceptualEngine.Dispose();
                _conceptualEngine = null;
            }

            if (_logger != null)
            {
                _logger.Dispose();
                _logger = null;
            }

            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        #endregion
    }
}
