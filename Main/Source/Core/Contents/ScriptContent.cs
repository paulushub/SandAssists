using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ScriptContent : BuildContent<ScriptItem, ScriptContent>
    {
        #region Private Fields

        private bool   _value;
        private string _name;
        private string _description;
        private string _tag;
        private string _helpId;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public ScriptContent()
            : this(Guid.NewGuid().ToString())
        {
        }

        public ScriptContent(string name)
            : base(new BuildKeyedList<ScriptItem>())
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;

            BuildKeyedList<ScriptItem> keyedList =
                this.List as BuildKeyedList<ScriptItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public ScriptContent(ScriptContent source)
            : base(source)
        {
            _dicItems = source._dicItems;
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

        public ScriptItem this[string itemName]
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

        #region Public Method

        public override void Add(ScriptItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Name))
            {
                if (_dicItems.ContainsKey(item.Name))
                {
                    this.Insert(_dicItems[item.Name], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemName))
            {
                return _dicItems[itemName];
            }

            return -1;
        }

        public bool Remove(string itemName)
        {
            int itemIndex = this.IndexOf(itemName);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemName))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(ScriptItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Name);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
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

        public override ScriptContent Clone()
        {
            ScriptContent content = new ScriptContent(this);

            this.Clone(content, new BuildKeyedList<ScriptItem>());

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
