using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class BibliographyItem : BuildItem<BibliographyItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _name;
        private string _link;
        private string _title;
        private string _author;
        private string _publisher;

        #endregion

        #region Constructors and Destructor

        public BibliographyItem()
        {
            _name      = String.Empty;
            _title     = String.Empty;
            _author    = String.Empty;
            _link      = String.Empty;
            _publisher = String.Empty;
        }

        public BibliographyItem(string name)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
        }

        public BibliographyItem(string name, string title, string author,
            string publisher, string link) : this()
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
            if (title != null)
            {
                _title = title;
            }
            if (author != null)
            {
                _author = author;
            }
            if (publisher != null)
            {
                _publisher = publisher;
            }
            if (link != null)
            {
                _link = link;
            }
        }

        public BibliographyItem(BibliographyItem source)
            : base(source)
        {
            _name      = source._name;
            _title     = source._title;
            _author    = source._author;
            _link      = source._link;
            _publisher = source._publisher;
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
                if (!String.IsNullOrEmpty(value))
                {
                    _name = value;
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _title = value;
            }
        }

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _author = value;
            }
        }

        public string Publisher
        {
            get
            {
                return _publisher;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _publisher = value;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _link = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(BibliographyItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._title, other._title))
            {
                return false;
            }
            if (!String.Equals(this._author, other._author))
            {
                return false;
            }
            if (!String.Equals(this._link, other._link))
            {
                return false;
            }
            if (!String.Equals(this._publisher, other._publisher))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            BibliographyItem other = obj as BibliographyItem;
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
            if (_title != null)
            {
                hashCode ^= _title.GetHashCode();
            }
            if (_author != null)
            {
                hashCode ^= _author.GetHashCode();
            }
            if (_link != null)
            {
                hashCode ^= _link.GetHashCode();
            }
            if (_publisher != null)
            {
                hashCode ^= _publisher.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override BibliographyItem Clone()
        {
            BibliographyItem item = new BibliographyItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_title != null)
            {
                item._title = String.Copy(_title);
            }
            if (_author != null)
            {
                item._author = String.Copy(_author);
            }
            if (_link != null)
            {
                item._link = String.Copy(_link);
            }
            if (_publisher != null)
            {
                item._publisher = String.Copy(_publisher);
            }

            return item;
        }

        #endregion
    }
}
