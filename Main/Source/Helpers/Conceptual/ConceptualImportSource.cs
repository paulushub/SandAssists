using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualImportSource : ConceptualSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.Conceptual.ConceptualImportSource";

        #endregion

        #region Private Fields

        private BuildFilePath _sourcePath;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualImportSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualDocProjSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualDocProjSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualImportSource(ConceptualImportSource source)
            : base(source)
        {
            _sourcePath = source._sourcePath;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ConceptualImportSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                return (_sourcePath != null && _sourcePath.Exists);
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

        public override ConceptualContent Create(BuildGroupContext groupContext)
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

            ConceptualContent content = new ConceptualContent(_sourcePath);

            content.Load();

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
                        switch (reader.GetAttribute("name").ToLower())
                        {
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
                    else if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _sourcePath = BuildFilePath.ReadLocation(reader);
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
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("location");  // start - location
            _sourcePath.WriteXml(writer);
            writer.WriteEndElement();              // end - location

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ConceptualSource Clone()
        {
            ConceptualImportSource source = new ConceptualImportSource(this);

            if (this.Title != null)
            {
                source.Title = String.Copy(this.Title);
            }
            if (_sourcePath != null)
            {
                source._sourcePath = _sourcePath.Clone();
            }

            return source;
        }

        #endregion
    }
}
