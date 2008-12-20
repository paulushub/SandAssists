using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public class HelpSharedItem : HelpObject<HelpSharedItem>
    {
        #region Private Fields

        private string _itemId;
        private string _itemName;
        private string _itemText;
        private string _itemDesc;

        private HelpSharedFormat _sharedFormat;

        #endregion

        #region Constructors and Destructor

        public HelpSharedItem()
        {
        }

        public HelpSharedItem(HelpSharedItem source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_itemId))
                {
                    return false;
                }

                return true;
            }
        }

        public string Id
        {
            get 
            { 
                return _itemId; 
            }
            set 
            { 
                _itemId = value; 
            }
        }

        public string Name
        {
            get 
            { 
                return _itemName; 
            }
            set 
            { 
                _itemName = value; 
            }
        }

        public string Text
        {
            get 
            { 
                return _itemText; 
            }
            set 
            { 
                _itemText = value; 
            }
        }

        public string Description
        {
            get 
            { 
                return _itemDesc; 
            }
            set 
            { 
                _itemDesc = value; 
            }
        }

        public HelpSharedFormat Format
        {
            get 
            { 
                return _sharedFormat; 
            }
            set 
            { 
                _sharedFormat = value; 
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

        public override HelpSharedItem Clone()
        {
            HelpSharedItem style = new HelpSharedItem(this);

            return style;
        }

        #endregion
    }
}
