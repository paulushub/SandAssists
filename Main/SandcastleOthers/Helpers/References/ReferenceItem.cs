using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceItem : BuildItem<ReferenceItem>
    {
        #region Private Fields

        private string _comments;
        private string _assembly;

        #endregion

        #region Constructors and Destructor

        public ReferenceItem()
        {
        }

        public ReferenceItem(string comments, string assembly)
        {
            _comments = comments;
            _assembly = assembly;
        }

        public ReferenceItem(ReferenceItem source)
            : base(source)
        {
            _comments = source._comments;
            _assembly = source._assembly;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_comments) &&
                    String.IsNullOrEmpty(_assembly))
                {
                    return true;
                }

                return false;
            }
        }

        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        public string Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                _assembly = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ReferenceItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._comments, other._comments))
            {
                return false;
            }
            if (!String.Equals(this._assembly, other._assembly))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ReferenceItem other = obj as ReferenceItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 19;
            if (_comments != null)
            {
                hashCode ^= _comments.GetHashCode();
            }
            if (_assembly != null)
            {
                hashCode ^= _assembly.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override ReferenceItem Clone()
        {
            ReferenceItem resource = new ReferenceItem(this);
            if (_comments != null)
            {
                resource._comments = String.Copy(_comments);
            }
            if (_assembly != null)
            {
                resource._assembly = String.Copy(_assembly);
            }

            return resource;
        }

        #endregion
    }
}
