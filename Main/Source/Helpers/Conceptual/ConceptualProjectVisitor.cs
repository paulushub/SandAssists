using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// <para>
    /// This is a conceptual group visitor which prepares the conceptual 
    /// project information for compilation.
    /// </para>
    /// <para>
    /// It creates the table of contents, the manifest, the metadata and
    /// the companion files required by the conceptual build process. It also
    /// creates the topic files in the format required by the build process.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some conceptual content sources may directly create some of these
    /// files, making part of these processing irrelevant.
    /// </para>
    /// <para>
    /// This uses private inner classes modeled on the adapter pattern to
    /// process the request.
    /// </para>
    /// </remarks>
    public sealed class ConceptualProjectVisitor : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualProjectVisitor";

        #endregion

        #region Private Fields

        private bool   _outputTopics;
        private bool   _applyAdapters;

        private string _ddueXmlDir;
        private string _ddueCompDir;
        private string _ddueHtmlDir;

        [NonSerialized]
        private ConceptualTocAdapter      _tocAdapter;
        [NonSerialized]
        private ConceptualMetadataAdapter _metadataAdapter;
        [NonSerialized]
        private ConceptualManifestAdapter _manifestAdapters;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualProjectVisitor"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualProjectVisitor"/> class
        /// to the default values.
        /// </summary>
        public ConceptualProjectVisitor()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualProjectVisitor"/> class
        /// with the specified group visitor name.
        /// </summary>
        /// <param name="visitorName">
        /// A <see cref="System.String"/> specifying the name of this group visitor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="visitorName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="visitorName"/> is empty.
        /// </exception>
        private ConceptualProjectVisitor(string visitorName)
            : base(visitorName)
        {
            _outputTopics  = true;
            _applyAdapters = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value specifying whether to apply the internal
        /// adapters, which are used to create the table of contents, manifest
        /// and metadata files.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the project files must
        /// be created; otherwise, this is <see langword="false"/>. The default
        /// is <see langword="true"/>.
        /// </value>
        public bool ApplyAdapters
        {
            get
            {
                return _applyAdapters;
            }
            set
            {
                _applyAdapters = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether to create the topic files
        /// in the format required by the build process and corresponding 
        /// companion files.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the topic and companion
        /// files are created for the build process; otherwise, this is
        /// <see langword="false"/>.
        /// </value>
        public bool OutputTopics
        {
            get
            {
                return _outputTopics;
            }
            set
            {
                _outputTopics = value;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies the processing operations defined by this visitor to the
        /// specified conceptual group.
        /// </summary>
        /// <param name="group">
        /// The <see cref="ConceptualGroup">conceptual group</see> to which 
        /// the processing operations defined by this visitor will be applied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="group"/> is <see langword="null"/>.
        /// </exception>
        protected override void OnVisit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "ConceptualProjectVisitor: The conceptual table of contents generator is not initialized.");
            }

            BuildContext context = this.Context;
            BuildLogger logger   = context.Logger;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }

            _tocAdapter       = new ConceptualTocAdapter();
            _metadataAdapter  = new ConceptualMetadataAdapter();
            _manifestAdapters = new ConceptualManifestAdapter();
                         
            try
            {
                _tocAdapter.Initialize(group, context);
                _metadataAdapter.Initialize(group, context);
                _manifestAdapters.Initialize(group, context);

                string workingDir = context.WorkingDirectory;

                _ddueXmlDir  = Path.Combine(workingDir, groupContext["$DdueXmlDir"]);
                _ddueCompDir = Path.Combine(workingDir, groupContext["$DdueXmlCompDir"]);
                _ddueHtmlDir = Path.Combine(workingDir, groupContext["$DdueHtmlDir"]);

                if (!Directory.Exists(_ddueXmlDir))
                {
                    Directory.CreateDirectory(_ddueXmlDir);
                }
                if (!Directory.Exists(_ddueCompDir))
                {
                    Directory.CreateDirectory(_ddueCompDir);
                }
                if (!Directory.Exists(_ddueHtmlDir))
                {
                    Directory.CreateDirectory(_ddueHtmlDir);
                }

                ConceptualContent content = group.Content;
                int itemCount = content.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    this.WriteTopic(content[i]);
                }    

                IList<ConceptualRelatedTopic> relatedTopics = content.RelatedTopics;
                if (relatedTopics != null && relatedTopics.Count != 0)
                {
                    itemCount = relatedTopics.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        ConceptualRelatedTopic topicItem = relatedTopics[i];
                        if (topicItem == null || topicItem.IsEmpty)
                        {
                            continue;
                        }

                        this.WriteTopic(topicItem);
                    }
                }      
            }
            finally
            {
                _tocAdapter.Uninitialize();
                _metadataAdapter.Uninitialize();
                _manifestAdapters.Uninitialize();
            }     

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        private void WriteTopic(ConceptualItem item)
        {
            if (item == null || item.IsEmpty)
            {
                return;
            }

            // Signal the beginning of a topic...
            this.OnBeginTopic(item);

            // Signal the writing of a topic...
            this.OnWriteTopic(item);

            // Create the output format for the current topic...
            if (_outputTopics)
            {
                item.CreateTopic(_ddueXmlDir, _ddueCompDir, _ddueHtmlDir);
            }

            // Write the sub-topics
            IList<ConceptualItem> listItems = item.Items;

            int itemCount = listItems == null ? 0 : listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.WriteTopic(listItems[i]);
            }

            // Signal the end of a topic...
            this.OnEndTopic(item);
        }

        private void OnBeginTopic(ConceptualItem item)
        {
            if (item == null || item.IsEmpty || !_applyAdapters)
            {
                return;
            }

            _tocAdapter.OnBeginTopic(item);
            _metadataAdapter.OnBeginTopic(item);
            _manifestAdapters.OnBeginTopic(item);
        }

        private void OnWriteTopic(ConceptualItem item)
        {
            if (item == null || item.IsEmpty || !_applyAdapters)
            {
                return;
            }

            _tocAdapter.OnWriteTopic(item);
            _metadataAdapter.OnWriteTopic(item);
            _manifestAdapters.OnWriteTopic(item);
        }

        private void OnEndTopic(ConceptualItem item)
        {
            if (item == null || item.IsEmpty || !_applyAdapters)
            {
                return;
            }

            _tocAdapter.OnEndTopic(item);
            _metadataAdapter.OnEndTopic(item);
            _manifestAdapters.OnEndTopic(item);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (_tocAdapter != null)
            {
                _tocAdapter.Dispose();
                _tocAdapter = null;
            }
            if (_metadataAdapter != null)
            {
                _metadataAdapter.Dispose();
                _metadataAdapter = null;
            }
            if (_manifestAdapters != null)
            {
                _manifestAdapters.Dispose();
                _manifestAdapters = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region ConceptualProjectAdapter Class

        public abstract class ConceptualProjectAdapter : BuildObject, IDisposable
        {
            #region Constructors and Destructor

            protected ConceptualProjectAdapter()
            {
            }

            ~ConceptualProjectAdapter()
            {
                this.Dispose(false);
            }

            #endregion

            #region Public Methods

            public abstract void Initialize(ConceptualGroup group,
                BuildContext context);
            public abstract void Uninitialize();

            public abstract void OnBeginTopic(ConceptualItem item);
            public abstract void OnWriteTopic(ConceptualItem item);
            public abstract void OnEndTopic(ConceptualItem item);

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
            }

            #endregion
        }

        #endregion

        #region ConceptualTocAdapter Class

        public sealed class ConceptualTocAdapter : ConceptualProjectAdapter
        {
            #region Private Fields

            private XmlWriter _tocWriter;

            #endregion

            #region Constructors and Destructor

            public ConceptualTocAdapter()
            {
            }

            #endregion

            #region Public Methods

            public override void Initialize(ConceptualGroup group,
                BuildContext context)
            {
                string workingDir = context.WorkingDirectory;

                BuildGroupContext groupContext = context.GroupContexts[group.Id];

                string tocFile = Path.Combine(workingDir, 
                    groupContext["$TocFile"]);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                _tocWriter = XmlWriter.Create(tocFile, settings);
                _tocWriter.WriteStartDocument();
                _tocWriter.WriteStartElement("topics"); // start - topics
            }

            public override void Uninitialize()
            {
                if (_tocWriter != null)
                {
                    _tocWriter.WriteEndElement();       // end - topics
                    _tocWriter.WriteEndDocument();
                    _tocWriter.Close();
                    _tocWriter = null;
                }
            }

            public override void OnBeginTopic(ConceptualItem item)
            {
                if (item == null || item.ItemType == ConceptualItemType.Related)
                {
                    return;
                }

                _tocWriter.WriteStartElement("topic");
                _tocWriter.WriteAttributeString("id",   item.TopicId);
                _tocWriter.WriteAttributeString("file", item.TopicId);
            }

            public override void OnWriteTopic(ConceptualItem item)
            {   
            }

            public override void OnEndTopic(ConceptualItem item)
            {
                if (item == null || item.ItemType == ConceptualItemType.Related)
                {
                    return;
                }

                _tocWriter.WriteEndElement();
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_tocWriter != null)
                {
                    _tocWriter.Close();
                    _tocWriter = null;
                }

                base.Dispose(disposing);
            }

            #endregion
        }

        #endregion

        #region ConceptualMetadataAdapter Class

        public sealed class ConceptualMetadataAdapter : ConceptualProjectAdapter
        {
            #region Private Fields

            private string _topicTypeId;
            private string _runningHeaderText;

            private XmlWriter _metadataWriter; 

            #endregion

            #region Constructors and Destructor

            public ConceptualMetadataAdapter()
            {
                _topicTypeId       = "1FE70836-AA7D-4515-B54B-E10C4B516E50"; // Generic conceptual topic
                _runningHeaderText = "runningHeaderText"; // "b1b997af-3127-4001-b77b-5263e4496939"
            }

            #endregion

            #region Public Methods

            public override void Initialize(ConceptualGroup group,
                BuildContext context)
            {
                string workingDir = context.WorkingDirectory;

                BuildGroupContext groupContext = context.GroupContexts[group.Id];

                string metadataFile = Path.Combine(workingDir,
                    groupContext["$MetadataFile"]);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                _metadataWriter = XmlWriter.Create(metadataFile, settings);
                _metadataWriter.WriteStartDocument();
                _metadataWriter.WriteStartElement("metadata"); // start - metadata
                _metadataWriter.WriteAttributeString("fileAssetGuid",
                    Guid.NewGuid().ToString());
                _metadataWriter.WriteAttributeString("assetTypeId", "ContentMetadata");
            }

            public override void Uninitialize()
            {
                if (_metadataWriter != null)
                {
                    _metadataWriter.WriteEndElement();       // end - metadata
                    _metadataWriter.WriteEndDocument();

                    _metadataWriter.Close();
                    _metadataWriter = null;
                }
            }

            public override void OnBeginTopic(ConceptualItem item)
            {
            }

            public override void OnWriteTopic(ConceptualItem item)
            {
                ConceptualItemType itemType = item.ItemType;

                if (itemType != ConceptualItemType.Topic &&
                    itemType != ConceptualItemType.Related)
                {
                    return;
                }          

                _metadataWriter.WriteStartElement("topic");
                _metadataWriter.WriteAttributeString("id", item.TopicId);
                _metadataWriter.WriteAttributeString("revisionNumber",
                    item.TopicRevisions.ToString());

                Version version = item.TopicVersion;
                if (version != null)
                {
                    _metadataWriter.WriteStartElement("item"); // start-item
                    _metadataWriter.WriteAttributeString("id", "PBM_FileVersion");
                    _metadataWriter.WriteString(version.ToString());
                    _metadataWriter.WriteEndElement();         // end-item 
                }
                else
                {
                    _metadataWriter.WriteStartElement("item");
                    _metadataWriter.WriteAttributeString("id", "PBM_FileVersion");
                    _metadataWriter.WriteString("1.0.0.0");
                    _metadataWriter.WriteEndElement();
                }

                _metadataWriter.WriteStartElement("title");
                _metadataWriter.WriteString(item.TopicTitle);
                _metadataWriter.WriteEndElement();

                if (!String.IsNullOrEmpty(item.TocTitle))
                {
                    _metadataWriter.WriteStartElement("tableOfContentsTitle");
                    _metadataWriter.WriteString(item.TocTitle);
                    _metadataWriter.WriteEndElement();
                }

                // Write the runningHeaderText tag...
                _metadataWriter.WriteStartElement("runningHeaderText");
                _metadataWriter.WriteAttributeString("uscid", _runningHeaderText);
                _metadataWriter.WriteEndElement();

                // Write the topicType tag...
                _metadataWriter.WriteStartElement("topicType");
                string topicTypeId = item.TopicTypeId;
                if (String.IsNullOrEmpty(topicTypeId))
                {
                    _metadataWriter.WriteAttributeString("id", _topicTypeId);
                }
                else
                {
                    _metadataWriter.WriteAttributeString("id", topicTypeId);
                }
                _metadataWriter.WriteEndElement();

                _metadataWriter.WriteEndElement();
            }

            public override void OnEndTopic(ConceptualItem item)
            {
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_metadataWriter != null)
                {
                    _metadataWriter.Close();
                    _metadataWriter = null;
                }

                base.Dispose(disposing);
            }

            #endregion
        }

        #endregion

        #region ConceptualManifestAdapter Class

        public sealed class ConceptualManifestAdapter : ConceptualProjectAdapter
        {
            #region Private Fields

            private BuildContext    _context;
            private ConceptualGroup _group;

            [NonSerialized]
            private BuildKeyedList<ConceptualMarkerTopic> _listMarkers;

            private XmlWriter _manifestWriter;

            #endregion

            #region Constructors and Destructor

            public ConceptualManifestAdapter()
            {
            }

            #endregion

            #region Public Methods

            public override void Initialize(ConceptualGroup group,
                BuildContext context)
            {
                _group   = group;
                _context = context;

                string workingDir = context.WorkingDirectory;

                BuildSettings settings = context.Settings;

                BuildGroupContext groupContext = context.GroupContexts[group.Id];

                string manifestFile = Path.Combine(workingDir,
                    groupContext["$ManifestFile"]);

                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.Encoding = Encoding.UTF8;

                _manifestWriter = XmlWriter.Create(manifestFile, xmlSettings);
                _manifestWriter.WriteStartDocument();
                _manifestWriter.WriteStartElement("topics"); // start - topics
            }

            public override void Uninitialize()
            {
                if (_manifestWriter != null)
                {
                    _manifestWriter.WriteEndElement();       // end - topics
                    _manifestWriter.WriteEndDocument();

                    _manifestWriter.Close();
                    _manifestWriter = null;
                }

                if (_listMarkers != null && _listMarkers.Count != 0)
                {
                    ConceptualGroupContext groupContext =
                        _context.GroupContexts[_group.Id] as ConceptualGroupContext;
                    if (groupContext == null)
                    {
                        throw new BuildException(
                            "The group context is not provided, and it is required by the build system.");
                    }

                    groupContext.MarkerTopics = _listMarkers;

                    // Raise the TOC Markers exist flag...
                    _context["$HelpTocMarkers"] = Boolean.TrueString;
                }
            }

            public override void OnBeginTopic(ConceptualItem item)
            {
            }

            public override void OnWriteTopic(ConceptualItem item)
            {
                ConceptualItemType itemType = item.ItemType;

                if (itemType == ConceptualItemType.Topic ||
                    itemType == ConceptualItemType.Related)
                {
                    _manifestWriter.WriteStartElement("topic"); // start-fileAsset
                    _manifestWriter.WriteAttributeString("id", item.TopicId);
                    _manifestWriter.WriteEndElement();          // end-fileAsset 
                }
                else if (itemType == ConceptualItemType.Html)
                {
                    _context[item.TopicId] = item.TopicTitle;
                }
                else if (itemType == ConceptualItemType.Marker)
                {
                    if (_listMarkers == null)
                    {
                        _listMarkers = new BuildKeyedList<ConceptualMarkerTopic>();
                    }

                    _listMarkers.Add((ConceptualMarkerTopic)item);
                }
            }

            public override void OnEndTopic(ConceptualItem item)
            {
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_manifestWriter != null)
                {
                    _manifestWriter.Close();
                    _manifestWriter = null;
                }

                base.Dispose(disposing);
            }

            #endregion
        }

        #endregion
    }
}
