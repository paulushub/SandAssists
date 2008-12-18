using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualCategoryContent : 
        BuildContent<ConceptualCategoryItem, ConceptualCategoryContent>
    {
        #region Private Fields

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public ConceptualCategoryContent()
            : base(new BuildKeyedList<ConceptualCategoryItem>())
        {
            BuildKeyedList<ConceptualCategoryItem> keyedList =
                this.List as BuildKeyedList<ConceptualCategoryItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public ConceptualCategoryContent(ConceptualCategoryContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public ConceptualCategoryItem this[string itemName]
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

        public override ConceptualCategoryContent Clone()
        {
            ConceptualCategoryContent content = new ConceptualCategoryContent(this);

            this.Clone(content, new BuildKeyedList<ConceptualCategoryItem>());

            return content;
        }

        #endregion
    }
}
