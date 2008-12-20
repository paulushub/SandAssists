using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the level of importance of a build log message.
    /// </summary>
    /// <seealso cref="BuildLogger"/>
    [Serializable]
    public enum BuildLoggerLevel
    {
        /// <summary>
        /// Indicates an unknown or unspecified message level.
        /// </summary>
        None      = 0,
        /// <summary>
        /// Indicates an operation or process is started.
        /// </summary>
        Started   = 1,
        /// <summary>
        /// Indicates an operation or process is ended.
        /// </summary>
        Ended     = 2,
        /// <summary>
        /// Indicates the content of the message is copyright notice.
        /// </summary>
        Copyright = 3,
        /// <summary>
        /// Indicates an information message content or level.
        /// </summary>
        Info      = 4,
        /// <summary>
        /// Indicates a warning message content or level.
        /// </summary>
        Warn      = 5,
        /// <summary>
        /// Indicates an error message content or level. 
        /// </summary>
        Error     = 6,
    }
}
