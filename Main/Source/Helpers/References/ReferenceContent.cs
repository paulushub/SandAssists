using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceContent : BuildContent<ReferenceItem, ReferenceContent>
    {
        #region Public Fields

        public const string TagName = "referenceContent";

        #endregion

        #region Private Fields

        private bool    _isLoaded;
        private string  _contentId;
        private Version _contentVersion;

        private BuildFilePath          _contentFile;
        private BuildDirectoryPath     _contentDir;

        private BuildFrameworkType     _frameworkType;

        private CommentContent         _commentContent;
        private DependencyContent      _dependencies;
        private HierarchicalTocContent _tocContent;

        private ReferenceRootFilter    _typeFilters;
        private ReferenceRootFilter    _attributeFilters;

        #endregion

        #region Constructors and Destructor

        public ReferenceContent()
        {
            _contentVersion   = new Version(1, 0, 0, 0);
            _contentId        = Guid.NewGuid().ToString();
            _frameworkType    = BuildFrameworkType.Framework20;
            _dependencies     = new DependencyContent();

            _tocContent       = new HierarchicalTocContent();
            _commentContent   = new CommentContent();
            _typeFilters      = new ReferenceRootFilter(false);
            _attributeFilters = new ReferenceRootFilter(true);
        }

        public ReferenceContent(string contentFile)
            : this(contentFile, String.Empty)
        {
        }

        public ReferenceContent(string contentFile, string contentDir)
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

        public ReferenceContent(ReferenceContent source)
            : base(source)
        {
            _isLoaded         = source._isLoaded;
            _contentId        = source._contentId;
            _contentVersion   = source._contentVersion;
            _contentFile      = source._contentFile;
            _contentDir       = source._contentDir;     
            _frameworkType    = source._frameworkType;
            _tocContent       = source._tocContent;
            _dependencies     = source._dependencies;
            _commentContent   = source._commentContent;
            _typeFilters      = source._typeFilters;
            _attributeFilters = source._attributeFilters;
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

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public BuildFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
            set
            {
                _frameworkType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public DependencyContent Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        public CommentContent Comments
        {
            get
            {
                return _commentContent;
            }
            set
            {
                if (value != null)
                {
                    _commentContent = value;
                }
            }
        }

        public HierarchicalTocContent HierarchicalToc
        {
            get
            {
                return _tocContent;
            }
            set
            {
                if (value != null)
                {
                    _tocContent = value;
                }
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
        public ReferenceRootFilter TypeFilters
        {
            get
            {
                return _typeFilters;
            }
            set
            {
                if (value != null)
                {
                    _typeFilters = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceRootFilter AttributeFilters
        {
            get
            {
                return _attributeFilters;
            }
            set
            {
                if (value != null)
                {
                    _attributeFilters = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceContent"/> class instance, this 
        /// property is <see cref="ReferenceContent.TagName"/>.
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

                _isLoaded = true;
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
                if (!this._isLoaded && base.IsEmpty)
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
                _isLoaded = true;
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

        #region Item Methods

        public void AddItem(string comments, string assembly)
        {
            if (String.IsNullOrEmpty(comments) && String.IsNullOrEmpty(assembly))
            {
                return;
            }

            this.Add(new ReferenceItem(comments, assembly));
        }

        public void AddDependency(string assembly)
        {
            if (String.IsNullOrEmpty(assembly))
            {
                return;
            }
            if (_dependencies == null)
            {
                _dependencies = new DependencyContent();
            }

            _dependencies.Add(new DependencyItem(
                Path.GetFileName(assembly), assembly));
        }

        #endregion

        #endregion

        #region Private Methods

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
                            case "frameworktype":
                                string tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _frameworkType = (BuildFrameworkType)
                                        BuildFrameworkType.Parse(tempText);
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

        #region ReadItems Method

        private void ReadItems(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startElement = reader.Name;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ReferenceItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceItem item = new ReferenceItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadContents Method

        private void ReadContents(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startElement = reader.Name;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "comments":
                                if (_commentContent == null)
                                {
                                    _commentContent = new CommentContent();
                                }
                                if (reader.IsEmptyElement)
                                {
                                    string sourceFile = reader.GetAttribute("source");
                                    if (!String.IsNullOrEmpty(sourceFile))
                                    {
                                        _commentContent.ContentFile = new BuildFilePath(sourceFile);
                                        _commentContent.Load();
                                    }
                                }
                                else
                                {
                                    if (reader.ReadToDescendant(CommentContent.TagName))
                                    {
                                        _commentContent.ReadXml(reader);
                                    }
                                }
                                break;
                            case "dependencies":
                                if (_dependencies == null)
                                {
                                    _dependencies = new DependencyContent();
                                }
                                if (!reader.IsEmptyElement && reader.ReadToDescendant(
                                    DependencyContent.TagName))
                                {
                                    _dependencies.ReadXml(reader);
                                }
                                break;
                            case "hierarchicaltoc":
                                if (_tocContent == null)
                                {
                                    _tocContent = new HierarchicalTocContent();
                                }
                                if (!reader.IsEmptyElement && reader.ReadToDescendant(
                                    HierarchicalTocContent.TagName))
                                {
                                    _tocContent.ReadXml(reader);
                                }
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadFilters Method

        private void ReadFilters(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startElement = reader.Name;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ReferenceRootFilter.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string filterType = reader.GetAttribute("type");
                        if (String.Equals(filterType, "ApiFilter", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (_typeFilters == null)
                            {
                                _typeFilters = new ReferenceRootFilter(false);
                            }

                            _typeFilters.ReadXml(reader);
                        }
                        else if (String.Equals(filterType, "AttributeFilter",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (_attributeFilters == null)
                            {
                                _attributeFilters = new ReferenceRootFilter(true);
                            }

                            _attributeFilters.ReadXml(reader);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            // Read the version information of the file format...
            string nodeText = reader.GetAttribute("version");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _contentVersion = new Version(nodeText);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "location":
                            if (!reader.IsEmptyElement)
                            {
                                _contentDir = BuildDirectoryPath.ReadLocation(reader);
                            }
                            break;
                        case "propertygroup":
                            this.ReadPropertyGroup(reader);
                            break;
                        case "referenceitems":
                            this.ReadItems(reader);
                            break;
                        case "contents":
                            this.ReadContents(reader);
                            break;
                        case "filters":
                            this.ReadFilters(reader);
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("version", _contentVersion.ToString(2));

            // 1. The content directory, if not the same as the content file.
            writer.WriteComment(
                " 1. The content directory, if not the same as the content file. ");
            writer.WriteStartElement("location"); // start - location
            if (_contentDir != null &&
                !_contentDir.IsDirectoryOf(_contentFile))
            {
                _contentDir.WriteXml(writer);
            }
            writer.WriteEndElement();             // end - location

            // 2. The general content settings
            writer.WriteComment(" 2. The general content settings ");
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");

            writer.WritePropertyElement("Id",            _contentId);
            writer.WritePropertyElement("FrameworkType", _frameworkType.ToString());

            writer.WriteEndElement();                   // end - propertyGroup

            // 3. The reference items defining the API content
            writer.WriteComment(" 3. The reference items defining the API content ");
            writer.WriteStartElement("referenceItems");  // start - referenceItems
            for (int i = 0; i < this.Count; i++)
            {
                ReferenceItem item = this[i];
                if (item != null && !item.IsEmpty)
                {
                    item.WriteXml(writer);
                }
            }
            writer.WriteEndElement();                    // end - referenceItems   

            // 4. The various contents required for documentation
            writer.WriteComment(" 4. The various contents required for documentation ");
            writer.WriteStartElement("contents");  // start - contents

            if (_commentContent != null)
            {
                BuildFilePath filePath = _commentContent.ContentFile;
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Comments");
                if (filePath != null && filePath.IsValid)
                {
                    BuildPathResolver resolver = BuildPathResolver.Resolver;
                    Debug.Assert(resolver != null && resolver.Id == _contentId);

                    writer.WriteAttributeString("source", 
                        resolver.ResolveRelative(filePath));
                    _commentContent.Save();
                }
                else
                {
                    _commentContent.WriteXml(writer);
                }
                writer.WriteEndElement();
            }   
            if (_dependencies != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Dependencies");
                _dependencies.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_tocContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "HierarchicalToc");
                _tocContent.WriteXml(writer);
                writer.WriteEndElement();
            }              
            writer.WriteEndElement();              // end - contents

            // 5. The API/attribute filters associated with the documentations
            writer.WriteComment(" 5. The API/attribute filters associated with the documentations ");
            writer.WriteStartElement("filters");  // start - filters
            if (_typeFilters != null)
            {
                _typeFilters.WriteXml(writer);
            }
            if (_attributeFilters != null)
            {
                _attributeFilters.WriteXml(writer);
            }
            writer.WriteEndElement();             // end - filters   

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceContent Clone()
        {
            ReferenceContent content = new ReferenceContent(this);

            this.Clone(content);

            if (_contentId != null)
            {
                content._contentId = String.Copy(_contentId);
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
            if (_commentContent != null)
            {
                content._commentContent = _commentContent.Clone();
            }
            if (_dependencies != null)
            {
                content._dependencies = _dependencies.Clone();
            }
            if (_tocContent != null)
            {
                content._tocContent = _tocContent.Clone();
            }
            if (_typeFilters != null)
            {
                content._typeFilters = (ReferenceRootFilter)_typeFilters.Clone();
            }
            if (_attributeFilters != null)
            {
                content._attributeFilters = (ReferenceRootFilter)_attributeFilters.Clone();
            }

            return content;
        }

        #endregion
    }
}
