using System;
using System.IO;
using System.Text;
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
        private List<BuildFormat>     _listFormats;
        private List<ReferenceGroup>  _listGroups;

        #endregion

        #region Constructors and Destructor

        public ReferenceEngine()
        {
        }

        public ReferenceEngine(BuildSettings settings)
            : base(settings)
        {
        }

        public ReferenceEngine(BuildLoggers logger)
            : base(logger)
        {
        }

        public ReferenceEngine(BuildSettings settings, BuildLoggers logger,
            BuildContext context, IncludeContentList configuration)
            : base(settings, logger, context, configuration)
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

        #region CreateSteps Method

        public override BuildStep CreateInitialSteps(BuildGroup group)
        {
            ReferenceGroup curGroup = group as ReferenceGroup;
            if (curGroup == null)
            {
                return null;
            }

            BuildMultiStep listSteps = new BuildMultiStep();
            listSteps.Message     = "References topics for the group: " + group.Name;
            listSteps.LogTitle    = String.Empty;
            listSteps.LogTimeSpan = true;

            this.CreateSteps(listSteps, curGroup);
            if (listSteps.Count != 0)
            {
                return listSteps;
            }

            return null;
        }

        public override BuildStep CreateFinalSteps(BuildGroup group)
        {
            ReferenceGroup curGroup = group as ReferenceGroup;
            if (curGroup == null)
            {
                return null;
            }

            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            string reflectionFile = group["$ReflectionFile"];
            string manifestFile   = group["$ManifestFile"];
            string configFile     = group["$ConfigurationFile"];
            string tocFile        = group["$TocFile"];

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

        #region Add Method

        public void Add(ReferenceGroup group)
        {
            if (group == null)
            {
                return;
            }

            if (_listGroups == null)
            {
                _listGroups = new List<ReferenceGroup>();
            }

            _listGroups.Add(group);
        }

        #endregion

        #region Initialize Method

        public override bool Initialize(BuildSettings settings)
        {
            bool initResult = base.Initialize(settings);
            if (initResult == false)
            {
                return initResult;
            }
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            _listFormats = new List<BuildFormat>();

            IList<BuildFormat> listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;

                return false;
            }
            int itemCount = listFormats.Count;
            _listFormats = new List<BuildFormat>(itemCount);
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

                return false;
            }
            itemCount = _listGroups.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceGroup group = _listGroups[i];

                if (group == null || group.IsEmpty || group.Exclude)
                {
                    continue;
                }

                string indexText = String.Empty;
                if (itemCount > 1)
                {
                    indexText = (i + 1).ToString();
                }

                group["$SharedContentFile"] =
                    String.Format("ApiSharedContent{0}.xml", indexText);
                group["$TocFile"] =
                    String.Format("ApiToc{0}.xml", indexText);
                group["$ManifestFile"] =
                    String.Format("ApiManifest{0}.xml", indexText);
                group["$ConfigurationFile"] =
                    String.Format("ApiBuildAssembler{0}.config", indexText);
                group["$ReflectionFile"] =
                    String.Format("Reflection{0}.xml", indexText);
                group["$ReflectionBuilderFile"] =
                    String.Format("MRefBuilder{0}.config", indexText);
                group["$GroupIndex"] = indexText;

                if (!group.Initialize(this.Context))
                {
                    this.IsInitialized = false;

                    initResult = false;
                    break;
                }
            }
                        
            return initResult;
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

                    if (group.Initialize(context))
                    {
                        _curGroup = group;

                        // Create the build steps...
                        BuildMultiStep listSteps = new BuildMultiStep();
                        this.CreateSteps(listSteps, group);

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

        #region CreateSteps Methods

        private void CreateSteps(BuildMultiStep listSteps, ReferenceGroup group)
        {
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return;
            }

            BuildSettings settings = this.Settings;
            BuildStyle outputStyle = settings.Style;
            string sandcastleDir   = settings.StylesDirectory;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return;
            }

            string helpStyle = BuildStyleUtils.StyleFolder(
                outputStyle.StyleType);  
            string workingDir = this.Context.WorkingDirectory;

            // 1. Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);

            // 2. Handle the resources...
            StepDirectoryCopy copyResources = new StepDirectoryCopy(
                workingDir);
            copyResources.LogTitle = String.Empty;
            copyResources.Message  = "Copying user-defined resources.";
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
                placeHolder.Message  = "Copying user-defined resources.";
                listSteps.Add(placeHolder);  
            }

            _listFolders = listFolders;

            this.CreateReflectionSteps(listSteps, group, 
                sandcastleDir, helpStyle);
        }

        #endregion

        #region CreateReflectionSteps Method

        private void CreateReflectionSteps(BuildMultiStep listSteps, 
            ReferenceGroup group, string sandcastleDir, string helpStyle)
        {
            BuildContext context     = this.Context;
            BuildSettings settings   = this.Settings;

            DependencyContent dependencies = group.Dependencies;
            ReferenceOptions options = group.Options;
            string assemblyDir       = group.AssemblyFolder;
            string dependencyDir     = group.DependencyFolder;

            string reflectionFile = group["$ReflectionFile"];
            string manifestFile   = group["$ManifestFile"];
            string refBuilderFile = group["$ReflectionBuilderFile"];
            string refInfoFile    = Path.ChangeExtension(reflectionFile, ".org");
            string configFile     = group["$ConfigurationFile"];
            string tocFile        = group["$TocFile"];

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
            bool? optionValue = options[ReferenceOptions.Included, "Internals"];
            if (optionValue != null && optionValue.Value)
            {
                textBuilder.Append(" /internal+");
            }
            else
            {
                textBuilder.Append(" /internal-");
            }
            
            string arguments = textBuilder.ToString();
            StepMrefBuilder mRefProcess = new StepMrefBuilder(workingDir,
                application, arguments);
            mRefProcess.Group    = group;
            mRefProcess.LogTitle = String.Empty;
            mRefProcess.Message  = "Creating XML-formatted reflection information.";
            mRefProcess.CopyrightNotice = 2;

            listSteps.Add(mRefProcess);

            textBuilder.Length = 0;

            // 2. Apply Transforms
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

            ReferenceNamingMethod namingMethod = options.Naming;
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

            bool rootContainer = settings.RootNamespaceContainer;
            string rootTitle   = settings.RootNamespaceTitle;
            if (rootContainer && String.IsNullOrEmpty(rootTitle) == false)
            {
                textBuilder.Append(" /arg:project=Project");
            }
            arguments = textBuilder.ToString();
            StepXslTransform applyDocProcess = new StepXslTransform(workingDir,
                application, arguments);
            applyDocProcess.LogTitle = String.Empty;
            applyDocProcess.Message  = "Xsl Transformation - Applying model and adding filenames";
            applyDocProcess.CopyrightNotice = 2;

            listSteps.Add(applyDocProcess);

            textBuilder.Length = 0;

            // 3. and finally the manifest...
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\ReflectionToManifest.xsl"  
            //   reflection.xml /out:manifest.xml
            application = Path.Combine(context.SandcastleToolsDirectory, 
                "XslTransform.exe");
            textTemp = Path.Combine(prodPath, "ReflectionToManifest.xsl");
            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
            textBuilder.AppendFormat(" {0} /out:{1}", reflectionFile, manifestFile);
            arguments = textBuilder.ToString();
            StepXslTransform manifestProcess = new StepXslTransform(workingDir,
                application, arguments);
            manifestProcess.LogTitle = String.Empty;
            manifestProcess.Message  = "Xsl Transformation - Reflection to Manifest";
            manifestProcess.CopyrightNotice = 2;

            listSteps.Add(manifestProcess);

            // 4. Create the reflection table of contents
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
            StepXslTransform tocProcess = new StepXslTransform(workingDir,
                application, arguments);
            tocProcess.LogTitle = String.Empty;
            tocProcess.Message  = "Xsl Transformation - Creating References TOC";
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
