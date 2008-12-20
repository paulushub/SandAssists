using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemColors : HelpObject<ItemColors>
    {
        #region Constructors and Destructor

        public ItemColors()
        {
        }

        public ItemColors(ItemColors source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public ItemColor this[int index]
        {
            get
            {
                return null;
            }

            set
            {   
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

        public override ItemColors Clone()
        {
            ItemColors configItem = new ItemColors(this);

            return configItem;
        }

        #endregion
    }
}
