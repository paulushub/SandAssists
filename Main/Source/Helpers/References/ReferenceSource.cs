using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ReferenceSource : BuildSource<ReferenceSource>
    {
        #region Public Fields

        public const string TagName = "contentSource";

        #endregion

        #region Private Fields

        private string                 _title;

        private CommentContent         _commentContent;
        private HierarchicalTocContent _tocContent;

        private ReferenceRootFilter    _typeFilters;
        private ReferenceRootFilter    _attributeFilters;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class
        /// with the default parameters.
        /// </summary>
        protected ReferenceSource()
        {
            _title            = String.Format("ReferenceSource{0:x}",
                Guid.NewGuid().ToString().GetHashCode());

            _tocContent       = new HierarchicalTocContent();
            _commentContent   = new CommentContent();
            _typeFilters      = new ReferenceRootFilter(false);
            _attributeFilters = new ReferenceRootFilter(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ReferenceSource(ReferenceSource source)
            : base(source)
        {
            _title            = source._title;
            _tocContent       = source._tocContent;
            _commentContent   = source._commentContent;
            _typeFilters      = source._typeFilters;
            _attributeFilters = source._attributeFilters;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this content source is a content
        /// generator.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this source is a content generator;
        /// otherwise, it is <see langword="false"/>. By default, this returns 
        /// <see langword="false"/>.
        /// </value>
        public override bool IsGenerator
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the title or description of this reference content
        /// source. This is the also the displayed name of this content source.
        /// </summary>
        /// <value>
        /// A string containing the descriptive name of this content source.
        /// </value>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (value != null && value.Length > 5)
                {
                    _title = value;
                }
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
        /// For the <see cref="ReferenceSource"/> class instance, this property is 
        /// <see cref="ReferenceSource.TagName"/>.
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupContext"></param>
        /// <returns></returns>
        public abstract ReferenceContent Create(BuildGroupContext groupContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceSource CreateSource(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            switch (name)
            {
                case ReferenceImportSource.SourceName:
                    return new ReferenceImportSource();
                case ReferenceVersionSource.SourceName:
                    return new ReferenceVersionSource();
                case ReferenceVsNetSource.SourceName:
                    return new ReferenceVsNetSource();
                case ReferenceAjaxDocSource.SourceName:
                    return new ReferenceAjaxDocSource();
                case ReferenceDirectorySource.SourceName:
                    return new ReferenceDirectorySource();
                case ReferenceLinkSource.SourceName:
                    return new ReferenceLinkSource();
                default:
                    throw new NotImplementedException(name);
            }
        }

        #endregion

        #region Protected Methods

        #region ReadContents Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadContents(XmlReader reader)
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
                            default:
                                this.OnReadContents(reader);
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

        protected virtual void OnReadContents(XmlReader reader)
        {   
        }

        #endregion

        #region ReadFilters Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadFilters(XmlReader reader)
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

        #region WriteContents Method

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
        protected virtual void WriteContents(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            // The various contents required for documentation
            writer.WriteComment(" The various contents required for documentation ");
            writer.WriteStartElement("contents");  // start - contents

            if (_commentContent != null)
            {
                BuildFilePath filePath = _commentContent.ContentFile;
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Comments");
                if (filePath != null && filePath.IsValid)
                {
                    BuildPathResolver resolver = BuildPathResolver.Resolver;

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
            if (_tocContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "HierarchicalToc");
                _tocContent.WriteXml(writer);
                writer.WriteEndElement();
            }

            this.OnWriteContents(writer);

            writer.WriteEndElement();              // end - contents
        }

        protected virtual void OnWriteContents(XmlWriter writer)
        {   
        }

        #endregion

        #region WriteFilters Method

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
        protected virtual void WriteFilters(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            // The API/attribute filters associated with the documentations
            writer.WriteComment(" The API/attribute filters associated with the documentations ");
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
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clonedSource"></param>
        /// <returns></returns>
        protected virtual ReferenceSource Clone(ReferenceSource clonedSource)
        {
            if (clonedSource == null)
            {
                clonedSource = (ReferenceSource)this.MemberwiseClone();
            }

            if (_title != null)
            {
                clonedSource._title = String.Copy(_title);
            }

            if (_commentContent != null)
            {
                clonedSource._commentContent = _commentContent.Clone();
            }
            if (_tocContent != null)
            {
                clonedSource._tocContent = _tocContent.Clone();
            }
            if (_typeFilters != null)
            {
                clonedSource._typeFilters = (ReferenceRootFilter)_typeFilters.Clone();
            }
            if (_attributeFilters != null)
            {
                clonedSource._attributeFilters = (ReferenceRootFilter)_attributeFilters.Clone();
            }

            return clonedSource;
        }

        #endregion
    }
}
