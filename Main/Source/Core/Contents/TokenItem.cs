using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TokenItem : BuildItem<TokenItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private string _itemId;
        private string _itemValue;

        #endregion

        #region Constructors and Destructor

        public TokenItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public TokenItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _itemId    = key;
            _itemValue = value;
        }

        public TokenItem(TokenItem source)
            : base(source)
        {
            _itemId    = source._itemId;
            _itemValue = source._itemValue;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_itemId))
                {
                    return true;
                }

                return false;
            }
        }

        public string Key
        {
            get
            {
                return _itemId;
            }
        }

        public string Value
        {
            get
            {
                return _itemValue;
            }
            set
            {
                _itemValue = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="TokenItem"/> class instance, this property is 
        /// <see cref="TokenItem.TagName"/>.
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

        public override bool Equals(TokenItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._itemId, other._itemId))
            {
                return false;
            }
            if (!String.Equals(this._itemValue, other._itemValue))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            TokenItem other = obj as TokenItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 11;
            if (_itemId != null)
            {
                hashCode ^= _itemId.GetHashCode();
            }
            if (_itemValue != null)
            {
                hashCode ^= _itemValue.GetHashCode();
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

            if (String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                _itemValue = reader.GetAttribute("id");
                _itemValue = reader.ReadInnerXml();
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

            writer.WriteStartElement(TagName);  // start - attribute
            writer.WriteAttributeString("id", _itemId);
            writer.WriteRaw(_itemValue);
            writer.WriteEndElement();           // end - attribute
        }

        #endregion

        #region ICloneable Members

        public override TokenItem Clone()
        {
            TokenItem item = new TokenItem(this);
            if (_itemId != null)
            {
                item._itemId = String.Copy(_itemId);
            }
            if (_itemValue != null)
            {
                item._itemValue = String.Copy(_itemValue);
            }

            return item;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _itemId; 
            }
        }

        #endregion
    }
}
