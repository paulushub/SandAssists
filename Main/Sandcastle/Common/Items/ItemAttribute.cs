using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemAttribute : HelpObject<ItemAttribute>
    {
        #region Constructors and Destructor

        public ItemAttribute()
        {
        }

        public ItemAttribute(ItemAttribute source)
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

        public override ItemAttribute Clone()
        {
            ItemAttribute style = new ItemAttribute(this);

            return style;
        }

        #endregion
    }

}
