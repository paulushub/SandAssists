using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the verbosity level or the level of details to show in the
    /// build logging.
    /// </summary>
    [Serializable]
    public enum BuildLoggerVerbosity
    {
        /// <summary>
        /// Indicates unknown or unspecified level of detail.
        /// </summary>
        None       = 0, 
        /// <summary>
        /// Indicates the quiet verbosity, which displays only the build summary.
        /// </summary>
        Quiet      = 1,
        /// <summary>
        /// Indicates the minimal verbosity, which displays errors, warnings, starting,
        /// ending, and a build summary.
        /// </summary>
        Minimal    = 2,
        /// <summary>
        /// Indicates the diagnostic verbosity, which displays all errors, warning,
        /// information, starting, ending, status events, and a build summary.
        /// <note type="important">
        /// The <see cref="BuildLoggerLevel.None"/> and <see cref="BuildLoggerLevel.Copyright"/>
        /// levels of information are not displayed.
        /// </note>
        /// </summary>
        Normal     = 3,
        /// <summary>
        /// Indicates the diagnostic verbosity, which displays all errors, warning,
        /// information, starting, ending, status events, and a build summary.
        /// <note type="important">
        /// The <see cref="BuildLoggerLevel.None"/> level message is not displayed, but 
        /// the <see cref="BuildLoggerLevel.Copyright"/> level message are displayed.
        /// </note>
        /// </summary>
        Detailed   = 4,
        /// <summary>
        /// Indicates the diagnostic verbosity, which displays all errors, warning,
        /// information, starting, ending, status events, and a build summary.
        /// <note type="important">
        /// The <see cref="BuildLoggerLevel.None"/> and <see cref="BuildLoggerLevel.Copyright"/>
        /// level messages are displayed.
        /// </note>
        /// </summary>
        Diagnostic = 5
    }
}
