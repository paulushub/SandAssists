using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Steps;
using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualEngine : BuildEngine
    {
        #region Private Fields

        private ConceptualGroup       _curGroup;
        private IList<string>         _listFolders;
        private List<BuildFormat>     _listFormats;
        private List<ConceptualGroup> _listGroups;

        #endregion

        #region Constructors and Destructor

        public ConceptualEngine()
        {
        }

        public ConceptualEngine(BuildSettings settings)
            : base(settings)
        {
        }

        public ConceptualEngine(BuildLoggers logger)
            : base(logger)
        {
        }

        public ConceptualEngine(BuildSettings settings, BuildLoggers logger, 
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
                return BuildEngineType.Conceptual;
            }
        }

        public IList<ConceptualGroup> Groups
        {
            get
            {
                if (_listGroups == null)
                {
                    _listGroups = new List<ConceptualGroup>();
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

        public void Add(ConceptualGroup group)
        {
            if (group == null)
            {
                return;
            }

            if (_listGroups == null)
            {
                _listGroups = new List<ConceptualGroup>();
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
            _listFormats  = new List<BuildFormat>(itemCount);
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
                ConceptualGroup group = _listGroups[i];

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
                    String.Format("TopicsSharedContent{0}.xml", indexText);
                group["$TocFile"] =
                    String.Format("TopicsToc{0}.xml", indexText);
                group["$ManifestFile"] =
                    String.Format("TopicsManifest{0}.xml", indexText);
                group["$ConfigurationFile"] =
                    String.Format("Conceptual{0}.config", indexText);
                group["$BuildManifestFile"] =
                    String.Format("BuildManifest{0}.proj.xml", indexText);
                group["$BuildTocFile"] =
                    String.Format("Toc{0}.xml", indexText);
                group["$BuildTocTechReviewFile"] =
                    String.Format("TocTechReview{0}.xml", indexText);
                group["$MetadataFile"] =
                    String.Format("ContentMetadata{0}.xml", indexText);
                group["$ProjSettings"] =
                    String.Format("ProjectSettings{0}.xml", indexText);
                group["$ProjSettingsLoc"] =
                    String.Format("ProjectSettings{0}.loc.xml", indexText);
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

        #region CreateSteps Method

        public override BuildStep CreateInitialSteps(BuildGroup group)
        {
            ConceptualGroup curGroup = group as ConceptualGroup;
            if (curGroup == null)
            {
                return null;
            }

            BuildMultiStep listSteps = new BuildMultiStep();
            listSteps.Message     = "Conceptual topics for the group: " + group.Name;
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
            ConceptualGroup curGroup = group as ConceptualGroup;
            if (curGroup == null)
            {
                return null;
            }

            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            string manifestFile      = group["$ManifestFile"];
            string buildManifestFile = group["$BuildManifestFile"];
            string configFile        = group["$ConfigurationFile"];
            string tocFile           = group["$TocFile"];
            string buildTocFile      = group["$BuildTocFile"];

            string workingDir = context.WorkingDirectory;

            // Assemble the help files using the BuildAssembler
            // BuildAssembler.exe /config:Project.config manifest.xml
            string application = Path.Combine(context.SandcastleToolsDirectory,
                "BuildAssembler.exe");
            //arguments = "/config:Conceptual.config Manifest.xml";
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

        #region Build Method

        public override bool Build()
        {
            bool buildResult = false;

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            BuildLogger logger     = this.Logger;
            BuildSettings settings = this.Settings;
            BuildContext context   = this.Context;

            try
            {
                int itemCount = _listGroups.Count;
                int initCount = 0;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualGroup group = _listGroups[i];
                    if (group == null || group.IsEmpty)
                    {
                        continue;
                    }

                    if (group.Initialize(context))
                    {
                        _curGroup = group;

                        BuildMultiStep listSteps = new BuildMultiStep();
                        this.CreateSteps(listSteps, group);

                        buildResult = this.RunSteps(listSteps.Steps);

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
            BuildSettings settings = this.Settings;
            if (settings == null)
            {
                return;
            }

            if (_listGroups != null)
            {
                int itemCount = _listGroups.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualGroup group = _listGroups[i];
                    if (group != null)
                    {
                        group.Uninitialize();
                    }
                }
            }

            base.Uninitialize();
        }

        #endregion

        #endregion

        #region Private Methods

        #region CreateSteps Methods

        private void CreateSteps(BuildMultiStep listSteps, ConceptualGroup group)
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

            string helpStyle  = BuildStyleUtils.StyleFolder(
                outputStyle.StyleType);
            string workingDir = this.Context.WorkingDirectory;

            // 1. Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount = listFolders.Count;

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

            this.CreateTopicSteps(listSteps, group, sandcastleDir, helpStyle);
        }

        #endregion

        #region CreateTopicSteps Method

        private void CreateTopicSteps(BuildMultiStep listSteps, 
            ConceptualGroup group, string sandcastleDir, string helpStyle)            
        {
            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            string manifestFile      = group["$ManifestFile"];
            string buildManifestFile = group["$BuildManifestFile"];
            string configFile        = group["$ConfigurationFile"];
            string tocFile           = group["$TocFile"];
            string buildTocFile      = group["$BuildTocFile"];

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = context.WorkingDirectory;

            string tempText = null;

            // 3. Create the conceptual manifest
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\dsmanifesttomanifest.xsl" 
            //        buildmanifest.proj.xml /out:manifest.xml
            string application = Path.Combine(context.SandcastleToolsDirectory, 
                "XslTransform.exe");
            tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\dsmanifesttomanifest.xsl");
            string arguments = String.Format("/xsl:\"{0}\" {1} /out:{2}", 
                tempText, buildManifestFile, manifestFile);
            StepXslTransform manifestProcess = new StepXslTransform(workingDir,
                application, arguments);
            manifestProcess.LogTitle = String.Empty;
            manifestProcess.Message  = "Xsl Transformation - Creating conceptual manifest";
            manifestProcess.CopyrightNotice = 2;
            
            listSteps.Add(manifestProcess);

            // 4. Create the conceptual table of contents
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\dstoctotoc.xsl" 
            //       extractedfiles\toc.xml /out:toc.xml
            tempText = Path.Combine(sandcastleDir, 
                @"ProductionTransforms\dstoctotoc.xsl");
            arguments = String.Format("/xsl:\"{0}\" Extractedfiles\\{1} /out:{2}",
                tempText, buildTocFile, tocFile);
            StepXslTransform tocProcess = new StepXslTransform(workingDir,
                application, arguments);
            tocProcess.LogTitle = String.Empty;
            tocProcess.Message  = "Xsl Transformation - Creating conceptual TOC";
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
