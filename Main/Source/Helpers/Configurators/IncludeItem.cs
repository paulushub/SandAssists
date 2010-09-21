using System;
using System.Text;

namespace Sandcastle.Configurators
{
    [Serializable]
    public class IncludeItem : BuildItem<IncludeItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _key;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public IncludeItem()
        {
        }

        public IncludeItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _key    = key;
            _value  = value;
        }

        public IncludeItem(IncludeItem source)
            : base(source)
        {
            _key    = source._key;
            _value  = source._value;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_key))
                {
                    return true;
                }

                return false;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
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

        public override bool Equals(IncludeItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._key, other._key))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            IncludeItem other = obj as IncludeItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_key != null)
            {
                hashCode ^= _key.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override IncludeItem Clone()
        {
            IncludeItem item = new IncludeItem(this);
            if (_key != null)
            {
                item._key = String.Copy(_key);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }

            return item;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _key; 
            }
        }

        #endregion
    }
}
