using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionSource : ReferenceSource, IBuildNamedItem
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceVersionSource";

        #endregion

        #region Private Fields

        private string           _sourceId;
        private string           _versionId;
        private string           _versionLabel;
        private ReferenceContent _content;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceVersionSource()
            : this(String.Format("VerId{0:x}", Guid.NewGuid().ToString().GetHashCode()))
        {
        }

        public ReferenceVersionSource(string versionId)
        {
            if (String.IsNullOrEmpty(versionId))
            {
                _versionId = String.Format("VerId{0:x}", Guid.NewGuid().ToString().GetHashCode());
                _sourceId  = String.Copy(_versionId);
            }
            else
            {
                _versionId = versionId;
                _sourceId  = String.Format("VerId{0:x}", Guid.NewGuid().ToString().GetHashCode());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceVersionSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVersionSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVersionSource(ReferenceVersionSource source)
            : base(source)
        {
            _content      = source._content;
            _sourceId     = source._sourceId;
            _versionId    = source._versionId;
            _versionLabel = source._versionLabel;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceVersionSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_versionLabel) || 
                    _content == null || _content.IsEmpty)
                {
                    return false;
                }

                return true;
            }
        }

        public string SourceId
        {
            get
            {
                return _sourceId;
            }
        }

        public string VersionId
        {
            get
            {
                return _versionId;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _versionId = value;
                }
            }
        }

        public string VersionLabel
        {
            get
            {
                return _versionLabel;
            }
            set
            {
                if (value != null)
                {
                    _versionLabel = value.Trim();
                }
                else
                {
                    _versionLabel = String.Empty;
                }
            }
        }

        public ReferenceContent Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        #endregion

        #region Public Methods

        public override ReferenceContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            return _content;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get
            {
                return _versionId;
            }
        }

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
                            case "sourceid":
                                _sourceId = reader.ReadString();
                                break;
                            case "versionid":
                                _versionId = reader.ReadString();
                                break;
                            case "versionlabel":
                                _versionLabel = reader.ReadString();
                                break;
                            case "title":
                                this.Title = reader.ReadString();
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

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadPropertyGroup(reader);
                    }
                    else if (String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (_content == null)
                        {
                            _content = new ReferenceContent();
                        }
                        if (reader.IsEmptyElement)
                        {
                            string sourceFile = reader.GetAttribute("source");
                            if (!String.IsNullOrEmpty(sourceFile))
                            {
                                _content.ContentFile = new BuildFilePath(sourceFile);
                                _content.Load();
                            }
                        }
                        else
                        {
                            if (reader.ReadToDescendant(ReferenceContent.TagName))
                            {
                                _content.ReadXml(reader);
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, "contents",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadContents(reader);
                    }
                    else if (String.Equals(reader.Name, "filters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadFilters(reader);
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

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", this.Name);

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("SourceId",     _sourceId);
            writer.WritePropertyElement("VersionId",    _versionId);
            writer.WritePropertyElement("VersionLabel", _versionLabel);
            writer.WritePropertyElement("Title",        this.Title);
            writer.WriteEndElement();                   // end - propertyGroup

            if (_content != null)
            {
                BuildFilePath filePath = _content.ContentFile;
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Reference");
                if (filePath != null && filePath.IsValid)
                {
                    BuildPathResolver resolver = BuildPathResolver.Resolver;

                    writer.WriteAttributeString("source",
                        resolver.ResolveRelative(filePath));

                    _content.Save();
                }
                else
                {
                    _content.WriteXml(writer);
                }
                writer.WriteEndElement();
            }

            // Write the user-defined contents...
            this.WriteContents(writer);

            // Write the filters...
            this.WriteFilters(writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ReferenceSource Clone()
        {
            ReferenceVersionSource source = new ReferenceVersionSource(this);

            this.Clone(source);

            if (this.Title != null)
            {
                source.Title = String.Copy(this.Title);
            }
            if (_sourceId != null)
            {
                source._sourceId = String.Copy(_sourceId);
            }
            if (_versionLabel != null)
            {
                source._versionLabel = String.Copy(_versionLabel);
            }
            if (_versionId != null)
            {
                source._versionId = String.Copy(_versionId);
            }
            if (_content != null)
            {
                source._content = _content.Clone();
            }

            return source;
        }

        #endregion
    }
}
