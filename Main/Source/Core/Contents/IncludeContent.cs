using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class IncludeContent : BuildContent<IncludeItem, IncludeContent>
    {
        #region Private Fields

        private string _contentName;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public IncludeContent()
            : this(String.Empty)
        {
        }

        public IncludeContent(string contentName)
            : base(new BuildKeyedList<IncludeItem>())
        {
            _contentName = contentName;
            if (_contentName == null)
            {
                _contentName = String.Empty;
            }

            BuildKeyedList<IncludeItem> keyedList =
                this.List as BuildKeyedList<IncludeItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public IncludeContent(IncludeContent source)
            : base(source)
        {
            _contentName = source._contentName;
            _dicItems    = source._dicItems;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return _contentName;
            }
        }

        public IncludeItem this[string itemKey]
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

        #region Public Methods

        public override void Add(IncludeItem item)
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

        public override bool Remove(IncludeItem item)
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
            base.Clear();

            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
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

        public override IncludeContent Clone()
        {
            IncludeContent content = new IncludeContent(this);

            this.Clone(content, new BuildKeyedList<IncludeItem>());

            if (_contentName != null)
            {
                content._contentName = String.Copy(_contentName);
            }

            return content;
        }

        #endregion
    }
}
