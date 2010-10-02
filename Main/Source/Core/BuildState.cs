using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies or indicates the current state of the build process.
    /// </summary>
    [Serializable]
    public enum BuildState
    {
        /// <summary>
        /// Indicates an unknown or unspecified state of the build process.
        /// </summary>
        None      = 0,
        /// <summary>
        /// Indicates the build process is started or starting.
        /// </summary>
        Started   = 1,
        /// <summary>
        /// Indicates the build process is currently running.
        /// </summary>
        Running   = 2,
        /// <summary>
        /// Indicates the build process is completed.
        /// </summary>
        Finished  = 3,
        /// <summary>
        /// Indicates an error occurs in the build process.
        /// </summary>
        Error     = 4,
        /// <summary>
        /// Indicates the build process is cancelled.
        /// </summary>
        Cancelled = 5
    }
}
