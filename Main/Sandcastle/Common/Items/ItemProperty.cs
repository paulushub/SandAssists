using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemProperty : HelpObject<ItemProperty>
    {
        #region Constructors and Destructor

        public ItemProperty()
        {
        }

        public ItemProperty(ItemProperty source)
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

        public override ItemProperty Clone()
        {
            ItemProperty configItem = new ItemProperty(this);

            return configItem;
        }

        #endregion
    }
}
