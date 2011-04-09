using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// This specifies the shape of an image region.
    /// </summary>
    [Serializable]
    public enum MediaImageAreaShape
    {
        /// <summary>
        /// Specifies the entire region.
        /// </summary>
        Default   = 0,
        /// <summary>
        /// Defines a rectangular region.
        /// </summary>
        Rectangle = 1,
        /// <summary>
        /// Defines a circular region.
        /// </summary>
        Circle    = 2,
        /// <summary>
        /// Defines a polygonal region.
        /// </summary>
        Polygon   = 3
    }
}
