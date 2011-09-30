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
    public class BuildKeyedList<T> : Collection<T>, IBuildNamedList<T>, ICloneable
         where T : class, IBuildNamedItem
    {
        #region Private Fields

        private int _version;
        private bool _suspendIndex;
        private Dictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildKeyedList{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildKeyedList{T}"/> class.
        /// </summary>
        public BuildKeyedList()
        {
            _dicItems = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildKeyedList{T}"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildKeyedList{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildKeyedList{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildKeyedList(BuildKeyedList<T> source)
        {
            BuildExceptions.NotNull(source, "source");

            _version  = source._version;
            _dicItems = source._dicItems;
        }   

        #endregion

        #region Public Events

        /// <summary>
        /// This event is raised whenever a change occurs in the 
        /// <see cref="BuildKeyedList{T}"/>.
        /// </summary>
        /// <seealso cref="BuildListEventArgs{T}"/>
        public event EventHandler<BuildListEventArgs<T>> Changed;

        #endregion

        #region Public Properties

        public T this[string itemName]
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

        /// <summary>
        /// Gets the lookup dictionary of this list.
        /// </summary>
        /// <value>
        /// The lookup dictionary of this list, if it exists; otherwise, 
        /// <see langword="null"/>.
        /// </value>
        public IDictionary<string, int> Dictionary
        {
            get
            {
                return _dicItems;
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
            if (items.Count == 1)
            {
                base.Insert(index, items[0]);
                return;
            }

            _suspendIndex = true;

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Insert(index++, items[i]);
            }

            // Recreate the index...
            _dicItems = new Dictionary<string, int>();
            for (int i = 0; i < this.Count; i++)
            {
                _dicItems[this[i].Name] = i;
            }

            _suspendIndex = false;
        }

        public void Insert(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            _suspendIndex = true;

            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    this.Insert(index++, enumerator.Current);
                }
            }

            // Recreate the index...
            _dicItems = new Dictionary<string, int>();
            for (int i = 0; i < this.Count; i++)
            {
                _dicItems[this[i].Name] = i;
            }

            _suspendIndex = false;
        }

        /// <summary>
        /// Determines whether this list contains a specified key.
        /// </summary>
        /// <param name="key">
        /// A string specifying the key to locate in the list.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if this list contains an element
        /// with the specified key; otherwise, it is <see langword="false"/>.
        /// </returns>
        public bool Contains(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }
            if (_dicItems != null && _dicItems.Count != 0)
            {
                return _dicItems.ContainsKey(key);
            }

            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="BuildKeyedList{T}">collection</see> 
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

            string itemName = newItem.Name;
            if (String.IsNullOrEmpty(itemName))
            {
                return;
            }
            int curIndex = -1;
            if (_dicItems.TryGetValue(itemName, out curIndex))
            {
                T replaced = Items[curIndex];
                base.SetItem(curIndex, newItem);

                _dicItems[itemName] = curIndex;

                if (Changed != null)
                {
                    Changed(this, new BuildListEventArgs<T>(
                        BuildListChangeType.Replaced, replaced, newItem, curIndex));
                }
            }
            else
            {
                if (_suspendIndex)
                {
                    base.InsertItem(index, newItem);
                }
                else
                {
                    if (index >= this.Count)
                    {
                        base.InsertItem(index, newItem);
                        _dicItems.Add(itemName, index);
                    }
                    else
                    {
                        base.InsertItem(index, newItem);

                        for (int i = index; i < this.Count; i++)
                        {
                            _dicItems[this[i].Name] = i;
                        }
                    }

                    _version++;

                    if (Changed != null)
                    {
                        Changed(this, new BuildListEventArgs<T>(
                            BuildListChangeType.Added, newItem, null, index));
                    }
                }
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

            string itemName = newItem.Name;
            if (String.IsNullOrEmpty(itemName))
            {
                return;
            }

            _version++;

            T replaced = Items[index];
            string replacedName = replaced.Name;
            if (!String.Equals(replacedName, itemName, 
                StringComparison.OrdinalIgnoreCase))
            {
                _dicItems.Remove(replacedName); // remove the current first
            }

            base.SetItem(index, newItem);

            _dicItems[itemName] = index;

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Replaced, replaced, newItem, index));
            }
        }

        /// <summary>
        /// This removes the element at the specified index of the 
        /// <see cref="BuildKeyedList{T}">collection</see>.
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

            // Recreate the index...
            _dicItems = new Dictionary<string, int>();
            for (int i = 0; i < this.Count; i++)
            {
                _dicItems[this[i].Name] = i;
            }

            if (Changed != null)
            {
                Changed(this, new BuildListEventArgs<T>(
                    BuildListChangeType.Removed, removedItem, null, index));
            }
        }

        /// <summary>
        /// This removes all elements from the <see cref="BuildKeyedList{T}">
        /// collection</see>. 
        /// </summary>
        protected override void ClearItems()
        {
            _version++;

            base.ClearItems();

            _dicItems.Clear();

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

        public virtual BuildKeyedList<T> Clone()
        {
            BuildKeyedList<T> clonedList = new BuildKeyedList<T>(this);

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
