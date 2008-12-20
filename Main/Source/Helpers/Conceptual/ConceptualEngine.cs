using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Steps;
using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualEngine : BuildEngine
    {
        #region Private Fields

        private ConceptualGroup       _curGroup;
        private BuildMultiStep        _listSteps;
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
            BuildContext context, BuildConfiguration configuration)
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

                if (!group.Initialize(settings))
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

        public override BuildStep CreateSteps(BuildGroup group)
        {
            ConceptualGroup curGroup = group as ConceptualGroup;
            if (curGroup == null)
            {
                return null;
            }

            if (this.CreateSteps(curGroup))
            {
                return _listSteps;
            }

            return null;
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

                    if (group.Initialize(settings))
                    {
                        _curGroup = group;

                        this.CreateSteps(group);

                        buildResult = this.RunSteps(_listSteps.Steps);

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

            bool cleanIntermediate = settings.CleanIntermediate;
            if (cleanIntermediate)
            {
                try
                {
                    CleanUpIntermediate();
                }
                catch (Exception ex)
                {
                    BuildLogger outputLogger = this.Logger;
                    if (outputLogger != null)
                    {
                        outputLogger.WriteLine(ex);
                    }
                }    
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

        private bool CreateSteps(ConceptualGroup group)
        {
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return false;
            }

            BuildSettings settings = this.Settings;
            BuildStyle outputStyle = settings.Style;
            string sandcastleDir   = settings.StylesDirectory;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return false;
            }

            _listSteps = new BuildMultiStep();

            BuildStyleType styleType = outputStyle.StyleType;
            string helpStyle = styleType.ToString();
            if (styleType == BuildStyleType.Whidbey)
            {
                helpStyle = BuildStyleType.Vs2005.ToString();
            }

            return CreateTopicSteps(group, sandcastleDir, helpStyle);
        }

        #endregion

        #region CreateTopicSteps Method

        private bool CreateTopicSteps(ConceptualGroup group, string sandcastleDir, 
            string helpStyle)            
        {
            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;
            int formatCount = _listFormats.Count;

            string manifestFile      = group["$ManifestFile"];
            string buildManifestFile = group["$BuildManifestFile"];
            string configFile        = group["$ConfigurationFile"];
            string tocFile           = group["$TocFile"];
            string buildTocFile      = group["$BuildTocFile"];

            // Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount = listFolders.Count;

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = settings.WorkingDirectory;

            string tempText = null;

            // 2. Handle the resources...
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
                    if (listResources != null && listResources.Count > 0)
                    {
                        StepDirectoryCopy copyResources = new StepDirectoryCopy(
                            workingDir);
                        int itemCount = listResources.Count;

                        for (int i = 0; i < itemCount; i++)
                        {
                            ResourceItem resource = listResources[i];
                            if (resource != null && resource.IsEmpty == false)
                            {
                                string destFolder = resource.Destination;
                                copyResources.Add(resource.Source, destFolder);

                                // Add this to the output folders so that it is copied
                                // to the final output/build directories...
                                if (destFolder.StartsWith("Output", 
                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    DirectoryInfo info = new DirectoryInfo(destFolder);
                                    destFolder = info.Name;
                                    if (String.IsNullOrEmpty(destFolder) == false &&
                                        dicFolders.ContainsKey(destFolder) == false)
                                    {
                                        dicFolders.Add(destFolder, true);
                                        listFolders.Add(destFolder);
                                    }
                                }
                            }
                        }

                        _listSteps.Add(copyResources);
                    }
                }
                // resource folder might have being added...
                folderCount = listFolders.Count; 
            }

            // 3. Create the conceptual manifest
            // XslTransform.exe 
            // /xsl:"%DXROOT%\ProductionTransforms\dsmanifesttomanifest.xsl" 
            //        buildmanifest.proj.xml /out:manifest.xml
            string application = "XslTransform.exe";
            tempText = Path.Combine(sandcastleDir,
                @"ProductionTransforms\dsmanifesttomanifest.xsl");
            string arguments = String.Format("/xsl:\"{0}\" {1} /out:{2}", 
                tempText, buildManifestFile, manifestFile);
            StepXslTransform manifestProcess = new StepXslTransform(workingDir,
                application, arguments);
            manifestProcess.Message = "XslTransform Tool - Conceptual Manifest";
            manifestProcess.CopyrightNotice = 2;
            _listSteps.Add(manifestProcess);

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
            tocProcess.Message = "XslTransform Tool - Creating Conceptual TOC";
            tocProcess.CopyrightNotice = 2;
            _listSteps.Add(tocProcess);

            // 6. Assemble the help files using the BuildAssembler
            // BuildAssembler.exe /config:Project.config manifest.xml
            application = "BuildAssembler.exe";
            //arguments = "/config:Conceptual.config Manifest.xml";
            arguments = String.Format(" /config:{0} {1}",
                configFile, manifestFile);
            StepAssembler buildAssProcess = new StepAssembler(workingDir,
                application, arguments);
            buildAssProcess.Group   = group;
            buildAssProcess.Message = "BuildAssembler Tool";
            buildAssProcess.CopyrightNotice = 2;
            _listSteps.Add(buildAssProcess);

            // 7. Create the final "Help" sub-directories
            StepDirectoryCreate helpDirs = new StepDirectoryCreate(workingDir);
            // 8. Copy outputs to the "Help" sub-directories
            StepDirectoryCopy copyDirs = new StepDirectoryCopy(workingDir);
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format  = _listFormats[i];
                string helpOutputDir = format.OutputFolder;

                for (int j = 0; j < folderCount; j++)
                {
                    string folder = listFolders[j];
                    tempText = String.Format(@"{0}\{1}", helpOutputDir, folder);
                    helpDirs.Add(tempText);
                    copyDirs.Add(String.Format(@"Output\{0}\", folder), tempText);
                }
            }
            _listSteps.Add(helpDirs);
            _listSteps.Add(copyDirs);

            return true;
        }

        #endregion

        #region CleanUpIntermediate

        private void CleanUpIntermediate()
        {
            BuildSettings settings = this.Settings;
            if (settings == null)
            {
                return;
            }

            string workingDir = settings.WorkingDirectory;

            string dduexmlDir   = Path.Combine(workingDir, "DdueXml");
            string compDir      = Path.Combine(workingDir, "XmlComp");
            string extractedDir = Path.Combine(workingDir, "ExtractedFiles");

            if (Directory.Exists(dduexmlDir))
            {
                Directory.Delete(dduexmlDir, true);
            }
            if (Directory.Exists(compDir))
            {
                Directory.Delete(compDir, true);
            }
            if (Directory.Exists(extractedDir))
            {
                Directory.Delete(extractedDir, true);
            }

            int groupCount = (_listGroups == null) ? 0 : _listGroups.Count;
            for (int i = 0; i < groupCount; i++)
            {
                string manifestFile = _listGroups[i].ManifestFile;
                if (!String.IsNullOrEmpty(manifestFile) && File.Exists(manifestFile))
                {
                    File.Delete(manifestFile);
                }
            }
            // Lookup and delete the manifest and toc dynamic files...
            string tempFile = Path.Combine(workingDir, "Manifest.xml");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, "ApiManifest.xml");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, "reflection.org");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, "reflection.xml");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, "TopicsToc.xml");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, "ApiToc.xml");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            tempFile = Path.Combine(workingDir, BuildToc.HelpToc);
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
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
