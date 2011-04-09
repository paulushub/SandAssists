using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class CategoryItem : BuildItem<CategoryItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "category";

        #endregion

        #region Private Fields

        private bool   _isEnabled;
        private string _name;
        private string _description;

        #endregion

        #region Constructors and Destructor

        public CategoryItem()
        {
        }

        public CategoryItem(string name, string description)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name        = name;
            _description = description;
        }

        public CategoryItem(CategoryItem source)
            : base(source)
        {
            _name        = source._name;
            _description = source._description;
        }

        #endregion

        #region Public Properties

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public string Name
        {
            get 
            { 
                return _name; 
            } 
            set 
            {
                if (!String.IsNullOrEmpty(_name))
                {
                    _name = value; 
                }
            }
        }

        public string Description
        {
            get 
            { 
                return _description; 
            } 
            set 
            { 
                _description = value; 
            }
        }

        public bool IsValid
        {
            get
            {
                return (!String.IsNullOrEmpty(_name));
            }
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
                _name        = reader.GetAttribute("name");
                _isEnabled   = Convert.ToBoolean(reader.GetAttribute("enabled"));
                _description = reader.GetAttribute("description");
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

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - category
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("enabled", _isEnabled.ToString());
            writer.WriteAttributeString("description", _description);
            writer.WriteEndElement();           // end - category
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(CategoryItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._description, other._description))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            CategoryItem other = obj as CategoryItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 31;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override CategoryItem Clone()
        {
            CategoryItem category = new CategoryItem(this);

            if (_name != null)
            {
                category._name = String.Copy(_name);
            }
            if (_description != null)
            {
                category._description = String.Copy(_description);
            }

            return category;
        }

        #endregion
    }
}
