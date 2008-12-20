using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemColor : HelpObject<ItemColor>
    {
        #region Constructors and Destructor

        public ItemColor()
        {
        }

        public ItemColor(ItemColor source)
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

        public override ItemColor Clone()
        {
            ItemColor style = new ItemColor(this);

            return style;
        }

        #endregion
    }
}
