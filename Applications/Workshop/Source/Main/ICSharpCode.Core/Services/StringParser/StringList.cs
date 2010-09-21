using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core
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
    public sealed class StringList : Collection<string>
    {
        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="StringList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="StringList"/> class.
        /// </summary>
        public StringList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringList"/> class,
        /// with initial elements from the specified list, and acts as a wrapper 
        /// for the specified list.
        /// </summary>
        /// <param name="list">
        /// The list that is wrapped by the new collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public StringList(IList<string> list)
            : base(list)
        {
        }

        #endregion

        #region Public Events

        /// <summary>
        /// This event is raised whenever a change occurs in the 
        /// <see cref="StringList"/>.
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the elements of the specified collection to the end of this list.
        /// </summary>
        /// <param name="items">
        /// The collection whose elements should be added to the end of this list.
        /// </param>
        public void Add(IList<string> items)
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

        #endregion

        #region Protected Methods

        /// <summary>
        /// This inserts an element into the <see cref="StringList">collection</see> 
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
        protected override void InsertItem(int index, string newItem)
        {
            if (newItem == null)
            {
                return;
            }

            base.InsertItem(index, newItem);

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
        protected override void SetItem(int index, string newItem)
        {
            if (newItem == null)
            {
                return;
            }

            base.SetItem(index, newItem);

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This removes the element at the specified index of the 
        /// <see cref="StringList">collection</see>.
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
            base.RemoveItem(index);

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This removes all elements from the <see cref="StringList">
        /// collection</see>. 
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
