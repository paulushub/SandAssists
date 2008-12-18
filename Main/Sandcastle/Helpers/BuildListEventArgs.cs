using System;

namespace Sandcastle
{
    /// <summary>
    /// This is an event argument used to store information for the changed event 
    /// in the generic collection, <see cref="BuildList{T}"/>, and its derivatives.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element or objects contained in the collection.
    /// </typeparam>
    /// <seealso cref="BuildListChangeType"/>
    /// <seealso cref="BuildList{T}"/>
    /// <seealso cref="BuildKeyedList{T}"/>
    [Serializable]
    public class BuildListEventArgs<T> : EventArgs
        where T : class
    {
        #region Private Fields

        private int _index;
        private T _changedItem;
        private T _replacedWith;
        private string _name;
        private BuildListChangeType _changeType;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListEventArgs{T}"/>
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
        public BuildListEventArgs(BuildListChangeType change,
            T item, T replacement, int index)
        {
            _index        = index;
            _changeType   = change;
            _changedItem  = item;
            _replacedWith = replacement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListEventArgs{T}"/>
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
        public BuildListEventArgs(BuildListChangeType change,
            T item, T replacement, int index, string name)
            : this(change, item, replacement, index)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
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
        /// Gets the name of the item in the change, where applicable.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the item, where
        /// applicable; otherwise it is <see langword="null"/>.
        /// </value>
        public string Name
        {
            get
            {
                return _name;
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
        /// An enumeration of the type <see cref="BuildListChangeType"/>
        /// specifying the type of the change.
        /// </value>
        public BuildListChangeType ChangeType
        {
            get
            {
                return _changeType;
            }
        }

        #endregion
    }
}
