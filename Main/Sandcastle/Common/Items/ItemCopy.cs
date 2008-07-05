using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemCopy : HelpObject<ItemCopy>
    {
        #region Constructors and Destructor

        public ItemCopy()
        {
        }

        public ItemCopy(ItemCopy source)
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

        public override ItemCopy Clone()
        {
            ItemCopy configItem = new ItemCopy(this);

            return configItem;
        }

        #endregion
    }
}
