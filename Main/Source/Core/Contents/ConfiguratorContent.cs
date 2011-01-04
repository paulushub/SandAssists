using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ConfiguratorContent : BuildContent<ConfiguratorItem, ConfiguratorContent>
    {
        #region Private Fields

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public ConfiguratorContent()
            : base(new BuildKeyedList<ConfiguratorItem>())
        {
            BuildKeyedList<ConfiguratorItem> keyedList =
               this.List as BuildKeyedList<ConfiguratorItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public ConfiguratorContent(ConfiguratorContent source)
            : base(source)
        {
            _dicItems = source._dicItems;
        }

        #endregion

        #region Public Properties

        public ConfiguratorItem this[string itemKeyword]
        {
            get
            {
                if (String.IsNullOrEmpty(itemKeyword))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemKeyword, out curIndex))
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

        public override void Add(ConfiguratorItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Keyword))
            {
                if (_dicItems.ContainsKey(item.Keyword))
                {
                    this.Insert(_dicItems[item.Keyword], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemKeyword)
        {
            if (String.IsNullOrEmpty(itemKeyword) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemKeyword);
        }

        public int IndexOf(string itemKeyword)
        {
            if (String.IsNullOrEmpty(itemKeyword) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemKeyword))
            {
                return _dicItems[itemKeyword];
            }

            return -1;
        }

        public bool Remove(string itemKeyword)
        {
            int itemIndex = this.IndexOf(itemKeyword);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemKeyword))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(ConfiguratorItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Keyword);
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

        public override ConfiguratorContent Clone()
        {
            ConfiguratorContent content = new ConfiguratorContent(this);

            this.Clone(content, new BuildKeyedList<ConfiguratorItem>());

            return content;
        }

        #endregion
    }
}
