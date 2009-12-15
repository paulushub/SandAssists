using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualSiteMap
    {
        #region Private Fields

        private bool    _isDefault;

        private ConceptualGroup _curGroup;

        #endregion

        #region Constructors and Destructor

        public ConceptualSiteMap()
        {
            _isDefault = true;
        }

        #endregion

        #region Public Methods

        public void Write(ConceptualGroup group, BuildSettings settings)
        {
            BuildExceptions.NotNull(group, "group");

            _curGroup = group;

            WriteContents();
        }

        #endregion

        #region WriteContents Method

        private void WriteContents()
        {
            string workingDir               = _curGroup.WorkingDirectory;
            IList<ConceptualItem> listItems = _curGroup.Items;
            XmlWriter writer  = null;

            try
            {
                string tocPath = Path.Combine(workingDir, "Help");
                if (!Directory.Exists(tocPath))
                {
                    Directory.CreateDirectory(tocPath);
                }
                tocPath = Path.Combine(tocPath, "HelpTopics.sitemap");
                File.SetAttributes(tocPath, FileAttributes.Normal);
                File.Delete(tocPath);

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
                writer.WriteStartElement("siteMap"); // start-siteMap

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
                        writer.WriteStartElement("siteMapNode"); // start-siteMapNode
                        writer.WriteAttributeString("title", projItem.FileTitle);
                        writer.WriteAttributeString("description", String.Empty);
                        writer.WriteAttributeString("url", String.Format(
                            @"Help\html\{0}.htm", projItem.FileGuid));
                        if (_isDefault)
                        {
                            writer.WriteAttributeString("isDefault", "true");
                            _isDefault = false;
                        }
                        writer.WriteEndElement(); // end-siteMapNode
                    }
                    else
                    {
                        WriteNode(projItem, writer);
                    }
                }

                writer.WriteEndElement(); // end-siteMap
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
            writer.WriteStartElement("siteMapNode"); // start-siteMapNode

            writer.WriteAttributeString("title", topicItem.FileTitle);
            writer.WriteAttributeString("description", String.Empty);
            writer.WriteAttributeString("url",
                String.Format(@"Help\html\{0}.htm", topicItem.FileGuid));
            if (_isDefault)
            {
                writer.WriteAttributeString("isDefault", "true");
                _isDefault = false;
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
                        writer.WriteStartElement("siteMapNode"); // start-siteMapNode
                        writer.WriteAttributeString("title", projItem.FileTitle);
                        writer.WriteAttributeString("description", String.Empty);
                        writer.WriteAttributeString("url",
                            String.Format(@"Help\html\{0}.htm", projItem.FileGuid));
                        if (_isDefault)
                        {
                            writer.WriteAttributeString("isDefault", "true");
                            _isDefault = false;
                        }
                        writer.WriteEndElement(); // end-siteMapNode
                    }
                    else
                    {
                        WriteNode(projItem, writer);
                    }
                }
            }

            writer.WriteEndElement(); // end-siteMapNode
        }

        #endregion
    }
}
