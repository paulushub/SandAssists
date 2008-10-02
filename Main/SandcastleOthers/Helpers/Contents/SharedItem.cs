using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SharedItem : BuildItem<SharedItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _key;
        private string _value;
        private SharedItemFormat _format;

        #endregion

        #region Constructors and Destructor

        public SharedItem()
        {
        }

        public SharedItem(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _key    = key;
            _value  = value;
            _format = SharedItemFormat.PlainText;
        }

        public SharedItem(SharedItem source)
            : base(source)
        {
            _key    = source._key;
            _value  = source._value;
            _format = source._format;
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

        public SharedItemFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(SharedItem other)
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
                return (this._format == other._format);
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            SharedItem other = obj as SharedItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 7;
            if (_key != null)
            {
                hashCode ^= _key.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
            }
            hashCode ^= (int)_format;

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override SharedItem Clone()
        {
            SharedItem item = new SharedItem(this);
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
