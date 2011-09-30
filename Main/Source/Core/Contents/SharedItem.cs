using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class SharedItem : BuildItem<SharedItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private string _id;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public SharedItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public SharedItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _id   = key;
            _value = value;
        }

        public SharedItem(SharedItem source)
            : base(source)
        {
            _id   = source._id;
            _value = source._value;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                {
                    return true;
                }

                return false;
            }
        }

        public string Id
        {
            get
            {
                return _id;
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
        /// For the <see cref="SharedItem"/> class instance, this property is 
        /// <see cref="SharedItem.TagName"/>.
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

        public override bool Equals(SharedItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._id, other._id))
            {
                return false;
            }
            if (!String.Equals(this._value, other._value))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            SharedItem other = obj as SharedItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 7;
            if (_id != null)
            {
                hashCode ^= _id.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override SharedItem Clone()
        {
            SharedItem item = new SharedItem(this);
            if (_id != null)
            {
                item._id = String.Copy(_id);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }

            return item;
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
                _id    = reader.GetAttribute("id");
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
            writer.WriteAttributeString("id", _id);
            writer.WriteString(_value);
            writer.WriteEndElement();           // end - attribute
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _id;
            }
        }

        #endregion
    }
}
