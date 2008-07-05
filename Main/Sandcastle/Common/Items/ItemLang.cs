using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemLang : HelpObject<ItemLang>
    {
        #region Constructors and Destructor

        public ItemLang()
        {
        }

        public ItemLang(ItemLang source)
            : base(source)
        {
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

        public override ItemLang Clone()
        {
            ItemLang configItem = new ItemLang(this);

            return configItem;
        }

        #endregion
    }
}
