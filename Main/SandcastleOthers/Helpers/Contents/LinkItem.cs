using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LinkItem : BuildItem<LinkItem>
    {
        #region Private Fields

        private bool   _isRecursive;
        private string _linkDir;
        private string _linkFile;

        #endregion

        #region Constructors and Destructor

        public LinkItem()
        {
        }

        public LinkItem(string linkFile)
        {
            _linkFile = linkFile;
        }

        public LinkItem(string linkDirectory, bool isRecursive)
        {
            _linkDir     = linkDirectory;
            _isRecursive = isRecursive;
        }

        public LinkItem(LinkItem source)
            : base(source)
        {
            _linkDir     = source._linkDir;
            _linkFile    = source._linkFile;
            _isRecursive = source._isRecursive;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (String.IsNullOrEmpty(_linkDir) &&
                    String.IsNullOrEmpty(_linkFile)) ? true : false;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return (String.IsNullOrEmpty(_linkDir) == false);
            }
        }

        public string LinkDirectory
        {
            get
            {
                return _linkDir;
            }
            set
            {
                _linkDir = value;
            }
        }

        public string LinkFile
        {
            get
            {
                return _linkFile;
            }
            set
            {
                _linkFile = value;
            }
        }

        public bool Recursive
        {
            get 
            { 
                return _isRecursive; 
            }
            set 
            { 
                _isRecursive = value; 
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(LinkItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._linkDir, other._linkDir))
            {
                return false;
            }
            if (!String.Equals(this._linkFile, other._linkFile))
            {
                return false;
            }

            return (this._isRecursive == other._isRecursive);
        }

        public override bool Equals(object obj)
        {
            LinkItem other = obj as LinkItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            if (_linkDir != null)
            {
                hashCode ^= _linkDir.GetHashCode();
            }
            if (_linkFile != null)
            {
                hashCode ^= _linkFile.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override LinkItem Clone()
        {
            LinkItem item = new LinkItem(this);
            if (_linkDir != null)
            {
                item._linkDir = String.Copy(_linkDir);
            }
            if (_linkFile != null)
            {
                item._linkFile = String.Copy(_linkFile);
            }

            return item;
        }

        #endregion
    }
}
