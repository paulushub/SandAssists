using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class MetadataItem : BuildItem<MetadataItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public MetadataItem()
        {
        }

        public MetadataItem(string name, string value)
        {
            _name  = name;
            _value = value;
        }

        public MetadataItem(MetadataItem source)
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

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MetadataItem other)
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
            MetadataItem other = obj as MetadataItem;
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

        public override MetadataItem Clone()
        {
            MetadataItem metadata = new MetadataItem(this);
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
