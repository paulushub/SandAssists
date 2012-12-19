using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Steps;
using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle.References
{
    public class ReferenceEngine : BuildEngine
    {
        #region Public Fields

        public static readonly Version ReflectionVersion  = new Version(4, 0, 0, 0);
        public static readonly string ReflectionDirectory = @"%DXROOT%\Data\Reflection\";

        #endregion

        #region Private Fields

        private IList<string>         _listFolders;
        private BuildFormatList       _listFormats;
        private ReferenceEngineSettings   _engineSettings;
        private BuildList<ReferenceGroup> _listGroups;
        private BuildList<ReferenceGroup> _linkGroups;

        #endregion

        #region Constructors and Destructor

        public ReferenceEngine()
        {
        }

        #endregion

        #region Public Properties

        public override BuildEngineType EngineType
        {
            get
            {
                return BuildEngineType.Reference;
            }
        }

        public IList<ReferenceGroup> Groups
        {
            get
            {
                if (_listGroups == null)
                {
                    _listGroups = new BuildList<ReferenceGroup>();
                }

                return _listGroups;
            }
        }

        public IList<ReferenceGroup> LinkGroups
        {
            get
            {
                if (_linkGroups == null)
                {
                    _linkGroups = new BuildList<ReferenceGroup>();
                }

                return _linkGroups;
            }
        }

        /// <summary>
        /// Gets the list of the folders created or are use by this build engine.
        /// </summary>
        public override IList<string> Folders
        {
            get
            {
                return _listFolders;
            }
        }

        #endregion

        #region Public Methods

        #region Add Method

        public void Add(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (group.IsEmpty || group.Exclude)
            {
                throw new BuildException(
                    "The build engine requires a valid build group.");
            }

            if (_listGroups == null)
            {
                _listGroups = new BuildList<ReferenceGroup>();
            }

            _listGroups.Add(group);
        }

        #endregion

        #region CreateInitialSteps Method

        public override BuildStep CreateInitialSteps(BuildGroup group)
        {
            ReferenceGroup refGroup = group as ReferenceGroup;
            if (refGroup == null)
            {
                throw new BuildException("The build engine requires reference group.");
            }
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return null;
            }

            BuildMultiStep listSteps = new BuildMultiStep();
            listSteps.Message     = "References topics for the group: " + group.Name;
            listSteps.LogTitle    = String.Empty;
            listSteps.LogTimeSpan = true;

            BuildSettings settings = this.Settings;
            string sandcastleDir   = this.Context.StylesDirectory;
            BuildStyle outputStyle = settings.Style;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return null;
            }

            // 1. Initialize the conceptual topics...
            StepReferenceInit stepInit = new StepReferenceInit(refGroup);
            stepInit.Message  = "Initializing and copying reference contents";
            stepInit.LogTitle = String.Empty;

            listSteps.Add(stepInit);

            string helpStyle = BuildStyle.StyleFolder(
                outputStyle.StyleType);
            string workingDir = this.Context.WorkingDirectory;

            // 2. Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);

            // 3. Handle the resources...
            StepDirectoryCopy copyResources = new StepDirectoryCopy(
                workingDir);
            copyResources.LogTitle = String.Empty;
            copyResources.Message = "Copying user-defined resources.";
            IList<ResourceContent> resourceContents = group.ResourceContents;
            if (resourceContents != null && resourceContents.Count != 0)
            {
                int contentCount = resourceContents.Count;
                for (int j = 0; j < contentCount; j++)
                {
                    ResourceContent resourceContent = resourceContents[j];
                    if (resourceContent == null || resourceContent.Count == 0)
                    {
                        continue;
                    }

                    int itemCount = resourceContent.Count;

                    for (int i = 0; i < itemCount; i++)
                    {
                        ResourceItem resource = resourceContent[i];
                        if (resource != null && !resource.IsEmpty)
                        {
                            string destFolder = resource.Destination;
                            copyResources.Add(resource.Source, destFolder);

                            // Add this to the output folders so that it is copied
                            // to the final output/build directories...
                            if (destFolder.StartsWith("Output",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                DirectoryInfo info = new DirectoryInfo(destFolder);
                                destFolder = info.Name;
                                if (!String.IsNullOrEmpty(destFolder) &&
                                    !dicFolders.ContainsKey(destFolder))
                                {
                                    dicFolders.Add(destFolder, true);
                                    listFolders.Add(destFolder);
                                }
                            }
                        }
                    }
                }
            }

            if (copyResources.IsValid)
            {
                listSteps.Add(copyResources);
            }
            else
            {
                StepNone placeHolder = new StepNone();
                placeHolder.LogTitle = String.Empty;
                placeHolder.Message  = "Copying user-defined resources.";
                listSteps.Add(placeHolder);
            }

            _listFolders = listFolders;

            this.CreateReflectionSteps(listSteps, refGroup,
                sandcastleDir, helpStyle);

            if (listSteps.Count != 0)
            {
                return listSteps;
            }

            return null;
        }

        #endregion

        #region CreateLinkSteps Method

        public override BuildStep CreateLinkSteps()
        {
            Debug.Assert(_engineSettings != null);
            if (_engineSettings == null)
            {
                return null;
            }

            BuildSettings settings = this.Settings;
            string workingDir      = this.Context.WorkingDirectory;
            //string sandcastleDir   = context.StylesDirectory;

            StepReferenceLinks referenceLinks = new StepReferenceLinks(
                workingDir, null);
            referenceLinks.BuildGroups = _listGroups;
            referenceLinks.LinkGroups = _linkGroups;

            return referenceLinks;
        }

        #endregion

        #region CreateFinalSteps Method

        public override BuildStep CreateFinalSteps(BuildGroup group)
        {
            if (group.GroupType != BuildGroupType.Reference)
            {
                throw new BuildException("The build engine requires reference group.");
            }

            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string reflectionFile = groupContext["$ReflectionFile"];
            string manifestFile   = groupContext["$ManifestFile"];
            string configFile     = groupContext["$ConfigurationFile"];
            string tocFile        = groupContext["$TocFile"];

            string sandcastleDir  = context.StylesDirectory;

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = context.WorkingDirectory;

            // Assemble the help files using the BuildAssembler
            // BuildAssembler.exe /config:Project.config manifest.xml
            string application = Path.Combine(context.SandcastleToolsDirectory,
                "BuildAssembler.exe");
            string arguments = String.Format(" /config:{0} {1}",
                configFile, manifestFile);
            StepAssembler buildAssProcess = new StepAssembler(workingDir,
                application, arguments);
            buildAssProcess.Group           = group;
            buildAssProcess.LogTitle        = String.Empty;
            buildAssProcess.Message         = "For the group: " + group.Name;
            buildAssProcess.CopyrightNotice = 2;

            return buildAssProcess;
       }

        #endregion

        #region Initialize Method

        public override void Initialize(BuildContext context)
        {
            if (this.IsInitialized)
            {
                return;
            }

            base.Initialize(context);
            if (!this.IsInitialized)
            {
                return;
            }

            if (_listGroups == null || _listGroups.Count == 0)
            {
                this.IsInitialized = false;
                return;
            }

            BuildSettings settings = this.Settings;

            _engineSettings = (ReferenceEngineSettings)settings.EngineSettings[
                BuildEngineType.Reference];
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                this.IsInitialized = false;
                return;
            }

            int itemCount = _listGroups.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceGroup group = _listGroups[i];

                BuildGroupContext groupContext = context.GroupContexts[group.Id];
                if (groupContext == null)
                {
                    throw new BuildException(
                        "The group context is not provided, and it is required by the build system.");
                }

                string indexText = String.Empty;
                if (itemCount > 1)
                {
                    indexText = (i + 1).ToString();
                }

                // Create the build dynamic properties...
                groupContext.CreateProperties(indexText);

                group.BeginSources(context);
            }

            // Turn the link sources to dynamic groups...
            this.CreateLinkGroups(context);

            // Cache the list of applicable formats...
            _listFormats = new BuildFormatList();

            BuildFormatList listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;
                return;
            }
            itemCount = listFormats.Count;
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
                this.IsInitialized = false;
                return;
            }

            // Finally, initialize the build groups...
            itemCount = _listGroups.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceGroup group = _listGroups[i];

                group.Initialize(context);
                if (!group.IsInitialized)
                {
                    this.IsInitialized = false;
                    break;
                }
            }
        }

        #endregion

        #region Uninitialize Method

        public override void Uninitialize()
        {
            if (_listGroups != null)
            {
                int itemCount = _listGroups.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceGroup group = _listGroups[i];
                    if (group != null)
                    {
                        group.EndSources();
                        group.Uninitialize();
                    }
                }
            }     

            base.Uninitialize();
        }

        #endregion

        #endregion

        #region Protected Methods

        #endregion

        #region Private Method

        #region CreateReflectionSteps Method

        private void CreateReflectionSteps(BuildMultiStep listSteps, 
            ReferenceGroup group, string sandcastleDir, string helpStyle)
        {
            Debug.Assert(_engineSettings != null);
            if (_engineSettings == null)
            {
                return;
            }

            BuildContext context     = this.Context;
            BuildSettings settings   = this.Settings;

            ReferenceGroupContext groupContext =
                context.GroupContexts[group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceContent content = group.Content;
            string assemblyDir       = groupContext.AssemblyFolder;
            string dependencyDir     = groupContext.DependencyFolder;

            DependencyContent dependencies = content.Dependencies;

            string reflectionFile = groupContext["$ReflectionFile"];
            string manifestFile   = groupContext["$ManifestFile"];
            string refBuilderFile = groupContext["$ReflectionBuilderFile"];
            string tocFile        = groupContext["$TocFile"];

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = context.WorkingDirectory;

            string tempText    = null;

            string arguments   = null;
            string application = null;

            bool ripOldApis    = true;

            string assemblyFiles   = null;
            string dependencyFiles = null;
            string outputFile  = null;
            string configurationFile = null;

            ReferenceVisibilityConfiguration visibility =
                _engineSettings.Visibility;
            Debug.Assert(visibility != null);
            // 1. Create the reflection and the manifest
            StringBuilder textBuilder = new StringBuilder();
            if (group.IsSingleVersion)
            {
                // Call MRefBuilder to generate the reflection...
                // MRefBuilder Assembly.dll 
                // /out:reflection.org /config:MRefBuilder.config 
                //   /internal-
                application = Path.Combine(context.SandcastleToolsDirectory,
                    "MRefBuilder.exe");

                assemblyFiles = String.Format(@"{0}\*.*", assemblyDir);
                dependencyFiles = String.Format(@"{0}\*.*", dependencyDir);
                outputFile = Path.ChangeExtension(reflectionFile, ".ver");
                configurationFile = Path.Combine(workingDir, refBuilderFile);

                textBuilder.Append(assemblyFiles);
                textBuilder.Append(" /dep:" + dependencyFiles);

                textBuilder.AppendFormat(" /out:{0} /config:{1}",
                    outputFile, refBuilderFile);
                if (visibility != null && visibility.IncludeInternalsMembers)
                {
                    textBuilder.Append(" /internal+");
                }
                else
                {
                    textBuilder.Append(" /internal-");
                }

                arguments = textBuilder.ToString();

                outputFile = Path.Combine(workingDir, outputFile);
            }
            else
            {
                // Call the VersionBuilder to create the combined reflection file...
                application = Path.Combine(context.SandcastleToolsDirectory,
                    "VersionBuilder.exe");

                textBuilder.AppendFormat(" /config:{0} /out:{1}",
                    groupContext["$ApiVersionsBuilderFile"], 
                    Path.ChangeExtension(reflectionFile, ".org"));

                configurationFile = Path.Combine(workingDir,
                    groupContext["$ApiVersionsBuilderFile"]);

                ReferenceVersionInfo versionInfo = group.VersionInfo;
                ripOldApis = versionInfo.RipOldApis;
                if (ripOldApis)
                {
                    textBuilder.Append(" /rip+");
                }
                else
                {
                    textBuilder.Append(" /rip-");
                }

                arguments = textBuilder.ToString();
            }
            StepReflectionBuilder refBuilder = new StepReflectionBuilder(workingDir,
                application, arguments);
            refBuilder.Group    = group;
            refBuilder.LogTitle = String.Empty;
            refBuilder.Message  = "Creating XML-formatted reflection information.";
            refBuilder.CopyrightNotice = 2;            

            // For the direct use of the Sandcastle library, we need the
            // following parameters...
            refBuilder.RipOldApis = ripOldApis;
            refBuilder.DocumentInternals = 
                (visibility != null && visibility.IncludeInternalsMembers);
            refBuilder.ConfigurationFile = configurationFile;
            refBuilder.ReflectionFile = outputFile;
            refBuilder.AssemblyFiles.Add(assemblyFiles);
            refBuilder.DependencyFiles.Add(dependencyFiles);

            listSteps.Add(refBuilder);

            textBuilder.Length = 0;

            // 2. Create the reflection and comment refining step...
            StepReferenceRefine referenceRefine =
                new StepReferenceRefine(workingDir);
            referenceRefine.LogTitle = String.Empty;
            referenceRefine.Message  = "Refining the reflection files - filtering and cleaning";
            referenceRefine.Group    = group;

            listSteps.Add(referenceRefine);

            textBuilder.Length = 0;

            // 3. Apply Transforms
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
            //    reflection.org 
            //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
            //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
            //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
            application = Path.Combine(context.SandcastleToolsDirectory, 
                "XslTransform.exe");
            string prodPath = Path.Combine(sandcastleDir, "ProductionTransforms");
            string textTemp = String.Empty;
            if (group.EnableXmlnsForXaml)
            {
                textTemp = Path.Combine(prodPath, "AddXamlSyntaxData.xsl");
                if (!String.IsNullOrEmpty(textTemp))
                {
                    textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                    textTemp = String.Empty;
                }
            }

            if (outputStyle == BuildStyleType.ClassicWhite ||
                outputStyle == BuildStyleType.ClassicBlue)
            {
                textTemp = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
            }
            else if (outputStyle == BuildStyleType.Lightweight)
            {
            }
            else if (outputStyle == BuildStyleType.ScriptFree)
            {
            }
            if (!String.IsNullOrEmpty(textTemp))
            {
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textTemp = String.Empty;
            }

            ReferenceNamingMethod namingMethod = _engineSettings.Naming;
            if (namingMethod == ReferenceNamingMethod.Guid)
            {
                textTemp = Path.Combine(prodPath, "AddGuidFilenames.xsl");
            }
            else if (namingMethod == ReferenceNamingMethod.MemberName)
            {
                textTemp = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
            }
            else
            {
                textTemp = Path.Combine(prodPath, "AddGuidFilenames.xsl");
            }
            if (!String.IsNullOrEmpty(textTemp))
            {
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textTemp = String.Empty;
            }

            textBuilder.Append(" " + Path.ChangeExtension(reflectionFile, ".org")); 
            textBuilder.AppendFormat(" /out:{0}", reflectionFile);
            textBuilder.AppendFormat(" /arg:IncludeAllMembersTopic={0}", 
                _engineSettings.IncludeAllMembersTopic ? "true" : "false");
            textBuilder.AppendFormat(" /arg:IncludeInheritedOverloadTopics={0}", 
                _engineSettings.IncludeInheritedOverloadTopics ? "true" : "false");

            bool rootContainer = _engineSettings.RootNamespaceContainer;
            string rootTitle   = group.RootNamespaceTitle;
            if (rootContainer && !String.IsNullOrEmpty(rootTitle))
            {
                textBuilder.Append(" /arg:project=Project");
                // Indicate that this reference group is rooted...
                groupContext["$IsRooted"] = Boolean.TrueString;
            }
            arguments = textBuilder.ToString();
            StepReferenceModel applyDocProcess = new StepReferenceModel(
                workingDir, application, arguments);
            applyDocProcess.Group    = group;
            applyDocProcess.LogTitle = String.Empty;
            applyDocProcess.Message  = "Xsl Transformation - Applying model and adding filenames";
            applyDocProcess.CopyrightNotice = 2;

            listSteps.Add(applyDocProcess);

            textBuilder.Length = 0;

            // 4. and finally the manifest...
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\ReflectionToManifest.xsl"  
            //   reflection.xml /out:manifest.xml
            application = Path.Combine(context.SandcastleToolsDirectory, 
                "XslTransform.exe");
            textTemp = Path.Combine(prodPath, "ReflectionToManifest.xsl");
            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
            textBuilder.AppendFormat(" {0} /out:{1}", reflectionFile, manifestFile);
            arguments = textBuilder.ToString();
            StepReferenceManifest manifestProcess = new StepReferenceManifest(
                workingDir, application, arguments);
            manifestProcess.Group    = group;
            manifestProcess.LogTitle = String.Empty;
            manifestProcess.Message  = "Xsl Transformation - Reflection to Manifest";
            manifestProcess.CopyrightNotice = 2;

            listSteps.Add(manifestProcess);

            // 5. Create the reflection table of contents
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\createvstoc.xsl" 
            //    reflection.xml /out:ApiToc.xml
            application = Path.Combine(context.SandcastleToolsDirectory,
                "XslTransform.exe");
            if (outputStyle == BuildStyleType.ClassicWhite ||
                outputStyle == BuildStyleType.ClassicBlue)
            {
                tempText = Path.Combine(sandcastleDir,
                    @"ProductionTransforms\CreateVSToc.xsl");
            }
            else if (outputStyle == BuildStyleType.Lightweight)
            {
            }
            else if (outputStyle == BuildStyleType.ScriptFree)
            {
            }
            arguments = String.Format("/xsl:\"{0}\" {1} /out:{2}",
                tempText, reflectionFile, tocFile);
            StepReferenceToc tocProcess = new StepReferenceToc(workingDir,
                application, arguments);
            tocProcess.LogTitle        = String.Empty;
            tocProcess.Message         = "Xsl Transformation - Creating References TOC";
            tocProcess.Group           = group;
            tocProcess.CopyrightNotice = 2;
            
            listSteps.Add(tocProcess);
        }

        #endregion

        #region CreateLinkGroups Method

        private void CreateLinkGroups(BuildContext context)
        {
            context["$EmbeddedScriptSharp"] = Boolean.FalseString;

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            BuildLogger logger = context.Logger;

            List<ReferenceGroup> buildGroups = new List<ReferenceGroup>();
            IList<BuildGroupContext> groupContexts = context.GroupContexts;

            bool hasScriptSharp = false;
            BuildFrameworkType latestScriptSharp = BuildFrameworkType.None;

            int itemCount = _listGroups.Count;
            int index = 0;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceGroup group = _listGroups[i];

                ReferenceContent content = group.Content;
                if (content != null)
                {
                    BuildFrameworkType frameworkType = content.FrameworkType;

                    if (frameworkType.Kind == BuildFrameworkKind.ScriptSharp)
                    {
                        hasScriptSharp = true;

                        if (frameworkType > latestScriptSharp)
                        {
                            latestScriptSharp = frameworkType;
                        }
                    }
                }    
            }

            // Include contents from the Script# framework for correct 
            // linking, since there is MSDN links for the Script#...
            if (hasScriptSharp && _engineSettings.EmbedScriptSharpFramework &&
                latestScriptSharp.Kind == BuildFrameworkKind.ScriptSharp)
            {
                BuildFramework framework = BuildFrameworks.GetFramework(latestScriptSharp);
                if (framework == null)
                {
                    framework = BuildFrameworks.LatestScriptSharp;
                }

                if (framework != null)
                {   
                    ReferenceGroup buildGroup = new ReferenceGroup(
                        "Embedded ScriptSharp - " + ReferenceGroup.NextGroupName(), 
                        Guid.NewGuid().ToString());

                    ReferenceContent content = buildGroup.Content;

                    string[] assemblies = Directory.GetFiles(framework.AssemblyDir,
                        "*.dll", SearchOption.AllDirectories);

                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        string assembly = assemblies[i];
                        string comments = Path.ChangeExtension(assembly, ".xml");

                        if (File.Exists(comments))
                        {
                            content.AddItem(comments, assembly);
                        }
                    }

                    buildGroup.ExcludeToc = true;
                    buildGroup.SyntaxType = BuildSyntaxType.CSharp | BuildSyntaxType.JavaScript;

                    buildGroups.Add(buildGroup);

                    // Create the group context...
                    ReferenceGroupContext buildGroupContext =
                        new ReferenceGroupContext(buildGroup);
                    buildGroupContext.IsEmbeddedGroup = true;
                    groupContexts.Add(buildGroupContext);

                    string indexText = (itemCount + index + 1).ToString();

                    // Create the build dynamic properties...
                    buildGroupContext.CreateProperties(indexText);

                    // This has no effect, since the newly created group will
                    // not have any content source.
                    buildGroup.BeginSources(context);

                    context["$EmbeddedScriptSharp"] = Boolean.TrueString;
                }
            }

            if (buildGroups.Count != 0)
            {
                _listGroups.Add(buildGroups);
            }

            // Process the user-provided link sources...
            List<ReferenceLinkSource> linkSources  = null;
            IList<ReferenceLinkSource> listSources = _engineSettings.LinkSources as IList<ReferenceLinkSource>;
            if (listSources != null && listSources.Count != 0)
            {
                for (int i = 0; i < listSources.Count; i++)
                {
                    ReferenceLinkSource linkSource = listSources[i];

                    if (linkSource == null || !linkSource.IsValid)
                    {
                        if (logger != null)
                        {
                            string title = linkSource.Title;
                            if (title == null)
                            {
                                title = String.Empty;
                            }
                            logger.WriteLine(String.Format(
                                "A provided reference link source titled = '{0}', at index = '{1}' is invalid.", 
                                title, i), BuildLoggerLevel.Warn);
                        }

                        continue;
                    }

                    if (linkSources == null)
                    {
                        linkSources = new List<ReferenceLinkSource>();
                    }

                    linkSources.Add(linkSource); 
                }
            }

            // Process the automatic link sources...
            BuildSpecialSdkType webMvcSdkType = _engineSettings.WebMvcSdkType;
            if (webMvcSdkType != BuildSpecialSdkType.None &&
                webMvcSdkType != BuildSpecialSdkType.Null)
            {
                BuildSpecialSdk webSdk = BuildSpecialSdks.GetSdk(webMvcSdkType,
                    BuildFrameworkKind.DotNet);

                if (webSdk != null)
                {     
                    ReferenceLinkSource linkSource = new ReferenceLinkSource();
                    linkSource.LinkType = BuildLinkType.Msdn;
                    linkSource.Title    = webMvcSdkType.Label;
                    linkSource.FrameworkType =
                        BuildFrameworks.LatestFramework.FrameworkType;

                    string aspMVCDir = webSdk.AssemblyDir;

                    string[] assemblyFiles = Directory.GetFiles(
                        webSdk.AssemblyDir, "*.dll", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < assemblyFiles.Length; i++)
                    {
                        string assemblyFile = assemblyFiles[i];
                        string commentFile = Path.ChangeExtension(assemblyFile,
                            ".xml");
                        if (File.Exists(commentFile))
                        {
                            ReferenceItem refItem = new ReferenceItem(
                                commentFile, assemblyFile);
                            refItem.XamlSyntax = false;
                            linkSource.Add(refItem);
                        }      
                    }

                    if (linkSource.IsValid)
                    {
                        if (linkSources == null)
                        {
                            linkSources = new List<ReferenceLinkSource>();
                        }

                        linkSources.Add(linkSource);
                    }
                }
            }

            if (linkSources != null && linkSources.Count != 0)
            {
                context.SetValue("$ReferenceLinkSources", linkSources);

                itemCount = linkSources.Count;
                if (_linkGroups == null)
                {
                    _linkGroups = new BuildList<ReferenceGroup>();
                }

                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceLinkSource linkSource = linkSources[i];

                    ReferenceGroup linkGroup = new ReferenceGroup(
                        "Reference Links - " + ReferenceGroup.NextGroupName(),
                        linkSource.SourceId, linkSource);

                    linkGroup.ExcludeToc = true;

                    _linkGroups.Add(linkGroup);

                    // Create the group context...
                    ReferenceGroupContext linkGroupContext =
                        new ReferenceGroupContext(linkGroup);
                    linkGroupContext.IsLinkGroup = true;
                    groupContexts.Add(linkGroupContext);

                    string indexText = String.Empty;
                    if (itemCount > 1)
                    {
                        indexText = (i + 1).ToString();
                    }

                    // Create the build dynamic properties...
                    linkGroupContext.CreateProperties(indexText);

                    // This has no effect, since the newly created group will
                    // not have any content source.
                    linkGroup.BeginSources(context);
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
