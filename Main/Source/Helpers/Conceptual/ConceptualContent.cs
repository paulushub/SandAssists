using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualContent : BuildContent<ConceptualItem, ConceptualContent>
    {
        #region Public Fields

        public const string TagName = "conceptualContent";

        #endregion

        #region Private Fields

        private bool   _isLoaded;
        private bool   _hasFilter;
        private bool   _documentExists;
        private bool   _companionFiles;

        private string _contentId;
        private string _defaultTopic;

        private Version _contentVersion;

        private BuildFilePath           _contentFile;
        private BuildDirectoryPath      _contentDir;

        private CategoryContent         _categories;
        private BuildList<ConceptualFilter> _listFilters;

        private BuildKeyedList<ConceptualRelatedTopic> _relatedTopics;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public ConceptualContent()
            : base(new BuildKeyedList<ConceptualItem>())
        {
            _contentId      = Guid.NewGuid().ToString();
            _documentExists = true;
            _categories     = new CategoryContent();

            BuildKeyedList<ConceptualItem> keyedList =
                this.List as BuildKeyedList<ConceptualItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
            _contentVersion = new Version(1, 0);
        }

        public ConceptualContent(string contentFile)
            : this(contentFile, String.Empty)
        {
        }

        public ConceptualContent(string contentFile, string contentDir)
            : this()
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            if (String.IsNullOrEmpty(contentDir))
            {
                contentDir = Path.GetDirectoryName(contentFile);
            }

            _contentFile = new BuildFilePath(contentFile);
            _contentDir  = new BuildDirectoryPath(contentDir);
        }

        public ConceptualContent(IList<ConceptualFilter> listFilters,
            string contentFile, string documentsDir)
            : this(contentFile, documentsDir)
        {
            if (listFilters != null)
            {
                _listFilters = new BuildList<ConceptualFilter>(listFilters);
            }
        }

        public ConceptualContent(ConceptualContent source)
            : base(source)
        {
            _isLoaded       = source._isLoaded;
            _documentExists = source._documentExists;
            _companionFiles = source._companionFiles;
            _defaultTopic   = source._defaultTopic;
            _contentVersion = source._contentVersion;
            _contentFile    = source._contentFile;
            _contentDir     = source._contentDir;
            _categories     = source._categories;
            _listFilters    = source._listFilters;
            _relatedTopics  = source._relatedTopics;
            _dicItems       = source._dicItems;
            _contentId      = source._contentId;
            _hasFilter      = source._hasFilter;
        }

        #endregion

        #region Public Properties

        public string Id
        {
            get
            {
                return _contentId;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (_relatedTopics != null && _relatedTopics.Count != 0)
                {
                    return false;
                }
                if (_contentFile != null && _contentFile.Exists)
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
            }
        }

        public override bool IsHierarchical
        {
            get
            {
                return true;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public ConceptualItem this[string itemId]
        {
            get
            {
                return this[itemId, true];
            }
        }

        public ConceptualItem this[string itemId, bool recursive]
        {
            get
            {
                if (String.IsNullOrEmpty(itemId))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemId, out curIndex))
                {
                    return this[curIndex];
                }

                if (!recursive)
                {
                    return null;
                }

                ConceptualItem itemFound = null;                
                for (int i = 0; i < this.Count; i++)
                {
                    // Get the item at the current index...
                    ConceptualItem item = this[i];

                    // Search this sub-item for the specified ID...
                    itemFound = item[itemId, true];
                    if (itemFound != null)
                    {
                        break;  // ends if found.
                    }
                }

                return itemFound;
            }
        }

        public bool DocumentMustExists
        {
            get
            {
                return _documentExists;
            }  
            set
            {
                _documentExists = value;
            }
        }

        public bool CompanionFiles
        {
            get
            {
                return _companionFiles;
            }
            set
            {
                _companionFiles = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DefaultTopic
        {
            get
            {
                return _defaultTopic;
            }
            set
            {
                _defaultTopic = value;
            }
        }

        public BuildFilePath ContentFile
        {
            get
            {
                return _contentFile;
            }
            set
            {
                if (value != null)
                {
                    _contentFile = value;

                    if (_contentDir == null)
                    {
                        _contentDir = new BuildDirectoryPath(
                            Path.GetDirectoryName(value.Path));
                    }
                }
            }
        }

        public BuildDirectoryPath ContentDir
        {
            get
            {
                return _contentDir;
            }
            set
            {
                if (value != null)
                {
                    _contentDir = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public CategoryContent Categories
        {
            get
            {
                return _categories;
            }
        }

        public IList<ConceptualFilter> Filters
        {
            get
            {
                return _listFilters;
            }
        }

        public IBuildNamedList<ConceptualRelatedTopic> RelatedTopics
        {
            get
            {
                return _relatedTopics;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ConceptualContent"/> class instance, this property is 
        /// <see cref="ConceptualContent.TagName"/>.
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

        #region Public Methods

        #region Load Method

        public void Load()
        {
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _contentId);

            this.Load(resolver);
        }

        public void Load(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_contentFile) ||
                File.Exists(_contentFile) == false)
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            _hasFilter = false;
            if (_listFilters != null && _listFilters.Count != 0)
            {
                int itemCount = _listFilters.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualFilter filter = _listFilters[i];
                    if (filter != null && filter.IsValid && filter.Enabled)
                    {
                        _hasFilter = true;
                        break;
                    }
                }
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_contentFile, settings);

                reader.MoveToContent();

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    this.ReadXml(reader);

                    BuildPathResolver.Pop(resolverId);
                }   

                if (String.IsNullOrEmpty(_defaultTopic))
                {
                    // If not set, use the first topic...
                    if (this.Count != 0)
                    {
                        _defaultTopic = this[0].TopicId;
                    }
                }

                _isLoaded     = true;
                this.Modified = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Reload()
        {
            _isLoaded = false;

            this.Load();
        }

        #endregion

        #region Save Method

        public void Save()
        {
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _contentId);

            this.Save(resolver);
        }

        public void Save(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            // If this is not yet located, and the contents is empty, we
            // will simply not continue from here...
            if (_contentFile != null && _contentFile.Exists)
            {
                if (!this._isLoaded && base.IsEmpty && 
                    (_relatedTopics == null || _relatedTopics.Count == 0))
                {
                    return;
                }

                // If loaded but not modified, there is no need to save it...
                if (this.IsLoaded && !this.Modified)
                {
                    return;
                }
            }

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(_contentFile, settings);

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    writer.WriteStartDocument();

                    this.WriteXml(writer);

                    writer.WriteEndDocument();

                    BuildPathResolver.Pop(resolverId);
                }

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded     = true;
                this.Modified = false;
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

        #region Category Methods

        public void AddCategory(string name, string description)
        {
            this.AddCategory(new CategoryItem(name, description));
        }

        public void AddCategory(CategoryItem category)
        {
            BuildExceptions.NotNull(category, "category");

            if (_categories == null)
            {
                _categories = new CategoryContent();
            }

            _categories.Add(category);
        }

        public void RemoveCategory(int index)
        {
            if (_categories != null)
            {
                _categories.Remove(index);
            }
        }

        public void RemoveCategory(CategoryItem category)
        {
            if (_categories != null && category != null)
            {
                _categories.Remove(category);
            }
        }

        public void ClearCategories()
        {
            if (_categories != null)
            {
                _categories.Clear();
            }
        }

        #endregion

        #region Item Methods

        public override void Add(ConceptualItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.TopicId))
            {
                item.Content = this;

                if (_dicItems.ContainsKey(item.TopicId))
                {
                    this.Insert(_dicItems[item.TopicId], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemId)
        {
            if (String.IsNullOrEmpty(itemId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemId);
        }

        public int IndexOf(string itemId)
        {
            if (String.IsNullOrEmpty(itemId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemId))
            {
                return _dicItems[itemId];
            }

            return -1;
        }

        public bool Remove(string itemId)
        {
            int itemIndex = this.IndexOf(itemId);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemId))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(ConceptualItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.TopicId);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
        }

        public virtual void AddRelated(ConceptualRelatedTopic item)
        {
            if (item == null)
            {
                return;
            }
            if (_relatedTopics == null)
            {
                _relatedTopics = new BuildKeyedList<ConceptualRelatedTopic>();
            }
            if (!_relatedTopics.Contains(item.TopicId))
            {
                _relatedTopics.Add(item);
            }
        }

        public virtual void RemoveRelated(int itemIndex)
        {
        }

        public virtual void RemoveRelated(ConceptualRelatedTopic item)
        {
            if (item == null ||
                (_relatedTopics == null || _relatedTopics.Count == 0))
            {
                return;
            }
            _relatedTopics.Remove(item);
        }

        public virtual void ClearRelated()
        {
            if (_relatedTopics != null)
            {
                _relatedTopics.Clear();
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region Filter Method

        private bool Filter(ConceptualItem item)
        {
            bool isFiltered = false;

            if (_listFilters == null || _listFilters.Count == 0)
            {
                return isFiltered;
            }

            int itemCount = _listFilters.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualFilter filter = _listFilters[i];
                if (filter == null || filter.IsValid == false ||
                    filter.Enabled == false)
                {
                    continue;
                }

                if (filter.Filter(item))
                {
                    isFiltered = true;
                    break;
                }
            }

            return isFiltered;
        }

        #endregion

        #region ReadContents Method

        private void ReadContents(XmlReader reader)
        {
            // Read the version information of the file format...
            string nodeText = reader.GetAttribute("version");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _contentVersion = new Version(nodeText);
            }

            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName      = null;

            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "location", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!reader.IsEmptyElement)
                        {
                            _contentDir = BuildDirectoryPath.ReadLocation(reader);
                        }
                    }
                    else if (String.Equals(nodeName, "propertyGroup", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadPropertyGroup(reader);
                    }
                    else if (String.Equals(nodeName, "categories",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadCategories(reader);
                    }
                    else if (String.Equals(nodeName, "topics", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadTopics(reader);
                    }
                    else if (String.Equals(nodeName, "relatedTopics", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadRelatedTopics(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadPropertyGroup Method

        private void ReadPropertyGroup(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "id":
                                _contentId = reader.ReadString();
                                break;
                            case "companionfiles":
                                string tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _companionFiles = Convert.ToBoolean(tempText);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadCategories Method

        private void ReadCategories(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            // handle the categories...
            if (_categories == null)
            {
                _categories = new CategoryContent();
            }

            _categories.ReadXml(reader);
        }

        #endregion

        #region ReadTopics Method

        private void ReadTopics(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            // handle the default topic id...
            string defaultTopic = reader.GetAttribute("default");
            if (ConceptualUtils.IsValidId(defaultTopic))
            {
                _defaultTopic = defaultTopic;
            }

            string startName = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ConceptualItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ConceptualItem topicItem = ConceptualItem.Create(
                            reader.GetAttribute("type"));

                        if (topicItem != null)
                        {
                            topicItem.Content = this;
                            topicItem.BeginInit();
                            topicItem.ReadXml(reader);
                            topicItem.EndInit();

                            this.Add(topicItem);
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

        #endregion

        #region ReadRelatedTopics Method

        private void ReadRelatedTopics(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_relatedTopics == null)
            {
                _relatedTopics = new BuildKeyedList<ConceptualRelatedTopic>();
            }

            string relatedType = ConceptualItemType.Related.ToString(); 
            string startName  = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ConceptualItem.TagName,
                        StringComparison.OrdinalIgnoreCase) && String.Equals(
                        reader.GetAttribute("type"), relatedType,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        ConceptualRelatedTopic topicItem = new ConceptualRelatedTopic();

                        if (topicItem != null)
                        {
                            topicItem.Content = this;
                            topicItem.BeginInit();
                            topicItem.ReadXml(reader);
                            topicItem.EndInit();

                            _relatedTopics.Add(topicItem);
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

        #endregion

        #region ReadFiles Methods

        private void ReadFiles(XmlReader reader)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName  = null;
            string fileName  = null;
            string fileTitle = null;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "file"))
                    {
                        fileName = reader.GetAttribute("name");
                        fileTitle = reader.GetAttribute("title");
                        if (!String.IsNullOrEmpty(fileName) &&
                            !String.IsNullOrEmpty(fileTitle))
                        {
                            string fullPath = Path.Combine(_contentDir, fileName);
                            ConceptualTopic docItem = new ConceptualTopic(
                                new BuildFilePath(fullPath), fileTitle, String.Empty);

                            docItem.Content = this;

                            docItem.BeginInit();

                            docItem.EndInit();

                            if (!reader.IsEmptyElement)
                            {
                                ReadFile(docItem, reader);
                            }

                            this.Add(docItem);
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "files"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadFile Method

        private void ReadFile(ConceptualItem parentItem, XmlReader reader)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName  = null;
            string fileName  = null;
            string fileTitle = null;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "file"))
                    {
                        fileName  = reader.GetAttribute("name");
                        fileTitle = reader.GetAttribute("title");
                        if (!String.IsNullOrEmpty(fileName) &&
                            !String.IsNullOrEmpty(fileTitle))
                        {
                            string fullPath = Path.Combine(_contentDir, fileName);
                            ConceptualTopic docItem = new ConceptualTopic(
                                new BuildFilePath(fullPath),
                                fileTitle, String.Empty);

                            docItem.Content = this;

                            docItem.BeginInit();

                            docItem.EndInit();

                            // handle the sub-item...
                            if (!reader.IsEmptyElement)
                            {
                                ReadFile(docItem, reader);
                            }

                            parentItem.Add(docItem);
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "file") ||
                        String.Equals(nodeName, "files"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

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
                StringComparison.OrdinalIgnoreCase) && !String.Equals(
                reader.Name, "files", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                // For the current and updated format....
                this.ReadContents(reader);
            }
            else if (String.Equals(reader.Name, "files",
                StringComparison.OrdinalIgnoreCase))
            {
                // For the old temporal format...
                this.ReadFiles(reader);
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName); //conceptualContent
            writer.WriteAttributeString("version", _contentVersion.ToString(2));

            writer.WriteComment(
                " 1. The content directory, if not the same as the content file. ");
            writer.WriteStartElement("location"); // location
            if (_contentDir != null && 
                !_contentDir.IsDirectoryOf(_contentFile))
            {
                _contentDir.WriteXml(writer);
            }
            writer.WriteEndElement();             // location

            writer.WriteComment(" 2. The general content settings ");
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");

            writer.WritePropertyElement("Id", _contentId);
            writer.WritePropertyElement("CompanionFiles", _companionFiles.ToString());

            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteComment(
                " 3. The list of categories defined by this conceptual contents. ");
            if (_categories != null)
            {
                _categories.WriteXml(writer);
            }
            else
            {   
                writer.WriteStartElement("categories"); // categories
                writer.WriteEndElement();             // categories
            }

            writer.WriteComment(
                " 4. The hierarchical list of topics in this contents. ");
            writer.WriteStartElement("topics"); // topics
            if (String.IsNullOrEmpty(_defaultTopic)) 
            {
                // If not set, use the first topic...
                if (this.Count != 0)
                {
                    _defaultTopic = this[0].TopicId;
                }
            }
            if (!String.IsNullOrEmpty(_defaultTopic))
            {
                writer.WriteAttributeString("default", _defaultTopic);
            }

            for (int i = 0; i < this.Count; i++)
            {
                ConceptualItem item = this[i];
                if (item != null && !item.IsEmpty)
                {
                    item.WriteXml(writer);
                }
            }
            writer.WriteEndElement();           // topics

            writer.WriteComment(
                " 5. A list of related topics, which may or may not be compiled, but not included in the TOC. ");
            writer.WriteStartElement("relatedTopics"); // relatedTopics
            if (_relatedTopics != null && _relatedTopics.Count != 0)
            {
                for (int i = 0; i < _relatedTopics.Count; i++)
                {
                    ConceptualRelatedTopic relatedTopic = _relatedTopics[i];
                    if (relatedTopic != null && !relatedTopic.IsEmpty)
                    {
                        relatedTopic.WriteXml(writer);
                    }
                }
            }
            writer.WriteEndElement();                  // relatedTopics

            writer.WriteEndElement();                      //conceptualContent
        }

        #endregion

        #region ICloneable Members

        public override ConceptualContent Clone()
        {
            ConceptualContent content = new ConceptualContent(this);

            this.Clone(content, new BuildKeyedList<ConceptualItem>());

            if (_contentId != null)
            {
                content._contentId = String.Copy(_contentId);
            }
            if (_defaultTopic != null)
            {
                content._defaultTopic = String.Copy(_defaultTopic);
            }

            if (_contentVersion != null)
            {
                content._contentVersion = (Version)_contentVersion.Clone();
            }
            if (_contentFile != null)
            {
                content._contentFile = _contentFile.Clone();
            }
            if (_contentDir != null)
            {
                content._contentDir = _contentDir.Clone();
            }
            if (_categories != null)
            {
                content._categories = _categories.Clone();
            }
            if (_listFilters != null)
            {
                content._listFilters = _listFilters.Clone();
            }
            if (_relatedTopics != null)
            {
                content._relatedTopics = _relatedTopics.Clone();
            }

            return content;
        }

        #endregion
    }
}
