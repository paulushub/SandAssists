using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildContent<T, U> : ICloneable, IXmlSerializable
        where T : BuildItem<T>
        where U : BuildContent<T, U>        
    {
        #region Private Fields

        private IList<T> _listItems;

        #endregion

        #region Constructors and Destructor

        protected BuildContent()
        {
            _listItems = new BuildList<T>();
        }

        protected BuildContent(IList<T> itemList)
        {
            BuildExceptions.NotNull(itemList, "itemList");

            _listItems = itemList;
        }

        protected BuildContent(BuildContent<T, U> source)
        {
            BuildExceptions.NotNull(source, "source");

            _listItems = source._listItems;
        }

        #endregion

        #region Public Properties

        public virtual int Count
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

        public virtual T this[int index]
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
                if (value != null)
                {
                    _listItems[index] = value;
                }
            }
        }

        public virtual IList<T> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<T>(_listItems);
                }

                return null;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (_listItems == null || _listItems.Count == 0);
            }
        }

        public virtual bool IsKeyed
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Protected Properties

        protected IList<T> List
        {
            get
            {
                return _listItems;
            }
        }

        #endregion

        #region Public Method

        public virtual void Add(T item)
        {
            BuildExceptions.NotNull(item, "item");

            _listItems.Add(item);
        }

        public virtual void Add(IList<T> items)
        {
            BuildExceptions.NotNull(items, "items");

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(items[i]);
            }
        }

        public virtual void Remove(int index)
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(index);
        }

        public virtual void Remove(T item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public virtual bool Contains(T item)
        {
            if (item == null || _listItems.Count == 0)
            {
                return false;
            }

            return _listItems.Contains(item);
        }

        public virtual void Clear()
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.Clear();
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
        }

        public virtual void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public abstract U Clone();

        protected virtual U Clone(U clonedContent)
        {
            if (_listItems != null)
            {
                int itemCount = _listItems.Count;
                BuildList<T> listItems = new BuildList<T>(
                    new List<T>(itemCount));

                for (int i = 0; i < itemCount; i++)
                {
                    listItems.Add(_listItems[i].Clone());
                }

                clonedContent._listItems = listItems;
            }

            return clonedContent;
        }

        protected virtual U Clone(U clonedContent, IList<T> listItems)
        {
            if (_listItems != null)
            {
                int itemCount = _listItems.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    listItems.Add(_listItems[i].Clone());
                }

                clonedContent._listItems = listItems;
            }

            return clonedContent;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
