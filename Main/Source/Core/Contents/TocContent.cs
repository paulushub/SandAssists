using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TocContent : BuildContent<TocItem, TocContent>
    {
        #region Public Fields

        public const string TagName = "tocContent";

        #endregion

        #region Private Fields

        private bool _isEnabled;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public TocContent()
            : base(new BuildKeyedList<TocItem>())
        {
            BuildKeyedList<TocItem> keyedList =
                this.List as BuildKeyedList<TocItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _isEnabled = true;
        }

        public TocContent(TocContent source)
            : base(source)
        {
            _isEnabled = source._isEnabled;
            _dicItems  = source._dicItems;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (base.IsEmpty)
                {
                    return true;
                }

                int validRoots = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    TocItem item = this[i];
                    if (!item.IsEmpty)
                    {
                        validRoots++;
                    }
                }

                return (validRoots == 0);
            }
        }

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public TocItem this[string itemName]
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

        public override bool IsHierarchical
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Method

        public override void Add(TocItem item)
        {
            if (!this.IsKeyed)
            {
                base.Add(item);
                return;
            }

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
            if (!this.IsKeyed)
            {
                return false;
            }

            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (!this.IsKeyed)
            {
                return -1;
            }

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
            if (!this.IsKeyed)
            {
                return false;
            }

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

        public override bool Remove(TocItem item)
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

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            string nodeText = reader.GetAttribute("enabled");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _isEnabled = Convert.ToBoolean(nodeText);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, TocItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        TocItem item = new TocItem();
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("enabled", _isEnabled.ToString());

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override TocContent Clone()
        {
            TocContent content = new TocContent(this);

            this.Clone(content, new BuildKeyedList<TocItem>());

            return content;
        }

        #endregion
    }
}
