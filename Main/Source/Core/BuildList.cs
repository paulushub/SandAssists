using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle
{
    /// <summary>
    /// This provides the base class for a generic collection with change
    /// information or events.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items or elements in the collection.
    /// </typeparam>
    /// <remarks>
    /// This provides a specialized generic collection for reference types only
    /// with supports events for changes, tracking for the modifications state 
    /// and no support for <see langword="null"/> elements.
    /// </remarks>
    /// <seealso cref="System.Collections.ObjectModel.Collection{T}"/>
    [Serializable]
    public class BuildList<T> : Collection<T>, ICloneable
         where T : class, ICloneable
    {
        #region Private Fields

        private int _version;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildList{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildList{T}"/> class.
        /// </summary>
        public BuildList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildList{T}"/> class,
        /// with initial elements from the specified list, and acts as a wrapper 
        /// for the specified list.
        /// </summary>
        /// <param name="list">
        /// The list that is wrapped by the new collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public BuildList(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildList{T}"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildList{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildList{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildList(BuildList<T> source)
        {
            BuildExceptions.NotNull(source, "source");

            _version = source._version;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// This event is raised whenever a change occurs in the 
        /// <see cref="BuildList{T}"/>.
        /// </summary>
        /// <seealso cref="BuildListEventArgs{T}"/>
        public event EventHandler<BuildListEventArgs<T>> Changed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating the current version of the collection.
        /// </summary>
        /// <value>
        /// A number specifying the current version or modified state of the
        /// collection.
        /// </value>
        /// <remarks>
        /// When newly created, the version is 0. The value of this property 
        /// changes as the collection is modified.
        /// </remarks>
        public int Version
        {
            get
            {
                return _version;
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
        public void Add(IList<T> items)
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

        public void Add(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    this.Add(enumerator.Current);
                }
            }
        }

        public void Insert(int index, IList<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Insert(index++, items[i]);
            }
        }

        public void Insert(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    this.Insert(index++, enumerator.Current);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="BuildList{T}">collection</see> 
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
        protected override void InsertItem(int index, T newItem)
        {
            BuildExceptions.NotNull(newItem, "newItem");

            _version++;

            base.InsertItem(index, newItem);

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Added, newItem, null, index));
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
        protected override void SetItem(int index, T newItem)
        {
            BuildExceptions.NotNull(newItem, "newItem");

            _version++;

            T replaced = Items[index];
            base.SetItem(index, newItem);

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Replaced, replaced, newItem, index));
            }
        }

        /// <summary>
        /// This removes the element at the specified index of the 
        /// <see cref="BuildList{T}">collection</see>.
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
            _version++;

            T removedItem = Items[index];
            base.RemoveItem(index);

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Removed, removedItem, null, index));
            }
        }

        /// <summary>
        /// This removes all elements from the <see cref="BuildList{T}">
        /// collection</see>. 
        /// </summary>
        protected override void ClearItems()
        {
            _version++;

            base.ClearItems();

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Cleared, null, null, -1));
            }
        }

        /// <summary>
        /// This raises the collection changed event with the specified event
        /// information
        /// </summary>
        /// <param name="e">
        /// The <see cref="BuildListEventArgs{T}"/> specifying the event
        /// information.
        /// </param>
        protected virtual void FireChanged(BuildListEventArgs<T> e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// This will reset the version number of the collection to zero.
        /// </summary>
        protected virtual void ResetVersion()
        {
            _version = 0;
        }

        /// <summary>
        /// This will increment the version number of the collection.
        /// </summary>
        protected virtual void UpdateVersion()
        {
            _version++;
        }

        #endregion

        #region ICloneable Members

        public BuildList<T> Clone()
        {
            BuildList<T> clonedList = new BuildList<T>(this);

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                clonedList.Add((T)this[i].Clone());
            }

            return clonedList;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
