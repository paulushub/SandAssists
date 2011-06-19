using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceToc : StepXslTransform
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

            BuildGroupContext groupContext = context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            bool buildResult = base.OnExecute(context);

            if (!buildResult)
            {
                return false;
            }

            buildResult = this.ProcessTocVisitors(context, groupContext);

            if (buildResult)
            {
                string tempText = context["$HelpHierarchicalToc"];

                string tocFilePath = String.Empty;
                if (!String.IsNullOrEmpty(tempText) && String.Equals(tempText,
                    Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    tocFilePath = Path.Combine(this.WorkingDirectory,
                       groupContext["$HierarchicalTocFile"]);
                }
                else
                {
                    tocFilePath = Path.Combine(this.WorkingDirectory,
                       groupContext["$TocFile"]);
                }

                if (!String.IsNullOrEmpty(tocFilePath) && File.Exists(tocFilePath))
                {
                    buildResult = ProcessRootToc(context, groupContext, tocFilePath);
                }
            }

            return buildResult;
        }

        #endregion

        #endregion

        #region Private Methods

        #region ProcessRootToc Method

        private bool ProcessRootToc(BuildContext context, 
            BuildGroupContext groupContext, string tocFilePath)
        {
            bool buildResult = true;

            string rootTopicId = _group.RootTopicId;
            if (String.IsNullOrEmpty(rootTopicId) ||
                !ConceptualUtils.IsValidId(rootTopicId))
            {
                return buildResult;
            }

            BuildLogger logger = context.Logger;

            BuildTocContext tocContext = context.TocContext;
            IBuildNamedList<BuildTopicTocInfo> relatedTopics =
                tocContext.RelatedTopics;
            if (relatedTopics == null || relatedTopics.Count == 0 ||
                relatedTopics[rootTopicId] == null)
            {   
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The related topic '{0}' for the reference group '{1}' is not defined.",
                        rootTopicId, _group.Name), BuildLoggerLevel.Warn);
                }

                return buildResult;
            }

            bool isRooted = Convert.ToBoolean(groupContext["$IsRooted"]);

            string tocFileBackup = Path.ChangeExtension(tocFilePath, ".tocbak");
            File.SetAttributes(tocFilePath, FileAttributes.Normal);
            File.Move(tocFilePath, tocFileBackup);

            XmlWriter writer = null;
            try
            {
                XmlWriterSettings writerSettings  = new XmlWriterSettings();
                writerSettings.Indent             = true;
                writerSettings.OmitXmlDeclaration = false;
                writer = XmlWriter.Create(tocFilePath, writerSettings);

                writer.WriteStartElement("topics"); // start: topics
                writer.WriteStartElement("topic");  // start: topic
                writer.WriteAttributeString("id", rootTopicId);
                writer.WriteAttributeString("file", rootTopicId);

                using (XmlReader reader = XmlReader.Create(tocFileBackup))
                {
                    if (reader.IsStartElement("topics"))
                    {
                        while (!reader.EOF)
                        {
                            if (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element &&
                                    String.Equals(reader.Name, "topic", 
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    if (isRooted)
                                    {
                                        isRooted = false;
                                        continue;
                                    }

                                    writer.WriteNode(reader, true);
                                }  
                            }
                        }
                    }
                }

                writer.WriteEndElement();           // end: topic
                writer.WriteEndElement();           // end: topics

                writer.Close();
                writer = null;

                return true;
            }
            catch (Exception ex)
            {                      
                File.Move(tocFileBackup, tocFilePath);

                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                buildResult = false;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }    

            return buildResult;
        }

        #endregion

        #region ProcessTocVisitors Method

        private bool ProcessTocVisitors(BuildContext context, BuildGroupContext groupContext)
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
                groupContext["$TocFile"]);
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

        #region ProcessDocuments Method

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

        #endregion

        #region PrepareVisitors Method

        private void PrepareVisitors(BuildContext context)
        {
            if (_dictVisitors == null)
            {
                _dictVisitors = new Dictionary<string, ReferenceTocVisitor>();
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
                        String.Equals(config.Category, "ReferenceTocVisitor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        ReferenceTocVisitor visitor = config.CreateVisitor() as ReferenceTocVisitor;
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
                        String.Equals(config.Category, "ReferenceTocVisitor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // Make sure there is a handler of this configuration...
                        ReferenceTocVisitor visitor = config.CreateVisitor() as ReferenceTocVisitor;
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

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
