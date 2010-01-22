using System;

namespace Sandcastle
{
    /// <summary>
    /// Indicates the type of change occurring in the <see cref="BuildList{T}"/>
    /// generic collection class and its derivatives.
    /// </summary>
    /// <seealso cref="BuildList{T}.Changed"/>
    /// <seealso cref="BuildKeyedList{T}.Changed"/>
    /// <seealso cref="BuildListEventArgs{T}"/>
    [Serializable]
    public enum BuildListChangeType
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
