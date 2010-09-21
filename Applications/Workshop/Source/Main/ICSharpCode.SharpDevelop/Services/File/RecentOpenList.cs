using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop
{
    /// <summary>
    /// This provides the base class for a generic collection with change
    /// information or events.
    /// </summary>
    /// <remarks>
    /// This provides a specialized generic collection for reference types only
    /// with supports events for changes, tracking for the modifications state 
    /// and no support for <see langword="null"/> elements.
    /// </remarks>
    [Serializable]                      
    public sealed class RecentOpenList : Collection<RecentOpenItem>,
        IXmlSerializable
    {
        #region Private Fields

        public const string XmlTagName = "recentItems";

        private int    _displayable;
        private int    _pinnedCount;
        private string _category;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="RecentOpenList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentOpenList"/> class.
        /// </summary>
        public RecentOpenList()
        {
            _displayable = 10;
            _category    = String.Empty;
        }

        /// <overloads>
        /// Initializes a new instance of the <see cref="RecentOpenList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentOpenList"/> class.
        /// </summary>
        public RecentOpenList(string category)
        {
            _displayable = 10;
            _category    = category;
            if (_category == null)
            {
                _category = String.Empty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentOpenList"/> class,
        /// with initial elements from the specified list, and acts as a wrapper 
        /// for the specified list.
        /// </summary>
        /// <param name="list">
        /// The list that is wrapped by the new collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public RecentOpenList(string category, IList<RecentOpenItem> list)
            : base(list)
        {
            _displayable = 10;
            _category    = category;
            if (_category == null)
            {
                _category = String.Empty;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// This event is raised whenever a change occurs in the 
        /// <see cref="RecentOpenList"/>.
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region Public Properties

        public string Category
        {
            get
            {
                return _category;
            }
        }

        public int Displayable
        {
            get
            {   
                return _displayable;
            }
            set
            {
                if (value >= 0)
                {
                    if (value > RecentOpen.MaxDisplayedSize)
                    {
                        _displayable = RecentOpen.MaxDisplayedSize;
                    }
                    else
                    {
                        _displayable = value;
                    }
                }
            }
        }

        public int Pinned
        {
            get
            {
                if (_pinnedCount < 0)
                {
                    _pinnedCount = 0;
                }               

                return _pinnedCount;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the elements of the specified collection to the end of this list.
        /// </summary>
        /// <param name="items">
        /// The collection whose elements should be added to the end of this list.
        /// </param>
        public void Add(IList<RecentOpenItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(items[i]);
            }
        }

        public void UpdatePinnedState()
        {
            _pinnedCount = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Pinned)
                {
                    _pinnedCount++;
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="RecentOpenList">collection</see> 
        /// at the specified index. 
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="newItem">
        /// The object to insert. The value cannot be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <typeparamref name="newItem"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than zero
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is greater than the total count.
        /// </exception>
        protected override void InsertItem(int index, RecentOpenItem newItem)
        {
            if (newItem == null)
            {
                return;
            }

            //if (index < this.Count)
            //{
            //    RecentOpenItem recentItem = this[index];
            //    if (recentItem.Pinned)
            //    {
            //        _pinnedCount--;
            //    }
            //}

            base.InsertItem(index, newItem);
            
            if (newItem.Pinned)
            {
                _pinnedCount++;
            }

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This replaces the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="newItem">
        /// The new value for the element at the specified index. 
        /// The value cannot be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <typeparamref name="newItem"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than zero
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is greater than the total count.
        /// </exception>
        protected override void SetItem(int index, RecentOpenItem newItem)
        {
            if (newItem == null)
            {
                return;
            }

            if (index < this.Count)
            {
                RecentOpenItem recentItem = this[index];
                if (recentItem.Pinned)
                {
                    _pinnedCount--;
                }
            }

            base.SetItem(index, newItem);
            
            if (newItem.Pinned)
            {
                _pinnedCount++;
            }

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This removes the element at the specified index of the 
        /// <see cref="RecentOpenList">collection</see>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than zero
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is greater than the total count.
        /// </exception>
        protected override void RemoveItem(int index)
        {
            RecentOpenItem recentItem = this[index];

            base.RemoveItem(index);
            if (recentItem.Pinned)
            {
                _pinnedCount--;
            }

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This removes all elements from the <see cref="RecentOpenList">
        /// collection</see>. 
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();

            _pinnedCount = 0;

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null || !String.Equals(reader.Name, XmlTagName))
            {
                return;
            }

            _category = reader.GetAttribute("category");
            string displayableText = reader.GetAttribute("displayable");
            if (!String.IsNullOrEmpty(displayableText))
            {
                _displayable = Convert.ToInt32(displayableText);
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, RecentOpenItem.XmlTagName))
                    {
                        RecentOpenItem recentItem = new RecentOpenItem();
                        recentItem.ReadXml(reader);
                        if (!recentItem.IsEmpty)
                        {
                            this.Add(recentItem);
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, XmlTagName))
                    {
                        break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement(XmlTagName);
            writer.WriteAttributeString("category", _category);
            writer.WriteAttributeString("displayable", _displayable.ToString());

            for (int i = 0; i < this.Count; i++)
            {
                RecentOpenItem recentItem = this[i];
                recentItem.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
