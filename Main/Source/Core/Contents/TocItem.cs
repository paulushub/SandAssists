using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TocItem : BuildItem<TocItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "tocItem";

        #endregion

        #region Private Fields

        private bool   _isRecursive;
        private string _name;
        private string _tocId;
        private string _sourceId;
        private TocItemSourceType _sourceType;
        private BuildKeyedList<TocItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public TocItem()
            : this(String.Format("tc{0:x}", Guid.NewGuid().ToString().GetHashCode()), String.Empty)
        {
        }

        public TocItem(string name, string tocItemId)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            if (String.IsNullOrEmpty(tocItemId))
            {
                tocItemId = String.Format("tcid{0:x}", Guid.NewGuid().ToString().GetHashCode());
            }

            _isRecursive = true;
            _name        = name;
            _tocId       = tocItemId;
            _sourceId    = String.Empty;
            _sourceType  = TocItemSourceType.None;
        }

        public TocItem(TocItem source)
            : base(source)
        {
            _name        = source._name;
            _tocId       = source._tocId;
            _sourceId    = source._sourceId;
            _sourceType  = source._sourceType;
            _listItems   = source._listItems;
            _isRecursive = source._isRecursive;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) || _sourceType == TocItemSourceType.None)
                {
                    return true;
                }

                return false;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Id
        {
            get
            {
                return _tocId;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _tocId = value;
            }
        }

        public string SourceId
        {
            get
            {
                return _sourceId;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _sourceId = value;
            }
        }

        public TocItemSourceType SourceType
        {
            get
            {
                return _sourceType;
            }
            set
            {
                _sourceType = value;
            }
        }

        public bool SourceRecursive
        {
            get
            {
                return _isRecursive;
            }
            set
            {
                _isRecursive = value;
            }
        }


        public TocItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public TocItem this[string name]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[name];
                }

                return null;
            }
        }

        public int ItemCount
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

        public IList<TocItem> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<TocItem>(_listItems);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="TocItem"/> class instance, this property is 
        /// <see cref="TocItem.TagName"/>.
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

        public void Add(TocItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems == null)
            {
                _listItems = new BuildKeyedList<TocItem>();
            }

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Add(item);
        }

        public void Insert(int index, TocItem item)
        {
            BuildExceptions.NotNull(item, "item");

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Insert(index, item);
        }

        public void Remove(int itemIndex)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(itemIndex);
        }

        public void Remove(TocItem item)
        {
            if (item == null)
            {
                return;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public void Clear()
        {
            if (_listItems != null)
            {
                _listItems = new BuildKeyedList<TocItem>();
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(TocItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            TocItem other = obj as TocItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_tocId != null)
            {
                hashCode ^= _tocId.GetHashCode();
            }

            return hashCode;
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
                return;
            }

            _name  = reader.GetAttribute("name");
            _tocId = reader.GetAttribute("id");    

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listItems == null)
            {
                _listItems = new BuildKeyedList<TocItem>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "source", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _sourceId = reader.GetAttribute("id");
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "topic":
                                _sourceType = TocItemSourceType.Topic;
                                break;
                            case "group":
                                _sourceType = TocItemSourceType.Group;
                                break;
                            case "namespace":
                                _sourceType = TocItemSourceType.Namespace;
                                break;
                            case "namespaceroot":
                                _sourceType = TocItemSourceType.NamespaceRoot;
                                break;
                            default:
                                _sourceType = TocItemSourceType.None;
                                break;
                        }
                        _isRecursive = Convert.ToBoolean(reader.GetAttribute("recursive"));
                    }
                    else if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        TocItem item = new TocItem();

                        item.ReadXml(reader);

                        if (!item.IsEmpty)
                        {
                            this.Add(item);
                        }
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - item
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("id",   _tocId);

            writer.WriteStartElement("source"); // start - source
            writer.WriteAttributeString("id",   _sourceId);
            writer.WriteAttributeString("type", _sourceType.ToString());
            writer.WriteAttributeString("recursive",  _isRecursive.ToString());
            writer.WriteEndElement();           // end - source

            if (_listItems != null && _listItems.Count != 0)
            {
                for (int i = 0; i < _listItems.Count; i++)
                {
                    _listItems[i].WriteXml(writer);
                }
            }

            writer.WriteEndElement();           // end - item
        }


        #endregion

        #region ICloneable Members

        public override TocItem Clone()
        {
            TocItem item = new TocItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_tocId != null)
            {
                item._tocId = String.Copy(_tocId);
            }

            return item;
        }

        #endregion
    }
}
