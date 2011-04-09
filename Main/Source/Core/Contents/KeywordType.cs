using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// This specifies the keyword type for a help topic.
    /// </summary>
    /// <seealso cref="KeywordItem"/>
    /// <seealso cref="KeywordContent"/>
    [Serializable]
    public enum KeywordType
    {
        /// <summary>
        /// Indicates unknown or unspecified keyword type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates the normal or index keyword type.
        /// </summary>
        K    = 1,
        /// <summary>
        /// Indicates the context-sensitive or <c>F1</c> keyword type.
        /// </summary>
        F    = 2,
        /// <summary>
        /// Indicates the search window keyword type.
        /// </summary>
        S    = 3,
        /// <summary>
        /// Indicates the dynamic-link keyword type.
        /// </summary>
        B    = 4,
        /// <summary>
        /// Indicates the associated-link keyword type.
        /// </summary>
        A    = 5
    }
}
