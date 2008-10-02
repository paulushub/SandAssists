using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class PropertyContent : BuildContent<PropertyItem, PropertyContent>
    {
        #region Private Fields

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public PropertyContent()
            : base(new BuildKeyedList<PropertyItem>())
        {
            BuildKeyedList<PropertyItem> keyedList =
                this.List as BuildKeyedList<PropertyItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public PropertyContent(PropertyContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public PropertyItem this[string itemName]
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

        public override PropertyContent Clone()
        {
            PropertyContent content = new PropertyContent(this);

            this.Clone(content, new BuildKeyedList<PropertyItem>());

            return content;
        }

        #endregion
    }
}
