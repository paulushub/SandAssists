using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OptionItem : BuildItem<OptionItem>, IBuildNamedItem
    {
        #region Private Fields

        private sbyte  _default;
        private bool   _value;
        private string _name;
        private string _description;
        private string _tag;
        private string _helpId;

        #endregion

        #region Constructors and Destructor

        public OptionItem()
        {
            _default = -1;
        }

        public OptionItem(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _default  = -1;
            _name  = name;
        }

        public OptionItem(string name, bool value)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name  = name;
            _value = value;
            _default  = value ? (sbyte)1 : (sbyte)0;
        }

        public OptionItem(OptionItem source)
            : base(source)
        {
            _tag         = source._tag;
            _name        = source._name;
            _value       = source._value;
            _helpId      = source._helpId;
            _description = source._description;
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

        public bool? Default
        {
            get
            {
                if (_default < 0)
                {
                    return null;
                }

                return (_default == 1);
            }
            set
            {
                if (value == null)
                {
                    _default = -1;
                }
                else
                {
                    _default = value.Value ? (sbyte)1 : (sbyte)0;
                }
            }
        }

        public bool Value
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

        public string HelpId
        {
            get
            {
                return _helpId;
            }
            set
            {
                _helpId = value;
            }
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            if (_default < 0)
            {
                return;
            }

            _value = (_default == 1);
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(OptionItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (this._value != other._value)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name, 
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._description, other._description,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._tag, other._tag,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._helpId, other._helpId,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            OptionItem other = obj as OptionItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 7;
            hashCode ^= _value.GetHashCode();
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }
            if (_helpId != null)
            {
                hashCode ^= _helpId.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override OptionItem Clone()
        {
            OptionItem item = new OptionItem(this);
            item._value = _value;
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_description != null)
            {
                item._description = String.Copy(_description);
            }
            if (_tag != null)
            {
                item._tag = String.Copy(_tag);
            }
            if (_helpId != null)
            {
                item._helpId = String.Copy(_helpId);
            }

            return item;
        }

        #endregion
    }
}
