using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CommentPart : BuildObject<CommentPart>, IEquatable<CommentPart>
    {
        #region Private Fields

        private string          _partText;
        private CommentPartType _partType;

        #endregion

        #region Constructors and Destructor

        public CommentPart()
        {
        }

        public CommentPart(string partText)
        {
            _partText = partText;
        }

        public CommentPart(string partText, CommentPartType partType)
        {
            _partText = partText;
            _partType = partType;
        }

        public CommentPart(CommentPart source)
            : base(source)
        {
            _partText = source._partText;
            _partType = source._partType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (String.IsNullOrEmpty(_partText));
            }
        }

        public CommentPartType PartType
        {
            get
            {
                return _partType;
            }
            set
            {
                _partType = value;
            }
        }

        public string Text
        {
            get
            {
                return _partText;
            }
            set
            {
                _partText = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public bool Equals(CommentPart other)
        {
            if (other == null)
            {
                return false;
            }
            if (this._partType != other._partType)
            {
                return false;
            }
            if (!String.Equals(this._partText, other._partText))
            {
                return false;
            }

            return (this._partType == other._partType);
        }

        public override bool Equals(object obj)
        {
            CommentPart other = obj as CommentPart;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            hashCode    ^= _partType.GetHashCode();
            if (_partText != null)
            {
                hashCode ^= _partText.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override CommentPart Clone()
        {
            CommentPart item = new CommentPart(this);
            if (_partText != null)
            {
                item._partText = String.Copy(_partText);
            }

            return item;
        }

        #endregion
    }
}
