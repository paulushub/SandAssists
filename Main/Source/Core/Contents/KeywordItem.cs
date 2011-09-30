using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class KeywordItem : BuildItem<KeywordItem>
    {
        #region Public Fields

        public const string TagName = "keyword";

        #endregion

        #region Private Fields

        private KeywordType       _type;
        private BuildList<string> _listTerms;

        #endregion

        #region Constructors and Destructor

        public KeywordItem()
        {
            _listTerms = new BuildList<string>();
        }

        public KeywordItem(KeywordType type, string term)
            : this()
        {
            _type = type;
            if (!String.IsNullOrEmpty(term))
            {
                _listTerms.Add(term);
            }
        }

        public KeywordItem(KeywordItem source)
            : base(source)
        {
            _type      = source._type;
            _listTerms = source._listTerms;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_type == KeywordType.None ||
                    (_listTerms == null || _listTerms.Count == 0))
                {
                    return true;
                }

                return false;
            }
        }

        public KeywordType Index
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public IList<string> Terms
        {
            get
            {
                return _listTerms;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="KeywordItem"/> class instance, this property is 
        /// <see cref="KeywordItem.TagName"/>.
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

        public static KeywordType ParseIndex(string indexText)
        {
            if (String.IsNullOrEmpty(indexText))
            {
                return KeywordType.None;
            }
            if (String.Equals(indexText, "K", StringComparison.OrdinalIgnoreCase))
            {
                return KeywordType.K;
            }
            if (String.Equals(indexText, "F", StringComparison.OrdinalIgnoreCase))
            {
                return KeywordType.F;
            }
            if (String.Equals(indexText, "S", StringComparison.OrdinalIgnoreCase))
            {
                return KeywordType.S;
            }
            if (String.Equals(indexText, "B", StringComparison.OrdinalIgnoreCase))
            {
                return KeywordType.B;
            }
            if (String.Equals(indexText, "A", StringComparison.OrdinalIgnoreCase))
            {
                return KeywordType.A;
            }

            return KeywordType.None;
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(KeywordItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._type, other._type))
            {
                return false;
            }
            int itemCount = _listTerms.Count;
            if (itemCount != other._listTerms.Count)
            {
                return false;
            }
            for (int i = 0; i < itemCount; i++)
            {
                if (!String.Equals(this._listTerms[i], other._listTerms[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            KeywordItem other = obj as KeywordItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 23;
            hashCode ^= _type.GetHashCode();
            if (_listTerms != null)
            {
                hashCode ^= _listTerms.GetHashCode();
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

            string keyIndex = reader.GetAttribute("index");
            string keyValue = reader.ReadString();
            if (keyValue != null)
            {
                keyValue = keyValue.Trim();
            }

            if (String.IsNullOrEmpty(keyIndex) ||
                String.IsNullOrEmpty(keyValue))
            {
                return;
            }

            _type = KeywordType.None;
            switch (keyIndex.ToUpper())
            {
                case "K":
                    _type = KeywordType.K;
                    break;
                case "F":
                    _type = KeywordType.F;
                    break;
                case "S":
                    _type = KeywordType.S;
                    break;
                case "B":
                    _type = KeywordType.B;
                    break;
                case "A":
                    _type = KeywordType.A;
                    break;
            }
            if (_type == KeywordType.None)
            {
                return;
            }
            _listTerms.Add(keyValue);

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    keyValue = reader.ReadString();
                    if (keyValue != null)
                    {
                        keyValue = keyValue.Trim();
                    }
                    if (!String.IsNullOrEmpty(keyValue))
                    {
                        _listTerms.Add(keyValue);
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

            string keyIndex = _type.ToString();

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("index", keyIndex);
            writer.WriteString(_listTerms[0]);

            if (_listTerms.Count > 1)
            {
                for (int j = 1; j < _listTerms.Count; j++)
                {
                    writer.WriteStartElement(TagName);
                    writer.WriteAttributeString("index", keyIndex);
                    writer.WriteString(_listTerms[j]);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override KeywordItem Clone()
        {
            KeywordItem resource = new KeywordItem(this);
            if (_listTerms != null)
            {
                resource._listTerms = _listTerms.Clone();
            }

            return resource;
        }

        #endregion
    }
}
