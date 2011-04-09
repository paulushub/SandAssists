using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathCommandItem : BuildItem<MathCommandItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "mathCommandItem";

        #endregion

        #region Private Fields

        private int    _arguments;
        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public MathCommandItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public MathCommandItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name      = name;
            _value     = value;
        }

        public MathCommandItem(string name, string value, int arguments)
            : this(name, value)
        {
            _arguments = arguments;
        }

        public MathCommandItem(MathCommandItem source)
            : base(source)
        {
            _name      = source._name;
            _value     = source._value;
            _arguments = source._arguments;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) ||
                    String.IsNullOrEmpty(_value))
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
                    _value = String.Empty;
                }
                else
                {
                    _value = value.Trim();
                }
            }
        }

        public int Arguments
        {
            get 
            { 
                return _arguments; 
            }
            set 
            { 
                _arguments = value; 
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MathCommandItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (this._arguments != other._arguments)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
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
            MathCommandItem other = obj as MathCommandItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 43;
            hashCode ^= _arguments.GetHashCode();
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
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
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
                _value = reader.GetAttribute("value");
                string nodeText = reader.GetAttribute("arguments");

                if (!String.IsNullOrEmpty(nodeText))
                {
                    _arguments = Convert.ToInt32(nodeText);
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
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

            writer.WriteStartElement(TagName);  // start - item
            writer.WriteAttributeString("name",      _name);
            writer.WriteAttributeString("value",     _value);
            writer.WriteAttributeString("arguments", _arguments.ToString());
            writer.WriteEndElement();           // end - item
        }

        #endregion

        #region ICloneable Members

        public override MathCommandItem Clone()
        {
            MathCommandItem metadata = new MathCommandItem(this);
            if (_name != null)
            {
                metadata._name = String.Copy(_name);
            }
            if (_value != null)
            {
                metadata._value = String.Copy(_value);
            }

            return metadata;
        }

        #endregion
    }
}
