using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public abstract class ConceptualItem : BuildItem<ConceptualItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "topic";

        #endregion

        #region Private Fields

        private int    _revNumber;
        private bool   _isVisible;
        private bool   _isInitialized;
        private bool   _includesTopicId;

        private string _topicId;
        private string _topicTitle;

        private string _topicTocTitle;
        private string _topicLinkText;
        private string _topicTypeId;

        private HashSet<string> _listExcludes;

        private Version _topicVersion;
        private BuildFilePath _filePath;
        private ConceptualAuthoring _authoring;

        private KeywordContent   _keywords;
        private AttributeContent _attributes;

        #endregion

        #region Constructors and Destructor

        protected ConceptualItem()
        {
            _revNumber       = 1;
            _isVisible       = true;
            _includesTopicId = false;
            _topicId         = String.Empty;
            _topicTitle      = String.Empty;
            _topicVersion    = new Version(1, 0, 0, 0); 
            _topicTocTitle   = String.Empty;
            _topicTypeId     = String.Empty;   
            _attributes      = new AttributeContent();
            _keywords        = new KeywordContent();
        }

        protected ConceptualItem(BuildFilePath filePath, string topicTitle, 
            string topicId) : this()
        {
            _filePath        = filePath;
            _topicTitle      = topicTitle;
            _topicId         = topicId;
        }

        protected ConceptualItem(ConceptualItem source)
            : base(source)
        {
            _isVisible       = source._isVisible;
            _topicVersion    = source._topicVersion;
        }

        #endregion

        #region Public Properties

        public abstract ConceptualItemType ItemType
        {
            get;
        }

        public bool Visible
        {
            get 
            { 
                return _isVisible; 
            }   
            set 
            { 
                _isVisible = value; 
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                if (_filePath == null || String.IsNullOrEmpty(_topicId) ||
                    String.IsNullOrEmpty(_topicTitle))
                {
                    return true;
                }
                if (!ConceptualUtils.IsValidId(_topicId))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public string TopicId
        {
            get
            {
                return _topicId;
            }
            set
            {
                if (!String.IsNullOrEmpty(value) &&
                    ConceptualUtils.IsValidId(value))
                {
                    _topicId = value;
                }
            }
        }

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
                    _topicTypeId = value.Trim();
                }
                else
                {
                    _topicTypeId = String.Empty;
                }
            }
        }

        public string TopicTitle
        {
            get
            {
                return _topicTitle;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                } 
                if (!String.IsNullOrEmpty(value))
                {
                    _topicTitle = value;
                }
            }
        }

        public string TocTitle
        {
            get
            {
                return _topicTocTitle;
            }
            set
            {
                if (value != null)
                {
                    _topicTocTitle = value.Trim();
                }
                else
                {
                    _topicTocTitle = String.Empty;
                }
            }
        }

        public string TopicLinkText
        {
            get
            {
                return _topicLinkText;
            }
            set
            {
                _topicLinkText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Version TopicVersion
        {
            get
            {
                return _topicVersion;
            }
            set
            {
                if (value == null)
                {
                    _topicVersion = value;
                }
            }
        }

        public int TopicRevisions
        {
            get
            {
                return _revNumber;
            }
            set
            {
                if (value >= 0)
                {
                    _revNumber = value;
                }
            }
        }

        public ICollection<string> Excludes
        {
            get
            {
                return _listExcludes;
            }  
        }

        public bool IncludesTopicId
        {
            get
            {
                return _includesTopicId;
            } 
            protected set
            {
                _includesTopicId = value;
            }
        }

        public BuildFilePath FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                _filePath = value;
            }
        }

        public ConceptualAuthoring Authoring
        {
            get
            {
                return _authoring;
            }
            set
            {
                _authoring = value;
            }
        }

        public AttributeContent Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public KeywordContent Keywords
        {
            get
            {
                return _keywords;
            }
        }

        public virtual int ItemCount
        {
            get
            {
                return 0;
            }
        }

        public virtual ConceptualItem this[int index]
        {
            get
            {
                return null;
            }
        }

        public ConceptualItem this[string topicId]
        {
            get
            {
                return this[topicId, false];
            }
        }

        public virtual ConceptualItem this[string topicId, bool recursive]
        {
            get
            {
                return null;
            }
        }

        public virtual IList<ConceptualItem> Items
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ConceptualItem"/> class instance, this property is 
        /// <see cref="ConceptualItem.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Internal Properties

        internal HashSet<string> ExcludesInternal
        {
            get
            {
                return _listExcludes;
            }
        }

        #endregion

        #region Public Methods

        public static ConceptualItem Create(string itemType)
        {
            if (String.IsNullOrEmpty(itemType))
            {
                return null;
            }

            switch (itemType.ToLower())
            {
                case "topic":
                    return new ConceptualTopic();
                case "related":
                    return new ConceptualRelatedTopic();
                case "marker":
                    return new ConceptualMarkerTopic();
                case "html":
                    return new ConceptualHtmlTopic();
            }

            return null;
        }

        public static ConceptualItem Create(ConceptualItemType itemType)
        {
            switch (itemType)
            {
                case ConceptualItemType.Topic:
                    return new ConceptualTopic();
                case ConceptualItemType.Related:
                    return new ConceptualRelatedTopic();
                case ConceptualItemType.Marker:
                    return new ConceptualMarkerTopic();
                case ConceptualItemType.Html:
                    return new ConceptualHtmlTopic();
            }

            return null;
        }

        public virtual void BeginInit()
        {
            _isInitialized = false;
        }

        public virtual void EndInit()
        {
            ConceptualItemType itemType = this.ItemType;

            if (itemType == ConceptualItemType.Related ||
                itemType == ConceptualItemType.Topic)
            {
                this.OnLoadTopicMetadata();
            }

            _isInitialized = true;
        }

        public virtual bool CreateTopic(string ddueXmlDir, string ddueCompDir, 
            string ddueHtmlDir)
        {
            return this.OnCreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir);
        }

        public virtual void Add(ConceptualItem item)
        {
        }

        public virtual void Insert(int index, ConceptualItem item)
        {   
        }

        public virtual void Remove(int index)
        {
        }

        public virtual void Remove(ConceptualItem item)
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void AddExclude(string exclude)
        {
            if (String.IsNullOrEmpty(exclude))
            {
                return;
            }

            if (_listExcludes == null)
            {
                _listExcludes = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);
            }

            _listExcludes.Add(exclude);
        }

        public virtual void RemoveExclude(string exclude)
        {
            if (String.IsNullOrEmpty(exclude))
            {
                return;
            }

            if (_listExcludes != null)
            {
                _listExcludes.Remove(exclude);
            }
        }

        public virtual void ClearExclude()
        {
            _listExcludes = null;
        }

        #endregion

        #region Protected Methods

        #region OnCreateTopic Method

        protected virtual bool OnCreateTopic(string ddueXmlDir,
            string ddueCompDir, string ddueHtmlDir)
        {
            if (!Directory.Exists(ddueXmlDir))
            {
                Directory.CreateDirectory(ddueXmlDir);
            }
            if (!Directory.Exists(ddueCompDir))
            {
                Directory.CreateDirectory(ddueCompDir);
            }
            if (_filePath == null || !_filePath.Exists)
            {
                return false;
            }
            if (String.IsNullOrEmpty(_topicId) ||
                String.IsNullOrEmpty(_topicTitle))
            {
                OnLoadTopicMetadata();
            }
            if (String.IsNullOrEmpty(_topicId) ||
                String.IsNullOrEmpty(_topicTitle))
            {
                return false;
            }

            string documentPath = Path.Combine(ddueXmlDir, _topicId + ".xml");
            if (File.Exists(documentPath))
            {
                File.SetAttributes(documentPath, FileAttributes.Normal);
                File.Delete(documentPath);
            }
            if (_includesTopicId)
            {
                File.Copy(_filePath, documentPath, true);
                File.SetAttributes(documentPath, FileAttributes.Normal);
            }
            else
            {
                StreamWriter textWriter = null;

                int bufferSize = 4096;
                char[] buffer = new char[bufferSize];

                using (StreamReader reader = new StreamReader(_filePath,
                    Encoding.UTF8, true, bufferSize + 1))
                {
                    textWriter = new StreamWriter(documentPath, false,
                        (Encoding)reader.CurrentEncoding.Clone(), bufferSize + 1);

                    textWriter.WriteLine(reader.ReadLine()); // write the <c>XML</c> declaration..
                    textWriter.WriteLine("<topic id=\"{0}\" revisionNumber=\"{1}\">",
                       _topicId, _revNumber);

                    while (reader.Peek() >= 0)
                    {
                        int count = reader.Read(buffer, 0, bufferSize);

                        if (count == bufferSize)
                        {
                            textWriter.Write(buffer);
                        }
                        else if (count > 0)
                        {
                            textWriter.Write(buffer, 0, count);
                        }
                    }

                    if (textWriter != null)
                    {
                        textWriter.WriteLine();
                        textWriter.WriteLine("</topic>");

                        textWriter.Close();
                    }
                }
            }

            string companionFile = Path.ChangeExtension(_filePath, ".cmp");            
            string companionOutputFile = Path.Combine(ddueCompDir, _topicId + ".cmp.xml");

            if (File.Exists(companionFile))
            {
                File.Copy(companionFile, companionOutputFile, true);
            }
            else
            {
                this.OnCreateCompanionFile(companionOutputFile, true);
            }  

            return true;
        }

        #endregion

        #region OnLoadTopicMetadata Method

        protected virtual bool OnLoadTopicMetadata()
        {
            if (_filePath == null || !_filePath.Exists)
            {
                return false;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments               = true;
            settings.IgnoreWhitespace             = true;
            settings.IgnoreProcessingInstructions = true;

            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(_filePath, settings);

                XmlNodeType nodeType = reader.MoveToContent();
                Debug.Assert(nodeType == XmlNodeType.Element);
                if (nodeType != XmlNodeType.Element)
                {
                    return false;
                }

                if (String.Equals(reader.Name, "topic", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    _includesTopicId = true;

                    _topicId = reader.GetAttribute("id");
                    string strTemp = reader.GetAttribute("revisionNumber");
                    if (!String.IsNullOrEmpty(strTemp))
                    {
                        _revNumber = Convert.ToInt32(strTemp);
                    }

                    string nodeName = null;
                    while (reader.Read())
                    {
                        nodeType = reader.NodeType;
                        nodeName = reader.Name;
                        if (nodeType == XmlNodeType.Element)
                        {
                            if (ConceptualUtils.IsValidDocumentTag(nodeName))
                            {
                                this.OnDocumentType(nodeName);

                                break;
                            }
                        }
                        else if (nodeType == XmlNodeType.EndElement)
                        {
                            if (String.Equals(nodeName, "topic") ||
                                ConceptualUtils.IsValidDocumentTag(nodeName))
                            {
                                this.OnDocumentType(nodeName);

                                break;
                            }
                        }
                    }
                }
                else if (ConceptualUtils.IsValidDocumentTag(reader.Name))
                {
                    _includesTopicId = false;

                    this.OnDocumentType(reader.Name);
                }    
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            // If the companion file is available, load it...
            string companionFile = Path.ChangeExtension(_filePath, ".cmp");

            if (!File.Exists(companionFile))
            {
                return true;
            }

            try
            {
                reader = XmlReader.Create(companionFile, settings);

                XmlNodeType nodeType = XmlNodeType.None;
                string nodeName      = null;
                string nodeText      = null;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;
                    nodeName = reader.Name;
                    if (nodeType == XmlNodeType.Element)
                    {
                        switch (nodeName)
                        {
                            case "title":
                                nodeText = reader.ReadString().Trim();
                                if (!String.IsNullOrEmpty(nodeText))
                                {
                                    _topicTitle = nodeText;
                                }
                                break;
                            case "tableOfContentsTitle":
                                nodeText = reader.ReadString().Trim();
                                if (!String.IsNullOrEmpty(nodeText))
                                {
                                    _topicTocTitle = nodeText;
                                }
                                break;
                            case "linkText":
                                nodeText = reader.ReadString().Trim();
                                if (!String.IsNullOrEmpty(nodeText))
                                {
                                    _topicLinkText = nodeText;
                                }
                                break;
                            case "keyword":
                                KeywordItem keyItem = new KeywordItem();
                                keyItem.ReadXml(reader);
                                if (!keyItem.IsEmpty)
                                {
                                    _keywords.Add(keyItem);
                                }
                                break;
                            case "attribute":
                                AttributeItem attrItem = new AttributeItem();
                                attrItem.ReadXml(reader);
                                if (!attrItem.IsEmpty)
                                {
                                    _attributes.Add(attrItem);
                                }
                                break;
                            case "authoring":
                                break;
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(nodeName, "metadata"))
                        {
                            this.OnDocumentType(nodeName);

                            break;
                        }
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return true;
        }

        #endregion

        #region OnDocumentType Method

        protected virtual void OnDocumentType(string documentTag)
        {   
            if (String.IsNullOrEmpty(_topicTypeId))
            {
                _topicTypeId = ConceptualUtils.ToTopicTypeId(documentTag);
            }
        }

        #endregion

        #region ReadXml Methods

        protected virtual void OnReadXmlTag(XmlReader reader)
        {   
        }

        protected virtual void OnReadExcludes(XmlReader reader)
        {
            //<excludes>
            //    <exclude name="catId1"/>
            //    <exclude name="catId2"/>
            //</excludes>

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listExcludes == null)
            {
                _listExcludes = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);
            }

            string startName = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "exclude",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string nodeText = reader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _listExcludes.Add(nodeText);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected virtual void OnReadMetadata(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startName = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "keyword":
                            KeywordItem keyItem = new KeywordItem();
                            keyItem.ReadXml(reader);
                            if (!keyItem.IsEmpty)
                            {
                                _keywords.Add(keyItem);
                            }
                            break;
                        case "attribute":
                            AttributeItem attrItem = new AttributeItem();
                            attrItem.ReadXml(reader);
                            if (!attrItem.IsEmpty)
                            {
                                _attributes.Add(attrItem);
                            }
                            break;
                        case "authoring":
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region WriteXml Methods

        protected virtual void OnWriteXmlTag(XmlWriter writer)
        {
        }

        protected virtual void OnWriteExcludes(XmlWriter writer)
        {
            writer.WriteStartElement("excludes");   // excludes
            if (_listExcludes != null && _listExcludes.Count != 0)
            {
                foreach (string exclude in _listExcludes)
                {
                    if (!String.IsNullOrEmpty(exclude))
                    {
                        writer.WriteStartElement("exclude");
                        writer.WriteAttributeString("name", exclude);
                        writer.WriteEndElement();
                    }
                }
            }
            writer.WriteEndElement();               // excludes
        }

        protected virtual void OnWriteMetadata(XmlWriter writer)
        {
            ConceptualContent content = this.Content as ConceptualContent;
            Debug.Assert(content != null);
            if (content == null)
            {
                throw new InvalidOperationException(
                    "A content is not associated with this topic.");
            }

            if (content.CompanionFiles)
            {
                string companionFile = Path.ChangeExtension(_filePath, ".cmp");
                this.OnCreateCompanionFile(companionFile, false);
            }
            else
            {
                this.OnWriteMetadata(writer, false, false);
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void OnCreateCompanionFile(string companionPath, bool isBuildOutput)
        {   
            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            if (File.Exists(companionPath))
            {
                File.SetAttributes(companionPath, FileAttributes.Normal);
                File.Delete(companionPath);
            }

            XmlWriter xmlWriter = XmlWriter.Create(companionPath, settings);

            xmlWriter.WriteStartDocument();

            this.OnWriteMetadata(xmlWriter, true, isBuildOutput);

            xmlWriter.WriteEndDocument();

            xmlWriter.Close();
        }

        private void OnWriteMetadata(XmlWriter writer, bool isStandalone,
            bool isBuildOutput)
        {
            writer.WriteStartElement("metadata"); // start - metadata
            if (isStandalone)
            {
                // We include identifiers in the standalone metadata...
                writer.WriteAttributeString("fileAssetGuid", _topicId);
                writer.WriteAttributeString("assetTypeId", "CompanionFile");

                // We include the topic tag in only standalone metadata...
                writer.WriteStartElement("topic"); // start - topic
                writer.WriteAttributeString("id", _topicId);

                writer.WriteElementString("title", _topicTitle);
            }

            if (!String.IsNullOrEmpty(_topicTocTitle))
            {
                writer.WriteElementString("tableOfContentsTitle", _topicTocTitle);
            }
            if (!String.IsNullOrEmpty(_topicLinkText))
            {
                writer.WriteElementString("linkText", _topicLinkText);
            }

            if (_attributes != null && _attributes.Count != 0)
            {
                if (!isBuildOutput)
                {
                    writer.WriteStartElement("attributes"); // start - attributes
                }

                for (int i = 0; i < _attributes.Count; i++)
                {
                    _attributes[i].WriteXml(writer);
                }

                if (!isBuildOutput)
                {
                    writer.WriteEndElement();               // end - attributes
                }
            }

            if (_keywords != null && _keywords.Count != 0)
            {
                if (!isBuildOutput)
                {
                    writer.WriteStartElement("keywords"); // start - keywords
                }

                for (int i = 0; i < _keywords.Count; i++)
                {
                    _keywords[i].WriteXml(writer);
                }

                if (!isBuildOutput)
                {
                    writer.WriteEndElement();             // end - keywords
                }
            }

            if (isStandalone)
            {   
                writer.WriteEndElement();          // end - topic
            }
            if (!isBuildOutput)
            {
                if (_authoring != null)
                {
                    _authoring.WriteXml(writer);
                }
            }

            writer.WriteEndElement();             // end - metadata
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConceptualItem other)
        {
            if (other == null)
            {
                return false;
            }
            //if (!String.Equals(this._name, other._name))
            //{
            //    return false;
            //}
            //if (!String.Equals(this._value, other._value))
            //{
            //    return false;
            //}

            return true;
        }

        public override bool Equals(object obj)
        {
            ConceptualItem other = obj as ConceptualItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 59;
            //if (_name != null)
            //{
            //    hashCode ^= _name.GetHashCode();
            //}
            //if (_value != null)
            //{
            //    hashCode ^= _value.GetHashCode();
            //}

            return hashCode;
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);

            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }
            if (!String.Equals(reader.Name, TagName, 
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string nodeText = reader.GetAttribute("id");
            if (ConceptualUtils.IsValidId(nodeText))
            {
                _topicId = nodeText;
            }
            nodeText = reader.GetAttribute("visible");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _isVisible = Convert.ToBoolean(nodeText);
            }
            nodeText = reader.GetAttribute("revision");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _revNumber = Convert.ToInt32(nodeText);
            }

            string nodeName      = null;
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeName = reader.Name;
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {   
                    switch (nodeName)
                    {
                        case "title":
                            nodeText = reader.ReadString();
                            if (!String.IsNullOrEmpty(nodeText))
                            {
                                _topicTitle = nodeText;
                            }
                            break;
                        case "path":
                            if (_filePath == null)
                            {
                                _filePath = new BuildFilePath();
                            }
                            _filePath.ReadXml(reader);
                            break;
                        case "excludes":
                            this.OnReadExcludes(reader);
                            break;
                        case "metadata":
                            this.OnReadMetadata(reader);
                            break;
                        case "topic":
                            ConceptualItem topicItem = ConceptualItem.Create(
                                reader.GetAttribute("type")); 
                            if (topicItem != null)
                            {
                                topicItem.Content = this.Content;
                                topicItem.BeginInit();
                                topicItem.ReadXml(reader);
                                topicItem.EndInit();

                                this.Add(topicItem);
                            }
                            break;
                        default:
                            this.OnReadXmlTag(reader);
                            break;
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(nodeName, TagName, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            //<topic id="2aca5da4-6f94-43a0-9817-5f413d16f100" type="topic" visible="True" revision="1">
            //    <title>Sandcastle For .NET</title>
            //    <path value="Sandcastle.aml">$(DXROOT)\SampleDir\Sandcastle.aml</path>
            //    <excludes>
            //        <exclude name="catId1"/>
            //        <exclude name="catId2"/>
            //    </excludes>
            //    <metadata>
            //        <keywords>
            //            <keyword type="K" value=""/>
            //        </keywords>
            //        <attributes>
            //            <attribute name="attrName1" value="attrValue1"/>
            //        </attributes>
            //    </metadata>
            //</topic>

            writer.WriteStartElement(TagName);  // topic
            writer.WriteAttributeString("id", _topicId);
            writer.WriteAttributeString("type", this.ItemType.ToString());
            writer.WriteAttributeString("visible", _isVisible.ToString());
            writer.WriteAttributeString("revision",
                this.TopicRevisions.ToString());

            writer.WriteElementString("title", _topicTitle);

            // Markers do not have file path...
            if (_filePath != null)
            {
                _filePath.WriteXml(writer);
            }

            this.OnWriteExcludes(writer);

            this.OnWriteMetadata(writer);

            this.OnWriteXmlTag(writer);

            for (int i = 0; i < this.ItemCount; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();           // topic
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _topicId; 
            }
        }

        #endregion
    }
}
