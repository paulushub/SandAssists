using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// This specifies equation or mathematic formula image file naming method.
    /// </summary>
    [Serializable]
    public enum MathNamingMethod
    {
        /// <summary>
        /// Indicates that equation image file names are generated as 
        /// globally unique identifier, <see cref="Guid"/>, values.
        /// </summary>
        Guid       = 0,
        /// <summary>
        /// Indicates that equation image file names are generated as random file
        /// names, using <see cref="System.IO.Path.GetRandomFileName"/>.
        /// </summary>
        Random     = 1,
        /// <summary>
        /// Indicates that equation image file names are generated sequentially 
        /// as the appear in the documentation process, such as <c>math0001.png</c>,
        /// <c>math0002.png</c>, etc.
        /// </summary>
        Sequential = 2        
    }
}
