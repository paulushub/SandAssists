using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ResourceItem : BuildItem<ResourceItem>
    {
        #region Private Fields

        private string _source;
        private string _destination;

        #endregion

        #region Constructors and Destructor

        public ResourceItem()
        {   
        }

        public ResourceItem(string source, string destination)
        {
            _source      = source;
            _destination = destination;
        }

        public ResourceItem(ResourceItem source)
            : base(source)
        {
            _source      = source._source;
            _destination = source._destination;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_source) ||
                    String.IsNullOrEmpty(_destination))
                {
                    return true;
                }

                return false;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public string Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ResourceItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._source, other._source))
            {
                return false;
            }
            if (!String.Equals(this._destination, other._destination))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ResourceItem other = obj as ResourceItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 23;
            if (_source != null)
            {
                hashCode ^= _source.GetHashCode();
            }
            if (_destination != null)
            {
                hashCode ^= _destination.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override ResourceItem Clone()
        {
            ResourceItem resource = new ResourceItem(this);
            if (_source != null)
            {
                resource._source = String.Copy(_source);
            }
            if (_destination != null)
            {
                resource._destination = String.Copy(_destination);
            }

            return resource;
        }

        #endregion
    }
}
