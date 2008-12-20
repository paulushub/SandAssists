using System;

namespace Sandcastle
{            
    /// <summary>
    /// This specifies the build output format.
    /// </summary>
    [Serializable]
    public enum BuildFormatType
    {
        /// <summary>
        /// Indicates unknown or unspecified build output format.
        /// </summary>
        None   = 0,
        Chm    = 1,
        Hxs    = 2,
        Htm    = 3,
        Custom = 4
    }
}
