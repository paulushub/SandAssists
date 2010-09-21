using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurators
{
    [Serializable]
    public class ConfiguratorContent : 
        BuildContent<ConfiguratorItem, ConfiguratorContent>
    {
        #region Private Fields

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
