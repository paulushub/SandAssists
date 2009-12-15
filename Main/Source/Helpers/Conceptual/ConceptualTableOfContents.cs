using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualTableOfContents
    {
        #region Private Fields

        private string  _docWriter;
        private string  _docEditor;
        private string  _docManager;

        private ConceptualGroup _curGroup;

        #endregion

        #region Constructors and Destructor

        public ConceptualTableOfContents()
        {   
        }

        #endregion

        #region Public Methods

        public void Write(ConceptualGroup group, BuildSettings settings)
        {
            BuildExceptions.NotNull(group, "group");

            _curGroup   = group;
            _docWriter  = _curGroup.DocumentWriter;
            _docEditor  = _curGroup.DocumentEditor;
            _docManager = _curGroup.DocumentManager;

            WriteContents(false);
            WriteContents(true);
        }

        #endregion

        #region WriteContents Method

        private void WriteContents(bool isTechReview)
        {
            Guid fileAsset = _curGroup.DocumentID;
            string workingDir = _curGroup.WorkingDirectory;
            IList<ConceptualItem> listItems = _curGroup.Items;
            XmlWriter writer  = null;

            try
            {
                string tocDir = Path.Combine(workingDir, "Extractedfiles");
                if (!Directory.Exists(tocDir))
                {
                    Directory.CreateDirectory(tocDir);
                }
                string tocPath = null;
                if (isTechReview)
                {
                    tocPath = Path.Combine(tocDir, _curGroup["$BuildTocTechReviewFile"]);
                }
                else
                {
                    tocPath = Path.Combine(tocDir, _curGroup["$BuildTocFile"]);
                }

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
    }
}
