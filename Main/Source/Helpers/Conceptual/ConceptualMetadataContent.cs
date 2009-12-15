using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualMetadataContent : 
        BuildContent<ConceptualMetadataItem, ConceptualMetadataContent>
    {
        #region Private Fields

        private string _topicType;
        private string _runningHeaderText;

        #endregion

        #region Constructors and Destructor

        public ConceptualMetadataContent()
        {
            CreateMetadata();
        }

        public ConceptualMetadataContent(ConceptualMetadataContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public string TopicType
        {
            get
            {
                return _topicType;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _topicType = value;
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

        #region Public Methods

        public void Write(ConceptualGroup group, BuildSettings settings)
        {
            BuildExceptions.NotNull(group, "group");

            WriteMetadata(group);
        }

        public void AddItem(string id, string name, string analysisProperty,
            string contentSet, string apProperty, string catalog,
            string valueId, string text)
        {
            ConceptualMetadataItem item = new ConceptualMetadataItem(id, name,
                analysisProperty, contentSet, apProperty, catalog,
                valueId, text);

            this.Add(item);
       }

        #endregion

        #region WriteMetadata Method

        private void WriteMetadata(ConceptualGroup group)
        {   
            Guid fileAsset    = group.DocumentID;
            string workingDir = group.WorkingDirectory;

            IList<ConceptualItem> listItems = group.Items;
            if (listItems == null)
            {
                return;
            }

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            string tocDir = Path.Combine(workingDir, ConceptualUtils.ExtractedFilesDir);
            if (!Directory.Exists(tocDir))
            {
                Directory.CreateDirectory(tocDir);
            }

            try
            {
                string settingPath = Path.Combine(tocDir, group["$MetadataFile"]);

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

            //_projMetadata.Write(writer);
            WriteMetadata(writer);

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

        private void WriteMetadata(XmlWriter writer)
        {
            // Write the runningHeaderText tag...
            writer.WriteStartElement("runningHeaderText");
            writer.WriteAttributeString("uscid", _runningHeaderText);
            writer.WriteEndElement();

            // Write the topicType tag...
            writer.WriteStartElement("topicType");
            writer.WriteAttributeString("id", _topicType);
            writer.WriteEndElement();

            // Finally, write the attribute tag...
            //if (_listItems == null)
            //{
            //    return;
            //}
            int itemCount = this.Count;
            if (itemCount == 0)
            {
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                ConceptualMetadataItem metaItem = this[i];
                //ConceptualMetadataItem metaItem = _listItems[i];
                if (metaItem == null)
                {
                    continue;
                }

                metaItem.Write(writer);
            }
        }

        #endregion

        #region CreateMetadata

        private void CreateMetadata()
        {
            _topicType         = "dac3a6a0-c863-4e5b-8f65-79efc6a4ba09";
            _runningHeaderText = "runningHeaderText"; // "b1b997af-3127-4001-b77b-5263e4496939"

            // These values or options are how they affect the output is not
            // well-known. Until, the meaning is made public, we use the 
            // defaults as provided in the sample files...
            this.AddItem("4b91e890-68ce-4966-b685-75eacf1b596e",
                "Catalog Container", "False", "False", "False", "True",
                "b8052700-9597-436d-a02e-e9f139e28af8",
                "System_Default_Catalog");
            this.AddItem("8ff54c74-d745-4a73-b058-023e47952242",
                "CommunityContent", "False", "False", "False", "False",
                "fe7d4cf5-877c-459c-a38e-0d9ce9796ab7", "0");
            this.AddItem("3785f925-c5d5-4d0b-9dd6-fd1d27a9e5c5",
                "Content Set Container", "False", "True", "False", "False",
                "c02fe60b-26c3-48db-9548-1b1cde9322df",
                "System_Default_Content_Set");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang",
                "False", "False", "False", "False",
                "95b32abd-126d-4071-900c-c2a0e23ae271", "aspx");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang",
                "False", "False", "False", "False",
                "26ae1a47-ce90-4c67-9f71-ae3a566ccad0", "C++");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang",
                "False", "False", "False", "False",
                "af7ca145-38b8-4e13-b414-ac79f10c1922", "CSharp");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang",
                "False", "False", "False", "False",
                "ce1c8d7b-1d73-438a-a55c-f2d883e702f8", "jsharp");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang",
                "False", "False", "False", "False",
                "96f36197-4b00-4c4d-b445-6512301ccb88", "VB");
            this.AddItem("80d89f21-9447-4d7d-a071-d6b488143d19", "ShippedIn",
                "False", "False", "False", "False",
                "3c19c0af-50f7-44e9-8b85-e16dcbcb1401", "vs.90");
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override ConceptualMetadataContent Clone()
        {
            ConceptualMetadataContent content = new ConceptualMetadataContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
