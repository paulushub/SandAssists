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

        private List<BuildStep>        _listSteps;
        private List<BuildFormat>     _listFormats;
        private List<ConceptualGroup>  _listGroups;

        #endregion

        #region Constructors and Destructor

        public ConceptualEngine()
        {
            Reset();
        }

        public ConceptualEngine(BuildSettings settings)
            : base(settings)
        {
            Reset();
        }

        public ConceptualEngine(BuildLoggers logger)
            : base(logger)
        {
            Reset();
        }

        public ConceptualEngine(BuildSettings settings, BuildLoggers logger, 
            BuildContext context, BuildConfiguration configuration)
            : base(settings, logger, context, configuration)
        {
            Reset();
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

        public override IList<BuildStep> Steps
        {
            get
            {
                return _listSteps;
            }
        }

        public override IList<BuildGroup> Groups
        {
            get
            {
                List<BuildGroup> listGroups = new List<BuildGroup>();
                if (_listGroups != null && _listGroups.Count != 0)
                {
                    int itemCount = _listGroups.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        listGroups.Add(_listGroups[i]);
                    }
                }

                return listGroups.AsReadOnly();
            }
        }

        public IList<ConceptualGroup> ConceptualGroups
        {
            get
            {
                return _listGroups;
            }
        }

        #endregion

        #region Public Methods

        #region AddGroup Method

        public void AddGroup(ConceptualGroup group)
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

            IList<BuildFormat> listFormats = this.Settings.OutputFormats;
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

            return initResult;
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

            BuildLogger logger    = this.Logger;
            BuildSettings settings = this.Settings;

            string workingDir = settings.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return buildResult;
            }

            ConceptualConfigurator assembler = new ConceptualConfigurator();
            try
            {
                assembler.Initialize(this);

                string configFile  = String.Empty;
                string finalConfig = String.Empty;
                if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
                {
                    configFile  = Path.Combine(configDir,  "Conceptual.config");
                    finalConfig = Path.Combine(workingDir, "Conceptual.config");
                }
                if (File.Exists(configFile) == false)
                {
                    configFile = String.Empty;
                }

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
                        // 1. Configure the build assembler...
                        if (!String.IsNullOrEmpty(configFile))
                        {
                            assembler.Configure(group, configFile, finalConfig);
                        }

                        this.CreateSteps(group);

                        buildResult = this.RunSteps(_listSteps);

                        if (buildResult)
                        {
                            initCount++;
                        }
                        else
                        {
                            break;
                        }
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

                    group.Uninitialize();
                }

                assembler.Uninitialize();

                buildResult = (initCount == itemCount);
            }
            catch (Exception ex)
            {
                assembler.Uninitialize();

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

        private void Reset()
        {
            _listGroups = new List<ConceptualGroup>();
        }

        #region CreateSteps Methods

        private bool CreateSteps(ConceptualGroup group)
        {
            if (_listFormats == null || _listFormats.Count == 0)
            {
                return false;
            }

            BuildSettings settings  = this.Settings;
            BuildStyle outputStyle = settings.Style;
            string sandcastleDir    = settings.StylesDirectory;

            if (String.IsNullOrEmpty(sandcastleDir))
            {
                return false;
            }

            _listSteps = new List<BuildStep>();

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
            BuildSettings settings = this.Settings;
            int formatCount = _listFormats.Count;

            // Ensure that we have a valid list of folders...
            IList<string> listFolders = new List<string>();
            IDictionary<string, bool> dicFolders = this.GetOutputFolders(listFolders);
            int folderCount = listFolders.Count;

            BuildStyleType outputStyle = settings.Style.StyleType;
            string workingDir = settings.WorkingDirectory;

            string tempText = null;

            // In the beginning... 
            // Close any current viewer of the compiled helps...
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];
                BuildStep closeViewer = format.CreateStep(this,
                    BuildStepType.CloseViewer, workingDir);
                
                if (closeViewer != null)
                {
                    _listSteps.Add(closeViewer);
                }
            }

            if (settings.IsCombinedBuild == false)
            {
                // 1. Clean up the current build output directories
                StepDirectoryDelete deleteDirs = new StepDirectoryDelete(workingDir);
                deleteDirs.Add("Output");
                deleteDirs.Add("Intellisense");
                _listSteps.Add(deleteDirs);

                // 2. Recreate the output directories
                StepDirectoryCreate createDir = new StepDirectoryCreate(workingDir);
                createDir.Add("Intellisense");
                for (int i = 0; i < folderCount; i++)
                {
                    createDir.Add(@"Output\" + listFolders[i]);
                }
                for (int i = 0; i < formatCount; i++)
                {
                    BuildFormat format = _listFormats[i];
                    createDir.Add(Path.Combine("Output", format.FormatFolder));
                    createDir.Add(format.OutputFolder);
                    // We have to delete the any existing output folder...
                    deleteDirs.Add(format.OutputFolder);
                }
                _listSteps.Add(createDir);

                // 3. Copy the resources files: icons, styles and scripts...
                StepDirectoryCopy copyOutput = new StepDirectoryCopy(workingDir);
                tempText = String.Format(@"Presentation\{0}\icons\", helpStyle);
                tempText = Path.Combine(sandcastleDir, tempText);
                copyOutput.Add(tempText, @"Output\icons\");

                tempText = String.Format(@"Presentation\{0}\scripts\",
                    helpStyle);
                tempText = Path.Combine(sandcastleDir, tempText);
                copyOutput.Add(tempText, @"Output\scripts\");

                if (outputStyle == BuildStyleType.Whidbey)
                {
                    tempText = String.Format(@"Presentation\{0}\styles\Whidbey",
                        helpStyle);
                }
                else
                {
                    tempText = String.Format(@"Presentation\{0}\styles\",
                        helpStyle);
                }
                tempText = Path.Combine(sandcastleDir, tempText);
                copyOutput.Add(tempText, @"Output\styles\");

                _listSteps.Add(copyOutput);
            }

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
            string arguments = String.Format(
                "/xsl:\"{0}\" BuildManifest.proj.xml /out:Manifest.xml", tempText);
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
            arguments = String.Format(
                "/xsl:\"{0}\" Extractedfiles\\Toc.xml /out:TopicsToc.xml",
                tempText);
            StepXslTransform tocProcess = new StepXslTransform(workingDir,
                application, arguments);
            tocProcess.Message = "XslTransform Tool - Conceptual TOC";
            tocProcess.CopyrightNotice = 2;
            _listSteps.Add(tocProcess);

            // 6. Assemble the help files using the BuildAssembler
            // BuildAssembler.exe /config:Project.config manifest.xml
            application = "BuildAssembler.exe";
            arguments = "/config:Conceptual.config Manifest.xml";
            StepAssembler buildAssProcess = new StepAssembler(workingDir,
                application, arguments);
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

            // Merge the table of contents...
            if (settings.IsCombinedBuild == true)
            {
                StepTocMerge tocMerge = new StepTocMerge(workingDir,
                    "TopicsToc.xml", "ApiToc.xml", "MergedToc.xml");

                _listSteps.Add(tocMerge);
            }
            
            // 9. Prepare the help html files, and create the html project
            // Build and/or compile the help files...
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                BuildStep compileHelp = format.CreateStep(this,
                    BuildStepType.Compilation, workingDir);

                if (compileHelp != null)
                {
                    _listSteps.Add(compileHelp);
                }
            }

            // 12. Finally, delete the intermediate "Output" directory...
            if (settings.CleanIntermediate)
            {
                StepDirectoryDelete outputDir = new StepDirectoryDelete(workingDir);
                outputDir.Add("Output");
                _listSteps.Add(outputDir);
            }

            // In the end...
            for (int i = 0; i < formatCount; i++)
            {
                BuildFormat format = _listFormats[i];

                if (format != null && format.ViewOnBuild)
                {
                    BuildStep startViewer = format.CreateStep(this,
                        BuildStepType.StartViewer, workingDir);

                    if (startViewer != null)
                    {
                        _listSteps.Add(startViewer);
                    }
                }
            }

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
            tempFile = Path.Combine(workingDir, "MergedToc.xml");
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
