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
    public class BuildKeyedList<T> : Collection<T>, ICloneable
         where T : class, IBuildNamedItem
    {
        #region Private Fields

        private int _version;
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
                StringComparer.CurrentCultureIgnoreCase);
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
                base.InsertItem(index, newItem);
                _dicItems.Add(itemName, index);

                _version++;

                if (Changed != null)
                {
                    Changed(this, new BuildListEventArgs<T>(
                        BuildListChangeType.Added, newItem, null, index));
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
                StringComparison.CurrentCultureIgnoreCase))
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

            _dicItems.Remove(removedItem.Name);

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

        public BuildKeyedList<T> Clone()
        {
            BuildKeyedList<T> clonedList = new BuildKeyedList<T>();

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                clonedList.Add((T)this[i].Clone());
            }

            return clonedList;
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
