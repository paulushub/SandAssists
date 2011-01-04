using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class StyleSheetItem : BuildItem<StyleSheetItem>, IBuildNamedItem
    {
        #region Private Fields

        private bool   _overrides;
        private string _tag;
        private string _name;
        private string _condition;
        private string _styleFile;
        private string _description;

        #endregion

        #region Constructors and Destructor

        public StyleSheetItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public StyleSheetItem(string name, string styleFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name        = name;
            _condition   = String.Empty;
            _styleFile   = styleFile;
        }

        public StyleSheetItem(StyleSheetItem source)
            : base(source)
        {
            _tag         = source._tag;
            _name        = source._name;
            _condition   = source._condition;
            _styleFile   = source._styleFile;
            _description = source._description;
            _overrides   = source._overrides;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) || 
                    String.IsNullOrEmpty(_styleFile))
                {
                    return true;
                }

                return false;
            }
        }

        public bool Overrides
        {
            get
            {
                return _overrides;
            }
            set
            {
                _overrides = value;
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

        public string StyleFile
        {
            get
            {
                return _styleFile;
            }
            set
            {
                _styleFile = value;
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

        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                if (value == null)
                {
                    _condition = String.Empty;
                }
                else
                {
                    _condition = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(StyleSheetItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._tag, other._tag,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._name, other._name,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (this._styleFile != other._styleFile)
            {
                return false;
            }
            if (!String.Equals(this._description, other._description,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._condition, other._condition,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            StyleSheetItem other = obj as StyleSheetItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 7;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            hashCode ^= _styleFile.GetHashCode();
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }
            if (_condition != null)
            {
                hashCode ^= _condition.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override StyleSheetItem Clone()
        {
            StyleSheetItem item = new StyleSheetItem(this);
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
            if (_styleFile != null)
            {
                item._styleFile = String.Copy(_styleFile);
            }
            if (_condition != null)
            {
                item._condition = String.Copy(_condition);
            }

            return item;
        }

        #endregion
    }
}
