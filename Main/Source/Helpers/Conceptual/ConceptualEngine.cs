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
            BuildExceptions.NotNull(group, "group");

            if (group.IsEmpty || group.Exclude)
            {
                throw new BuildException(
                    "The build engine requires a valid build group.");
            }

            if (_listGroups == null)
            {
                _listGroups = new List<ConceptualGroup>();
            }

            _listGroups.Add(group);
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

            _listFormats = new List<BuildFormat>();

            BuildFormatList listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;
                return;
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
                return;
            }
            itemCount = _listGroups.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualGroup group = _listGroups[i];

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
                    String.Format("TopicsSharedContent{0}.xml", indexText);
                groupContext["$TocFile"] =
                    String.Format("TopicsToc{0}.xml", indexText);
                groupContext["$ManifestFile"] =
                    String.Format("TopicsManifest{0}.xml", indexText);
                groupContext["$ConfigurationFile"] =
                    String.Format("TopicsBuildAssembler{0}.config", indexText);
                groupContext["$MetadataFile"] =
                    String.Format("TopicsMetadata{0}.xml", indexText);
                groupContext["$ProjSettings"] =
                    String.Format("TopicsProjectSettings{0}.xml", indexText);
                groupContext["$ProjSettingsLoc"] =
                    String.Format("TopicsProjectSettings{0}.loc.xml", indexText);
                groupContext["$VersionFile"] =
                    String.Format("TopicsVersions{0}.xml", indexText);
                groupContext["$TokenFile"] = String.Format("TopicsTokens{0}.xml", indexText);
                groupContext["$IndexFile"] = String.Format("TopicsIndex{0}.xml", indexText);

                groupContext["$DdueXmlDir"] = String.Format("DdueXml{0}", indexText);
                groupContext["$DdueXmlCompDir"] = String.Format("DdueXmlComp{0}", indexText);
                groupContext["$DdueHtmlDir"] = String.Format("DdueHtml{0}", indexText);

                groupContext["$GroupIndex"] = indexText;
                
                group.Initialize(context);

                if (!group.IsInitialized)
                {
                    this.IsInitialized = false;
                    break;
                }
            }
        }

        #endregion

        #region CreateInitialSteps Method

        public override BuildStep CreateInitialSteps(BuildGroup group)
        {
            ConceptualGroup curGroup = group as ConceptualGroup;
            if (curGroup == null)
            {
                throw new BuildException("The build engine requires conceptual group.");
            }

            //this.CreateSteps(listSteps, curGroup);
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return null;
            }

            BuildSettings settings = this.Settings;
            BuildStyle outputStyle = settings.Style;
            string sandcastleDir   = settings.StylesDirectory;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return null;
            }

            BuildMultiStep listSteps = new BuildMultiStep();
            listSteps.Message     = "Conceptual topics for the group: " + group.Name;
            listSteps.LogTitle    = String.Empty;
            listSteps.LogTimeSpan = true;

            // 1. Initialize the conceptual topics...
            StepConceptualInit stepInit = new StepConceptualInit(curGroup);
            stepInit.Message  = "Initializing and generating conceptual contents";
            stepInit.LogTitle = String.Empty;

            listSteps.Add(stepInit);

            string helpStyle  = BuildStyleUtils.StyleFolder(
                outputStyle.StyleType);
            string workingDir = this.Context.WorkingDirectory;

            // 2. Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount = listFolders.Count;

            // 3. Handle the resources...
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
            if (group.GroupType != BuildGroupType.Conceptual)
            {
                throw new BuildException("The build engine requires conceptual group.");
            }

            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string manifestFile      = groupContext["$ManifestFile"];
            string buildManifestFile = groupContext["$BuildManifestFile"];
            string configFile        = groupContext["$ConfigurationFile"];
            string tocFile           = groupContext["$TocFile"];
            string buildTocFile      = groupContext["$BuildTocFile"];

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
                    
                    group.Initialize(context);
                    if (group.IsInitialized)
                    {
                        _curGroup = group;

                        BuildMultiStep listSteps = new BuildMultiStep();
                        BuildStep initialSteps = this.CreateInitialSteps(group);
                        if (initialSteps != null)
                        {
                            listSteps.Add(initialSteps);
                        }
                        BuildStep finalSteps = this.CreateFinalSteps(group);
                        if (finalSteps != null)
                        {
                            listSteps.Add(finalSteps);
                        }

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

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
