using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all the build contents, which
    /// are containers for the build items, <see cref="BuildItem{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The content item, <see cref="BuildItem{T}"/>, which is the basic unit of
    /// the contents.
    /// </typeparam>
    /// <typeparam name="U">
    /// The underlying value type of the <see cref="BuildContent{T, U}"/> generic type. 
    /// </typeparam>
    [Serializable]
    public abstract class BuildContent<T, U> : BuildObject, IBuildContent
        where T : BuildItem<T>
        where U : BuildContent<T, U>        
    {
        #region Private Fields

        private bool     _isModified;
        private bool     _isInitialized;
        private IList<T> _listItems;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildContent{T, U}"/> 
        /// class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContent{T, U}"/> 
        /// class with the default parameters.
        /// </summary>
        protected BuildContent()
            : this(new BuildList<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContent{T, U}"/> 
        /// class with the specified list of items.
        /// </summary>
        /// <param name="itemList">
        /// An initial list of content items of the type, <see cref="BuildItem{T}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="itemList"/> is <see langword="null"/>.
        /// </exception>
        protected BuildContent(IList<T> itemList)
        {                                          
            BuildExceptions.NotNull(itemList, "itemList");

            _listItems = itemList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContent{T, U}"/>
        /// class with initial parameters copied from the specified instance of 
        /// the specified <see cref="BuildContent{T, U}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildContent{T, U}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildContent(BuildContent<T, U> source)
        {
            BuildExceptions.NotNull(source, "source");

            _listItems = source._listItems;
        }

        #endregion

        #region Public Events

        public event EventHandler ModifiedChanged;

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
                return _listItems;
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

        public virtual bool IsHierarchical
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

            item.Content = this;

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

        public virtual void Insert(int index, T item)
        {
            BuildExceptions.NotNull(item, "item");

            item.Content = this;

            _listItems.Insert(index, item);
        }

        public virtual void Remove(int index)
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(index);
        }

        public virtual bool Remove(T item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems.Count == 0)
            {
                return false;
            }

            return _listItems.Remove(item);
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

        #region Protected Methods

        protected virtual void OnModifiedChanged()
        {
            EventHandler handler = this.ModifiedChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IBuildContent Members

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public bool Modified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = value;

                if (_isInitialized)
                {
                    this.OnModifiedChanged();
                } 
            }
        }

        public virtual void ItemModified(IBuildItem item)
        {
            if (!_isInitialized)
            {
                return;
            }

            _isModified = true;

            this.OnModifiedChanged();
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the XML representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

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
        public virtual void ReadXml(XmlReader reader)
        {
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
                BuildList<T> listItems = new BuildList<T>(new List<T>(itemCount));

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

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            _isInitialized = false;
        }

        public virtual void EndInit()
        {
            _isInitialized = true;
            _isModified    = false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
