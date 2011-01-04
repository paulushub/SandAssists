using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ScriptItem : BuildItem<ScriptItem>, IBuildNamedItem
    {
        #region Private Fields

        private bool   _overrides;
        private string _tag;
        private string _name;
        private string _condition;
        private string _scriptFile;
        private string _description;

        #endregion

        #region Constructors and Destructor

        public ScriptItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public ScriptItem(string name, string scriptFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name       = name;
            _condition  = String.Empty;
            _scriptFile = scriptFile;
        }

        public ScriptItem(ScriptItem source)
            : base(source)
        {
            _tag         = source._tag;
            _name        = source._name;
            _condition   = source._condition;
            _scriptFile  = source._scriptFile;
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
                    String.IsNullOrEmpty(_scriptFile))
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

        public string ScriptFile
        {
            get
            {
                return _scriptFile;
            }
            set
            {
                _scriptFile = value;
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

        public override bool Equals(ScriptItem other)
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
            if (this._scriptFile != other._scriptFile)
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
            ScriptItem other = obj as ScriptItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 67;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            hashCode ^= _scriptFile.GetHashCode();
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

        public override ScriptItem Clone()
        {
            ScriptItem item = new ScriptItem(this);
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
            if (_scriptFile != null)
            {
                item._scriptFile = String.Copy(_scriptFile);
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
