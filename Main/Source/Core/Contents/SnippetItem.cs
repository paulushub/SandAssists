using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class SnippetItem : BuildItem<SnippetItem>
    {
        #region Public Fields

        public const string TagName = "snippetItem";

        #endregion

        #region Private Fields

        private string _itemId;
        private BuildDirectoryPath _source;

        #endregion

        #region Constructors and Destructor

        public SnippetItem()
        {
            _itemId = Guid.NewGuid().ToString();
        }

        public SnippetItem(string source)
            : this()
        {
            BuildExceptions.PathMustExist(source, "source");

            _source = new BuildDirectoryPath(source);
        }

        public SnippetItem(BuildDirectoryPath source)
            : this()
        {
            BuildExceptions.NotNull(source, "source");

            _source = source;
        }

        public SnippetItem(string source, string itemId)
            : this()
        {
            BuildExceptions.PathMustExist(source, "source");

            if (!String.IsNullOrEmpty(itemId))
            {
                _itemId = itemId;
            }
            _source = new BuildDirectoryPath(source);
        }

        public SnippetItem(BuildDirectoryPath source, string itemId)
            : this()
        {
            BuildExceptions.NotNull(source, "source");

            if (!String.IsNullOrEmpty(itemId))
            {
                _itemId = itemId;
            }   
            _source = source;
        }

        public SnippetItem(SnippetItem source)
            : base(source)
        {
            _itemId = source._itemId;
            _source = source._source;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_source == null || !_source.Exists);
            }
        }

        public string Id
        {
            get
            {
                return _itemId;
            }
        }

        public BuildDirectoryPath Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (value != null)
                {
                    _source = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="SnippetItem"/> class instance, this property is 
        /// <see cref="SnippetItem.TagName"/>.
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

        public override bool Equals(SnippetItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._itemId, other._itemId))
            {
                return false;
            }
            if (this._source != other._source)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            SnippetItem other = obj as SnippetItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 23;
            if (_itemId != null)
            {
                hashCode ^= _itemId.GetHashCode();
            }
            if (_source != null)
            {
                hashCode ^= _source.GetHashCode();
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
                return;
            }
            _itemId = reader.GetAttribute("id");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {   
                    if (String.Equals(reader.Name, BuildFilePath.TagName, 
                        StringComparison.OrdinalIgnoreCase))
                    {   
                        if (_source == null)
                        {
                            _source = new BuildDirectoryPath();
                        }

                        _source.ReadXml(reader);
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - snippet
            writer.WriteAttributeString("id", _itemId);

            _source.WriteXml(writer);

            writer.WriteEndElement();           // end - snippet
        }

        #endregion

        #region ICloneable Members

        public override SnippetItem Clone()
        {
            SnippetItem resource = new SnippetItem(this);
            if (_itemId != null)
            {
                resource._itemId = String.Copy(_itemId);
            }
            if (_source != null)
            {
                resource._source = _source.Clone();
            }

            return resource;
        }

        #endregion
    }
}
