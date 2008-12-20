using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemSave : HelpObject<ItemSave>
    {
        #region Constructors and Destructor

        public ItemSave()
        {
        }

        public ItemSave(ItemSave source)
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

        public override ItemSave Clone()
        {
            ItemSave configItem = new ItemSave(this);

            return configItem;
        }

        #endregion
    }
}
