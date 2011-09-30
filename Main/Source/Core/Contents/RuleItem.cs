using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class RuleItem : BuildItem<RuleItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "ruleItem";

        #endregion

        #region Private Fields

        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public RuleItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public RuleItem(string name)
            : this(name, String.Empty)
        {
        }

        public RuleItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name  = name;
            _value = value;
        }

        public RuleItem(RuleItem source)
            : base(source)
        {
            _name  = source._name;
            _value = source._value;
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

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }

                _value = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="RuleItem"/> class instance, this property is 
        /// <see cref="RuleItem.TagName"/>.
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

        public override bool Equals(RuleItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            RuleItem other = obj as RuleItem;
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
                _name  = reader.GetAttribute("name");
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

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", _name);
            writer.WriteString(_value);
            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override RuleItem Clone()
        {
            RuleItem item = new RuleItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }

            return item;
        }

        #endregion
    }
}
