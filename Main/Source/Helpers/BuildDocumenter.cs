using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Steps;
using Sandcastle.References;
using Sandcastle.Conceptual;
using Sandcastle.Configurations;

namespace Sandcastle
{
    /// <summary>
    /// This is a generic or base documenter. A documenter builds or generates a
    /// documentation using contents defined by various build groups.
    /// </summary>
    /// <remarks>
    /// The generic documenter is independent of any content or project file format, 
    /// and will build contents or content locations loaded into memory.
    /// <para>
    /// The build step derives from <see cref="MarshalByRefObject"/> so that it can 
    /// be instantiated in its own app domain.
    /// </para>
    /// </remarks>
    public class BuildDocumenter : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private int                        _references;
        private int                        _conceptuals;

        private bool                       _isInitialized;
        private bool                       _isBuildSuccess;
        private BuildToc                   _helpToc;
        private BuildLoggers               _helpLogger;
        private BuildContext               _context;
        private BuildSettings              _settings;
        private BuildConfiguration         _configuration;
        private List<BuildStep>            _listSteps;
        private BuildList<BuildFormat>     _listFormats;
        private BuildKeyedList<BuildGroup> _listGroups;

        private ReferenceEngine            _referenceEngine;
        private ConceptualEngine           _conceptualEngine;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildDocumenter()
        {
            _helpToc       = new BuildToc();
            _listGroups    = new BuildKeyedList<BuildGroup>();
            _settings      = new BuildSettings();
            _configuration = new BuildConfiguration();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class with
        /// the specified list of build groups to be initially added to 
        /// this documenter.
        /// </summary>
        /// <param name="groups">
        /// A list, <see cref="IList{T}"/>, specifying the build groups 
        /// <see cref="BuildGroup"/> to be initially added to this documenter.
        /// </param>
        public BuildDocumenter(IList<BuildGroup> groups)
            : this()
        {
            if (groups != null && groups.Count != 0)
            {
                _listGroups.Add(groups);
            }
        }

        /// <summary>
        /// This allows the <see cref="BuildDocumenter"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildDocumenter"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildDocumenter()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public BuildGroup this[int index]
        {
            get
            {
                if (_listGroups != null)
                {
                    return _listGroups[index];
                }

                return null;
            }
        }

        public BuildGroup this[string groupName]
        {
            get
            {
                if (String.IsNullOrEmpty(groupName))
                {
                    return null;
                }

                if (_listGroups != null)
                {
                    return _listGroups[groupName];
                }

                return null;
            }
        }

        public bool IsInitialize
        {
            get
            {
                return _isInitialized;
            }
        }

        public BuildToc Toc
        {
            get
            {
                return _helpToc;
            }
            set
            {
                if (value != null)
                {
                    _helpToc = value;
                }
            }
        }

        public BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public BuildConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public IList<BuildGroup> Groups
        {
            get
            {
                if (_listGroups != null)
                {
                    return new ReadOnlyCollection<BuildGroup>(_listGroups);
                }

                return null;
            }
        }

        public bool Success
        {
            get { return _isBuildSuccess; }
        }

        #endregion

        #region Public Methods

        public virtual bool Load(string contentsPath)
        {
            return true;
        }

        public virtual bool Save(string contentsPath)
        {
            return true;
        }

        #region Initialize Method

        public virtual bool Initialize(BuildContext context, BuildLoggers logger)
        {
            if (_isInitialized)
            {
                return false;
            }
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(logger, "logger");

            _isInitialized = false;

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            _isBuildSuccess   = false;

            _helpLogger    = logger;
            _context       = context;

            _settings.Initialize();

            _context.Initialize(_settings, _helpLogger, _configuration);

            // 1. If there is no logger, we try creating a default logger...
            if (_helpLogger.Count == 0)
            {
                BuildLogger defLogger = _context.CreateLogger(_settings);
                if (defLogger != null)
                {
                    _helpLogger.Add(defLogger);
                }
            }

            // 2. If the logger is not initialized, we initialize it now...
            if (!_helpLogger.IsInitialize)
            {
                _helpLogger.Initialize(_settings);
            }

            IList<BuildFormat> listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                return false;
            }
            int itemCount = listFormats.Count;
            _listFormats = new BuildList<BuildFormat>();
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
                return false;
            }

