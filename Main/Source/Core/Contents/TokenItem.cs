using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TokenItem : BuildItem<TokenItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _key;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public TokenItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public TokenItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _key   = key;
            _value = value;
        }

        public TokenItem(TokenItem source)
            : base(source)
        {
            _key   = source._key;
            _value = source._value;
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

        public override bool Equals(TokenItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._key, other._key))
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
            TokenItem other = obj as TokenItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 11;
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

        public override TokenItem Clone()
        {
            TokenItem item = new TokenItem(this);
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
