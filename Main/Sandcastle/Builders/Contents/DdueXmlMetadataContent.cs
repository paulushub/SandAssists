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
    public sealed class DdueXmlMetadataContent : DdueXmlContent
    {
        #region Private Fields

        private DdueXmlMetadata _projMetadata;

        #endregion

        #region Constructors and Destructor

        public DdueXmlMetadataContent()
        {
            CreateMetadata();
        }

        public DdueXmlMetadataContent(ConceptualContext context)
            : base(context)
        {
            CreateMetadata();
        }

        public DdueXmlMetadataContent(DdueXmlMetadataContent source)
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
                throw new InvalidOperationException();
            }
            IList<ConceptualItem> items = context.Items;
            if (items == null)
            {
                throw new InvalidOperationException();
            }

            WriteMetadata(items);
        }

        public override void Create(ConceptualContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context",
                    "The context object cannot be null (or Nothing).");
            }

            this.Context = context;

            this.Create();
        }

        #endregion

        #region WriteMetadata Method

        private void WriteMetadata(IList<ConceptualItem> listItems)
        {
            if (listItems == null)
            {
                return;
            }

            if (_projMetadata == null)
            {
                CreateMetadata();
            }

            ConceptualContext context = this.Context;
            Guid fileAsset    = context.DocumentID;
            string workingDir = context.WorkingDirectory;

            XmlWriterSettings xmlSettings  = new XmlWriterSettings();

            xmlSettings.Indent             = true;
            xmlSettings.Encoding           = Encoding.UTF8;
            xmlSettings.OmitXmlDeclaration = false;
            xmlSettings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            string tocDir = Path.Combine(workingDir, 
                ConceptualUtils.ExtractedFilesDir);
            if (!Directory.Exists(tocDir))
            {
                Directory.CreateDirectory(tocDir);
            }

            try
            {
                string settingPath = Path.Combine(tocDir, "ContentMetadata.xml");

                if (File.Exists(settingPath))
                {
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, xmlSettings);

                writer.WriteStartDocument();
                writer.WriteStartElement("metadata"); // start-metadata
                writer.WriteAttributeString("fileAssetGuid", 
                    fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ContentMetadata");

                // We write each topic
                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem projItem = listItems[i];
                    if (projItem == null || projItem.IsEmpty)
                    {
                        continue;
                    }

                    WriteMetadataItem(projItem, writer);
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

        private void WriteMetadataItem(ConceptualItem projItem, XmlWriter writer)
        {   
            // <topic id="2aca5da4-6f94-43a0-9817-5f413d16f550" 
            //    revisionNumber="1" author="PS">
            writer.WriteStartElement("topic"); // start-topic
            writer.WriteAttributeString("id", 
                projItem.FileGuid);
            writer.WriteAttributeString("revisionNumber", 
                projItem.RevisionNumber.ToString());
            writer.WriteAttributeString("author", projItem.Editor);

            _projMetadata.Write(writer);

            writer.WriteEndElement(); // end-topic 

            int subCount = projItem.ItemCount;
            for (int i = 0; i < subCount; i++)
            {
                ConceptualItem subItem = projItem[i];
                if (subItem == null || subItem.IsEmpty)
                {
                    continue;
                }

                WriteMetadataItem(subItem, writer);
            }
        }

        #endregion

        #region CreateMetadata

        private void CreateMetadata()
        {
            _projMetadata = new DdueXmlMetadata(
                "dac3a6a0-c863-4e5b-8f65-79efc6a4ba09",
                "b1b997af-3127-4001-b77b-5263e4496939");

            _projMetadata.AddItems();
        }

        #endregion

        #region ICloneable Members

        public override DdueXmlContent Clone()
        {
            DdueXmlMetadataContent content = new DdueXmlMetadataContent(this);

            return content;
        }

        #endregion
    }
}
