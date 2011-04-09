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
            this.BeginDocumentation();

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
            DateTime startTime = DateTime.Now;

            _logger.WriteLine("Completion of the documentation.",
                BuildLoggerLevel.Started);

            this.EndDocumentation();

            try
            {
                this.OnUninitialize();

                DateTime endTime = DateTime.Now;
                _endTime = endTime;

                TimeSpan timeElapsed = endTime - startTime;

                _logger.WriteLine("Successfully completed in: " + 
                    timeElapsed.ToString(), BuildLoggerLevel.Info);

                _logger.WriteLine("Build completed at: " +
                    _endTime.ToString(), BuildLoggerLevel.Info);

                timeElapsed = _endTime - _startTime;

                _logger.WriteLine("Total time of completion: " +
                    timeElapsed.ToString(), BuildLoggerLevel.Info);
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

            try
            {
                _logger.Uninitialize();
            }
            catch
            {              	
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

        #endregion

        #endregion

        #region Protected Methods

        #region Begin/EndDocumentation Methods

        protected virtual void BeginDocumentation()
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

            // 2. Initialize the logger...
            if (!_logger.IsInitialized)
            {
                _logger.Initialize(_context.BaseDirectory, _settings.HelpTitle);
            }
        }

        protected virtual void EndDocumentation()
        {
            _context.EndGroups();
            _context.EndDirectories(_settings);
        }

        #endregion

        #region Build Method

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
                _isBuildSuccess = this.RunSteps(_listSteps);
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

            // 1. If there is no logger, we try creating a default logger...
            if (_logger.Count == 0)
            {
                BuildLogger defLogger = this.OnCreateLogger();
                if (defLogger != null)
                {
                    _logger.Add(defLogger);
                }
            }

            // 2. If the logger is not initialized, we initialize it now...
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
            _context["$HelpTocFile"]             = BuildToc.HelpToc;
            _context["$HelpTocMarkers"]          = Boolean.FalseString;
            _context["$HelpHierarchicalTocFile"] = BuildToc.HierarchicalToc;
            _context["$HelpHierarchicalToc"]     = Boolean.FalseString;

            _isInitialized = this.CreateSteps();
        }

        protected virtual BuildLogger OnCreateLogger()
        {
            string workingDir  = _context.BaseDirectory;
            string logFileName = _settings.LogFileName;

            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = _context.WorkingDirectory;
                if (String.IsNullOrEmpty(workingDir))
                {
                    workingDir = _settings.WorkingDirectory;
                }
            }

            if (!String.IsNullOrEmpty(logFileName))
            {
                if (!String.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
                {
                    string logFullPath = Path.Combine(workingDir, logFileName);

                    try
                    {
                        if (File.Exists(logFullPath))
                        {
                            File.SetAttributes(logFullPath, FileAttributes.Normal);
                            File.Delete(logFullPath);
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            if (String.IsNullOrEmpty(workingDir) ||
                _buildSystem == BuildSystem.Console)
            {
                if (!String.IsNullOrEmpty(logFileName) || _settings.UseLogFile)
                {
                    ConsoleLogger logger = new ConsoleLogger(logFileName);
                    logger.KeepLog = _settings.KeepLogFile;

                    return logger;
                }
                else
                {
                    return new ConsoleLogger();
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(logFileName) || _settings.UseLogFile)
                {
                    FileLogger logger = new FileLogger(logFileName);
                    logger.KeepLog    = _settings.KeepLogFile;

                    return logger;
                }
            }

            return null;
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

        #region CreateSteps Method

        protected virtual bool CreateSteps()
        {
            try
            {
                if (!this.CreatePreBuildSteps())
                {
                    return false;
                }

                // 1. Create the initial build steps...
                if (!this.CreateInitialSteps())
                {
                    return false;
                }

                // 2. Create the table of contents and other steps...
                if (!this.CreateMergingSteps())
                {
                    return false;
                }

                // 3. Create the final build steps...
                if (!this.CreateFinalSteps())
                {
                    return false;
                }

                if (!this.CreatePostBuildSteps())
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

        #region CreateInitialSteps Method

        protected virtual bool CreateInitialSteps()
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

        #region CreateMergingSteps Method

        protected virtual bool CreateMergingSteps()
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
                        if (group.ExcludeToc)
                        {
                            continue;
                        }

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
                                group.Id, false);

                            if (!Path.IsPathRooted(topicsToc))
                            {
                                topicsToc = Path.Combine(workingDir, topicsToc);
                            }

                            ConceptualGroupTocInfo groupTocInfo = new ConceptualGroupTocInfo(
                                group.Id, topicsToc);

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
                        if (group.ExcludeToc)
                        {
                            continue;
                        }

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
                                group.Id, Convert.ToBoolean(groupContext["$IsRooted"]));

                            if (!Path.IsPathRooted(topicsToc))
                            {
                                topicsToc = Path.Combine(workingDir, topicsToc);
                            }

                            ReferenceGroupTocInfo tocInfo = new ReferenceGroupTocInfo(
                                group.Id, topicsToc);

                            tocContext.Add(tocInfo);
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

        #region CreateFinalSteps Method

        protected virtual bool CreateFinalSteps()
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

        #region CreatePreBuildSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreatePreBuildSteps()
        {
            BuildStyle buildStyle = _settings.Style;
            string sandcastleDir  = _settings.StylesDirectory;
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
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount   = listFolders.Count;

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

            //// Add the Intellisense directory for deletion, may not exists...
            //deleteDirs.Add("Intellisense");
            _listSteps.Add(deleteDirs);

            // 3. Recreate the output directories
            StepDirectoryCreate createDir = new StepDirectoryCreate(workingDir);
            createDir.LogTitle = "Creating standard and formats directories.";
            //createDir.Add("Intellisense");
            for (int i = 0; i < folderCount; i++)
            {
                createDir.Add(@"Output\" + listFolders[i]);
            }
            //for (int i = 0; i < formatCount; i++)
            //{
            //    BuildFormat format = _listFormats[i];
            //    createDir.Add(Path.Combine("Output", format.FormatFolder));
            //    createDir.Add(format.OutputFolder);
            //    // We have to delete the any existing output folder...
            //    deleteDirs.Add(format.OutputFolder);
            //}
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

        #region CreatePostBuildSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreatePostBuildSteps()
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

        #region RunSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listSteps"></param>
        /// <returns></returns>
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
            bool buildResult = false;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listFolders"></param>
        /// <returns></returns>
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
