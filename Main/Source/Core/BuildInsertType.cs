using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies how a build object action is inserted into another action.
    /// </summary>
    public enum BuildInsertType
    {
        /// <summary>
        /// Indicates an unknown, unspecified or default insertion action.
        /// </summary>
        None    = 0,
        /// <summary>
        /// Indicates an action or execution before the target action.
        /// </summary>
        Before  = 1,
        /// <summary>
        /// Indicates an action or execution after the target action.
        /// </summary>
        After   = 2,
        /// <summary>
        /// Indicates an action or execution instead the target action. The
        /// target action or execution is replaced by the this.
        /// </summary>
        Replace = 3
    }
}
