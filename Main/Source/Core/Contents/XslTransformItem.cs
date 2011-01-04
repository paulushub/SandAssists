using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class XslTransformItem : BuildItem<XslTransformItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _tag;
        private string _name;
        private string _description;
        private string _transformFile;

        #endregion

        #region Constructors and Destructor

        public XslTransformItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public XslTransformItem(string name, string transformFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _tag           = String.Empty;
            _name          = name;
            _transformFile = transformFile;
        }

        public XslTransformItem(XslTransformItem source)
            : base(source)
        {
            _tag           = source._tag;
            _name          = source._name;
            _description   = source._description;
            _transformFile = source._transformFile;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) ||
                    String.IsNullOrEmpty(_transformFile))
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

        public string TransformFile
        {
            get
            {
                return _transformFile;
            }
            set
            {
                _transformFile = value;
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

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(XslTransformItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._transformFile, other._transformFile))
            {
                return (this._tag == other._tag);
            }
            if (!String.Equals(this._description, other._description))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            XslTransformItem other = obj as XslTransformItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 29;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_transformFile != null)
            {
                hashCode ^= _transformFile.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override XslTransformItem Clone()
        {
            XslTransformItem item = new XslTransformItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_transformFile != null)
            {
                item._transformFile = String.Copy(_transformFile);
            }
            if (_tag != null)
            {
                item._tag = String.Copy(_tag);
            }
            if (_description != null)
            {
                item._description = String.Copy(_description);
            }

            return item;
        }

        #endregion
    }
}