            _referenceEngine  = new ReferenceEngine(_settings, _helpLogger,
                _context, _configuration);
            _conceptualEngine = new ConceptualEngine(_settings, _helpLogger,
                _context, _configuration);

            int namingApi      = 0;
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

                    ReferenceGroup refGroup = (ReferenceGroup)group;

                    namingApi = Math.Max((int)refGroup.Options.Naming, namingApi);

                    _referenceEngine.Add(refGroup);
                }
                else if (groupType == BuildGroupType.Conceptual)
                {
                    includedTopics++;
                    _conceptualEngine.Add((ConceptualGroup)group);
                }
            }

            context.ApiNamingMethod = namingApi;

            bool buildApi    = (includedApi != 0 && _references > 0 && 
                _settings.BuildReferences);
            bool buildTopics = (includedTopics != 0 && _conceptuals > 0 && 
                _settings.BuildConceptual);

            if (buildApi)
            {
                try
                {
                    if (!_referenceEngine.Initialize(_settings))
                    {
                        _isInitialized = false;

                        _helpLogger.WriteLine("Error in reference build initialization.",
                            BuildLoggerLevel.Error);
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (buildTopics)
            {
                try
                {
                    if (!_conceptualEngine.Initialize(_settings))
                    {
                        _isInitialized = false;

                        _helpLogger.WriteLine("Error in reference build initialization.",
                            BuildLoggerLevel.Error);
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            _listSteps = new List<BuildStep>(16);

            _context["$HelpTocFile"] = BuildToc.HelpToc;

            _isInitialized = true;

            return _isInitialized;
        }

        public virtual bool Initialize(BuildContext context, BuildLoggers logger,
            BuildSettings settings, BuildConfiguration configuration)
        {
            if (_isInitialized)
            {
                return false;
            }
            if (settings != null)
            {
                _settings = settings;
            }
            if (configuration != null)
            {
                _configuration = configuration;
            }

            return this.Initialize(context, logger);
        }

        #endregion

        #region Build Method

        public virtual bool Build()
        {
            _isBuildSuccess = false;

            if (_context != null)
            {
                _context.BuildResult = _isBuildSuccess;
            }

            if (_isInitialized == false)
            {
                if (_helpLogger != null)
                {
                    _helpLogger.WriteLine(
                        "The project must be initialized before building.",
                        BuildLoggerLevel.Error);
                }

                return _isBuildSuccess;
            }

            if (!_helpLogger.IsInitialize)
            {
                _helpLogger.Initialize(_settings);
            }

            if (!CreatePreBuildSteps())
            {
                return _isBuildSuccess;
            }

            _isBuildSuccess = true;

            if (_settings.BuildReferences &&
                (_referenceEngine != null && _referenceEngine.IsInitialized))
            {   
                try
                {
                    BuildStep apiSteps = null;

                    IList<ReferenceGroup> groups = _referenceEngine.Groups;
                    if (groups != null)
                    {
                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ReferenceGroup group = groups[i];
                            if (group != null && group.IsEmpty == false)
                            {
                                apiSteps = _referenceEngine.CreateSteps(group);
                                if (apiSteps != null)
                                {
                                    _listSteps.Add(apiSteps);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _isBuildSuccess = false;

                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (_isBuildSuccess == false)
            {
                return _isBuildSuccess;
            }

            if (_settings.BuildConceptual && 
                (_conceptualEngine != null && _conceptualEngine.IsInitialized))
            {   
                try
                {
                    BuildStep topicSteps = null;

                    IList<ConceptualGroup> groups = _conceptualEngine.Groups;
                    if (groups != null)
                    {
                        int itemCount = groups.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ConceptualGroup group = groups[i];
                            if (group != null && group.IsEmpty == false)
                            {
                                topicSteps = _conceptualEngine.CreateSteps(group);
                                if (topicSteps != null)
                                {
                                    _listSteps.Add(topicSteps);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _isBuildSuccess = false;

                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (_isBuildSuccess == false)
            {
                return _isBuildSuccess;
            }

            if (!CreatePostBuildSteps())
            {
                return false;
            }

            try
            {
                _isBuildSuccess = this.RunSteps(_listSteps);
            }
            catch (Exception ex)
            {
                _isBuildSuccess = false;

                _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
            }

            if (_context != null)
            {
                _context.BuildResult = _isBuildSuccess;
            }

            return _isBuildSuccess;
        }

        #endregion

        #region Uninitialize Method

        public virtual void Uninitialize()
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
                if (_helpLogger != null)
                {
                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
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
                if (_helpLogger != null)
                {
                    _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);
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

        #region Group Methods

        public void Add(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null)
            {
                _listGroups = new BuildKeyedList<BuildGroup>();
            }

            BuildGroupType groupType = group.GroupType;
            if (groupType == BuildGroupType.Reference)
            {
                _references++;
            }
            else if (groupType == BuildGroupType.Conceptual)
            {
                _conceptuals++;
            }

            _listGroups.Add(group);
        }

        public void Add(IList<BuildGroup> groups)
        {
            BuildExceptions.NotNull(groups, "groups");

            int groupCount = groups.Count;
            if (groupCount == 0)
            {
                return;
            }

            for (int i = 0; i < groupCount; i++)
            {
                this.Add(groups[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            BuildGroup group = _listGroups[index];
            if (group != null)
            {
                BuildGroupType groupType = group.GroupType;
                if (groupType == BuildGroupType.Reference)
                {
                    _references--;
                }
                else if (groupType == BuildGroupType.Conceptual)
                {
                    _conceptuals--;
                }
            }

            _listGroups.RemoveAt(index);
        }

        public void Remove(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            if (_listGroups.Remove(group))
            {
                BuildGroupType groupType = group.GroupType;
                if (groupType == BuildGroupType.Reference)
                {
                    _references--;
                }
                else if (groupType == BuildGroupType.Conceptual)
                {
                    _conceptuals--;
                }
            }
        }

        public bool Contains(BuildGroup group)
        {
            if (group == null || _listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            return _listGroups.Contains(group);
        }

        public void Clear()
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _references  = 0;
            _conceptuals = 0;

            _listGroups.Clear();
        }

        #endregion

        #endregion

        #region Protected Methods

        #region CreatePreBuildSteps Method

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreatePreBuildSteps()
        {
            if (_listSteps == null)
            {
                _listSteps = new List<BuildStep>(16);
            }
            BuildStyle buildStyle = _settings.Style;
            string sandcastleDir  = _settings.StylesDirectory; 
            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return false;
            }

            _listSteps = new List<BuildStep>();

            BuildStyleType styleType = buildStyle.StyleType;
            string helpStyle = styleType.ToString();
            if (styleType == BuildStyleType.Whidbey)
            {
                helpStyle = BuildStyleType.Vs2005.ToString();
            }

            int formatCount = _listFormats.Count;

            // Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount = listFolders.Count;

            string baseDir = _context.BaseDirectory;
            string workingDir = _context.WorkingDirectory;

            string tempText = null;

            // In the beginning... 
            // Close any current viewer of the compiled helps...
            // 1. Clean up the current build output directories
            StepDirectoryDelete deleteDirs = new StepDirectoryDelete(baseDir);
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];
                BuildStep closeViewer = format.CreateStep(_context,
                    BuildStage.CloseViewer, workingDir);
                
                if (closeViewer != null)
                {
                    _listSteps.Add(closeViewer);
                }
                string formatFolder = format.OutputFolder;
                if (!String.IsNullOrEmpty(formatFolder))
                {
                    deleteDirs.Add(formatFolder);
                }
            }

            deleteDirs.Add("Intellisense");
            _listSteps.Add(deleteDirs);

            // 2. Recreate the output directories
            StepDirectoryCreate createDir = new StepDirectoryCreate(workingDir);
            createDir.Add("Intellisense");
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

            // 3. Copy the resources files: icons, styles and scripts...
            StepDirectoryCopy copyOutput = new StepDirectoryCopy(workingDir);
            tempText = String.Format(@"Presentation\{0}\icons\", helpStyle);
            tempText = Path.Combine(sandcastleDir, tempText);
            copyOutput.Add(tempText, @"Output\icons\");

            tempText = String.Format(@"Presentation\{0}\scripts\", helpStyle);
            tempText = Path.Combine(sandcastleDir, tempText);
            copyOutput.Add(tempText, @"Output\scripts\");

            if (styleType == BuildStyleType.Whidbey)
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
            if (_listSteps == null)
            {
                return false;
            }

            string workingDir = _context.WorkingDirectory;

            // Merge the table of contents...
            StepTocMerge tocMerge = null;
            if (_helpToc == null || _helpToc.IsEmpty == true)
            {
                tocMerge = new StepTocMerge(workingDir, BuildToc.HelpToc);

                IList<ConceptualGroup> topicsGroups =
                    _conceptualEngine.Groups;

                if (topicsGroups != null && topicsGroups.Count != 0)
                {
                    int itemCount = topicsGroups.Count;

                    for (int i = 0; i < itemCount; i++)
                    {
                        ConceptualGroup group = topicsGroups[i];
                        if (group == null || group.IsEmpty || group.Exclude ||
                            group.ExcludeToc)
                        {
                            continue;
                        }

                        string topicsToc = group["$TocFile"];
                        if (!String.IsNullOrEmpty(topicsToc))
                        {
                            tocMerge.Add(topicsToc, BuildGroupType.Conceptual);
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
                        if (group == null || group.IsEmpty || group.Exclude ||
                            group.ExcludeToc)
                        {
                            continue;
                        }

                        string topicsToc = group["$TocFile"];
                        if (!String.IsNullOrEmpty(topicsToc))
                        {
                            tocMerge.Add(topicsToc, BuildGroupType.Reference);
                        }
                    }
                }
            }
            else
            {
                tocMerge = new StepTocMerge(workingDir, _helpToc);
            }

            if (tocMerge != null)
            {
                _listSteps.Add(tocMerge);
            }

            // 9. Prepare the help html files, and create the html project
            // Build and/or compile the help files...
            int formatCount = _listFormats.Count;
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                BuildStep compileHelp = format.CreateStep(_context,
                    BuildStage.Compilation, workingDir);

                if (compileHelp != null)
                {
                    _listSteps.Add(compileHelp);
                }
            }

            // In the end...
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                if (format != null && format.ViewOnBuild)
                {
                    BuildStep startViewer = format.CreateStep(_context,
                        BuildStage.StartViewer, workingDir);

                    if (startViewer != null)
                    {
                        _listSteps.Add(startViewer);
                    }
                }
            }

            //// 12. Finally, delete the intermediate "Output" directory...
            //if (_settings.CleanIntermediate)
            //{
            //    StepDirectoryDelete outputDir = new StepDirectoryDelete();
            //    outputDir.Add(workingDir);
            //    //outputDir.Add("Output");
            //    //if (_settings.BuildReferences)
            //    //{
            //    //    outputDir.Add("Comments");
            //    //    outputDir.Add("Assemblies");
            //    //}
            //    _listSteps.Add(outputDir);
            //}

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
                        if (buildStep.Initialize(_context) == false)
                        {
                            _helpLogger.WriteLine(
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
                        _helpLogger.WriteLine(
                            "An error occurred in the step = " + i.ToString(),
                            BuildLoggerLevel.Error);

                        _context.StepError(buildStep);

                        buildResult = false;
                        break;
                    }

                    _context.StepEnds(buildStep);

                    _helpLogger.WriteLine();
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
                _helpLogger.WriteLine(ex, BuildLoggerLevel.Error);

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

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs documentation tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs documentation tasks associated with freeing, releasing, or 
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
        }

        #endregion
    }
}
