using System;
using System.Text;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class RuleItem : BuildItem<RuleItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public RuleItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public RuleItem(string name)
            : this(name, String.Empty)
        {
        }

        public RuleItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name  = name;
            _value = value;
        }

        public RuleItem(RuleItem source)
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
                if (String.IsNullOrEmpty(_name))
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

        public override bool Equals(RuleItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            RuleItem other = obj as RuleItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
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

        public override RuleItem Clone()
        {
            RuleItem item = new RuleItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }

            return item;
        }

        #endregion
    }
}
