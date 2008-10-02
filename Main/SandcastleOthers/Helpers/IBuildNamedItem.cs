using System;

namespace Sandcastle
{
    /// <summary>
    /// This defines a common interface for build items that must have a name or key
    /// property.
    /// </summary>
    public interface IBuildNamedItem : ICloneable
    {
        /// <summary>
        /// Gets the name or key of the build item.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name or the key.
        /// </value>
        string Name
        {
            get;
        }
    }
}
