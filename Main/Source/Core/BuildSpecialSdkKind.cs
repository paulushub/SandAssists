using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the kind of the special Microsoft <c>SDK</c>.
    /// </summary>
    [Serializable]
    public enum BuildSpecialSdkKind
    {
        /// <summary>
        /// Indicates unknown/unspecified <c>SDK</c> or library.
        /// </summary>
        None   = 0,
        /// <summary>
        /// Indicates the <c>Blend SDK</c> or library.
        /// </summary>
        Blend  = 1,
        /// <summary>
        /// Indicates the <c>ASP.NET MVC SDK</c> or library.
        /// </summary>
        WebMvc = 2
    }
}
