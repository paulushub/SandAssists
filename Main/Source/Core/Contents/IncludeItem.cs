using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class IncludeItem : BuildItem<IncludeItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "includeItem";

        #endregion

        #region Private Fields

        private string _key;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public IncludeItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public IncludeItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _key    = key;
            _value  = value;
        }

        public IncludeItem(IncludeItem source)
            : base(source)
        {
            _key    = source._key;
            _value  = source._value;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_key) || String.IsNullOrEmpty(_value))
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
                return _key;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="IncludeItem"/> class instance, this property is 
        /// <see cref="IncludeItem.TagName"/>.
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

        public override bool Equals(IncludeItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._key, other._key))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            IncludeItem other = obj as IncludeItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_key != null)
            {
                hashCode ^= _key.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
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
                _key   = reader.GetAttribute("key");
                _value = reader.ReadString();
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
            writer.WriteAttributeString("key", _key);
            writer.WriteString(_value);
            writer.WriteEndElement();           // end - attribute
        }

        #endregion

        #region ICloneable Members

        public override IncludeItem Clone()
        {
            IncludeItem item = new IncludeItem(this);
            if (_key != null)
            {
                item._key = String.Copy(_key);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }

            return item;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _key; 
            }
        }

        #endregion
    }
}
