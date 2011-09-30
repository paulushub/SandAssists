using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// It imports its reference contents from another project, and provides
    /// the means to override the project and namespace summaries, custom
    /// table of contents and <c>API</c> filters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The target contents may not be part of another project, however, it
    /// will not be editable in the project of this object. The actual 
    /// reference contents are only loaded during the build processing.
    /// </para>
    /// <para>
    /// As a reference source, this object provides <see cref="ReferenceSource.Comments"/>,
    /// <see cref="ReferenceSource.HierarchicalToc"/>, <see cref="ReferenceSource.TypeFilters"/>
    /// and <see cref="ReferenceSource.AttributeFilters"/>, which can be used
    /// to customize the output, overriding the imported contents.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class ReferenceImportSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.References.ReferenceImportSource";

        #endregion

        #region Private Fields
                           
        private bool? _overrideComments;
        private bool? _overrideFilters;
        private bool? _overrideHierarchicalToc;

        private BuildFilePath _sourcePath;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceImportSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceImportSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceImportSource()
        {
            _overrideFilters         = null;
            _overrideComments        = null;
            _overrideHierarchicalToc = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceImportSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceImportSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceImportSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceImportSource(ReferenceImportSource source)
            : base(source)
        {
            _sourcePath              = source._sourcePath;
            _overrideFilters         = source._overrideFilters;
            _overrideComments        = source._overrideComments;
            _overrideHierarchicalToc = source._overrideHierarchicalToc;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceImportSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                return (_sourcePath != null && _sourcePath.Exists);
            }
        }

        public bool? OverrideFilters
        {
            get
            {
                return _overrideFilters;
            }
            set
            {
                _overrideFilters = value;
            }
        }

        public bool? OverrideComments
        {
            get
            {
                return _overrideComments;
            }
            set
            {
                _overrideComments = value;
            }
        }

        public bool? OverrideHierarchicalToc
        {
            get
            {
                return _overrideHierarchicalToc;
            }
            set
            {
                _overrideHierarchicalToc = value;
            }
        }

        public BuildFilePath SourcePath
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
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

            ReferenceContent content = new ReferenceContent(_sourcePath);

            content.Load();

            CommentContent sourceComments = this.Comments;
            if (_overrideComments == null)
            {   
                // Provide other user-supplied information to the content...
                if (sourceComments != null && !sourceComments.IsEmpty)
                {
                    // Merge the comments, overriding any existing...
                    CommentContent importComments = content.Comments;
                    if (importComments == null || importComments.IsEmpty)
                    {
                        content.Comments = sourceComments; 
                    }
                    else
                    {
                        for (int i = 0; i < sourceComments.Count; i++)
                        {
                            CommentItem sourceItem = sourceComments[i];
                            if (!sourceItem.IsEmpty)
                            {
                                importComments.Add(sourceItem);
                            }
                        }
                    }      
                }
            }
            else if (_overrideComments.Value)
            {
                content.Comments = sourceComments; 
            }

            HierarchicalTocContent hierarchicalToc = this.HierarchicalToc;
            if (_overrideHierarchicalToc == null)
            {
                if (hierarchicalToc != null && !hierarchicalToc.IsEmpty)
                {
                    content.HierarchicalToc = hierarchicalToc;
                }
            }
            else if (_overrideHierarchicalToc.Value)
            {
                content.HierarchicalToc = hierarchicalToc;
            }

            ReferenceRootFilter typeFilters      = this.TypeFilters;
            ReferenceRootFilter attributeFilters = this.AttributeFilters;
            if (_overrideFilters == null)
            {
                if (typeFilters != null && !typeFilters.IsEmpty)
                {
                    content.TypeFilters = typeFilters;
                }
                if (attributeFilters != null && !attributeFilters.IsEmpty)
                {
                    content.AttributeFilters = attributeFilters;
                }
            }
            else if (_overrideFilters.Value)
            {
                content.TypeFilters      = typeFilters;
                content.AttributeFilters = attributeFilters;
            }

            return content;
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
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "title":
                                this.Title = reader.ReadString();
                                break;
                            case "overridecomments":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText) ||
                                    String.Equals(tempText, "null", StringComparison.OrdinalIgnoreCase))
                                {
                                    _overrideComments = null;
                                }
                                else
                                {
                                    _overrideComments = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "overridefilters":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText) ||
                                    String.Equals(tempText, "null", StringComparison.OrdinalIgnoreCase))
                                {
                                    _overrideFilters = null;
                                }
                                else
                                {
                                    _overrideFilters = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "overridehierarchicaltoc":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText) ||
                                    String.Equals(tempText, "null", StringComparison.OrdinalIgnoreCase))
                                {
                                    _overrideHierarchicalToc = null;
                                }
                                else
                                {
                                    _overrideHierarchicalToc = Convert.ToBoolean(tempText);
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
                    else if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _sourcePath = BuildFilePath.ReadLocation(reader);
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
            writer.WritePropertyElement("Title", this.Title);
            writer.WritePropertyElement("OverrideComments",        _overrideComments.ToString());
            writer.WritePropertyElement("OverrideFilters",         _overrideFilters.ToString());
            writer.WritePropertyElement("OverrideHierarchicalToc", _overrideHierarchicalToc.ToString());
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("location");  // start - location
            _sourcePath.WriteXml(writer);
            writer.WriteEndElement();              // end - location

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
            ReferenceImportSource source = new ReferenceImportSource(this);

            this.Clone(source);

            if (_sourcePath != null)
            {
                source._sourcePath = _sourcePath.Clone();
            }

            return source;
        }

        #endregion
    }
}
