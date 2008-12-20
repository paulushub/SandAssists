using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the style type for the build process. These are the standard
    /// styles defined and supported by the Sandcastle help compile system.
    /// </summary>
    /// <seealso cref="BuildStyle"/>
    /// <seealso cref="BuildStyle.StyleType"/>
    [Serializable]
    public enum BuildStyleType
    {
        /// <summary>
        /// Indicates the Visual Studio 2005 and 2008 help build style.
        /// </summary>
        Vs2005    = 0,
        /// <summary>
        /// Indicates the new and experimental Havana build style.
        /// </summary>
        Hana      = 1,
        /// <summary>
        /// Indicates the prototype build style.
        /// </summary>
        Prototype = 2,
        /// <summary>
        /// Indicates the new version of the Whidbey (or VS.NET 2003) build style.
        /// </summary>
        Whidbey   = 3,
        /// <summary>
        /// Indicates a user-defined build style.
        /// </summary>
        Custom    = 4
    }
}
