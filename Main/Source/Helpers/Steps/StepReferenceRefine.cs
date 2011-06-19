using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceRefine : BuildStep
    {
        #region Private Fields

        [NonSerialized]
        private ReferenceGroup _group;

        [NonSerialized]
        private List<ReferenceConfiguration> _listConfigurations;

        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;

        [NonSerialized]
        private List<ReferenceDocument> _listDocuments;

        [NonSerialized]
        private Dictionary<string, ReferenceVisitor> _dictVisitors;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="StepReflectionRefine"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="StepReflectionRefine"/> class
        /// to the default properties or values.
        /// </summary>
        public StepReferenceRefine()
        {
            this.ConstructorDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepReflectionRefine"/> class with the
        /// specified working directory.
        /// </summary>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory.
        /// </param>
        public StepReferenceRefine(string workingDir)
            : base(workingDir)
        {
            this.ConstructorDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepReflectionRefine"/> class with the
        /// specified name and working directory.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/> containing the name of this build step.
        /// </param>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory of this
        /// build step.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="name"/> is empty.
        /// </exception>
        public StepReferenceRefine(string name, string workingDir)
            : base(name, workingDir)
        {
            this.ConstructorDefaults();
        }

        private void ConstructorDefaults()
        {
            this.LogTitle = "Refining the reflection files - filtering and cleaning";

            _dictVisitors = new Dictionary<string, ReferenceVisitor>();
        }

        #endregion

        #region Public Properties

        public IList<ReferenceDocument> Documents
        {
            get 
            { 
                return _listDocuments; 
            }
        }

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

        protected override bool OnExecute(BuildContext context)
        {
            Debug.Assert(_group != null);
            if (_group == null)
            {
                return false;
            }

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            // Create the documents...
            string reflectionFile = groupContext["$ReflectionFile"];
            string refInfoFile    = Path.ChangeExtension(reflectionFile, ".org");

            string workingDir = this.WorkingDirectory;
            _listDocuments = new List<ReferenceDocument>();
            // the reflection file...
            ReferenceDocument document = new ReferenceDocument(
                Path.Combine(workingDir, refInfoFile), 
                ReferenceDocumentType.Reflection);
            _listDocuments.Add(document);
            // the comment files...
            IList<string> commentFiles = groupContext.CommentFiles;
            if (commentFiles != null && commentFiles.Count != 0)
            {
                for (int i = 0; i < commentFiles.Count; i++)
                {
                    document = new ReferenceDocument(commentFiles[i],
                        ReferenceDocumentType.Comments);
                    _listDocuments.Add(document);
                }
            }

            if (_listDocuments == null || _listDocuments.Count == 0)
            {
                return false;
            }

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

            // 1. Create all the reference visitors...
            this.PrepareVisitors(context); 
            if ((_dictVisitors == null || _dictVisitors.Count == 0) ||
                (_listConfigurations == null || _listConfigurations.Count == 0))
            {
                return true;
            }

            // 2. Initialize all the reference visitors...
            ICollection<ReferenceVisitor> listVisitors = _dictVisitors.Values;
            foreach (ReferenceVisitor visitor in listVisitors)
            {
                visitor.Initialize(context, _group);
            }

            // 3. Process the configurations...
            this.ProcessDocuments(context); 

            // 4. Un-initialize all the reference visitors...
            foreach (ReferenceVisitor visitor in listVisitors)
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
            if (_listConfigurations.Count == 0)
            {
                return;
            }

            FileSystemWatcher fileWatcher = null;
            try
            {
                fileWatcher  = new FileSystemWatcher(); 

                // Initialize the configurations and get them ready for processing...
                for (int i = 0; i < _listConfigurations.Count; i++)
                {
                    _listConfigurations[i].Initialize(context);
                }

                int itemCount = _listDocuments.Count;  
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceDocument document = _listDocuments[i];
                    if (document == null || document.IsEmpty)
                    {
                        continue;
                    }

                    this.ProcessDocument(fileWatcher, context, document);
                }   
        
                // Un-initialize the configurations after the processing...
                for (int i = 0; i < _listConfigurations.Count; i++)
                {
                    _listConfigurations[i].Uninitialize();
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
            BuildContext context, ReferenceDocument document)
        {
            try
            {
                if (!document.BeginEdit(fileWatcher))
                {
                    return;
                }

                // For each configuration, retrieve the processor (visitor) and
                // process the document...
                int itemCount = _listConfigurations.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildConfiguration configuration = _listConfigurations[i];

                    string configName = configuration.Name;
                    if (_dictVisitors.ContainsKey(configName))
                    {
                        ReferenceVisitor visitor = _dictVisitors[configName];
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
                _dictVisitors = new Dictionary<string, ReferenceVisitor>();
            }

            _listConfigurations = new List<ReferenceConfiguration>();

            // List out enabled Sandcastle Assist and the Plugin configurations...
            IBuildNamedList<BuildConfiguration> dicAssistConfigs
                = _engineSettings.Configurations;
            if (dicAssistConfigs != null && dicAssistConfigs.Count != 0)
            {
                foreach (ReferenceConfiguration config in dicAssistConfigs)
                {
                    // It must be both enabled to be used and active/valid...
                    if (config.Enabled && config.IsActive &&
                        String.Equals(config.Category, "ReferenceVisitor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        ReferenceVisitor visitor = config.CreateVisitor();
                        if (visitor != null)
                        {
                            _listConfigurations.Add(config);

                            if (!_dictVisitors.ContainsKey(config.Name))
                            {
                                _dictVisitors.Add(config.Name, visitor);
                            }
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
                        String.Equals(config.Category, "ReferenceVisitor", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        ReferenceVisitor visitor = config.CreateVisitor();
                        if (visitor != null)
                        {
                            _listConfigurations.Add(config);

                            if (!_dictVisitors.ContainsKey(config.Name))
                            {
                                _dictVisitors.Add(config.Name, visitor);
                            }
                        }
                    }
                }
            }
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
