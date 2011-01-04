using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class AttributeItem : BuildItem<AttributeItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public AttributeItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public AttributeItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name  = name;
            _value = value;
        }

        public AttributeItem(AttributeItem source)
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
                _value = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(AttributeItem other)
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
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            AttributeItem other = obj as AttributeItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 43;
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

        #region ICloneable Members

        public override AttributeItem Clone()
        {
            AttributeItem attrItem = new AttributeItem(this);
            if (_name != null)
            {
                attrItem._name = String.Copy(_name);
            }
            if (_value != null)
            {
                attrItem._value = String.Copy(_value);
            }

            return attrItem;
        }

        #endregion
    }
}
