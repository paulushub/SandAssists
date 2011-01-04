using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaContent : BuildContent<MediaItem, MediaContent>
    {
        #region Private Fields

        private string _contentsName;
        private string _contentsPath;
        private string _contentsFile;
        private string _outputBase;
        private string _outputPath;
        private string _outputLink;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public MediaContent()
            : base(new BuildKeyedList<MediaItem>())
        {
            BuildKeyedList<MediaItem> keyedList =
                this.List as BuildKeyedList<MediaItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _outputPath = "string('media')";
            _outputLink = "media";
            _outputBase = @".\Output";
        }

        public MediaContent(string contentsFile)
            : this()
        {
            _contentsName = String.Empty;
            if (String.IsNullOrEmpty(contentsFile) == false)
            {
                _contentsPath = Path.GetDirectoryName(contentsFile);
            }
            _contentsFile = contentsFile;
        }

        public MediaContent(MediaContent source)
            : base(source)
        {
            _contentsName = source._contentsName;
            _contentsPath = source._contentsPath;
            _contentsFile = source._contentsFile;
            _outputPath   = source._outputPath;
            _outputLink   = source._outputLink;
            _outputBase   = source._outputBase;
            _dicItems     = source._dicItems;
        }

        #endregion

        #region Public Properties

        public MediaItem this[string mediaId]
        {
            get
            {
                if (String.IsNullOrEmpty(mediaId))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(mediaId, out curIndex))
                {
                    return this[curIndex];
                }

                return null;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_contentsFile) == false)
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public string ContentsName
        {
            get 
            { 
                return _contentsName; 
            }
            set 
            { 
                _contentsName = value; 
            }
        }
        
        public string ContentsPath
        {
            get 
            { 
                return _contentsPath; 
            }
            set 
            { 
                _contentsPath = value; 
            }
        }

        public string ContentsFile
        {
            get 
            { 
                return _contentsFile; 
            }
            set 
            { 
                _contentsFile = value; 
            }
        }

        public string OutputBase
        {
            get 
            {
                return _outputBase; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputBase = value; 
                }
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputPath = value;
                }
            }
        }

        public string OutputLink
        {
            get 
            { 
                return _outputLink; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputLink = value; 
                }
            }
        }

        #endregion

        #region Public Method

        public override void Add(MediaItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.MediaId))
            {
                if (_dicItems.ContainsKey(item.MediaId))
                {
                    this.Insert(_dicItems[item.MediaId], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string mediaId)
        {
            if (String.IsNullOrEmpty(mediaId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(mediaId);
        }

        public int IndexOf(string mediaId)
        {
            if (String.IsNullOrEmpty(mediaId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(mediaId))
            {
                return _dicItems[mediaId];
            }

            return -1;
        }

        public bool Remove(string mediaId)
        {
            int itemIndex = this.IndexOf(mediaId);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(mediaId))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(MediaItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.MediaId);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override MediaContent Clone()
        {
            MediaContent content = new MediaContent(this);

            this.Clone(content, new BuildKeyedList<MediaItem>());

            if (_contentsName != null)
            {
                content._contentsName = String.Copy(_contentsName);
            }
            if (_contentsPath != null)
            {
                content._contentsPath = String.Copy(_contentsPath);
            }
            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }
            if (_outputBase != null)
            {
                content._outputBase = String.Copy(_outputBase);
            }
            if (_outputPath != null)
            {
                content._outputPath = String.Copy(_outputPath);
            }
            if (_outputLink != null)
            {
                content._outputLink = String.Copy(_outputLink);
            }

            return content;
        }

        #endregion
    }
}
