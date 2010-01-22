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
        None      = 0,
        WebHelp   = 1,
        HtmlHelp1 = 2,
        HtmlHelp2 = 3,
        HtmlHelp3 = 4,
        Custom    = 5
    }
}
