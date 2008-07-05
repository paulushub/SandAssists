using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Builders.Conceptual;

namespace Sandcastle.Builders.Contents
{
    [Serializable]
    public sealed class DdueXmlTableOfContents : DdueXmlContent
    {
        #region Private Fields

        private string _docWriter;
        private string _docEditor;
        private string _docManager;

        #endregion

        #region Constructors and Destructor

        public DdueXmlTableOfContents()
        {   
        }

        public DdueXmlTableOfContents(ConceptualContext context)
            : base(context)
        {
        }

        public DdueXmlTableOfContents(DdueXmlTableOfContents source)
            : base(source)
        {   
        }

        #endregion

        #region Public Methods

        public override void Create()
        {
            ConceptualContext context = this.Context;
            if (context == null)
            {
                throw new InvalidOperationException(
                    "There is no context set to this object.");
            }
            IList<ConceptualItem> items = context.Items;
            if (items == null)
            {
                throw new InvalidOperationException();
            }

            _docWriter  = context.DocumentWriter;
            _docEditor  = context.DocumentEditor;
            _docManager = context.DocumentManager;

            WriteContents(items, false);
            WriteContents(items, true);
        }

        public override void Create(ConceptualContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", 
                    "The context object cannot be null or Nothing.");
            }

            this.Context = context;

            this.Create();
        }

        #endregion

        #region Private Methods

        #region WriteContents Method

        private void WriteContents(IList<ConceptualItem> listItems, bool isTechReview)
        {
            ConceptualContext context = this.Context;
            Guid fileAsset    = context.DocumentID;
            string workingDir = context.WorkingDirectory;
            XmlWriter writer  = null;

            try
            {
                string tocDir = Path.Combine(workingDir, 
                    ConceptualUtils.ExtractedFilesDir);
                if (!Directory.Exists(tocDir))
                {
                    Directory.CreateDirectory(tocDir);
                }
                string tocPath = null;
                if (isTechReview)
                {
                    tocPath = Path.Combine(tocDir, "TocTechReview.xml");
                }
                else
                {
                    tocPath = Path.Combine(tocDir, "Toc.xml");
                }

                XmlWriterSettings xmlSettings  = new XmlWriterSettings();

                xmlSettings.Indent             = true;
                xmlSettings.Encoding           = Encoding.UTF8;
                xmlSettings.OmitXmlDeclaration = false;
                xmlSettings.ConformanceLevel   = ConformanceLevel.Document;

                if (File.Exists(tocPath))
                {
                    File.Delete(tocPath);
                }

                writer = XmlWriter.Create(tocPath, xmlSettings);

                writer.WriteStartDocument();
                writer.WriteStartElement("tableOfContents"); // start-tableOfContents
                writer.WriteAttributeString("fileAssetGuid", fileAsset.ToString());
                if (isTechReview)
                {
                    writer.WriteAttributeString("assetTypeId", "TocTechReview");
                }
                else
                {
                    writer.WriteAttributeString("assetTypeId", "Toc");
                }

                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem projItem = listItems[i];
                    if (projItem == null || projItem.IsEmpty)
                    {
                        continue;
                    }

                    int subCount = projItem.ItemCount;
                    if (subCount == 0)
                    {
                        writer.WriteStartElement("topic"); // start-topic
                        writer.WriteAttributeString("isCategoryOnly", "False");
                        writer.WriteAttributeString("id", projItem.FileGuid);
                        if (isTechReview)
                        {
                            //Writer="PS" Editor="PS" Manager="PS"
                            writer.WriteAttributeString("Writer", _docWriter);
                            writer.WriteAttributeString("Editor", _docEditor);
                            writer.WriteAttributeString("Manager", _docManager);
                        }
                        writer.WriteEndElement(); // end-topic
                    }
                    else
                    {
                        WriteNode(projItem, writer, isTechReview);
                    }
                }

                writer.WriteEndElement(); // end-tableOfContents
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

        private void WriteNode(ConceptualItem topicItem,
            XmlWriter writer, bool isTechReview)
        {
            writer.WriteStartElement("topic"); // start-topic

            writer.WriteAttributeString("isCategoryOnly", "False");
            writer.WriteAttributeString("id", topicItem.FileGuid);
            if (isTechReview)
            {
                //Writer="PS" Editor="PS" Manager="PS"
                writer.WriteAttributeString("Writer", _docWriter);
                writer.WriteAttributeString("Editor", _docEditor);
                writer.WriteAttributeString("Manager", _docManager);
            }

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
                        writer.WriteAttributeString("isCategoryOnly", "False");
                        writer.WriteAttributeString("id", projItem.FileGuid);
                        if (isTechReview)
                        {
                            //Writer="PS" Editor="PS" Manager="PS"
                            writer.WriteAttributeString("Writer", _docWriter);
                            writer.WriteAttributeString("Editor", _docEditor);
                            writer.WriteAttributeString("Manager", _docManager);
                        }
                        writer.WriteEndElement(); // end-topic
                    }
                    else
                    {
                        WriteNode(projItem, writer, isTechReview);
                    }
                }
            }

            writer.WriteEndElement(); // end-topic
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override DdueXmlContent Clone()
        {
            DdueXmlTableOfContents content = new DdueXmlTableOfContents(this);

            return content;
        }

        #endregion
    }
}
