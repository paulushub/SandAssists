using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class IncludeContent : BuildContent<IncludeItem, IncludeContent>
    {
        #region Public Fields

        public const string TagName = "includeContent";

        #endregion

        #region Private Fields

        private string _contentName;

        [NonSerialized]
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
            _dicItems    = source._dicItems;
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

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="IncludeContent"/> class instance, this property is 
        /// <see cref="IncludeContent.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

        public override void Add(IncludeItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Key))
            {
                if (_dicItems.ContainsKey(item.Key))
                {
                    this.Insert(_dicItems[item.Key], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemKey)
        {
            if (String.IsNullOrEmpty(itemKey) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemKey);
        }

        public int IndexOf(string itemKey)
        {
            if (String.IsNullOrEmpty(itemKey) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemKey))
            {
                return _dicItems[itemKey];
            }

            return -1;
        }

        public bool Remove(string itemKey)
        {
            int itemIndex = this.IndexOf(itemKey);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemKey))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(IncludeItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Key);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            base.Clear();

            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            _contentName = reader.GetAttribute("name");
            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, IncludeItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        IncludeItem item = new IncludeItem();
                        item.Content = this;
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
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("name", _contentName);

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
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
