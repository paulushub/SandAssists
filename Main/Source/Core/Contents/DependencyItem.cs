using System;
using System.IO;
using System.Text;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class DependencyItem : BuildItem<DependencyItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _path;

        #endregion

        #region Constructors and Destructor

        public DependencyItem()
            : this(Guid.NewGuid().ToString())
        {
            _path = String.Empty;
        }

        public DependencyItem(string name, string path)
            : this(name)
        {
            BuildExceptions.PathMustExist(path, "path");

            _name = name;
            _path = path;
        }

        public DependencyItem(DependencyItem source)
            : base(source)
        {
            _name = source._name;
            _path = source._path;
        }

        private DependencyItem(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
            _path = String.Empty;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_path))
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

        public string Location
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(DependencyItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._path, other._path))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            DependencyItem other = obj as DependencyItem;
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
            if (_path != null)
            {
                hashCode ^= _path.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override DependencyItem Clone()
        {
            DependencyItem item = new DependencyItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_path != null)
            {
                item._path = String.Copy(_path);
            }

            return item;
        }

        #endregion
    }
}
