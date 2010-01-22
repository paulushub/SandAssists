using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class MetadataContent : BuildContent<MetadataItem, MetadataContent>
    {
        #region Private Fields

        private MetadataType _metaType;
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public MetadataContent()
            : this(MetadataType.Attribute)
        {
            BuildKeyedList<MetadataItem> keyedList =
                this.List as BuildKeyedList<MetadataItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public MetadataContent(MetadataType type)
            : base(new BuildKeyedList<MetadataItem>())
        {
            _metaType = type;
        }

        public MetadataContent(MetadataContent source)
            : base(source)
        {
            _metaType = source._metaType;
        }

        #endregion

        #region Public Properties

        public MetadataType MetadataType
        {
            get
            {
                return _metaType;
            }
        }

        public MetadataItem this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemName, out curIndex))
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

        public override MetadataContent Clone()
        {
            MetadataContent content = new MetadataContent(this);

            this.Clone(content, new BuildKeyedList<MetadataItem>());

            return content;
        }

        #endregion
    }
}
