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
        /// Indicates the total time taken to complete the process.
        /// </summary>
        Duration  = 3,
        /// <summary>
        /// Indicates the version information of the tool, where applicable.
        /// </summary>
        Version   = 4,
        /// <summary>
        /// Indicates the content of the message is copyright notice.
        /// </summary>
        Copyright = 5,
        /// <summary>
        /// Indicates an information message content or level.
        /// </summary>
        Info      = 6,
        /// <summary>
        /// Indicates a warning message content or level.
        /// </summary>
        Warn      = 7,
        /// <summary>
        /// Indicates an error message content or level. 
        /// </summary>
        Error     = 8,
    }
}
