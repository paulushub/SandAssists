using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class DependencyContent : BuildContent<DependencyItem, DependencyContent>
    {
        #region Private Fields

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public DependencyContent()
            : base(new BuildKeyedList<DependencyItem>())
        {
            BuildKeyedList<DependencyItem> keyedList =
                this.List as BuildKeyedList<DependencyItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public DependencyContent(DependencyContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public DependencyItem this[string itemName]
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

        public override DependencyContent Clone()
        {
            DependencyContent content = new DependencyContent(this);

            this.Clone(content, new BuildKeyedList<DependencyItem>());

            return content;
        }

        #endregion
    }
}
