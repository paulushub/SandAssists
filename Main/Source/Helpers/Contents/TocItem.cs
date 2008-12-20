using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Contents
{
    [Serializable]
    public class TocItem : BuildItem<TocItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _path;
        private string _project;
        private BuildList<TocItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public TocItem()
        {
            _name    = String.Empty;
            _path    = String.Empty;
            _project = String.Empty;
        }

        public TocItem(string path)
        {
            BuildExceptions.PathMustExist(path, "path");

            _name = Path.GetFileName(path);
            _path = path;
        }

        public TocItem(string name, string path)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");
            BuildExceptions.PathMustExist(path, "path");

            _name = name;
            _path = path;
        }

        public TocItem(string name, string path, string project)
            : this(name, path)
        {
            _project = project;
        }

        public TocItem(TocItem source)
            : base(source)
        {
            _name = source._name;
            _path = source._path;
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
            set
            {
                _name = value;
            }
        }

        public string File
        {
            get
            {
                return _path;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _path = value;
            }
        }

        public string Project
        {
            get
            {
                return _project;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _project = value;
            }
        }

        public TocItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public int ItemCount
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public IList<TocItem> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<TocItem>(_listItems);
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Add(TocItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems == null)
            {
                _listItems = new BuildList<TocItem>();
            }

            _listItems.Add(item);
        }

        public void Remove(int itemIndex)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(itemIndex);
        }

        public void Remove(TocItem item)
        {
            if (item == null)
            {
                return;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public void Clear()
        {
            if (_listItems != null)
            {
                _listItems = new BuildList<TocItem>();
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(TocItem other)
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
            TocItem other = obj as TocItem;
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

        public override TocItem Clone()
        {
            TocItem item = new TocItem(this);
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
