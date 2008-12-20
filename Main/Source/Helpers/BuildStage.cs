using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies a stage in the build process, and may be equivalent to single
    /// or multiple build steps.
    /// </summary>
    [Serializable]
    public enum BuildStage
    {
        /// <summary>
        /// Indicates an unknown or unspecified build stage.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Indicates closing a help viewer, usually the Help 1.x and Help 2.x viewers.
        /// </summary>
        CloseViewer = 1,
        /// <summary>
        /// Indicates starting a help viewer, usually the Help 1.x and Help 2.x viewers.
        /// </summary>
        StartViewer = 2,
        /// <summary>
        /// Indicates running the build assembler tool.
        /// </summary>
        Assembler   = 3,
        /// <summary>
        /// Indicates compiling the help, with either the Help 1.x or Help 2.x compilers.
        /// </summary>
        Compilation = 4,
        /// <summary>
        /// Indicates a custom or user-defined build stage.
        /// </summary>
        Custom      = 5
    }
}
