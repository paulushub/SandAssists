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
    public class ConceptualEngine : BuildEngine
    {
        #region Private Fields

        private IList<string> _listFolders;
        private BuildFormatList _listFormats;
        private BuildList<ConceptualGroup> _listGroups;

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
                    _listGroups = new BuildList<ConceptualGroup>();
                }

                return _listGroups;
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
                _listGroups = new BuildList<ConceptualGroup>();
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

            int itemCount = _listGroups.Count;
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

                // Create the build dynamic properties...
                groupContext.CreateProperties(indexText);

                group.BeginSources(context);
            }

            _listFormats = new BuildFormatList();
            BuildFormatList listFormats = this.Settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;
                return;
            }

            itemCount    = listFormats.Count;
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
            string sandcastleDir   = this.Context.StylesDirectory;
            BuildStyle outputStyle = settings.Style;

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

            string helpStyle  = BuildStyle.StyleFolder(
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

        #region Uninitialize Method

        public override void Uninitialize()
        {
            if (_listGroups != null)
            {
                int itemCount = _listGroups.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualGroup group = _listGroups[i];
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
