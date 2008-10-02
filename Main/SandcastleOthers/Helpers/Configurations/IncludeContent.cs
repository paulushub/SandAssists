using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurations
{
    [Serializable]
    public class IncludeContent : BuildContent<IncludeItem, IncludeContent>
    {
        #region Private Fields

        private string _contentName;
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public IncludeContent()
            : this(String.Empty)
        {
        }

        public IncludeContent(string contentName)
            : base(new BuildKeyedList<IncludeItem>())
        {
            _contentName = contentName;
            if (_contentName == null)
            {
                _contentName = String.Empty;
            }

            BuildKeyedList<IncludeItem> keyedList =
                this.List as BuildKeyedList<IncludeItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public IncludeContent(IncludeContent source)
            : base(source)
        {
            _contentName = source._contentName;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return _contentName;
            }
        }

        public IncludeItem this[string itemKey]
        {
            get
            {
                if (String.IsNullOrEmpty(itemKey))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemKey, out curIndex))
                {
                    return this[curIndex];
                }

                return null;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
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

        public override IncludeContent Clone()
        {
            IncludeContent content = new IncludeContent(this);

            this.Clone(content, new BuildKeyedList<IncludeItem>());

            if (_contentName != null)
            {
                content._contentName = String.Copy(_contentName);
            }

            return content;
        }

        #endregion
    }
}
