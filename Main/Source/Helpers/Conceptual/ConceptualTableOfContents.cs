using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualTableOfContents : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualTableOfContents";

        #endregion

        #region Private Fields

        private string  _docWriter;
        private string  _docEditor;
        private string  _docManager;

        private ConceptualGroup _group;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualTableOfContents"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualTableOfContents"/> class
        /// to the default values.
        /// </summary>
        public ConceptualTableOfContents()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualTableOfContents"/> class
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
        private ConceptualTableOfContents(string visitorName)
            : base(visitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualTableOfContents"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualTableOfContents"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualTableOfContents"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualTableOfContents(ConceptualTableOfContents source)
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

            _group   = group;
            _docWriter  = _group.DocumentWriter;
            _docEditor  = _group.DocumentEditor;
            _docManager = _group.DocumentManager;

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Conceptual Table of Contents.",
                    BuildLoggerLevel.Info);
            }

            WriteContents();

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Table of Contents.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region WriteContents Method

        private void WriteContents()
        {
            BuildGroupContext groupContext = this.Context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            Guid fileAsset    = _group.DocumentID;
            string workingDir = _group.WorkingDirectory;
            ConceptualContent topicItems = _group.Content;
            if (topicItems == null || topicItems.IsEmpty)
            {
                return;
            }
            XmlWriter writer  = null;

            try
            {
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
                string tocPath = Path.Combine(workingDir, groupContext["$TocFile"]);

                XmlWriterSettings settings = new XmlWriterSettings();

                settings.Indent             = true;
                settings.Encoding           = Encoding.UTF8;
                settings.OmitXmlDeclaration = false;
                settings.ConformanceLevel   = ConformanceLevel.Document;

                if (File.Exists(tocPath))
                {
                    File.SetAttributes(tocPath, FileAttributes.Normal);
                    File.Delete(tocPath);
                }

                writer = XmlWriter.Create(tocPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("topics"); // start-topics

                int itemCount = topicItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem projItem = topicItems[i];
                    if (projItem == null || projItem.IsEmpty)
                    {
                        continue;
                    }

                    int subCount = projItem.ItemCount;
                    if (subCount == 0)
                    {
                        writer.WriteStartElement("topic"); // start-topic
                        writer.WriteAttributeString("id", projItem.TopicId);
                        writer.WriteAttributeString("file", projItem.TopicId);
                        writer.WriteEndElement();          // end-topic
                    }
                    else
                    {
                        WriteNode(projItem, writer);
                    }
                }

                writer.WriteEndElement(); // end-topics
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

        #region WriteNode Method

        private void WriteNode(ConceptualItem topicItem, XmlWriter writer)
        {
            writer.WriteStartElement("topic"); // start-topic

            writer.WriteAttributeString("id", topicItem.TopicId);
            writer.WriteAttributeString("file", topicItem.TopicId);

            int subTopics = topicItem.ItemCount;
            if (subTopics != 0)
            {
                for (int i = 0; i < subTopics; i++)
                {
                    ConceptualItem projItem = topicItem[i];

                    if (projItem == null || projItem.IsEmpty)
                    {
                        continue;
                    }

                    int subCount = projItem.ItemCount;
                    if (subCount == 0)
                    {
                        writer.WriteStartElement("topic"); // start-topic
                        writer.WriteAttributeString("id",   projItem.TopicId);
                        writer.WriteAttributeString("file", projItem.TopicId);
                        writer.WriteEndElement();          // end-topic
                    }
                    else
                    {
                        WriteNode(projItem, writer);
                    }
                }
            }

            writer.WriteEndElement();          // end-topic
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
            ConceptualTableOfContents visitor = new ConceptualTableOfContents(this);

            return visitor;
        }

        #endregion
    }
}
