using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathPackageItem : BuildItem<MathPackageItem>
    {
        #region Private Fields

        private string            _use;
        private BuildList<string> _listOptions;

        #endregion

        #region Constructors and Destructor

        public MathPackageItem()
            : this(String.Empty)
        {
        }

        public MathPackageItem(string use)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use         = use;
            _listOptions = new BuildList<string>();
        }

        public MathPackageItem(string use, string option)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use = use;
            _listOptions = new BuildList<string>();
            if (!String.IsNullOrEmpty(option))
            {
                _listOptions.Add(option);
            }
        }

        public MathPackageItem(string use, params string[] options)
        {
            if (!String.IsNullOrEmpty(use))
            {
                use = use.Trim();
            }

            _use         = use;
            _listOptions = new BuildList<string>();

            if (options != null && options.Length != 0)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    string option = options[i];
                    if (!String.IsNullOrEmpty(option))
                    {
                        _listOptions.Add(option);
                    }
                }
            }
        }

        public MathPackageItem(MathPackageItem source)
            : base(source)
        {
            _use         = source._use;
            _listOptions = source._listOptions;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_use);
            }
        }

        public string Use
        {
            get
            {
                return _use;
            }
            set
            {
                _use = value;
            }
        }

        public IList<string> Options
        {
            get
            {
                return _listOptions;
            }
        }

        #endregion

        #region Public Methods

        public bool FormatOptions(StringBuilder builder)
        {
            BuildExceptions.NotNull(builder, "builder");

            builder.Length = 0;
            if (_listOptions != null && _listOptions.Count != 0)
            {
                int itemCount = _listOptions.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    builder.Append(_listOptions[i]);
                    if (i != (itemCount - 1))
                    {
                        builder.Append(",");
                    }
                }

                return true;
            }

            return false;
        }   

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MathPackageItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._use, other._use))
            {
                return false;
            }
            if (this._listOptions != null && other._listOptions != null)
            {
                if (this._listOptions.Count != other._listOptions.Count)
                {
                    return false;
                }
                for (int i = 0; i < this._listOptions.Count; i++)
                {
                    if (!String.Equals(this._listOptions[i], other._listOptions[i]))
                    {
                        return false;
                    }
                }
            }
            else if (this._listOptions != null)
            {
                return false;
            }
            else if (other._listOptions != null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            MathPackageItem other = obj as MathPackageItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            if (_use != null)
            {
                hashCode ^= _use.GetHashCode();
            }
            if (_listOptions != null)
            {
                int itemCount = _listOptions.Count;
                hashCode ^= itemCount.GetHashCode();

                for (int i = 0; i < itemCount; i++)
                {
                    hashCode ^= _listOptions[i].GetHashCode();
                }
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override MathPackageItem Clone()
        {
            MathPackageItem item = new MathPackageItem(this);
            if (_use != null)
            {
                item._use = String.Copy(_use);
            }
            if (_listOptions != null)
            {
                item._listOptions = _listOptions.Clone();
            }

            return item;
        }

        #endregion
    }
}
