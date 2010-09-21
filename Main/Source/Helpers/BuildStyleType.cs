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
        ClassicWhite  = 0,
        /// <summary>
        /// Indicates the new version of the Whidbey (or VS.NET 2003) build style.
        /// </summary>
        ClassicBlue   = 1,
        /// <summary>
        /// Indicates the lightweight build style used by the Visual Studio 2010.
        /// </summary>
        Lightweight   = 2,
        /// <summary>
        /// indicates the script free build style used by the Visual Studio 2010.
        /// </summary>
        ScriptFree    = 3,
        /// <summary>
        /// Indicates a user-defined build style.
        /// </summary>
        Custom        = 4
    }
}
