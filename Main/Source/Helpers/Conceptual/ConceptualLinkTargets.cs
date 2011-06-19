using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualLinkTargets : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualLinkTarget";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualLinkTarget"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualLinkTarget"/> class
        /// to the default values.
        /// </summary>
        public ConceptualLinkTargets()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualLinkTarget"/> class
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
        private ConceptualLinkTargets(string visitorName)
            : base(visitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualLinkTarget"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualLinkTarget"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualLinkTarget"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualLinkTargets(ConceptualLinkTargets source)
            : base(source)
        {
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
                logger.WriteLine("Begin - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }

            WriteIndexTargets(group);

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region WriteIndexTargets Method

        private void WriteIndexTargets(ConceptualGroup group)
        {
            BuildContext context = this.Context;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ConceptualContent content = group.Content;
            if (content == null || content.IsEmpty)
            {
                return;
            }

            string workingDir = context.WorkingDirectory;

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
                string settingPath = Path.Combine(workingDir, groupContext["$IndexFile"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("topics"); // start-topics
                writer.WriteAttributeString("id", group.Id);
                
                // We write each topic
                int itemCount = content.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem topicItem = content[i];
                    if (topicItem == null || topicItem.IsEmpty)
                    {
                        continue;
                    }

                    this.WriteIndexItem(topicItem, writer);
                }

                writer.WriteEndElement();          // end-topics
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

        #region WriteIndexItem Method

        private void WriteIndexItem(ConceptualItem topicItem, XmlWriter writer)
        {
            ConceptualItemType itemType = topicItem.ItemType;

            if (itemType != ConceptualItemType.Marker &&
                itemType != ConceptualItemType.Custom)
            {
                string linkText = topicItem.TopicLinkText;
                if (String.IsNullOrEmpty(linkText))
                {
                    linkText = topicItem.TopicTitle;
                }            

                // <topic id="2aca5da4-6f94-43a0-9817-5f413d16f550" 
                //    title="This is the link text"/>
                
                writer.WriteStartElement("topic"); // start-topic
                writer.WriteAttributeString("id", topicItem.TopicId);
                writer.WriteAttributeString("title", linkText);

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

                WriteIndexItem(subItem, writer);
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
            ConceptualLinkTargets visitor = new ConceptualLinkTargets(this);

            return visitor;
        }

        #endregion
    }
}
