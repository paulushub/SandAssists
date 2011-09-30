using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateAuthoring : BuildObject<TemplateAuthoring>
    {
        #region Private Fields

        private const string TagName = "TemplateAuthoring";

        private string _author;
        private string _createdDate;
        private string _modifiedDate;
        private string _copyright;

        #endregion

        #region Constructors and Destructor

        public TemplateAuthoring()
        {
            _author       = String.Empty;
            _createdDate  = String.Empty;
            _modifiedDate = String.Empty;
            _copyright    = String.Empty;
        }

        public TemplateAuthoring(TemplateAuthoring source)
            : base(source)
        {
            _author       = source._author;
            _createdDate  = source._createdDate;
            _modifiedDate = source._modifiedDate;
            _copyright    = source._copyright;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_author) && String.IsNullOrEmpty(_createdDate)
                    && String.IsNullOrEmpty(_modifiedDate) && String.IsNullOrEmpty(_copyright);
            }
        }

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
            }
        }

        public string CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
            }
        }

        public string ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
            set
            {
                _modifiedDate = value;
            }
        }

        public string Copyright
        {
            get
            {
                return _copyright;
            }
            set
            {
                _copyright = value;
            }
        }

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
                return;
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            _author       = String.Empty;
            _createdDate  = String.Empty;
            _modifiedDate = String.Empty;
            _copyright    = String.Empty;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {   
                    switch (reader.Name)
                    {
                        case "Author":
                            _author = reader.ReadString();
                            break;
                        case "Created":
                            _createdDate = reader.ReadString();
                            break;
                        case "Modified":
                            _modifiedDate = reader.ReadString();
                            break;
                        case "Copyright":
                            _copyright = reader.ReadString();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName, StringComparison.OrdinalIgnoreCase))
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TemplateAuthoring 
            writer.WriteTextElement("Author",    _author);
            writer.WriteTextElement("Created",   _createdDate);
            writer.WriteTextElement("Modified",  _modifiedDate);
            writer.WriteTextElement("Copyright", _copyright);
            writer.WriteEndElement();           // end - TemplateAuthoring
        }

        #endregion

        #region ICloneable Members

        public override TemplateAuthoring Clone()
        {
            TemplateAuthoring authoring = new TemplateAuthoring(this);

            if (_author != null)
            {
                authoring._author = String.Copy(_author);
            }
            if (_createdDate != null)
            {
                authoring._createdDate = String.Copy(_createdDate);
            }
            if (_modifiedDate != null)
            {
                authoring._modifiedDate = String.Copy(_modifiedDate);
            }
            if (_copyright != null)
            {
                authoring._copyright = String.Copy(_copyright);
            }

            return authoring;
        }

        #endregion
    }
}
