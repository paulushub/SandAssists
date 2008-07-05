using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemIndex : HelpObject<ItemIndex>
    {
        #region Constructors and Destructor

        public ItemIndex()
        {
        }

        public ItemIndex(ItemIndex source)
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

        public override ItemIndex Clone()
        {
            ItemIndex configItem = new ItemIndex(this);

            return configItem;
        }

        #endregion
    }
}
