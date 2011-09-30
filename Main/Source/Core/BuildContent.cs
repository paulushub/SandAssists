using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

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
    /// <remarks>
    /// <para>
    /// The <c>Sandcastle Assist</c> framework provides a number of objects
    /// for defining various documentation information items. This provides
    /// the corresponding container for such items.
    /// </para>
    /// <para>
    /// Most of these items and containers are defined in the <see cref="Sandcastle.Contents"/>
    /// namespace.
    /// </para>
    /// <para>
    /// Containers are created with either the <see cref="BuildList{T}"/> for
    /// non-keyed items or the <see cref="BuildKeyedList{T}"/> for keyed items.
    /// </para>
    /// <para>
    /// The keyed containers provide an additional indexer for accessing the
    /// items by the string keys or names.
    /// </para>
    /// <note type="caution">
    /// <para>
    /// Some containers are backed by a file, and to conserve computer resources,
    /// the items may not be loaded into memory until requested.
    /// </para>
    /// <para>
    /// Such containers provide <c>IsLoaded</c> property to make it easier to
    /// check that state of the container.
    /// </para>
    /// <para>
    /// A reliable way of checking whether a container is empty or not, is 
    /// using the <see cref="IsEmpty"/> property instead of the 
    /// <see cref="Count"/> property.
    /// </para>
    /// </note>
    /// </remarks>
    /// <seealso cref="BuildItem{T}"/>
    /// <seealso cref="IBuildContent"/>
    /// <seealso cref="Sandcastle.Contents"/>
    [Serializable]
    public abstract class BuildContent<T, U> : BuildObject, IBuildContent
        where T : BuildItem<T>
        where U : BuildContent<T, U>        
    {
        #region Private Fields

        private bool     _isModified;
        private bool?    _isInitialized;
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

            _listItems     = itemList;
            _isInitialized = null;
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

            _listItems     = source._listItems;
            _isModified    = source._isModified;
            _isInitialized = source._isInitialized;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when the value of the <see cref="Modified"/> property 
        /// has changed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event is raised if the <see cref="Modified"/> property is changed by 
        /// either a programmatic modification or user interaction. 
        /// </para>
        /// <para>
        /// This event is only raised after the initialization of this content.
        /// </para>
        /// </remarks>
        /// <seealso cref="Modified"/>
        /// <seealso cref="OnModifiedChanged()"/>
        public event EventHandler ModifiedChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of items in this content.
        /// </summary>
        /// <value>
        /// The number of items contained in this content.
        /// </value>
        /// <remarks>
        /// <para>
        /// A value of <c>0</c> is not necessarily an indication of an empty
        /// container, since some contents are backed by files and may not 
        /// load their items until requested.
        /// </para>
        /// <para>
        /// Checking the <see cref="IsEmpty"/> property value will be a better
        /// way of testing if the container is empty. The implementation of the
        /// <see cref="IsEmpty"/> property depends on the container.
        /// </para>
        /// <para>
        /// This is done to improve speed and save computer memory or resources.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsEmpty"/>
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

        /// <summary>
        /// Gets the item at the specified index. 
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <value>
        /// The item, <see cref="BuildItem{T}"/>, at the specified index.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than <c>0</c>.
        /// <para>-or-</para>
        /// If the <paramref name="index"/> is equal to or greater than the
        /// <see cref="Count"/>.
        /// </exception>
        public T this[int index]
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

        /// <summary>
        /// Gets a value indicating whether this container is empty.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this container is empty;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// For most containers, this simply checks whether the <see cref="Count"/>
        /// property value is <c>0</c>.
        /// </para>
        /// <para>
        /// For file-based containers, a further check is required to indicate
        /// whether the container list is actually empty, since the file may
        /// not be loaded to conserve computer resources.
        /// </para>
        /// </remarks>
        public virtual bool IsEmpty
        {
            get
            {
                return (_listItems == null || _listItems.Count == 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this container is keyed, with each
        /// item having a unique name or identifier.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this container is keyed;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The items, <seealso cref="BuildItem{T}"/>, of keyed containers
        /// implement the <see cref="IBuildNamedItem"/> to provide unique name
        /// or identifiers. The actual property name defining the unique name
        /// depends on the item.
        /// </remarks>
        public virtual bool IsKeyed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this container is hierarchical.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this container is 
        /// hierarchical; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The hierarchical items and container define a tree structure
        /// documentation information model.
        /// </remarks>
        public virtual bool IsHierarchical
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that exposes an enumerator, which supports a simple
        /// iteration over the collection of the items of in this container.
        /// </summary>
        /// <value>
        /// An object providing a simple iteration over the collection of the
        /// items in this container.
        /// </value>
        /// <typeparam name="T">
        /// The type of the items in this container.
        /// </typeparam>
        public IEnumerable<T> Items
        {
            get
            {
                return _listItems;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// </value>
        public abstract string XmlTagName
        {
            get;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the list of items in this container.
        /// </summary>
        /// <value>
        /// An object providing a simple iteration over the collection of the
        /// items in this container.
        /// </value>
        /// <typeparam name="T">
        /// The type of the items in this container.
        /// </typeparam>
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

        /// <summary>
        /// Raises the <see cref="ModifiedChanged"/> event, whenever the state
        /// of this content is changed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// As a <c>.NET</c> framework feature, raising an event invokes the 
        /// event handler through a delegate.
        /// </para>
        /// <para>
        /// This method allows the derived classes to handle the event without
        /// attaching the delegate. The derived classes can then perform custom
        /// actions before and/or after the event.
        /// </para>
        /// <note type="inheritinfo">
        /// When overriding this method in a derived class, be sure to call 
        /// the base class's method so that registered delegates receive 
        /// the event.
        /// </note>
        /// </remarks>
        /// <seealso cref="Modified"/>
        /// <seealso cref="ModifiedChanged"/>
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

        /// <summary>
        /// 
        /// </summary>
        public bool? IsInitialized
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

        /// <summary>
        /// Gets or sets a value that indicates that the build content has 
        /// been modified by the user since the object was created or 
        /// its contents were last set.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the object's contents 
        /// have been modified; otherwise, <see langword="false"/>. The 
        /// default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is set to <see langword="false"/> when the initialization of
        /// this object is completed.
        /// </para>
        /// <para>
        /// If this object is already initialized, the <see cref="ModifiedChanged"/>
        /// event is raised when this property changes.
        /// </para>
        /// </remarks>
        /// <seealso cref="ModifiedChanged"/>
        /// <seealso cref="OnModifiedChanged()"/>
        public bool Modified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = value;

                if (_isInitialized == null ||
                    (_isInitialized != null && _isInitialized.Value))
                {
                    this.OnModifiedChanged();
                } 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public virtual void ItemModified(IBuildItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_isInitialized != null && !_isInitialized.Value)
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
        /// An <see cref="XmlSchema"/> that describes the <c>XML</c> representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

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
        public virtual void ReadXml(XmlReader reader)
        {
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
