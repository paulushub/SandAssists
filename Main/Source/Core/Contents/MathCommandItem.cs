using System;
using System.Text;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathCommandItem : BuildItem<MathCommandItem>, IBuildNamedItem
    {
        #region Private Fields

        private int    _arguments;
        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        public MathCommandItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public MathCommandItem(string name, string value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name      = name;
            _value     = value;
        }

        public MathCommandItem(string name, string value, int arguments)
            : this(name, value)
        {
            _arguments = arguments;
        }

        public MathCommandItem(MathCommandItem source)
            : base(source)
        {
            _name      = source._name;
            _value     = source._value;
            _arguments = source._arguments;
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

        public int Arguments
        {
            get 
            { 
                return _arguments; 
            }
            set 
            { 
                _arguments = value; 
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MathCommandItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (this._arguments != other._arguments)
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
            MathCommandItem other = obj as MathCommandItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 43;
            hashCode ^= _arguments.GetHashCode();
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

        public override MathCommandItem Clone()
        {
            MathCommandItem metadata = new MathCommandItem(this);
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
