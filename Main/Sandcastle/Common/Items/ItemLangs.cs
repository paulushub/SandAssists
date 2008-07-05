using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    [Serializable]
    public class ItemLangs : HelpObject<ItemLangs>
    {
        #region Constructors and Destructor

        public ItemLangs()
        {
        }

        public ItemLangs(ItemLangs source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public ItemLang this[int index]
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

        public override ItemLangs Clone()
        {
            ItemLangs configItem = new ItemLangs(this);

            return configItem;
        }

        #endregion
    }
}
