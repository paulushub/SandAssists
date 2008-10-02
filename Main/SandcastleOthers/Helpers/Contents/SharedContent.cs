using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class SharedContent : BuildContent<SharedItem, SharedContent>
    {
        #region Private Fields

        private string _contentName;
        private string _contentsFile;
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public SharedContent()
            : base(new BuildKeyedList<SharedItem>())
        {
            BuildKeyedList<SharedItem> keyedList =
                this.List as BuildKeyedList<SharedItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public SharedContent(string contentName, string contentFile)
            : base(new BuildKeyedList<SharedItem>())
        {
            _contentName  = contentName;
            _contentsFile = contentFile;

            BuildKeyedList<SharedItem> keyedList =
                this.List as BuildKeyedList<SharedItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public SharedContent(SharedContent source)
            : base(source)
        {
            _contentsFile = source._contentsFile;
        }

        #endregion

        #region Public Properties

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

        public string Name
        {
            get
            {
                return _contentName;
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

        public SharedItem this[string itemKey]
        {
            get
            {
                if (String.IsNullOrEmpty(itemKey))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemKey, out curIndex))
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

        public override SharedContent Clone()
        {
            SharedContent content = new SharedContent(this);

            this.Clone(content, new BuildKeyedList<SharedItem>());

            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }

            return content;
        }

        #endregion
    }
}
