using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemData : HelpObject<ItemData>
    {
        #region Constructors and Destructor

        public ItemData()
        {
        }

        public ItemData(ItemData source)
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

        public override ItemData Clone()
        {
            ItemData configItem = new ItemData(this);

            return configItem;
        }

        #endregion
    }
}
