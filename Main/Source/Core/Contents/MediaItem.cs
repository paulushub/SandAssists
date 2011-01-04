using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaItem : BuildItem<MediaItem>, IBuildNamedItem
    {
        #region Private Fields

        private string        _mediaId;
        private string        _mediaPath;
        private string        _altText;
        private MediaItemType _itemType;

        #endregion

        #region Constructors and Destructor

        public MediaItem()
            : this(Guid.NewGuid().ToString(), String.Empty, String.Empty)
        {
            _itemType = MediaItemType.Image;
        }

        public MediaItem(string id, string path, string alternateText)
        {
            BuildExceptions.NotNullNotEmpty(id, "id");

            _mediaId   = id;
            _mediaPath = path;
            _altText   = alternateText;
            _itemType  = MediaItemType.Image;
        }

        public MediaItem(MediaItem source)
            : base(source)
        {
            _mediaId   = source._mediaId;
            _mediaPath = source._mediaPath;
            _altText   = source._altText;
            _itemType  = source._itemType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_mediaPath) ||
                    String.IsNullOrEmpty(_mediaId))
                {
                    return true;
                }

                return false;
            }
        }

        public MediaItemType MediaType
        {
            get
            {
                return _itemType;
            }
            set
            {
                _itemType = value;
            }
        }
        
        public string MediaId
        {
            get 
            { 
                return _mediaId; 
            }
        }

        public string MediaPath
        {
            get
            {
                return _mediaPath;
            }
            set
            {
                _mediaPath = value;
            }
        }

        public string AlternateText
        {
            get
            {
                return _altText;
            }
            set
            {
                _altText = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(MediaItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._mediaId, other._mediaId))
            {
                return false;
            }
            if (!String.Equals(this._mediaPath, other._mediaPath))
            {
                return false;
            }
            if (!String.Equals(this._altText, other._altText))
            {
                return false;
            }

            return (this._itemType == other._itemType);
        }

        public override bool Equals(object obj)
        {
            MediaItem other = obj as MediaItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 17;
            if (_mediaId != null)
            {
                hashCode ^= _mediaId.GetHashCode();
            }
            if (_mediaPath != null)
            {
                hashCode ^= _mediaPath.GetHashCode();
            }
            if (_altText != null)
            {
                hashCode ^= _altText.GetHashCode();
            }
            hashCode ^= (int)_itemType;

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override MediaItem Clone()
        {
            MediaItem resource = new MediaItem(this);
            if (_mediaPath != null)
            {
                resource._mediaPath = String.Copy(_mediaPath);
            }
            if (_mediaId != null)
            {
                resource._mediaId = String.Copy(_mediaId);
            }
            if (_altText != null)
            {
                resource._altText = String.Copy(_altText);
            }

            return resource;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _mediaId;
            }
        }

        #endregion
    }
}
