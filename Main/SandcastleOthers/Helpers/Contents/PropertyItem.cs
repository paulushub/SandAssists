using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class PropertyItem : BuildItem<PropertyItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _tag;
        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public PropertyItem()
        {
        }

        public PropertyItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name  = name;
            _value = value;
            _tag   = String.Empty;
        }

        public PropertyItem(PropertyItem source)
            : base(source)
        {
            _name  = source._name;
            _value = source._value;
            _tag   = source._tag;
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
            set
            {
                _name = value;
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

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(PropertyItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._value, other._value))
            {
                return (this._tag == other._tag);
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            PropertyItem other = obj as PropertyItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 29;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override PropertyItem Clone()
        {
            PropertyItem item = new PropertyItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }
            if (_tag != null)
            {
                item._tag = String.Copy(_tag);
            }

            return item;
        }

        #endregion
    }
}
