using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// This specifies the type of the build group, which is a categorization of the
    /// build contents.
    /// </summary>
    /// <seealso cref="TocContent"/>
    [Serializable]
    public enum TocGroupType
    {
        /// <summary>
        /// Indicates an unknown or unspecified build group.
        /// </summary>
        None       = 0,
        /// <summary>
        /// Indicates a reference or application programming interface (API) 
        /// build group.
        /// </summary>
        Reference  = 1,
        /// <summary>
        /// Indicates the conceptual topics build group.
        /// </summary>
        Conceptual = 2
    }
}
