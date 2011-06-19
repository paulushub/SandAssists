using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualMetadata : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualMetadata";

        #endregion

        #region Private Fields

        private string _topicTypeId;
        private string _runningHeaderText;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualMetadata"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualMetadata"/> class
        /// to the default values.
        /// </summary>
        public ConceptualMetadata()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualMetadata"/> class
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
        private ConceptualMetadata(string visitorName)
            : base(visitorName)
        {
            _topicTypeId       = "1FE70836-AA7D-4515-B54B-E10C4B516E50"; // Generic conceptual topic
            _runningHeaderText = "runningHeaderText"; // "b1b997af-3127-4001-b77b-5263e4496939"
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualMetadata"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualMetadata"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualMetadata"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualMetadata(ConceptualMetadata source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public string TopicTypeId
        {
            get
            {
                return _topicTypeId;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _topicTypeId = value;
                }
            }
        }

        public string RunningHeaderText
        {
            get
            {
                return _runningHeaderText;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _runningHeaderText = value;
                }
            }
        }

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

            BuildContext context = this.Context;
            BuildLogger logger   = context.Logger;

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Conceptual Metadata.",
                    BuildLoggerLevel.Info);
            }

            WriteMetadata(group);

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Metadata.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region WriteMetadata Method

        private void WriteMetadata(ConceptualGroup group)
        {
            BuildContext context = this.Context;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            Guid fileAsset    = group.DocumentID;
            string workingDir = context.WorkingDirectory;

            ConceptualContent content = group.Content;
            if (content == null || content.IsEmpty)
            {
                return;
            }

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            try
            {
                string settingPath = Path.Combine(workingDir, groupContext["$MetadataFile"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("metadata"); // start-metadata
                writer.WriteAttributeString("fileAssetGuid", 
                    fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ContentMetadata");

                // We write each topic
                int itemCount = content.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem topicItem = content[i];
                    if (topicItem == null || topicItem.IsEmpty)
                    {
                        continue;
                    }

                    this.WriteMetadataItem(topicItem, writer);
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

                        this.WriteMetadataItem(topicItem, writer);
                    }
                }

                writer.WriteEndElement(); // end-metadata
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

        #region WriteMetadataItem Method

        private void WriteMetadataItem(ConceptualItem topicItem, XmlWriter writer)
        {
            ConceptualItemType itemType = topicItem.ItemType;

            if (itemType == ConceptualItemType.Topic ||
                itemType == ConceptualItemType.Related)
            {
                // <topic id="2aca5da4-6f94-43a0-9817-5f413d16f550" 
                //    revisionNumber="1" author="PS">
                writer.WriteStartElement("topic"); // start-topic
                writer.WriteAttributeString("id", topicItem.TopicId);
                writer.WriteAttributeString("revisionNumber", 
                    topicItem.TopicRevisions.ToString());

                //_projMetadata.Write(writer);
                WriteItemDetails(topicItem, writer);

                writer.WriteEndElement(); // end-topic 
            }     

            int subCount = topicItem.ItemCount;
            for (int i = 0; i < subCount; i++)
            {
                ConceptualItem subItem = topicItem[i];
                if (subItem == null || subItem.IsEmpty)
                {
                    continue;
                }

                WriteMetadataItem(subItem, writer);
            }
        }

        #endregion

        #region WriteItemDetails Method

        private void WriteItemDetails(ConceptualItem topicItem, XmlWriter writer)
        {
            Version version = topicItem.TopicVersion;
            if (version != null)
            {   
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_FileVersion");
                writer.WriteString(version.ToString());
                writer.WriteEndElement();         // end-item 
            }
            writer.WriteStartElement("title"); // start-title
            writer.WriteString(topicItem.TopicTitle);
            writer.WriteEndElement();          // end-title 

            // Write the runningHeaderText tag...
            writer.WriteStartElement("runningHeaderText");
            writer.WriteAttributeString("uscid", _runningHeaderText);
            writer.WriteEndElement();

            // Write the topicType tag...
            writer.WriteStartElement("topicType");
            string topicTypeId = topicItem.TopicTypeId;
            if (String.IsNullOrEmpty(topicTypeId))
            {
                writer.WriteAttributeString("id", _topicTypeId);
            }
            else
            {
                writer.WriteAttributeString("id", topicTypeId);
            }
            writer.WriteEndElement();

            // Finally, write the attribute tag...
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
            ConceptualMetadata visitor = new ConceptualMetadata(this);

            return visitor;
        }

        #endregion
    }
}
