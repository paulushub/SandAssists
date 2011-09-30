using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaItem : BuildItem<MediaItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private string        _altText;
        private string        _itemId;
        private string        _itemName;
        private BuildFilePath _itemPath;
        private MediaItemType _itemType;

        // For the item dimensions...
        private int           _itemWidth;
        private int           _itemHeight;
        private MediaSizeUnit _itemUnit;

        // For the image maps support...
        private bool          _resolveLinks;
        private string        _useMap; 
        private MediaImageMap _imageMap;

        #endregion

        #region Constructors and Destructor

        public MediaItem()
            : this(Guid.NewGuid().ToString())
        {
        }

        public MediaItem(string id)
        {
            BuildExceptions.NotNullNotEmpty(id, "id");

            _itemId       = id;
            _useMap       = String.Empty;
            _itemType     = MediaItemType.Image;
            _itemUnit     = MediaSizeUnit.None;
        }

        public MediaItem(string id, BuildFilePath path, string alternateText)
            : this(id)
        {
            _itemPath     = path;
            _altText      = alternateText == null ? String.Empty : alternateText;
        }

        public MediaItem(MediaItem source)
            : base(source)
        {
            _itemId       = source._itemId;
            _itemPath     = source._itemPath;
            _altText      = source._altText;
            _itemType     = source._itemType;
            _itemUnit     = source._itemUnit;
            _itemWidth    = source._itemWidth;
            _itemHeight   = source._itemHeight;
            _useMap       = source._useMap;
            _imageMap     = source._imageMap;  
            _resolveLinks = source._resolveLinks;
        }

        #endregion

        #region Public Properties

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
                return _itemId;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if ((_itemPath == null && _itemName == null) || 
                    String.IsNullOrEmpty(_itemId))
                {
                    return true;
                }

                return false;
            }
        }

        public string MediaName
        {
            get
            {
                return _itemName;
            }
            set
            {
                _itemName = value;
            }
        }

        public MediaSizeUnit MediaUnits
        {
            get
            {
                return _itemUnit;
            }
            set
            {
                _itemUnit = value;
            }
        }

        public BuildFilePath MediaPath
        {
            get
            {
                return _itemPath;
            }
            set
            {
                _itemPath = value;
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
                if (value != null)
                {
                    _altText = value.Trim();
                }
                else
                {
                    _altText = String.Empty;
                }
            }
        }

        public bool ResolveMapLinks
        {
            get
            {
                return _resolveLinks;
            }
            set
            {
                _resolveLinks = value;
            }
        }

        public string UseMap
        {
            get
            {
                return _useMap;
            }
            set
            {
                _useMap = value;
            }
        }

        public MediaImageMap ImageMap
        {
            get
            {
                return _imageMap;
            }
            set
            {
                _imageMap = value;
            }
        }

        public bool HasImageMap
        {
            get
            {
                // Verify that we have a valid image map...
                // 1. The item type must be image or none (default)...
                bool hasImageMap = (_itemType == MediaItemType.Image ||
                    _itemType == MediaItemType.None);
                if (hasImageMap)
                {
                    // 2. The use map must be valid, starting with #...
                    hasImageMap = !String.IsNullOrEmpty(_useMap) &&
                        _useMap.StartsWith("#", StringComparison.Ordinal);

                    if (hasImageMap)
                    {
                        // 3. The image area must be valid and match the use map...
                        hasImageMap = _imageMap != null && !_imageMap.IsEmpty
                            && String.Equals(_imageMap.Name, _useMap.Substring(1),
                            StringComparison.OrdinalIgnoreCase);
                    }
                }

                return hasImageMap;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="MediaItem"/> class instance, this property is 
        /// <see cref="MediaItem.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
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
            if (!String.Equals(this._itemId, other._itemId))
            {
                return false;
            }
            if (!String.Equals(this._itemPath, other._itemPath))
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
            if (_itemId != null)
            {
                hashCode ^= _itemId.GetHashCode();
            }
            if (_itemPath != null)
            {
                hashCode ^= _itemPath.GetHashCode();
            }
            if (_altText != null)
            {
                hashCode ^= _altText.GetHashCode();
            }
            hashCode ^= (int)_itemType;

            return hashCode;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }          
            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _itemId         = reader.GetAttribute("id");
            string nodeText = reader.GetAttribute("type");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _itemType   = MediaItemType.None;

                switch (nodeText.ToLower())
                {
                    case "image":
                        _itemType = MediaItemType.Image;
                        break;
                    case "video":
                        _itemType = MediaItemType.Video;
                        break;
                    case "flash":
                        _itemType = MediaItemType.Flash;
                        break;
                    case "silverlight":
                        _itemType = MediaItemType.Silverlight;
                        break;
                    case "youtube":
                        _itemType = MediaItemType.YouTube;
                        break;
                    case "pdf":
                        _itemType = MediaItemType.Pdf;
                        break;
                    case "xps":
                        _itemType = MediaItemType.Xps;
                        break;
                }
            }
            nodeText = reader.GetAttribute("width");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _itemWidth  = Convert.ToInt32(nodeText);
            }
            nodeText = reader.GetAttribute("height");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _itemHeight = Convert.ToInt32(nodeText);
            }
            nodeText = reader.GetAttribute("unit");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _itemUnit = MediaSizeUnit.None;

                switch (nodeText.ToLower())
                {
                    case "pixel":
                        _itemUnit = MediaSizeUnit.Pixel;
                        break;
                    case "point":
                        _itemUnit = MediaSizeUnit.Point;
                        break;
                    case "percent":
                        _itemUnit = MediaSizeUnit.Percent;
                        break;
                    case "em":
                        _itemUnit = MediaSizeUnit.Em;
                        break;
                    case "pica":
                        _itemUnit = MediaSizeUnit.Pica;
                        break;
                    case "ex":
                        _itemUnit = MediaSizeUnit.Ex;
                        break;
                }
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeText = reader.Name;
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "image",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (_itemType == MediaItemType.YouTube)
                        {
                            _itemName = reader.GetAttribute("file");
                        }
                        else
                        {
                            _itemPath = new BuildFilePath(reader.GetAttribute("file"));
                        }
                        _useMap   = reader.GetAttribute("usemap");
                        nodeText  = reader.GetAttribute("resolveMapLinks");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _resolveLinks = Convert.ToBoolean(nodeText);
                        }
                    }
                    else if (String.Equals(reader.Name, "altText",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _altText = reader.ReadString();
                    }
                    else if (String.Equals(reader.Name, "map",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _imageMap = new MediaImageMap();
                        _imageMap.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "item", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }    

            bool hasImageMap = this.HasImageMap;

            writer.WriteStartElement(TagName);  // start - item
            writer.WriteAttributeString("id", _itemId);
            if (_itemType != MediaItemType.None && 
                _itemType != MediaItemType.Image)
            {
                writer.WriteAttributeString("type", _itemType.ToString());
            }     
            if (_itemWidth > 0 && _itemHeight > 0)
            {
                writer.WriteAttributeString("width",  _itemWidth.ToString());
                writer.WriteAttributeString("height", _itemHeight.ToString());
                if (_itemUnit != MediaSizeUnit.None)
                {
                    writer.WriteAttributeString("unit", _itemUnit.ToString());
                }
            }

            BuildPathResolver pathResolver = BuildPathResolver.Resolver;

            writer.WriteStartElement("image");  // start - image
            if (_itemType == MediaItemType.YouTube)
            {
                writer.WriteAttributeString("file", _itemName);
            }
            else
            {
                writer.WriteAttributeString("file", 
                    pathResolver.ResolveRelative(_itemPath));
                //writer.WriteAttributeString("file", _itemPath.Name);
            }
            if (hasImageMap)
            {
                writer.WriteAttributeString("usemap", _useMap);
                writer.WriteAttributeString("resolveMapLinks", 
                    _resolveLinks.ToString());
            }
            writer.WriteStartElement("altText");  // start - altText
            writer.WriteString(_altText);
            writer.WriteEndElement();             // end - altText 
            writer.WriteEndElement();           // end - image

            if (hasImageMap)
            {
                _imageMap.WriteXml(writer);
            }

            writer.WriteEndElement();           // end - item
        }

        #endregion

        #region ICloneable Members

        public override MediaItem Clone()
        {
            MediaItem resource = new MediaItem(this);
            if (_itemPath != null)
            {
                resource._itemPath = _itemPath.Clone();
            }
            if (_itemId != null)
            {
                resource._itemId = String.Copy(_itemId);
            }
            if (_altText != null)
            {
                resource._altText = String.Copy(_altText);
            }
            if (_useMap != null)
            {
                resource._useMap = String.Copy(_useMap);
            }
            if (_imageMap != null)
            {
                resource._imageMap = _imageMap.Clone();
            }

            return resource;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _itemId;
            }
        }

        #endregion
    }
}
