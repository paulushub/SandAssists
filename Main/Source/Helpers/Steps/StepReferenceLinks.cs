using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Tools;
using Sandcastle.Contents;
using Sandcastle.Utilities;
using Sandcastle.References;
using Sandcastle.ReflectionData;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceLinks : StepProcess
    {
        #region Private Fields

        private bool _documentInternals;
        private bool _ignoreXsltWhitespace;

        private AppDomain                _reflectorDomain;
        private SandcastleTransformTool  _transformProxy;
        private SandcastleReflectionTool _reflectorProxy;

        private ReferenceGroup _currentGroup;

        private BuildList<string>     _dependencyDirs;
        private IList<ReferenceGroup> _linkGroups;
        private IList<ReferenceGroup> _buildGroups;

        private BuildLoggerVerbosity    _verbosity;
        private ReferenceEngineSettings _engineSettings;

        private List<String> _deleteLaterFiles;

        #endregion

        #region Constructors and Destructor

        public StepReferenceLinks()
        {
            this.Construction();
        }

        public StepReferenceLinks(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.Construction();
        }

        public StepReferenceLinks(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.Construction();
        }

        private void Construction()
        {
            _verbosity            = BuildLoggerVerbosity.None;
            _ignoreXsltWhitespace = true;

            this.LogTitle         = "Preparing Reflection Links";
            this.CopyrightNotice  = 2;

            _deleteLaterFiles     = new List<string>();
        }

        #endregion

        #region Public Properties

        public IList<ReferenceGroup> BuildGroups
        {
            get
            {
                return _buildGroups;
            }
            set
            {
                _buildGroups = value;
            }
        }

        public IList<ReferenceGroup> LinkGroups
        {
            get
            {
                return _linkGroups;
            }
            set
            {
                _linkGroups = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            if (_buildGroups == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            _engineSettings = context.Settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                return false;
            }

            string sandcastleDir = context.StylesDirectory;

            bool buildResult = false;

            // Keep all possibly directory for dependent assembly locations,
            // for use later in resolving link assembly dependencies that may
            // dynamic loaders like NuGet...            
            _dependencyDirs  = new BuildList<string>();

            try
            {
                if (context.IsDirectSandcastle)
                {
                    _reflectorDomain = AppDomain.CreateDomain(
                        "Sandcastle.BuildLinksDomain");

                    _reflectorProxy =
                        (SandcastleReflectionTool)_reflectorDomain.CreateInstanceAndUnwrap(
                        typeof(SandcastleReflectionTool).Assembly.FullName,
                        typeof(SandcastleReflectionTool).FullName);

                    _transformProxy =
                        (SandcastleTransformTool)_reflectorDomain.CreateInstanceAndUnwrap(
                        typeof(SandcastleTransformTool).Assembly.FullName,
                        typeof(SandcastleTransformTool).FullName);
                }

                for (int i = 0; i < _buildGroups.Count; i++)
                {
                    ReferenceGroup buildGroup = _buildGroups[i];

                    ReferenceGroupContext groupContext =
                        context.GroupContexts[buildGroup.Id] as ReferenceGroupContext;

                    Debug.Assert(groupContext != null, "The group context is required.");

                    _currentGroup = buildGroup;

                    // Make sure Silverlight and Expression SDK reflections are
                    // created, if installed...
                    buildResult = this.OnCreateFrameworkReflection(context, 
                        groupContext, sandcastleDir);
                    if (!buildResult)
                    {
                        return false;
                    }

                    string tempText = groupContext.AssemblyDir;
                    if (!String.IsNullOrEmpty(tempText) && Directory.Exists(tempText))
                    {
                        _dependencyDirs.Add(tempText);
                    }
                    tempText = groupContext.DependencyDir;
                    if (!String.IsNullOrEmpty(tempText) && Directory.Exists(tempText))
                    {
                        _dependencyDirs.Add(tempText);
                    }
                }

                _currentGroup = null;

                // Create the reflections for the link sources, if any...
                buildResult = this.OnCreateLinksReflection(context, sandcastleDir);
                if (!buildResult)
                {
                    return false;
                }

                return buildResult;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex);

                return false;
            }
            finally
            {
                // Delete all file marked for later deletion...
                if (_deleteLaterFiles != null && _deleteLaterFiles.Count != 0)
                {
                    List<String> undeletedFiles = new List<string>();
                    foreach (string sourceFile in _deleteLaterFiles)
                    {
                        try
                        {
                            if (File.Exists(sourceFile)) 
                                File.Delete(sourceFile);
                        }
                        catch
                        {
                            undeletedFiles.Add(sourceFile);
                        }
                    }

                    if (undeletedFiles.Count != 0)
                    {
                        _deleteLaterFiles = undeletedFiles;
                    }
                }

                if (context.IsDirectSandcastle)
                {
                    if (_reflectorDomain != null)
                    {
                        AppDomain.Unload(_reflectorDomain);
                        _reflectorDomain = null;
                    }
                }
            }
        }

        #endregion

        #region OnDataReceived Method

        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger == null || _verbosity == BuildLoggerVerbosity.Quiet)
            {
                return;
            }
            _messageCount++;

            if (_messageCount <= _copyright)
            {
                return;
            }

            string textData = e.Data;
            if (String.IsNullOrEmpty(textData))
            {
                if (!_ignoreWhitespace)
                {
                    _logger.WriteLine(String.Empty, BuildLoggerLevel.None);
                }
                return;
            }

            int findPos = textData.IndexOf(':');
            if (findPos <= 0)
            {
                // 1. Check for no options/arguments...
                if (textData.StartsWith("MRefBuilder", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    return;
                }
                if (textData.StartsWith("VersionBuilder", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    return;
                }

                // 2. Check for missing or extra assembly directories...
                if (textData.StartsWith("Specify", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    return;
                }

                // 3. Check for valid target platform...
                if (textData.StartsWith("Unknown target", StringComparison.OrdinalIgnoreCase))
                {
                    // For the MRefBuilder
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    return;
                }
                if (textData.StartsWith("No non-option", StringComparison.OrdinalIgnoreCase))
                {
                    // For the VersionBuilder...
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    return;
                }

                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                return;
            }

            string levelText = textData.Substring(0, findPos);
            string messageText = textData.Substring(findPos + 1).Trim();
            if (String.Equals(levelText, "Info"))
            {
                if (_verbosity != BuildLoggerVerbosity.Minimal)
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Info);
                }
            }
            else if (String.Equals(levelText, "Warn"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Warn);
            }
            else if (String.Equals(levelText, "Error"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Error);
            }
            else
            {
                // Check for invalid options...
                if (String.Equals(levelText, "?",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else if (String.Equals(levelText, "config",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else if (String.Equals(levelText, "dep",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else if (String.Equals(levelText, "internal",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else if (String.Equals(levelText, "rip",
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Only for the VersionBuilder...
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                }
                else
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Info);
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region OnCreateLinksReflection Method

        private bool OnCreateLinksReflection(BuildContext context,
            string sandcastleDir)
        {
            bool isSuccessful = true;

            IList<ReferenceLinkSource> linkSources = context.GetValue(
                "$ReferenceLinkSources") as IList<ReferenceLinkSource>;
            if (linkSources == null || linkSources.Count == 0)
            {
                // There is no link source, a valid situation...
                return isSuccessful;
            }

            BuildLogger logger = context.Logger;
            if (_linkGroups != null && _linkGroups.Count != 0)
            {
                for (int i = 0; i < _linkGroups.Count; i++)
                {
                    // Set the target group for processing...
                    _currentGroup = _linkGroups[i];

                    this.OnCreateLinkDependencies(context);
                }
            }

            string linkSourcesDir = Path.Combine(context.WorkingDirectory,
                ReferenceGroupContext.LinkSourcesFolder);

            if (!Directory.Exists(linkSourcesDir))
            {
                Directory.CreateDirectory(linkSourcesDir);
            }

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Reflection Data for Links.", 
                    BuildLoggerLevel.Info);
            }

            List<string> linkDirs = new List<string>();
            context.SetValue("$ReferenceLinkDirectories", linkDirs);

            string configFile = null;
            int itemCount = linkSources.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceLinkSource linkSource = linkSources[i];
                string indexText = String.Empty;
                if (itemCount > 1)
                {
                    indexText = (i + 1).ToString();
                }
                string outputDir = Path.Combine(linkSourcesDir,
                    "Link" + indexText);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                BuildFramework referencedFramework =
                    BuildFrameworks.GetFramework(linkSource.FrameworkType);

                // We write configuration file name like LinkSourceA.config,
                // LinkSourceB.config, LinkSourceC.config etc
                // "65" is the integer value of the letter "A".
                configFile = String.Format("LinkSource{0}.config", 
                    Convert.ToChar(65 + i));

                isSuccessful = this.OnCreateLinksReflection(context, 
                    sandcastleDir, outputDir, linkSource,
                    configFile);

                if (isSuccessful)
                {
                    linkDirs.Add(outputDir);
                }
                else
                {
                    break;
                }
            }
            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Reflection Data for Links.",
                    BuildLoggerLevel.Info);
            }

            return isSuccessful;
        }

        private bool OnCreateLinksReflection(BuildContext context,
            string sandcastleDir, string outputDir, 
            ReferenceLinkSource linkSource, string configFile)
        {
            BuildLogger logger = context.Logger;

            BuildList<string> assemblyFiles = new BuildList<string>();
            IEnumerable<ReferenceItem> items = linkSource.Items;

            string dependencyDir = null;
            ReferenceVisibilityConfiguration visibility =
                _engineSettings.Visibility;
            Debug.Assert(visibility != null);

            _documentInternals = (visibility != null && visibility.IncludeInternalsMembers);

            ReferenceGroupContext groupContext =
                context.GroupContexts[linkSource.SourceId] as ReferenceGroupContext;
            if (groupContext != null)
            {
                dependencyDir = groupContext.DependencyDir;
            }

            // For search-redirection of assembly if different versions...
            List<string> assemblySources = new List<string>(); 
            foreach (ReferenceItem item in items)
            {
                BuildFilePath assemblyPath = item.Assembly;
                if (assemblyPath == null || !assemblyPath.Exists)
                {
                    continue;
                }

                assemblyFiles.Add(assemblyPath.Path);
                assemblySources.Add(assemblyPath.Directory.FullName);
            }

            if (assemblyFiles.Count == 0)
            {         
                if (logger != null)
                {
                    string title = linkSource.Title;
                    if (title == null)
                    {
                        title = String.Empty;
                    }
                    logger.WriteLine(String.Format(
                        "The reference link source '{0}' is invalid.", title), 
                        BuildLoggerLevel.Error);
                }

                return false;
            }
            StringBuilder textBuilder = new StringBuilder();

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            if (String.IsNullOrEmpty(configFile) || configFile.Length < 8)
            {
                configFile = "LinkSource.config";
            }

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(this.WorkingDirectory, configFile);
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.BindingSources = assemblySources;

                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_currentGroup, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            ReferenceNamingMethod namingMethod = _engineSettings.Naming;
            string prodPath     = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir = context.SandcastleToolsDirectory;
            string refInfoFile  = String.Empty;
            bool buildResult    = false;

            // The list of transformations to apply...
            string[] transformFiles = new string[2];
            transformFiles[0] = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            if (namingMethod == ReferenceNamingMethod.Guid)
            {
                transformFiles[1] = Path.Combine(prodPath, "AddGuidFilenames.xsl");
            }
            else if (namingMethod == ReferenceNamingMethod.MemberName)
            {
                transformFiles[1] = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
            }
            else
            {
                transformFiles[1] = Path.Combine(prodPath, "AddGuidFilenames.xsl");
            }

            // The arguments to the transformations...
            Dictionary<string, string> transformArguments = 
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            transformArguments.Add("IncludeAllMembersTopic", "true");
            transformArguments.Add("IncludeInheritedOverloadTopics", "true");

            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);

                refInfoFile = Path.Combine(outputDir, fileName + ".org");
                string application = null;

                string dependencyFiles = String.Format("\"{0}\\*.*\"", dependencyDir);

                // For the direct Sandcastle library option...
                if (context.IsDirectSandcastle && _reflectorProxy != null)
                {
                    dependencyFiles = dependencyFiles.Replace("\"", String.Empty);

                    _reflectorProxy.ReflectionFile    = refInfoFile;
                    _reflectorProxy.ConfigurationFile = finalRefBuilder;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles     = new string[] { assemblyFile };
                    _reflectorProxy.DependencyFiles   = new string[] { dependencyFiles };

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {  
                    textBuilder.Length = 0;

                    // Create the reflection and the manifest
                    // 1. Call MRefBuilder to generate the reflection...
                    // MRefBuilder Assembly.dll 
                    // /out:reflection.org /config:MRefBuilder.config 
                    //   /internal-
                    application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                    textBuilder.AppendFormat("\"{0}\"", assemblyFile);
                    if (!String.IsNullOrEmpty(dependencyDir) &&
                        Directory.Exists(dependencyDir) && !DirectoryUtils.IsDirectoryEmpty(dependencyDir))
                    {
                        textBuilder.Append(" /dep:" + dependencyFiles);
                    }

                    textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                        refInfoFile, finalRefBuilder);
                    if (_documentInternals)
                    {
                        textBuilder.Append(" /internal+");
                    }
                    else
                    {
                        textBuilder.Append(" /internal-");
                    }

                    string arguments = textBuilder.ToString();

                    buildResult = base.Run(logger, outputDir,
                        application, arguments);
                }

                if (!buildResult)
                {
                    break;
                }

                string sourceFile      = refInfoFile;
                string destinationFile = Path.Combine(outputDir, fileName + ".xml");

                if (context.IsDirectSandcastle && _transformProxy != null)
                {   
                    if (logger != null)
                    {
                        logger.WriteLine("Started Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    _transformProxy.InputFile        = sourceFile;
                    _transformProxy.OutputFile       = destinationFile;
                    _transformProxy.Arguments        = transformArguments;
                    _transformProxy.TransformFiles   = transformFiles;

                    buildResult = _transformProxy.Run(context);

                    if (logger != null)
                    {
                        logger.WriteLine("Completed Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }
                else
                {
                    textBuilder.Length = 0;

                    // XslTransform.exe       
                    // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                    //    reflection.org 
                    //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                    //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                    //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                    application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[0]);
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[1]);
                    textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                    textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                    textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
                   
                    StepXslTransform modelTransform = new StepXslTransform(
                        outputDir, application, textBuilder.ToString());
                    modelTransform.LogTitle        = String.Empty;
                    modelTransform.Message         = "Xsl Transformation - Applying model and adding filenames.";
                    modelTransform.LogTimeSpan     = false;
                    modelTransform.CopyrightNotice = 2;

                    modelTransform.Initialize(context);
                    if (modelTransform.IsInitialized)
                    {
                        buildResult = modelTransform.Execute();

                        modelTransform.Uninitialize();
                        if (!buildResult)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // There is an error, do not continue...
                        break;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch
                    {
                        _deleteLaterFiles.Add(sourceFile);
                    }
                }
            }

            return buildResult;
        }

        #endregion

        #region OnCreateLinkDependencies Method

        private bool OnCreateLinkDependencies(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            ReferenceGroupContext groupContext =
                context.GroupContexts[_currentGroup.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceContent content = _currentGroup.Content;
            if (content == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: There is no content associated with the reference group.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildFrameworkType frameworkType = content.FrameworkType;
            if (frameworkType == BuildFrameworkType.Null ||
                frameworkType == BuildFrameworkType.None)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: There is no valid framework type specified for this reference group.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildFramework framework = BuildFrameworks.GetFramework(frameworkType);
            if (framework == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: The specified framework type for this reference group is not installed.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            // We use the link directory as the working directory...
            string workingDir = Path.Combine(context.WorkingDirectory,
                ReferenceGroupContext.LinkSourcesFolder);

            groupContext.Framework = framework;

            string commentDir  = groupContext.CommentFolder;
            string assemblyDir = groupContext.AssemblyFolder;
            if (String.IsNullOrEmpty(commentDir))
            {
                commentDir = "Comments";
            }
            if (!Path.IsPathRooted(commentDir))
            {
                commentDir = Path.Combine(workingDir, commentDir);
            }
            if (!Directory.Exists(commentDir))
            {
                Directory.CreateDirectory(commentDir);
            }

            if (String.IsNullOrEmpty(assemblyDir))
            {
                assemblyDir = "Assemblies";
            }
            if (!Path.IsPathRooted(assemblyDir))
            {
                assemblyDir = Path.Combine(workingDir, assemblyDir);
            }
            if (!Directory.Exists(assemblyDir))
            {
                Directory.CreateDirectory(assemblyDir);
            }

            string dependencyDir = groupContext.DependencyFolder;
            if (String.IsNullOrEmpty(dependencyDir))
            {
                dependencyDir = "Dependencies";
            }
            if (!Path.IsPathRooted(dependencyDir))
            {
                dependencyDir = Path.Combine(workingDir, dependencyDir);
            }
            if (!Directory.Exists(dependencyDir))
            {
                Directory.CreateDirectory(dependencyDir);
            }

            groupContext.CommentDir    = commentDir;
            groupContext.AssemblyDir   = assemblyDir;
            groupContext.DependencyDir = dependencyDir;

            // 1. Copy the dependencies to the expected directory...
            ReferenceProjectVisitor dependencyResolver =
                new ReferenceProjectVisitor();

            if (_dependencyDirs != null && _dependencyDirs.Count != 0)
            {
                IList<string> bindingSource = groupContext.BindingSources;
                if (bindingSource == null)
                {
                    groupContext.BindingSources = _dependencyDirs;
                }
                else
                {
                    foreach (string dir in _dependencyDirs)
                    {
                        bindingSource.Add(dir);
                    }                           
                }
            }
            dependencyResolver.Initialize(context);
            dependencyResolver.Visit(_currentGroup);
            dependencyResolver.Uninitialize();

            return true;
        }

        #endregion

        #region OnCreateFrameworkReflection Method

        private bool OnCreateFrameworkReflection(BuildContext context,
            ReferenceGroupContext groupContext, string sandcastleDir)
        {
            bool buildResult = true;

            BuildFramework framework = groupContext.Framework;
            BuildFrameworkKind kind  = framework.FrameworkType.Kind;

            BuildFramework referencedFramework = null;

            // For these frameworks, we use the latest versions since that is
            // most likely available on the MSDN...
            if (kind == BuildFrameworkKind.Silverlight)
            {
                Version latestVersion = BuildFrameworks.LatestSilverlightVersion;
                if (latestVersion != null)
                {
                    BuildFramework silverFramework = BuildFrameworks.GetFramework(
                        latestVersion.Major, BuildFrameworkKind.Silverlight);

                    referencedFramework = silverFramework;

                    buildResult = this.OnCreateSilverlightReflection(context,
                        silverFramework, framework, sandcastleDir);
                } 
            }
            else if (kind == BuildFrameworkKind.Portable)
            {
                Version latestVersion = BuildFrameworks.LatestPortableVersion;
                if (latestVersion != null)
                {
                    BuildFramework portableFramework = BuildFrameworks.GetFramework(
                        latestVersion.Major, BuildFrameworkKind.Portable);

                    referencedFramework = portableFramework;

                    buildResult = this.OnCreatePortableReflection(context,
                        portableFramework, framework, sandcastleDir);
                }
            }
            else if (kind == BuildFrameworkKind.ScriptSharp)
            {
                Version latestVersion = BuildFrameworks.LatestScriptSharpVersion;
                if (latestVersion != null)
                {
                    BuildFramework scriptSharpFramework = BuildFrameworks.GetFramework(
                        latestVersion.Major, BuildFrameworkKind.ScriptSharp);

                    referencedFramework = scriptSharpFramework;

                    buildResult = this.OnCreateScriptSharpReflection(context,
                        scriptSharpFramework, framework, sandcastleDir);
                }
            }

            // If the build failed, do not continue...
            if (!buildResult)
            {
                return buildResult;
            }

            // Clearly, the Portable Class Libraries, Compact Framework and Compact 
            // Framework projects will not support the Blend SDK...
            if (kind == BuildFrameworkKind.Portable ||
                kind == BuildFrameworkKind.Compact  ||
                kind == BuildFrameworkKind.ScriptSharp)
            {
                return buildResult;
            }
            string tempText = context["$EmbeddedScriptSharp"];
            if (!String.IsNullOrEmpty(tempText) && Convert.ToBoolean(tempText))
            {
                tempText = groupContext["$IsEmbeddedGroup"];
                if (!String.IsNullOrEmpty(tempText) && Convert.ToBoolean(tempText))
                {
                    return buildResult;
                }
            } 

            string blendDir      = null;
            Version version      = framework.Version;
            Version blendVersion = null;
            string programFiles  = PathUtils.ProgramFiles32;

            if (version.Major >= 2)
            {
                BuildSpecialSdk latestBlendSdk = null;
                if (kind == BuildFrameworkKind.Silverlight)
                {
                    latestBlendSdk = BuildSpecialSdks.LatestBlendSilverlightSdk;

                    if (latestBlendSdk != null) 
                    {
                        blendVersion = latestBlendSdk.Version;
                    }
                    if (blendVersion.Major == 5 && blendVersion.Minor > 0)
                    {
                    }
                }
                else
                {
                    latestBlendSdk = BuildSpecialSdks.LatestBlendWpfSdk;
                }
                if (latestBlendSdk != null)
                {
                    blendDir = latestBlendSdk.AssemblyDir;
                    if (Directory.Exists(blendDir))
                    {
                        blendVersion = latestBlendSdk.Version;

                        // Silverlight 5.1+ requires special treatment...
                        if (kind == BuildFrameworkKind.Silverlight)
                        {
                            if (blendVersion.Major == 5 && blendVersion.Minor > 0)
                            {
                                blendVersion = new Version(5, 0, 0, 0);
                            }
                        }
                    }
                }
            }

            if (blendVersion == null || String.IsNullOrEmpty(blendDir))
            {
                return buildResult;
            }

            buildResult = this.OnCreateBlendReflection(context, framework, 
                referencedFramework, blendVersion, blendDir, sandcastleDir);

            return buildResult;
        }

        #endregion

        #region OnCreateBlendReflection Method

        private bool OnCreateBlendReflection(BuildContext context,
            BuildFramework framework, BuildFramework referencedFramework, 
            Version blendVersion, string blendDir, string sandcastleDir)
        {
            string reflectionDir = context.ReflectionDataDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            bool isSilverlight = false;
            string blendDataDir = null;
            // Blend SDK is separately available for Silverlight and WPF...
            if (framework.FrameworkType.Kind == BuildFrameworkKind.Silverlight)
            {
                blendDataDir = Path.Combine(reflectionDir,
                    @"Blend\Silverlight\v" + blendVersion.ToString(2));

                isSilverlight = true;
            }
            else
            {
                blendDataDir = Path.Combine(reflectionDir,
                    @"Blend\Wpf\v" + blendVersion.ToString(2));
            }

            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(blendDataDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDataDir))
            {
                context.GroupContexts[_currentGroup.Id]["$BlendDataDir"] = blendDataDir;

                // Build the various persistent databases, if not available...
                return OnCreateBlendDatabase(context, blendVersion,
                    isSilverlight, blendDataDir);
            }

            string[] assemblies = Directory.GetFiles(blendDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }
            List<string> assemblyFiles = new List<string>(assemblies);

            // For search-redirection of assembly if different versions...
            List<string> assemblySources = new List<string>();
            assemblySources.Add(blendDir);
            if (referencedFramework != null && referencedFramework != framework)
            {
                assemblySources.Add(referencedFramework.AssemblyDir);
            }
            assemblySources.Add(framework.AssemblyDir);

            if (framework.FrameworkType.Kind == BuildFrameworkKind.Silverlight)
            {
                string programFiles = PathUtils.ProgramFiles32;

                Version version = framework.Version;

                string searchDir = Path.Combine(programFiles,
                    @"Reference Assemblies\Microsoft\Framework\Silverlight\v" + version.ToString(2));
                if (Directory.Exists(searchDir))
                {
                    assemblySources.Add(searchDir);
                }

                searchDir = Path.Combine(programFiles,
                    @"Microsoft SDKs\Silverlight\v" + version.ToString(2));
                if (Directory.Exists(searchDir))
                {
                    assemblySources.Add(searchDir);

                    string tempDir = String.Copy(searchDir);
                    searchDir = Path.Combine(tempDir, @"Libraries\Client");
                    if (Directory.Exists(searchDir))
                    {
                        assemblySources.Add(searchDir);
                    }
                    searchDir = Path.Combine(tempDir, @"Libraries\Server");
                    if (Directory.Exists(searchDir))
                    {
                        assemblySources.Add(searchDir);
                    }
                }
            }
            else
            {
                // Earlier versions of the Blend SDK depends on WPF Toolkit
                if (blendVersion.Major == 3)
                {
                    string programFiles = PathUtils.ProgramFiles32;
                    string searchDir = Path.Combine(programFiles, "WPF Toolkit");
                    if (Directory.Exists(searchDir))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(searchDir);
                        DirectoryInfo[] versionDirs = dirInfo.GetDirectories();
                        if (versionDirs != null && versionDirs.Length != 0)
                        {
                            for (int i = 0; i < versionDirs.Length; i++)
                            {
                                // The valid directories start with 'v'...
                                dirInfo = versionDirs[i];
                                string versionFolder = dirInfo.Name;
                                if (versionFolder.StartsWith("v",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    assemblySources.Add(dirInfo.FullName);
                                }
                            }
                        }
                    }    
                }
            }

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger = context.Logger;

            Directory.CreateDirectory(blendDataDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder = Path.Combine(this.WorkingDirectory, "ExpressionBlend.config");
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.BindingSources = assemblySources;

                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_currentGroup, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            string prodPath     = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir = context.SandcastleToolsDirectory;
            string refInfoFile  = String.Empty;
            bool buildResult    = false;

            // The list of transformations to apply...
            string[] transformFiles = new string[2];
            transformFiles[0] = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            transformFiles[1] = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");

            // The arguments to the transformations...
            Dictionary<string, string> transformArguments =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            transformArguments.Add("IncludeAllMembersTopic", "true");
            transformArguments.Add("IncludeInheritedOverloadTopics", "true");

            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);
                refInfoFile = Path.Combine(blendDataDir, fileName + ".org");

                string application = null;

                // For the direct Sandcastle library option...
                if (context.IsDirectSandcastle && _reflectorProxy != null)
                {
                    _reflectorProxy.ReflectionFile = refInfoFile;
                    _reflectorProxy.ConfigurationFile = finalRefBuilder;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles = new string[] { assemblyFile };
                    _reflectorProxy.DependencyFiles = null;

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {   
                    textBuilder.Length = 0;
                    // Create the reflection and the manifest
                    // 1. Call MRefBuilder to generate the reflection...
                    // MRefBuilder Assembly.dll 
                    // /out:reflection.org /config:MRefBuilder.config 
                    //   /internal-
                    application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                    textBuilder.AppendFormat("\"{0}\"", assemblyFile);

                    textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                        refInfoFile, finalRefBuilder);

                    string arguments = textBuilder.ToString();

                    buildResult = base.Run(logger, blendDataDir,
                        application, arguments);
                }

                if (!buildResult)
                {
                    break;
                }

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(blendDataDir, 
                    fileName + ".xml");

                if (context.IsDirectSandcastle && _transformProxy != null)
                {   
                    if (logger != null)
                    {
                        logger.WriteLine("Started Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    _transformProxy.InputFile        = sourceFile;
                    _transformProxy.OutputFile       = destinationFile;
                    _transformProxy.Arguments        = transformArguments;
                    _transformProxy.TransformFiles   = transformFiles;

                    buildResult = _transformProxy.Run(context);

                    if (logger != null)
                    {
                        logger.WriteLine("Completed Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }
                else
                {   
                    textBuilder.Length = 0;

                    // XslTransform.exe       
                    // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                    //    reflection.org 
                    //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                    //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                    //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                    application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[0]);
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[1]);
                    textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                    textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                    textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
                   
                    StepXslTransform modelTransform = new StepXslTransform(
                        blendDataDir, application, textBuilder.ToString());
                    modelTransform.LogTitle        = String.Empty;
                    modelTransform.Message         = "Xsl Transformation - Applying model and adding filenames.";
                    modelTransform.CopyrightNotice = 2;
                    modelTransform.LogTimeSpan     = false;

                    modelTransform.Initialize(context);
                    if (modelTransform.IsInitialized)
                    {
                        buildResult = modelTransform.Execute();

                        modelTransform.Uninitialize();
                        if (!buildResult)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }  
                }

                if (File.Exists(sourceFile))
                {
                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch
                    {
                        _deleteLaterFiles.Add(sourceFile);
                    }
                }
            }

            if (buildResult)
            {
                context.GroupContexts[_currentGroup.Id]["$BlendDataDir"] = blendDataDir;
            }
            else
            {
                return false;
            }

            // Build the various persistent databases...
            buildResult = OnCreateBlendDatabase(context, blendVersion, 
                isSilverlight, blendDataDir);

            return buildResult;
        }

        private bool OnCreateBlendDatabase(BuildContext context,
            Version blendVersion, bool isSilverlight, string blendDataDir)
        {
            BuildLogger logger = context.Logger;

            bool buildResult = false;

            // Building the target database...
            DataSource dataSource = new DataSource(true, true, true, isSilverlight,
                DataSourceType.Blend);
            dataSource.Version = blendVersion;

            dataSource.SetDirectories(blendDataDir,
                context.TargetDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Targets Database: Blend SDK",
                        BuildLoggerLevel.Info);
                }      
                using (DatabaseTargetTextBuilder targetBuilder =
                    new DatabaseTargetTextBuilder(dataSource))
                {
                    if (targetBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Blend SDK successful.",
                                BuildLoggerLevel.Info);
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Blend SDK failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Targets Database: Blend SDK",
                        BuildLoggerLevel.Info);
                }
            }

            if (!buildResult)
            {
                return false;
            }

            // Building the indexed database...
            dataSource = new DataSource(true, true, true, isSilverlight,
                DataSourceType.Blend);
            dataSource.Version = blendVersion;

            dataSource.SetDirectories(blendDataDir,
                context.IndexedDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Indexed Database: Blend SDK",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseIndexedBuilder indexedBuilder =
                    new DatabaseIndexedBuilder(dataSource))
                {
                    if (indexedBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Blend SDK successful.",
                                BuildLoggerLevel.Info);
                        }

                        buildResult = true;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Blend SDK failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Indexed Database: Blend SDK",
                        BuildLoggerLevel.Info);
                }
            }

            return buildResult;
        }

        #endregion

        #region OnCreateSilverlightReflection Method

        private bool OnCreateSilverlightReflection(BuildContext context,
            BuildFramework framework, BuildFramework referencedFramework,
            string sandcastleDir)
        {
            if (framework.FrameworkType.Kind != BuildFrameworkKind.Silverlight)
            {
                return true;
            }
            string reflectionDir = context.ReflectionDataDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            Version version = framework.Version;

            string silverlightDataDir = Path.Combine(reflectionDir,
                @"Silverlight\v" + version.ToString(2));

            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(silverlightDataDir) &&
                !DirectoryUtils.IsDirectoryEmpty(silverlightDataDir))
            {
                context.GroupContexts[_currentGroup.Id]["$SilverlightDataDir"] = silverlightDataDir;

                // Build the various persistent database, if not available...
                return OnCreateSilverlightDatabase(context,
                    version, silverlightDataDir);
            }

            string silverlightDir = framework.AssemblyDir;
            string[] assemblies = Directory.GetFiles(silverlightDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }

            string programFiles = PathUtils.ProgramFiles32;
            List<string> assemblyFiles = new List<string>(assemblies);

            // For search-redirection of assembly if different versions...
            List<string> assemblySources = new List<string>();
            assemblySources.Add(silverlightDir);

            string otherDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Silverlight\v" + version.ToString(2));
            if (Directory.Exists(otherDir))
            {
                otherDir = Path.Combine(otherDir, @"Libraries\Client");
            }
            else
            {
                if (version.Major == 5 && version.Minor > 0)
                {
                    // For Silverlight 5.1, the assemblies are in different places...
                    otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Silverlight\v5.0");
                    if (Directory.Exists(otherDir))
                    {
                        otherDir = Path.Combine(otherDir, @"Libraries\Client");
                    }
                }
            }

            if (Directory.Exists(otherDir))
            {
                assemblies = Directory.GetFiles(otherDir,
                    "*.dll", SearchOption.TopDirectoryOnly);
                if (assemblies != null && assemblies.Length != 0)
                {
                    assemblyFiles.AddRange(assemblies);

                    assemblySources.Add(otherDir);
                }
            }
            else
            {
                otherDir = null;
            }
            if (referencedFramework != null && referencedFramework != framework)
            {
                assemblySources.Add(referencedFramework.AssemblyDir);
            }

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger        = context.Logger;

            Directory.CreateDirectory(silverlightDataDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(this.WorkingDirectory, "Silverlight.config");
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.BindingSources = assemblySources;

                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_currentGroup, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            HashSet<string> excludeSet = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            excludeSet.Add("agcore");
            excludeSet.Add("coreclr");
            excludeSet.Add("dbgshim");
            excludeSet.Add("mscordaccore");
            excludeSet.Add("mscordbi");
            excludeSet.Add("mscorrc");
            excludeSet.Add("npctrl");
            excludeSet.Add("npctrlui");
            excludeSet.Add("Silverlight.ConfigurationUI");
            excludeSet.Add("SLMSPRBootstrap");
            excludeSet.Add("sos");

            string prodPath       = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir   = context.SandcastleToolsDirectory;
            string refInfoFile    = String.Empty;
            bool buildResult = false;

            // The list of transformations to apply...
            string[] transformFiles = new string[2];
            transformFiles[0] = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            transformFiles[1] = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");

            // The arguments to the transformations...
            Dictionary<string, string> transformArguments =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            transformArguments.Add("IncludeAllMembersTopic", "true");
            transformArguments.Add("IncludeInheritedOverloadTopics", "true");

            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);

                if (fileName.EndsWith(".ni", StringComparison.OrdinalIgnoreCase) || 
                    fileName.IndexOf("debug", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    excludeSet.Contains(fileName))
                {
                    continue;
                }

                excludeSet.Add(fileName);

                refInfoFile = Path.Combine(silverlightDataDir, fileName + ".org");
                string application = null;

                // For the direct Sandcastle library option...
                if (context.IsDirectSandcastle && _reflectorProxy != null)
                {
                    string dependencyFiles = String.Format("{0}\\*.dll", silverlightDir);

                    _reflectorProxy.ReflectionFile    = refInfoFile;
                    _reflectorProxy.ConfigurationFile = finalRefBuilder;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles     = new string[] { assemblyFile };
                    if (!String.IsNullOrEmpty(otherDir))
                    {                           
                        _reflectorProxy.DependencyFiles = new string[] 
                        { 
                            dependencyFiles, String.Format("{0}\\*.dll", otherDir) 
                        };
                    }
                    else
                    {
                        _reflectorProxy.DependencyFiles = new string[] { dependencyFiles };
                    }

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {   
                    textBuilder.Length = 0;
                    // Create the reflection and the manifest
                    // 1. Call MRefBuilder to generate the reflection...
                    // MRefBuilder Assembly.dll 
                    // /out:reflection.org /config:MRefBuilder.config 
                    //   /internal-
                    application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                    textBuilder.AppendFormat("\"{0}\"", assemblyFile);
                    textBuilder.AppendFormat(" /dep:\"{0}\\*.dll\"", silverlightDir);
                    if (!String.IsNullOrEmpty(otherDir))
                    {
                        textBuilder.AppendFormat(" /dep:\"{0}\\*.dll\"", otherDir);
                    }

                    textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                        refInfoFile, finalRefBuilder);

                    string arguments = textBuilder.ToString();

                    buildResult = base.Run(logger, silverlightDataDir,
                        application, arguments);
                }

                if (!buildResult)
                {
                    break;
                }

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(silverlightDataDir, 
                    fileName + ".xml");

                if (context.IsDirectSandcastle && _transformProxy != null)
                {   
                    if (logger != null)
                    {
                        logger.WriteLine("Started Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    _transformProxy.InputFile        = sourceFile;
                    _transformProxy.OutputFile       = destinationFile;
                    _transformProxy.Arguments        = transformArguments;
                    _transformProxy.TransformFiles   = transformFiles;

                    buildResult = _transformProxy.Run(context);

                    if (logger != null)
                    {
                        logger.WriteLine("Completed Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }
                else
                {   
                    textBuilder.Length = 0;

                    // XslTransform.exe       
                    // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                    //    reflection.org 
                    //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                    //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                    //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                    application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[0]);
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[1]);
                    textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                    textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                    textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
                   
                    StepXslTransform modelTransform = new StepXslTransform(
                        silverlightDataDir, application, textBuilder.ToString());
                    modelTransform.LogTitle        = String.Empty;
                    modelTransform.Message         = "Xsl Transformation - Applying model and adding filenames.";
                    modelTransform.CopyrightNotice = 2;
                    modelTransform.LogTimeSpan     = false;

                    modelTransform.Initialize(context);
                    if (modelTransform.IsInitialized)
                    {
                        buildResult = modelTransform.Execute();

                        modelTransform.Uninitialize();
                        if (!buildResult)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    try
                    {
	                    File.Delete(sourceFile);
                    }
                    catch
                    {   
                        _deleteLaterFiles.Add(sourceFile);
                    }
                }
            }

            if (buildResult)
            {
                context.GroupContexts[_currentGroup.Id]["$SilverlightDataDir"] = silverlightDataDir;
            }
            else
            {
                return false;
            }

            // Build the various persistent database...
            buildResult = OnCreateSilverlightDatabase(context, 
                version, silverlightDataDir);

            return buildResult;
        }

        private bool OnCreateSilverlightDatabase(BuildContext context,
            Version version, string silverlightDataDir)
        {
            bool buildResult   = false;
            BuildLogger logger = context.Logger;

            DataSource dataSource = new DataSource(true, true, true, true,
                DataSourceType.Silverlight);
            dataSource.Version = version;

            dataSource.SetDirectories(silverlightDataDir,
                context.TargetDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Targets Database: Silverlight",
                        BuildLoggerLevel.Info);
                }
                // Building the targets database...
                using (DatabaseTargetTextBuilder targetBuilder = 
                    new DatabaseTargetTextBuilder(dataSource))
                {
                    if (targetBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Silverlight successful.",
                                BuildLoggerLevel.Info);
                        }

                        buildResult = true;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Silverlight failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Targets Database: Silverlight",
                        BuildLoggerLevel.Info);
                }
            }

            if (!buildResult)
            {
                return false;
            }

            dataSource = new DataSource(true, true, true, true,
                DataSourceType.Silverlight);
            dataSource.Version = version;

            dataSource.SetDirectories(silverlightDataDir,
                context.IndexedDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {   
                // Building the indexed database...
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Indexed Database: Silverlight",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseIndexedBuilder indexedBuilder =
                    new DatabaseIndexedBuilder(dataSource))
                {
                    if (indexedBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Silverlight successful.",
                                BuildLoggerLevel.Info);
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Silverlight failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Indexed Database: Silverlight",
                        BuildLoggerLevel.Info);
                }
            }

            return buildResult;
        }

        #endregion

        #region OnCreatePortableReflection Method

        private bool OnCreatePortableReflection(BuildContext context,
            BuildFramework framework, BuildFramework referencedFramework,
            string sandcastleDir)
        {
            string reflectionDir = context.ReflectionDataDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            Version version    = framework.Version;
            string portableDir = framework.AssemblyDir;

            string portableDataDir = Path.Combine(reflectionDir,
                @"Portable\v" + version.ToString(2));

            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(portableDataDir) &&
                !DirectoryUtils.IsDirectoryEmpty(portableDataDir))
            {
                context.GroupContexts[_currentGroup.Id]["$PortableDataDir"] = portableDataDir;

                // Create the various persistent databases, if not available...
                return OnCreatePortableDatabase(context, version,
                    portableDataDir);
            }

            string[] assemblies = Directory.GetFiles(portableDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }
            List<string> assemblyFiles = new List<string>(assemblies);

            // For search-redirection of assembly if different versions...
            List<string> assemblySources = new List<string>();
            assemblySources.Add(portableDir);
            if (referencedFramework != null && referencedFramework != framework)
            {
                assemblySources.Add(referencedFramework.AssemblyDir);
            }

            // We may have assemblies in a different directory. Portable Class
            // Libraries are based on profiles, and there is no single profile
            // that applies to all cases...
            IEnumerable<string> commentFiles = framework.CommentFiles;
            if (commentFiles != null)
            {
                foreach (string commentFile in commentFiles)
                {
                    string assemblyFile = Path.ChangeExtension(commentFile,
                        ".dll");
                    if (File.Exists(assemblyFile))
                    {
                        assemblyFiles.Add(assemblyFile);
                    }
                }
            }

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger = context.Logger;

            Directory.CreateDirectory(portableDataDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(this.WorkingDirectory, "PortableClassLibrary.config");
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.BindingSources = assemblySources;

                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_currentGroup, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            string prodPath     = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir = context.SandcastleToolsDirectory;
            string refInfoFile  = String.Empty;
            bool buildResult    = false;

            // The list of transformations to apply...
            string[] transformFiles = new string[2];
            transformFiles[0] = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            transformFiles[1] = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");

            // The arguments to the transformations...
            Dictionary<string, string> transformArguments =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            transformArguments.Add("IncludeAllMembersTopic", "true");
            transformArguments.Add("IncludeInheritedOverloadTopics", "true");

            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);
                
                refInfoFile = Path.Combine(portableDataDir, fileName + ".org");
                string application = null;

                // For the direct Sandcastle library option...
                if (context.IsDirectSandcastle && _reflectorProxy != null)
                {
                    _reflectorProxy.ReflectionFile = refInfoFile;
                    _reflectorProxy.ConfigurationFile = finalRefBuilder;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles = new string[] { assemblyFile };
                    _reflectorProxy.DependencyFiles = null;

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {   
                    textBuilder.Length = 0;
                    // Create the reflection and the manifest
                    // 1. Call MRefBuilder to generate the reflection...
                    // MRefBuilder Assembly.dll 
                    // /out:reflection.org /config:MRefBuilder.config 
                    //   /internal-
                    application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                    textBuilder.AppendFormat("\"{0}\"", assemblyFile);

                    textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                        refInfoFile, finalRefBuilder);

                    string arguments = textBuilder.ToString();

                    buildResult = base.Run(logger, portableDataDir,
                        application, arguments);
                }

                if (!buildResult)
                {
                    break;
                }

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(portableDataDir, 
                    fileName + ".xml");

                if (context.IsDirectSandcastle && _transformProxy != null)
                {   
                    if (logger != null)
                    {
                        logger.WriteLine("Started Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    _transformProxy.InputFile        = sourceFile;
                    _transformProxy.OutputFile       = destinationFile;
                    _transformProxy.Arguments        = transformArguments;
                    _transformProxy.TransformFiles   = transformFiles;

                    buildResult = _transformProxy.Run(context);

                    if (logger != null)
                    {
                        logger.WriteLine("Completed Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }
                else
                {   
                    textBuilder.Length = 0;

                    // XslTransform.exe       
                    // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                    //    reflection.org 
                    //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                    //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                    //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                    application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[0]);
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[1]);
                    textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                    textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                    textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
                   
                    StepXslTransform modelTransform = new StepXslTransform(
                        portableDataDir, application, textBuilder.ToString());
                    modelTransform.LogTitle        = String.Empty;
                    modelTransform.Message         = "Xsl Transformation - Applying model and adding filenames.";
                    modelTransform.CopyrightNotice = 2;
                    modelTransform.LogTimeSpan     = false;

                    modelTransform.Initialize(context);
                    if (modelTransform.IsInitialized)
                    {
                        buildResult = modelTransform.Execute();

                        modelTransform.Uninitialize();
                        if (!buildResult)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch
                    {
                        _deleteLaterFiles.Add(sourceFile);
                    }
                }
            }

            if (buildResult)
            {
                context.GroupContexts[_currentGroup.Id]["$PortableDataDir"] = portableDataDir;
            }
            else
            {
                return false;
            }

            // Create the various persistent databases...
            buildResult = OnCreatePortableDatabase(context, version, 
                portableDataDir);

            return buildResult;
        }

        private bool OnCreatePortableDatabase(BuildContext context,
            Version version, string portableDataDir)
        {
            bool buildResult   = false;
            BuildLogger logger = context.Logger;

            DataSource dataSource = new DataSource(true, true, true, false,
                DataSourceType.Portable);
            dataSource.Version = version;

            dataSource.SetDirectories(portableDataDir,
                context.TargetDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {   
                // Building the targets database...
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Targets Database: Portable Class Library",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseTargetTextBuilder targetBuilder =
                    new DatabaseTargetTextBuilder(dataSource))
                {
                    if (targetBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Portable Class Library successful.",
                                BuildLoggerLevel.Info);
                        }

                        buildResult = true;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building targets database for Portable Class Library failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Targets Database: Portable Class Library",
                        BuildLoggerLevel.Info);
                }
            }

            if (!buildResult)
            {
                return false;
            }

            dataSource = new DataSource(true, true, true, false,
                DataSourceType.Portable);
            dataSource.Version = version;

            dataSource.SetDirectories(portableDataDir,
                context.IndexedDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {   
                // Building the indexed database...
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Indexed Database: Portable Class Library",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseIndexedBuilder indexedBuilder =
                    new DatabaseIndexedBuilder(dataSource))
                {
                    if (indexedBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Portable Class Library successful.",
                                BuildLoggerLevel.Info);
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Portable Class Library failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Indexed Database: Portable Class Library",
                        BuildLoggerLevel.Info);
                }
            }

            return buildResult;
        }

        #endregion

        #region OnCreateScriptSharpReflection Method

        private bool OnCreateScriptSharpReflection(BuildContext context,
            BuildFramework framework, BuildFramework referencedFramework,
            string sandcastleDir)
        {
            string reflectionDir = context.ReflectionDataDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            Version version       = framework.Version;
            string scriptSharpDir = framework.AssemblyDir;

            if (version.Major < 1)
            {
                version = new Version(1, 0, 0, 0);
            }
            string compactDataDir = Path.Combine(reflectionDir,
                @"ScriptSharp\v" + version.ToString(2));

            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(compactDataDir) &&
                !DirectoryUtils.IsDirectoryEmpty(compactDataDir))
            {
                context.GroupContexts[_currentGroup.Id]["$ScriptSharpDataDir"] = compactDataDir;

                // Create the various persistent databases, if not available...
                return OnCreateScriptSharpDatabase(context, version,
                    compactDataDir);
            }

            string[] assemblies = Directory.GetFiles(scriptSharpDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }
            List<string> assemblyFiles = new List<string>(assemblies);

            // For search-redirection of assembly if different versions...
            List<string> assemblySources = new List<string>();
            assemblySources.Add(scriptSharpDir);
            if (referencedFramework != null && referencedFramework != framework)
            {
                assemblySources.Add(referencedFramework.AssemblyDir);
            }

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger = context.Logger;

            Directory.CreateDirectory(compactDataDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(this.WorkingDirectory, "ScriptSharp.config");
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.BindingSources = assemblySources;

                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_currentGroup, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            string prodPath     = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir = context.SandcastleToolsDirectory;
            string refInfoFile  = String.Empty;
            bool buildResult    = false;

            // The list of transformations to apply...
            string[] transformFiles = new string[2];
            transformFiles[0] = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            transformFiles[1] = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");

            // The arguments to the transformations...
            Dictionary<string, string> transformArguments =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            transformArguments.Add("IncludeAllMembersTopic", "true");
            transformArguments.Add("IncludeInheritedOverloadTopics", "true");

            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);
                string application = null;

                // For the direct Sandcastle library option...
                if (context.IsDirectSandcastle && _reflectorProxy != null)
                {
                    _reflectorProxy.ReflectionFile = refInfoFile;
                    _reflectorProxy.ConfigurationFile = finalRefBuilder;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles = new string[] { assemblyFile };
                    _reflectorProxy.DependencyFiles = null;

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {   
                    textBuilder.Length = 0;
                    // Create the reflection and the manifest
                    // 1. Call MRefBuilder to generate the reflection...
                    // MRefBuilder Assembly.dll 
                    // /out:reflection.org /config:MRefBuilder.config 
                    //   /internal-
                    application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                    textBuilder.AppendFormat("\"{0}\"", assemblyFile);

                    refInfoFile = Path.Combine(compactDataDir, fileName + ".org");
                    textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                        refInfoFile, finalRefBuilder);

                    string arguments = textBuilder.ToString();

                    buildResult = base.Run(logger, compactDataDir,
                        application, arguments);
                }

                if (!buildResult)
                {
                    break;
                }

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(compactDataDir, 
                    fileName + ".xml");

                if (context.IsDirectSandcastle && _transformProxy != null)
                {   
                    if (logger != null)
                    {
                        logger.WriteLine("Started Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    _transformProxy.InputFile        = sourceFile;
                    _transformProxy.OutputFile       = destinationFile;
                    _transformProxy.Arguments        = transformArguments;
                    _transformProxy.TransformFiles   = transformFiles;

                    buildResult = _transformProxy.Run(context);

                    if (logger != null)
                    {
                        logger.WriteLine("Completed Xsl Transformation - Applying model and adding filenames.",
                            BuildLoggerLevel.Info);
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }
                else
                {   
                    textBuilder.Length = 0;

                    // XslTransform.exe       
                    // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                    //    reflection.org 
                    //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                    //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                    //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                    application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[0]);
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFiles[1]);
                    textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                    textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                    textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
                   
                    StepXslTransform modelTransform = new StepXslTransform(
                        compactDataDir, application, textBuilder.ToString());
                    modelTransform.LogTitle        = String.Empty;
                    modelTransform.Message         = "Xsl Transformation - Applying model and adding filenames.";
                    modelTransform.CopyrightNotice = 2;
                    modelTransform.LogTimeSpan     = false;

                    modelTransform.Initialize(context);
                    if (modelTransform.IsInitialized)
                    {
                        buildResult = modelTransform.Execute();

                        modelTransform.Uninitialize();
                        if (!buildResult)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    try
                    {
                        File.Delete(sourceFile);
                    }
                    catch
                    {
                        _deleteLaterFiles.Add(sourceFile);
                    }
                }
            }

            if (buildResult)
            {
                context.GroupContexts[_currentGroup.Id]["$ScriptSharpDataDir"] = compactDataDir;
            }
            else
            {
                return false;
            }

            // Create the various persistent databases...
            buildResult = OnCreateScriptSharpDatabase(context, version,
                compactDataDir);

            return buildResult;
        }

        private bool OnCreateScriptSharpDatabase(BuildContext context,
            Version version, string compactDataDir)
        {
            bool buildResult   = false;
            BuildLogger logger = context.Logger;

            DataSource dataSource = new DataSource(true, true, true, false,
                DataSourceType.ScriptSharp);
            dataSource.Version = version;

            dataSource.SetDirectories(compactDataDir,
                context.TargetDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {   
                // Building the targets database...
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Targets Database: Compact Framework",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseTargetTextBuilder targetBuilder =
                    new DatabaseTargetTextBuilder(dataSource))
                {
                    if (targetBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Compact Framework successful.",
                                BuildLoggerLevel.Info);
                        }

                        buildResult = true;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Compact Framework failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Targets Database: Compact Framework",
                        BuildLoggerLevel.Info);
                }
            }

            if (!buildResult)
            {
                return false;
            }

            dataSource = new DataSource(true, true, true, false,
                DataSourceType.ScriptSharp);
            dataSource.Version = version;

            dataSource.SetDirectories(compactDataDir,
                context.IndexedDataDirectory);

            if (dataSource.Exists)
            {
                buildResult = true;
            }
            else
            {   
                // Building the indexed database...
                if (logger != null)
                {
                    logger.WriteLine("Begin Building Indexed Database: Compact Framework",
                        BuildLoggerLevel.Info);
                }
                using (DatabaseIndexedBuilder indexedBuilder =
                    new DatabaseIndexedBuilder(dataSource))
                {
                    if (indexedBuilder.Build())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Compact Framework successful.",
                                BuildLoggerLevel.Info);
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("Building indexed database for Compact Framework failed.",
                                BuildLoggerLevel.Error);
                        }

                        buildResult = false;
                    }
                }
                if (logger != null)
                {
                    logger.WriteLine("Completed Building Indexed Database: Compact Framework",
                        BuildLoggerLevel.Info);
                }   
            }

            return buildResult;
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_deleteLaterFiles != null && _deleteLaterFiles.Count != 0)
            {
                foreach (string sourceFile in _deleteLaterFiles)
                {
                    try
                    {
                        if (File.Exists(sourceFile))
                            File.Delete(sourceFile);
                    }
                    catch
                    {
                        // For the exceptions...
                    }
                }
            }

            if (_reflectorDomain != null)
            {
                AppDomain.Unload(_reflectorDomain);
                _reflectorDomain = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
