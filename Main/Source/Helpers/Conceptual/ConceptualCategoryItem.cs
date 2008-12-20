using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualCategoryItem : BuildItem<ConceptualCategoryItem>,
        IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _description;

        #endregion

        #region Constructors and Destructor

        public ConceptualCategoryItem()
        {
        }

        public ConceptualCategoryItem(string name, string description)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name        = name;
            _description = description;
        }

        public ConceptualCategoryItem(ConceptualCategoryItem source)
            : base(source)
        {
            _name        = source._name;
            _description = source._description;
        }

        #endregion

        #region Publi Properties

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
                return (String.IsNullOrEmpty(_name) == false);
            }
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConceptualCategoryItem other)
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
            ConceptualCategoryItem other = obj as ConceptualCategoryItem;
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

        public override ConceptualCategoryItem Clone()
        {
            ConceptualCategoryItem category = new ConceptualCategoryItem(this);

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
