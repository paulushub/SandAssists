using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public class HelpSharedContent : HelpObject<HelpSharedContent>
    {
        #region Private Fields

        private List<HelpSharedItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public HelpSharedContent()
        {
            _listItems = new List<HelpSharedItem>();
        }

        public HelpSharedContent(HelpSharedContent source)
            : base(source)
        {
            _listItems = source._listItems;
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public HelpSharedItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }

            set
            {
                if (_listItems != null && value != null)
                {
                    _listItems[index] = value;
                }
            }
        }

        public IList<HelpSharedItem> Items
        {
            get
            {
                return _listItems;
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

        public override HelpSharedContent Clone()
        {
            HelpSharedContent content = new HelpSharedContent(this);

            if (_listItems != null)
            {
                int itemCount = _listItems.Count;
                List<HelpSharedItem> clonedList = 
                    new List<HelpSharedItem>(itemCount);

                for (int i = 0; i < itemCount; i++)
                {
                    clonedList.Add(_listItems[i].Clone());
                }

                content._listItems = clonedList;
            }

            return content;
        }

        #endregion
    }
}
