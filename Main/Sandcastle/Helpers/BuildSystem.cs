using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the build system or environment under which the
    /// current process is running.
    /// </summary>
    [Serializable]
    public enum BuildSystem
    {
        /// <summary>
        /// Indicates unknown or unspecified build system.
        /// </summary>
        None    = 0,
        /// <summary>
        /// Indicates a console build system.
        /// </summary>
        Console = 1,
        /// <summary>
        /// Indicates an MSBuild build system.
        /// </summary>
        MSBuild = 2,
        /// <summary>
        /// Indicates an NAnt build system.
        /// </summary>
        NAnt    = 3,
        /// <summary>
        /// Indicates a custom build system.
        /// </summary>
        Custom  = 4
    }
}
