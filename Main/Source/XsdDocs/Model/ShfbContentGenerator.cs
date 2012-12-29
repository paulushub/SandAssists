using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using XsdDocumentation.Markup;

namespace XsdDocumentation.Model
{
    public sealed class ShfbContentGenerator : ContentGenerator
    {
        public ShfbContentGenerator(IMessageReporter messageReporter, 
            Configuration configuration) : base(messageReporter, configuration)
        {   
        }   

        protected override void GenerateMediaFiles()
        {
            var mediaFolder = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "Media");
            Directory.CreateDirectory(MediaFolder);
            foreach (var artItem in ArtItem.ArtItems)
            {
                var sourceFile = Path.Combine(mediaFolder, artItem.FileName);
                var destinationFile = Path.Combine(MediaFolder, artItem.FileName);
                File.Copy(sourceFile, destinationFile, true);

                var mediaItem = new MediaItem(artItem, destinationFile);
                MediaItems.Add(mediaItem);
            }
        }

        protected override void GenerateContentFile()
        {
            var doc = new XmlDocument();
            var rootNode = doc.CreateElement("Topics");
            doc.AppendChild(rootNode);

            GenerateContentFileElements(rootNode, 
                this.Context.TopicManager.Topics);

            var directory = Path.GetDirectoryName(this.ContentFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            doc.Save(ContentFile);
        }

        protected override string GetAbsoluteFileName(string topicsFolder,
            Topic topic)
        {
            return Path.Combine(topicsFolder,
                Path.ChangeExtension(topic.Id, ".aml"));
        } 

        private static void GenerateContentFileElements(XmlNode parentNode, 
            IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                var doc = parentNode.OwnerDocument;
                var topicElement = doc.CreateElement("Topic");
                topicElement.SetAttribute("id", topic.Id);
                topicElement.SetAttribute("visible", XmlConvert.ToString(true));
                topicElement.SetAttribute("title", topic.Title);
                if (!String.IsNullOrEmpty(topic.TocTitle))
                {
                    topicElement.SetAttribute("tocTitle", topic.TocTitle);
                }
                parentNode.AppendChild(topicElement);

                if (topic.KeywordsK.Count > 0 || topic.KeywordsF.Count > 0)
                {
                    var helpKeywordsElement = doc.CreateElement("HelpKeywords");
                    topicElement.AppendChild(helpKeywordsElement);
                    AddKeywords(helpKeywordsElement, topic.KeywordsK, "K");
                    AddKeywords(helpKeywordsElement, topic.KeywordsF, "F");
                }

                GenerateContentFileElements(topicElement, topic.Children);
            }
        }

        private static void AddKeywords(XmlNode helpKeywordsElement, 
            IEnumerable<string> keywordsF, string index)
        {
            foreach (var keywordF in keywordsF)
            {
                var helpKeywordElement = 
                    helpKeywordsElement.OwnerDocument.CreateElement("HelpKeyword");
                helpKeywordElement.SetAttribute("index", index);
                helpKeywordElement.SetAttribute("term", keywordF);
                helpKeywordsElement.AppendChild(helpKeywordElement);
            }
        }
    }
}
