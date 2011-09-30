using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class BibliographyItem : BuildItem<BibliographyItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "bibItem";

        #endregion

        #region Private Fields

        private string _name;
        private string _link;
        private string _title;
        private string _author;
        private string _publisher;

        #endregion

        #region Constructors and Destructor

        public BibliographyItem()
            : this(Guid.NewGuid().ToString())
        {
        }

        public BibliographyItem(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name      = name;
            _title     = String.Empty;
            _author    = String.Empty;
            _link      = String.Empty;
            _publisher = String.Empty;
        }

        public BibliographyItem(string name, string title, string author,
            string publisher, string link) : this(name)
        {
            if (title != null)
            {
                _title = title;
            }
            if (author != null)
            {
                _author = author;
            }
            if (publisher != null)
            {
                _publisher = publisher;
            }
            if (link != null)
            {
                _link = link;
            }
        }

        public BibliographyItem(BibliographyItem source)
            : base(source)
        {
            _name      = source._name;
            _title     = source._title;
            _author    = source._author;
            _link      = source._link;
            _publisher = source._publisher;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                {
                    return true;
                }

                return false;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _title = value;
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
                if (value == null)
                {
                    value = String.Empty;
                }
                _author = value;
            }
        }

        public string Publisher
        {
            get
            {
                return _publisher;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _publisher = value;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _link = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BibliographyItem"/> class instance, this property is 
        /// <see cref="BibliographyItem.TagName"/>.
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

        #region IEquatable<T> Members

        public override bool Equals(BibliographyItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._title, other._title))
            {
                return false;
            }
            if (!String.Equals(this._author, other._author))
            {
                return false;
            }
            if (!String.Equals(this._link, other._link))
            {
                return false;
            }
            if (!String.Equals(this._publisher, other._publisher))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            BibliographyItem other = obj as BibliographyItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_title != null)
            {
                hashCode ^= _title.GetHashCode();
            }
            if (_author != null)
            {
                hashCode ^= _author.GetHashCode();
            }
            if (_link != null)
            {
                hashCode ^= _link.GetHashCode();
            }
            if (_publisher != null)
            {
                hashCode ^= _publisher.GetHashCode();
            }

            return hashCode;
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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            _name = reader.GetAttribute("name");
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
                        case "author":
                            _author = reader.ReadString();
                            break;
                        case "title":
                            _title = reader.ReadString();
                            break;
                        case "link":
                            _link = reader.ReadString();
                            break;
                        case "publisher":
                            _publisher = reader.ReadString();
                            break;
                        default:
                            throw new NotSupportedException(String.Format(
                                "The element '{0}' is not supported.", reader.Name));
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
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void ImportXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, "reference"));
                return;
            }

            _name = reader.GetAttribute("name");
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
                        case "author":
                            _author = reader.ReadString();
                            break;
                        case "title":
                            _title = reader.ReadString();
                            break;
                        case "link":
                            _link = reader.ReadString();
                            break;
                        case "publisher":
                            _publisher = reader.ReadString();
                            break;
                        default:
                            throw new NotSupportedException(String.Format(
                                "The element '{0}' is not supported.", reader.Name));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "reference",
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", _name);
            writer.WriteTextElement("author",    _author);
            writer.WriteTextElement("title",     _title);
            writer.WriteTextElement("publisher", _publisher);
            writer.WriteTextElement("link",      _link);
            writer.WriteEndElement();           // end - TagName
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
        public void ExportXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement("reference");  // start - reference
            writer.WriteAttributeString("name", _name);
            writer.WriteTextElement("author",    _author);
            writer.WriteTextElement("title",     _title);
            writer.WriteTextElement("publisher", _publisher);
            writer.WriteTextElement("link",      _link);
            writer.WriteEndElement();               // end - reference
        }

        #endregion

        #region ICloneable Members

        public override BibliographyItem Clone()
        {
            BibliographyItem item = new BibliographyItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_title != null)
            {
                item._title = String.Copy(_title);
            }
            if (_author != null)
            {
                item._author = String.Copy(_author);
            }
            if (_link != null)
            {
                item._link = String.Copy(_link);
            }
            if (_publisher != null)
            {
                item._publisher = String.Copy(_publisher);
            }

            return item;
        }

        #endregion
    }
}
