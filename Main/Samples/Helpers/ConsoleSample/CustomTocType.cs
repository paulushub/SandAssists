using System;

namespace ConsoleSample
{
    /// <summary>
    /// Specifies the option for the documentation table of contents.
    /// </summary>
    enum CustomTocType
    {
        /// <summary>
        /// No custom TOC is applied.
        /// </summary>
        None          = 0,
        /// <summary>
        /// Applies a TOC that looks similar to the default layout.
        /// </summary>
        Default       = 1,
        /// <summary>
        /// Simple: If there is a reference root container, uses it as 
        /// the root for all.
        /// </summary>
        ReferenceRoot = 2,
        /// <summary>
        /// Indicates a specified topic at the "Root" of the documentation.
        /// </summary>
        TopicRoot     = 3
    }
}
