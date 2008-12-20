using System;
using System.Collections.Generic;

namespace Sandcastle.Items
{
    /// <summary>
    /// This is an event argument used to store information for the changed event 
    /// in the generic collection, <see cref="ItemList{T}"/>, and its derivatives.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element or objects contained in the collection.
    /// </typeparam>
    /// <seealso cref="ItemListChangeType"/>
    /// <seealso cref="ItemList{T}"/>
    [Serializable]
    public class ItemListEventArgs<T> : EventArgs
        where T : class
    {
        #region Private Fields

        private int _index;
        private T _changedItem;
        private T _replacedWith;
        private ItemListChangeType _changeType;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemListEventArgs{T}"/>
        /// class with the given parameters.
        /// </summary>
        /// <param name="change">The type of the change.</param>
        /// <param name="item">The item affected by the change.</param>
        /// <param name="replacement">
        /// A replacement for the affected item, where available or applicable.
        /// </param>
        /// <param name="index">
        /// The zero-based index at which the change is occurring.
        /// </param>
        public ItemListEventArgs(ItemListChangeType change,
            T item, T replacement, int index)
        {
            _index = index;
            _changeType = change;
            _changedItem = item;
            _replacedWith = replacement;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the zero-based index at which the change is occurring.
        /// </summary>
        /// <value>
        /// A number specifying the index at which to insert or remove at item.
        /// </value>
        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// Gets the item inserted or added.
        /// </summary>
        /// <value>
        /// The object inserted or added, with the type depending on 
        /// the collection or <see langword="null"/>, if no object is 
        /// inserted or added to the collection.
        /// </value>
        public T ChangedItem
        {
            get
            {
                return _changedItem;
            }
        }

        /// <summary>
        /// Gets the new value for the item.
        /// </summary>
        /// <value>
        /// The new value of the item or <see langword="null"/> if not applicable
        /// to the change.
        /// </value>
        public T ReplacedWith
        {
            get
            {
                return _replacedWith;
            }
        }

        /// <summary>
        /// Gets the type of the change.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ItemListChangeType"/>
        /// specifying the type of the change.
        /// </value>
        public ItemListChangeType ChangeType
        {
            get
            {
                return _changeType;
            }
        }

        #endregion
    }
}
