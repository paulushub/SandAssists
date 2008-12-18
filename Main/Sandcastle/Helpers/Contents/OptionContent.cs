using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class OptionContent : BuildContent<OptionItem, OptionContent>
    {
        #region Private Fields

        private bool   _value;
        private string _name;
        private string _description;
        private string _tag;
        private string _helpId;

        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public OptionContent()
            : this(Guid.NewGuid().ToString())
        {
        }

        public OptionContent(string name)
            : base(new BuildKeyedList<OptionItem>())
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;

            BuildKeyedList<OptionItem> keyedList =
                this.List as BuildKeyedList<OptionItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public OptionContent(OptionContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public string HelpId
        {
            get
            {
                return _helpId;
            }
            set
            {
                _helpId = value;
            }
        }

        public OptionItem this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemName, out curIndex))
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

        public override OptionContent Clone()
        {
            OptionContent content = new OptionContent(this);

            this.Clone(content, new BuildKeyedList<OptionItem>());

            content._value = _value;
            if (_name != null)
            {
                content._name = String.Copy(_name);
            }
            if (_description != null)
            {
                content._description = String.Copy(_description);
            }
            if (_tag != null)
            {
                content._tag = String.Copy(_tag);
            }
            if (_helpId != null)
            {
                content._helpId = String.Copy(_helpId);
            }

            return content;
        }

        #endregion
    }
}
