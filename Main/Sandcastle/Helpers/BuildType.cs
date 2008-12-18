using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the build configuration, similar to the "Debug" and "Release"
    /// configurations in a compiled language project.
    /// </summary>
    [Serializable]
    public enum BuildType
    {
        /// <summary>
        /// This indicates unspecified or unknown configuration.
        /// </summary>
        None        = 0,
        /// <summary>
        /// This indicates a development stage configuration, equivalent to "Debug".
        /// </summary>
        Development = 1,
        /// <summary>
        /// This indicates the testing stage configuration.
        /// </summary>
        Testing     = 2,
        /// <summary>
        /// This indicates the deployment stage configuration, equivalent to "Release".
        /// </summary>
        Deployment  = 3
    }
}
