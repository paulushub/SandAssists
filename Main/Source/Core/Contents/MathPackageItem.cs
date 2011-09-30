using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathPackageItem : BuildItem<MathPackageItem>
    {
        #region Public Fields

        public const string TagName = "mathPackageItem";

        #endregion

        #region Private Fields

        private string            _use;
        private BuildList<string> _listOptions;

        #endregion

        #region Constructors and Destructor

        public MathPackageItem()
            : this(String.Empty)
        {
        }

        public MathPackageItem(string use)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use         = use;
            _listOptions = new BuildList<string>();
        }

        public MathPackageItem(string use, string option)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use = use;
            _listOptions = new BuildList<string>();
            if (!String.IsNullOrEmpty(option))
            {
                _listOptions.Add(option);
            }
        }

        public MathPackageItem(string use, params string[] options)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use         = use;
            _listOptions = new BuildList<string>();

            if (options != null && options.Length != 0)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    string option = options[i];
                    if (!String.IsNullOrEmpty(option))
                    {
                        _listOptions.Add(option);
                    }
                }
            }
        }

        public MathPackageItem(MathPackageItem source)
            : base(source)
        {
            _use         = source._use;
            _listOptions = source._listOptions;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_use);
            }
        }

        public string Use
        {
            get
            {
                return _use;
            }
            set
            {
                _use = value;
            }
        }

        public IList<string> Options
        {
            get
            {
                return _listOptions;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="MathPackageItem"/> class instance, this property is 
        /// <see cref="MathPackageItem.TagName"/>.
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

        public bool FormatOptions(StringBuilder builder)
        {
            BuildExceptions.NotNull(builder, "builder");

            builder.Length = 0;
            if (_listOptions != null && _listOptions.Count != 0)
            {
                int itemCount = _listOptions.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    builder.Append(_listOptions[i]);
                    if (i != (itemCount - 1))
                    {
                        builder.Append(",");
                    }
                }

                return true;
            }

            return false;
        }   

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MathPackageItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._use, other._use))
            {
                return false;
            }
            if (this._listOptions != null && other._listOptions != null)
            {
                if (this._listOptions.Count != other._listOptions.Count)
                {
                    return false;
                }
                for (int i = 0; i < this._listOptions.Count; i++)
                {
                    if (!String.Equals(this._listOptions[i], other._listOptions[i]))
                    {
                        return false;
                    }
                }
            }
            else if (this._listOptions != null)
            {
                return false;
            }
            else if (other._listOptions != null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            MathPackageItem other = obj as MathPackageItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            if (_use != null)
            {
                hashCode ^= _use.GetHashCode();
            }
            if (_listOptions != null)
            {
                int itemCount = _listOptions.Count;
                hashCode ^= itemCount.GetHashCode();

                for (int i = 0; i < itemCount; i++)
                {
                    hashCode ^= _listOptions[i].GetHashCode();
                }
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
            _use = reader.GetAttribute("use");

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listOptions == null)
            {
                _listOptions = new BuildList<string>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {   
                    if (String.Equals(reader.Name, "option", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _listOptions.Add(reader.GetAttribute("value"));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "options",
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

            writer.WriteStartElement(TagName);  // start - item
            writer.WriteAttributeString("use", _use);
            if (_listOptions != null && _listOptions.Count != 0)
            {
                writer.WriteStartElement("options");  // start - options
                for (int i = 0; i < _listOptions.Count; i++)
                {   
                    writer.WriteStartElement("option"); // start - option
                    writer.WriteAttributeString("value", _listOptions[i]);
                    writer.WriteEndElement();           // end - option
                }
                writer.WriteEndElement();           // end - options
            }
            writer.WriteEndElement();           // end - item
        }

        #endregion

        #region ICloneable Members

        public override MathPackageItem Clone()
        {
            MathPackageItem item = new MathPackageItem(this);
            if (_use != null)
            {
                item._use = String.Copy(_use);
            }
            if (_listOptions != null)
            {
                item._listOptions = _listOptions.Clone();
            }

            return item;
        }

        #endregion
    }
}
