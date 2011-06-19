using System;

namespace ConsoleSample
{
    /// <summary>
    /// Specifies the options for the API reference help table of contents.
    /// </summary>
    enum ReferencesTocType
    {
        /// <summary>
        /// Builds a sample for testing reference documentations
        /// </summary>
        Flat         = 0,
        /// <summary>
        /// Builds a sample for testing reference documentations with
        /// hierarchical table of contents.
        /// </summary>
        Hierarchical = 1,
    }
}
