using System;
using System.Collections.Generic;
using System.Text;

namespace Sandcastle.Items
{
    /// <summary>
    /// Indicates the type of change occurring in the <see cref="ItemList{T}"/>
    /// generic collection class and its derivatives.
    /// </summary>
    /// <seealso cref="ItemList{T}.Changed"/>
    /// <seealso cref="ItemListEventArgs{T}"/>
    [Serializable]
    public enum ItemListChangeType
    {
        /// <summary>
        /// Indicates a new item or object is added.
        /// </summary>
        Added    = 0,

        /// <summary>
        /// Indicates an existing item is removed.
        /// </summary>
        Removed  = 1,

        /// <summary>
        /// Indicates an existing item is replaced.
        /// </summary>
        Replaced = 2,

        /// <summary>
        /// Indicates the collection is cleared and all items removed.
        /// </summary>
        Cleared  = 3
    }
}
