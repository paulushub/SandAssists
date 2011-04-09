using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the build group, which is a categorization of the
    /// build contents.
    /// </summary>
    /// <seealso cref="TocContent"/>
    [Serializable]
    public enum BuildTocInfoType
    {
        /// <summary>
        /// Indicates an unknown or unspecified build group.
        /// </summary>
        None       = 0,
        /// <summary>
        /// Indicates a topic in a conceptual group.
        /// </summary>
        Topic      = 1,
        /// <summary>
        /// Indicates a reference or application programming interface (API) 
        /// build group.
        /// </summary>
        Reference  = 2,
        /// <summary>
        /// Indicates the conceptual topics build group.
        /// </summary>
        Conceptual = 3
    }
}
