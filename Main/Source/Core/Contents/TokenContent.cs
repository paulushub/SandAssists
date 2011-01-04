using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TokenContent : BuildContent<TokenItem, TokenContent>
    {
        #region Private Fields

        private string _contentsFile;
        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public TokenContent()
            : base(new BuildKeyedList<TokenItem>())
        {
            BuildKeyedList<TokenItem> keyedList =
                this.List as BuildKeyedList<TokenItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public TokenContent(string contentFile)
            : base(new BuildKeyedList<TokenItem>())
        {
            _contentsFile = contentFile;

            BuildKeyedList<TokenItem> keyedList =
                this.List as BuildKeyedList<TokenItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public TokenContent(TokenContent source)
            : base(source)
        {
            _contentsFile = source._contentsFile;
            _dicItems     = source._dicItems;
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

        public TokenItem this[string itemKey]
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

        #region Public Method

        public override void Add(TokenItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Key))
            {
                if (_dicItems.ContainsKey(item.Key))
                {
                    this.Insert(_dicItems[item.Key], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemKey)
        {
            if (String.IsNullOrEmpty(itemKey) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemKey);
        }

        public int IndexOf(string itemKey)
        {
            if (String.IsNullOrEmpty(itemKey) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemKey))
            {
                return _dicItems[itemKey];
            }

            return -1;
        }

        public bool Remove(string itemKey)
        {
            int itemIndex = this.IndexOf(itemKey);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemKey))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(TokenItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Key);
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

        public override TokenContent Clone()
        {
            TokenContent content = new TokenContent(this);

            this.Clone(content, new BuildKeyedList<TokenItem>());

            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }

            return content;
        }

        #endregion
    }
}
