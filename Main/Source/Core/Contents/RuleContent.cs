using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class RuleContent : BuildContent<RuleItem, RuleContent>
    {
        #region Private Fields

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public RuleContent()
            : base(new BuildKeyedList<RuleItem>())
        {
            BuildKeyedList<RuleItem> keyedList =
                this.List as BuildKeyedList<RuleItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public RuleContent(RuleContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public RuleItem this[string itemName]
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

        public override RuleContent Clone()
        {
            RuleContent content = new RuleContent(this);

            this.Clone(content, new BuildKeyedList<RuleItem>());

            return content;
        }

        #endregion
    }
}
