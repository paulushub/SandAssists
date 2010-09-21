using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the Visual Studio release target for help integration.
    /// </summary>
    public enum BuildIntegrationTarget
    {
        /// <summary>
        /// Indicates an unspecified or unknown Visual Studio release.
        /// </summary>
        None   = 0,
        /// <summary>
        /// Indicates Visual Studio 2002 as the target for help integration.
        /// </summary>
        VS2002 = 1,
        /// <summary>
        /// Indicates Visual Studio 2003 as the target for help integration.
        /// </summary>
        VS2003 = 2,
        /// <summary>
        /// Indicates Visual Studio 2005 as the target for help integration.
        /// </summary>
        VS2005 = 3,
        /// <summary>
        /// Indicates Visual Studio 2008 as the target for help integration.
        /// </summary>
        VS2008 = 4,
        /// <summary>
        /// Indicates Visual Studio 2010 as the target for help integration.
        /// </summary>
        VS2010 = 5,
    }
}
