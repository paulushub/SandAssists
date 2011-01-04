using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    public class StepReferenceToc : StepXslTransform
    {
        #region Private Fields

        [NonSerialized]
        private ReferenceGroup _group;

        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;
        [NonSerialized]
        private List<ReferenceDocument> _listDocuments;
        [NonSerialized]
        private Dictionary<string, ReferenceTocVisitor> _dictVisitors;

        #endregion

        #region Constructors and Destructor

        public StepReferenceToc()
        {
        }

        public StepReferenceToc(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepReferenceToc(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
        {
            get 
            { 
                return _group; 
            }
            set 
            { 
                _group = value; 
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            Debug.Assert(_group != null);
            if (_group == null)
            {
                throw new BuildException(
                    "A build group is required, but none is attached to this task.");
            }

            bool buildResult = base.OnExecute(context);

            if (!buildResult)
            {
                return false;
            }

            buildResult = this.ProcessHierarchicalToc(context);

            return buildResult;
        }

        #endregion

        #endregion

        #region ProcessHierarchicalToc Method

        private bool ProcessHierarchicalToc(BuildContext context)
        {
            // We need the list of the available configurations from the
            // reference settings...
            BuildSettings settings = context.Settings;
            Debug.Assert(settings != null,
                "The settings is not associated with the context.");
            if (settings == null)
            {
                return false;
            }
            BuildEngineSettingsList listSettings = settings.EngineSettings;
            Debug.Assert(listSettings != null,
                "The settings does not include the engine settings.");
            if (listSettings == null || listSettings.Count == 0)
            {
                return false;
            }
            _engineSettings = listSettings[BuildEngineType.Reference] as ReferenceEngineSettings;

            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                return false;
            }

            string tocFilePath = Path.Combine(this.WorkingDirectory,
                _group["$TocFile"]);
            if (File.Exists(tocFilePath))
            {
                _listDocuments = new List<ReferenceDocument>();

                ReferenceDocument document = new ReferenceDocument(
                    tocFilePath, ReferenceDocumentType.TableOfContents);

                _listDocuments.Add(document);
            }

            if (_listDocuments == null || _listDocuments.Count == 0)
            {
                return false;
            }

            // 1. Create all the reference visitors...
            this.PrepareVisitors(context);
            if (_dictVisitors == null || _dictVisitors.Count == 0)
            {
                return true;
            }

            // 2. Initialize all the reference visitors...
            ICollection<ReferenceTocVisitor> listVisitors = _dictVisitors.Values;
            foreach (ReferenceTocVisitor visitor in listVisitors)
            {
                visitor.Initialize(context, _group);
            }

            // 3. Process the configurations...
            this.ProcessDocuments(context);

            // 4. Un-initialize all the reference visitors...
            foreach (ReferenceTocVisitor visitor in listVisitors)
            {
                visitor.Uninitialize();
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void ProcessDocuments(BuildContext context)
        {
            if (_listDocuments == null || _listDocuments.Count == 0)
            {
                return;
            }

            List<BuildConfiguration> listConfigurations =
                new List<BuildConfiguration>();

            // List out enabled Sandcastle Assist and the Plugin configurations...
            IBuildNamedList<BuildConfiguration> dicAssistConfigs
                = _engineSettings.Configurations;
            if (dicAssistConfigs != null && dicAssistConfigs.Count != 0)
            {
                foreach (ReferenceConfiguration config in dicAssistConfigs)
                {
                    // It must be both enabled to be used and active/valid...
                    if (config.Enabled && config.IsActive &&
                        String.Equals(config.Category, "ReferenceTocVisitor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        if (_dictVisitors.ContainsKey(config.Name))
                        {
                            listConfigurations.Add(config);
                        }
                    }
                }
            }
            IBuildNamedList<BuildConfiguration> dicPluginConfigs
                = _engineSettings.PluginConfigurations;
            if (dicPluginConfigs != null && dicPluginConfigs.Count != 0)
            {
                foreach (ReferenceConfiguration config in dicPluginConfigs)
                {
                    // It must be both enabled to be used and active/valid...
                    if (config.Enabled && config.IsActive &&
                        String.Equals(config.Category, "ReferenceTocVisitor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        if (_dictVisitors.ContainsKey(config.Name))
                        {
                            listConfigurations.Add(config);
                        }
                    }
                }
            }
            if (listConfigurations.Count == 0)
            {
                return;
            }
            // Initialize the configurations and get them ready for processing...
            for (int i = 0; i < listConfigurations.Count; i++)
            {
                listConfigurations[i].Initialize(context);
            }

            FileSystemWatcher fileWatcher = null;
            try
            {
                fileWatcher = new FileSystemWatcher();
                int itemCount = _listDocuments.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceDocument document = _listDocuments[i];
                    if (document == null || document.IsEmpty)
                    {
                        continue;
                    }

                    this.ProcessDocument(fileWatcher, context,
                        document, listConfigurations);
                }

                // Un-initialize the configurations after the processing...
                for (int i = 0; i < listConfigurations.Count; i++)
                {
                    listConfigurations[i].Uninitialize();
                }
            }
            finally
            {
                if (fileWatcher != null)
                {
                    fileWatcher.EnableRaisingEvents = false;
                    fileWatcher.Dispose();
                    fileWatcher = null;
                }
            }
        }

        private void ProcessDocument(FileSystemWatcher fileWatcher,
            BuildContext context, ReferenceDocument document,
            IList<BuildConfiguration> configurations)
        {
            try
            {
                if (!document.BeginEdit(fileWatcher))
                {
                    return;
                }

                // For each configuration, retrieve the processor (visitor) and
                // process the document...
                int itemCount = configurations.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildConfiguration configuration = configurations[i];

                    string configName = configuration.Name;
                    if (_dictVisitors.ContainsKey(configName))
                    {
                        ReferenceTocVisitor visitor = _dictVisitors[configName];
                        if (visitor != null && visitor.IsInitialized)
                        {
                            visitor.Visit(document);
                        }
                    }
                }
            }
            finally
            {
                document.EndEdit();
            }
        }

        private void PrepareVisitors(BuildContext context)
        {
            if (_dictVisitors == null)
            {
                _dictVisitors = new Dictionary<string, ReferenceTocVisitor>();
            }

            _dictVisitors.Add(
                ReferenceTocExcludeConfiguration.ConfigurationName,
                new ReferenceTocExcludeVisitor());
            _dictVisitors.Add(
                ReferenceTocLayoutConfiguration.ConfigurationName,
                new ReferenceTocLayoutVisitor());
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
