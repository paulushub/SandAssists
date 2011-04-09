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
        #region Private Fields

        private ReferenceGroup        _curGroup;
        private IList<string>         _listFolders;
        private BuildFormatList       _listFormats;
        private List<ReferenceGroup>  _listGroups;
        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;

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
                    _listGroups = new List<ReferenceGroup>();
                }

                return _listGroups.AsReadOnly();
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
                _listGroups = new List<ReferenceGroup>();
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
            BuildStyle outputStyle = settings.Style;
            string sandcastleDir = settings.StylesDirectory;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return null;
            }

            // 1. Initialize the conceptual topics...
            StepReferenceInit stepInit = new StepReferenceInit(refGroup);
            stepInit.Message  = "Initializing and copying reference contents";
            stepInit.LogTitle = String.Empty;

            listSteps.Add(stepInit);

            string helpStyle = BuildStyleUtils.StyleFolder(
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
                    IList<ResourceItem> listResources = resourceContent.Items;
                    if (listResources == null || listResources.Count == 0)
                    {
                        continue;
                    }

                    int itemCount = listResources.Count;

                    for (int i = 0; i < itemCount; i++)
                    {
                        ResourceItem resource = listResources[i];
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
                placeHolder.Message = "Copying user-defined resources.";
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

            string sandcastleDir = settings.StylesDirectory;

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

            _listFormats = new BuildFormatList();

            BuildFormatList listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;
                return;
            }
            int itemCount = listFormats.Count;
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
            itemCount = _listGroups.Count;
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

                groupContext["$SharedContentFile"] =
                    String.Format("ApiSharedContent{0}.xml", indexText);
                groupContext["$TocFile"] =
                    String.Format("ApiToc{0}.xml", indexText);
                groupContext["$HierarchicalTocFile"] =
                    String.Format("ApiHierarchicalToc{0}.xml", indexText);
                groupContext["$ManifestFile"] =
                    String.Format("ApiManifest{0}.xml", indexText);
                groupContext["$RootNamespaces"] =
                    String.Format("ApiRootNamespaces{0}.xml", indexText);
                groupContext["$ConfigurationFile"] =
                    String.Format("ApiBuildAssembler{0}.config", indexText);
                groupContext["$ReflectionFile"] =
                    String.Format("Reflection{0}.xml", indexText);
                groupContext["$ReflectionBuilderFile"] =
                    String.Format("MRefBuilder{0}.config", indexText);

                groupContext["$AssembliesFolder"] =
                    String.Format("Assemblies{0}", indexText);
                groupContext["$CommentsFolder"] =
                    String.Format("Comments{0}", indexText);
                groupContext["$DependenciesFolder"] =
                    String.Format("Dependencies{0}", indexText);

                groupContext["$IsRooted"] = Boolean.FalseString;

                groupContext["$GroupIndex"] = indexText;
                
                group.Initialize(this.Context);
                if (!group.IsInitialized)
                {
                    this.IsInitialized = false;
                    break;
                }
            }
        }

        #endregion

        #region Build Method

        public override bool Build()
        {
            bool buildResult = false;

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return buildResult;
            }

            BuildLogger logger     = this.Logger;
            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            try
            {
                int itemCount = _listGroups.Count;
                int initCount = 0;
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceGroup group = _listGroups[i];
                    if (group == null || group.IsEmpty)
                    {
                        continue;
                    }

                    group.Initialize(context);
                    if (group.IsInitialized)
                    {
                        _curGroup = group;

                        // Create the build steps...
                        BuildMultiStep listSteps = new BuildMultiStep();
                        //this.CreateInitialSteps(listSteps, group);

                        if (listSteps.Count != 0)
                        {
                            buildResult = this.RunSteps(listSteps.Steps);
                        }

                        if (buildResult)
                        {
                            initCount++;
                        }

                        _curGroup = null;

                        group.Uninitialize();
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(
                                "An error occurred in the initialization of group = " + i.ToString(),
                                BuildLoggerLevel.Error);
                        }
                    }

                    if (!buildResult)
                    {
                        break;
                    }
                }

                buildResult = (initCount == itemCount);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                buildResult = false;
            }

            return buildResult;
        }

        #endregion

        #region Uninitialize Method

        public override void Uninitialize()
        {
            base.Uninitialize();

            //BuildSettings settings = this.Settings;
            //if (settings == null)
            //{
            //    return;
            //}
            //bool cleanIntermediate = settings.CleanIntermediate;

            //string workingDir = context.WorkingDirectory;

            //if (settings.IsCombinedBuild == false)
            //{
            //    if (cleanIntermediate)
            //    {
            //        try
            //        {
            //            string tempFile = Path.Combine(workingDir, 
            //                "ApiManifest.xml");
            //            if (File.Exists(tempFile))
            //            {
            //                File.Delete(tempFile);
            //            }
            //            tempFile = Path.Combine(workingDir, "reflection.org");
            //            if (File.Exists(tempFile))
            //            {
            //                File.Delete(tempFile);
            //            }
            //            tempFile = Path.Combine(workingDir, "reflection.xml");
            //            if (File.Exists(tempFile))
            //            {
            //                File.Delete(tempFile);
            //            }
            //            tempFile = Path.Combine(workingDir, "ApiToc.xml");
            //            if (File.Exists(tempFile))
            //            {
            //                File.Delete(tempFile);
            //            }
            //        }
            //        catch
            //        {   
            //        }   
            //    }
            //}
        }

        #endregion

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
            string refInfoFile    = Path.ChangeExtension(reflectionFile, ".org");
            string configFile     = groupContext["$ConfigurationFile"];
            string tocFile        = groupContext["$TocFile"];

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = context.WorkingDirectory;

            string tempText = null;

            // Create the reflection and the manifest
            StringBuilder textBuilder = new StringBuilder();
            // 1. Call MRefBuilder to generate the reflection...
            // MRefBuilder Assembly.dll 
            // /out:reflection.org /config:MRefBuilder.config 
            //   /internal-
            string application = Path.Combine(context.SandcastleToolsDirectory, 
                "MRefBuilder.exe");
            textBuilder.AppendFormat(@"{0}\*.dll", assemblyDir);
            if (dependencies != null && dependencies.Count > 0)
            {
                textBuilder.AppendFormat(@" /dep:{0}\*.dll", dependencyDir);
            }

            textBuilder.AppendFormat(" /out:{0} /config:{1}", 
                refInfoFile, refBuilderFile);
            ReferenceVisibilityConfiguration visibility = 
                _engineSettings.Visibility;
            Debug.Assert(visibility != null);
            if (visibility != null && visibility.IncludeInternalsMembers)
            {
                textBuilder.Append(" /internal+");
            }
            else
            {
                textBuilder.Append(" /internal-");
            }
            
            string arguments = textBuilder.ToString();
            StepReflectionBuilder mRefProcess = new StepReflectionBuilder(workingDir,
                application, arguments);
            mRefProcess.Group    = group;
            mRefProcess.LogTitle = String.Empty;
            mRefProcess.Message  = "Creating XML-formatted reflection information.";
            mRefProcess.CopyrightNotice = 2;

            listSteps.Add(mRefProcess);

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
            string textTemp = prodPath;
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
            textBuilder.AppendFormat("/xsl:\"{0}\" ", textTemp);
            textBuilder.Append(refInfoFile);

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

            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
            textBuilder.AppendFormat(" /out:{0}", reflectionFile);
            textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
            textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");

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

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
