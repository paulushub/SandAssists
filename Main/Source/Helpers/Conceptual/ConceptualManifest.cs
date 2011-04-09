using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualManifest : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualManifest";

        #endregion

        #region Private Fields

        private int    _lcid;
        private string _fileAsset;
        private string _projectAsset;
        private string _repositoryAsset;
        private string _projectName;
        private string _workingDir;

        private BuildSettings   _settings;
        private ConceptualGroup _group;

        private BuildKeyedList<ConceptualMarkerTopic> _listMarkers;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualManifest"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualManifest"/> class
        /// to the default values.
        /// </summary>
        public ConceptualManifest()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualManifest"/> class
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
        private ConceptualManifest(string visitorName)
            : base(visitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualManifest"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualManifest"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualManifest"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualManifest(ConceptualManifest source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override void OnVisit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "ConceptualTableOfContents: The conceptual table of contents generator is not initialized.");
            }

            BuildContext context   = this.Context;
            BuildLogger logger     = context.Logger; 
            BuildSettings settings = context.Settings;

            _group           = group;
            _settings        = settings;

            _lcid            = _settings.CultureInfo.LCID;
            
            _fileAsset       = _group.DocumentID.ToString();
            _projectAsset    = _group.ProjectID.ToString();
            _repositoryAsset = _group.RepositoryID.ToString();
            _projectName     = _group.ProjectName;
            _workingDir      = _group.WorkingDirectory;

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Conceptual Manifest (List of Help Topics).",
                    BuildLoggerLevel.Info);
            }

            WriteManifest();

            if (_listMarkers != null && _listMarkers.Count != 0)
            {
                ConceptualGroupContext groupContext =
                    context.GroupContexts[group.Id] as ConceptualGroupContext;
                if (groupContext == null)
                {
                    throw new BuildException(
                        "The group context is not provided, and it is required by the build system.");
                }

                groupContext.MarkerTopics = _listMarkers;

                // Raise the TOC Markers exist flag...
                context["$HelpTocMarkers"] = Boolean.TrueString;
            }

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Manifest (List of Help Topics).",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region WriteManifest Method

        private void WriteManifest()
        {
            BuildGroupContext groupContext = this.Context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string workingDir = _group.WorkingDirectory;

            ConceptualContent topicItems = _group.Content;
            if (topicItems == null || topicItems.IsEmpty)
            {
                return;
            }

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            try
            {
                string settingPath = Path.Combine(workingDir, groupContext["$ManifestFile"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("topics"); // start-topics

                WriteTopics(writer);

                writer.WriteEndElement();           // end-topics
                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #region WriteTopics Method

        private void WriteTopics(XmlWriter writer)
        {
            ConceptualContent content = _group.Content;

            // We write each topic
            int itemCount = content.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualItem topicItem = content[i];
                if (topicItem == null || topicItem.IsEmpty)
                {
                    continue;
                }

                this.WriteTopic(topicItem, writer);
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

                    this.WriteTopic(topicItem, writer);
                }
            }
        }

        private void WriteTopic(ConceptualItem topicItem, XmlWriter writer)
        {
            ConceptualItemType itemType = topicItem.ItemType;

            if (itemType == ConceptualItemType.Topic ||
                itemType == ConceptualItemType.Related)
            {
                writer.WriteStartElement("topic"); // start-fileAsset
                writer.WriteAttributeString("id", topicItem.TopicId);
                writer.WriteEndElement();          // end-fileAsset 
            }
            else if (itemType == ConceptualItemType.Html)
            {
                this.Context[topicItem.TopicId] = topicItem.TopicTitle;
            }
            else if (itemType == ConceptualItemType.Marker)
            {
                if (_listMarkers == null)
                {
                    _listMarkers = new BuildKeyedList<ConceptualMarkerTopic>();
                }

                _listMarkers.Add((ConceptualMarkerTopic)topicItem);
            }

            int subCount = topicItem.ItemCount;
            for (int i = 0; i < subCount; i++)
            {
                ConceptualItem subItem = topicItem[i];
                if (subItem == null || subItem.IsEmpty)
                {
                    continue;
                }

                WriteTopic(subItem, writer);
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override ConceptualGroupVisitor Clone()
        {
            ConceptualManifest visitor = new ConceptualManifest(this);

            return visitor;
        }

        #endregion
    }
}
