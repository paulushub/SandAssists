using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurations
{
    [Serializable]
    public class ConfigurationContent : 
        BuildContent<ConfigurationItem, ConfigurationContent>
    {
        #region Private Fields

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public ConfigurationContent()
            : base(new BuildKeyedList<ConfigurationItem>())
        {
            BuildKeyedList<ConfigurationItem> keyedList =
               this.List as BuildKeyedList<ConfigurationItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public ConfigurationContent(ConfigurationContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public ConfigurationItem this[string itemKeyword]
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

        public override ConfigurationContent Clone()
        {
            ConfigurationContent content = new ConfigurationContent(this);

            this.Clone(content, new BuildKeyedList<ConfigurationItem>());

            return content;
        }

        #endregion
    }
}
