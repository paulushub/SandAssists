using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualShfbReader : ConceptualReader
    {
        #region Private Fields

        private string _contentDir;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualShfbSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualShfbSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualShfbReader()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override ConceptualContent Read(string contentFile)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentDir = Path.GetDirectoryName(contentFile);

            return null;
        }

        #endregion

        #region Private Methods

        #region ReadTopics Method

        private void ReadTopics(XmlReader reader, ConceptualContent content)
        {
            // 1. Grab the version information of the file format...
            float version = 1.0f;
            string versionText = reader.GetAttribute("version");
            if (String.IsNullOrEmpty(versionText) == false)
            {
                version = Convert.ToSingle(versionText);
            }

            XmlNodeType nodeType = reader.NodeType;
            string nodeName = reader.Name;
            if (nodeType == XmlNodeType.Element && String.Equals(nodeName, "Topics"))
            {
                // handle the default topic id...
                string defaultTopic = reader.GetAttribute("default");
                if ((defaultTopic == null) ||
                    (defaultTopic != null && defaultTopic.Length == 36))
                {
                    content.DefaultTopic = defaultTopic;
                }
                // handle the items...
                ReadTopic(reader, content);
            }
            else
            {
                while (reader.Read())
                {
                    nodeType = reader.NodeType;
                    if (nodeType == XmlNodeType.Element)
                    {
                        nodeName = reader.Name;
                        if (String.Equals(nodeName, "Topics"))
                        {
                            // handle the default topic id...
                            string defaultTopic = reader.GetAttribute("default");
                            if ((defaultTopic == null) ||
                                (defaultTopic != null && defaultTopic.Length == 36))
                            {
                                content.DefaultTopic = defaultTopic;
                            }
                            // handle the items...
                            ReadTopic(reader, content);
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "Topics"))
                        {
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region ReadTopic Methods

        private void ReadTopic(XmlReader reader, ConceptualContent content)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;
            string textTemp = null;
            int revNumber = 1;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "Topic"))
                    {
                        string docId = reader.GetAttribute("id");
                        if (ConceptualUtils.IsValidId(docId))
                        {
                            bool isVisible = true;

                            textTemp = reader.GetAttribute("visible");
                            if (!String.IsNullOrEmpty(textTemp))
                            {
                                isVisible = Convert.ToBoolean(textTemp);
                            }

                            string docTitle = reader.GetAttribute("title");

                            if (!String.IsNullOrEmpty(docTitle))
                            {
                                string fullPath = Path.Combine(_contentDir, docId + ".aml");
                                ConceptualItem docItem = new ConceptualTopic(
                                    new BuildFilePath(fullPath),
                                    docTitle, docId);

                                docItem.Content = content;

                                docItem.BeginInit();

                                docItem.Visible = isVisible;
                                docItem.TopicRevisions = revNumber;
                                //docItem.IncludesTopicId = true;

                                docItem.EndInit();

                                // handle the sub-item...
                                if (!reader.IsEmptyElement)
                                {                 
                                    ReadTopic(reader, content, docItem);
                                }

                                content.Add(docItem);
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Topics"))
                    {
                        break;
                    }
                }
            }
        }
                                         
        private void ReadTopic(XmlReader reader, ConceptualContent content, 
            ConceptualItem parentItem)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;
            int revNumber   = 1;

            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "Topic"))
                    {
                        bool isVisible = true;

                        string docId = reader.GetAttribute("id");
                        if (ConceptualUtils.IsValidId(docId))
                        {
                            string textTemp = reader.GetAttribute("visible");
                            if (!String.IsNullOrEmpty(textTemp))
                            {
                                isVisible = Convert.ToBoolean(textTemp);
                            }

                            string docTitle = reader.GetAttribute("title");
                            if (!String.IsNullOrEmpty(docTitle))
                            {
                                string fullPath = Path.Combine(_contentDir,
                                    docId + ".aml");
                                ConceptualTopic docItem = new ConceptualTopic(
                                    new BuildFilePath(fullPath), docTitle, docId);

                                docItem.Content = content;

                                docItem.BeginInit();

                                docItem.Visible = isVisible;
                                docItem.TopicRevisions = revNumber;
                                //docItem.IncludesTopicId = true;

                                docItem.EndInit();

                                // handle the sub-item...
                                if (!reader.IsEmptyElement)
                                {                   
                                    ReadTopic(reader, content, docItem);
                                }

                                parentItem.Add(docItem);
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "HelpKeyword"))
                    {
                        string index = reader.GetAttribute("index");
                        string term = reader.GetAttribute("term");
                        if (!String.IsNullOrEmpty(index) &&
                            !String.IsNullOrEmpty(term))
                        {
                            KeywordItem keyword = new KeywordItem(
                                KeywordItem.ParseIndex(index), term);

                            parentItem.Keywords.Add(keyword);
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Topic"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
