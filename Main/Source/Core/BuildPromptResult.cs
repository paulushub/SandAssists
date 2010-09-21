using System;

namespace Sandcastle
{
    public enum BuildPromptResult
    {
        /// <summary>
        /// The prompt returns no result.
        /// </summary>
        None   = 0,
        /// <summary>
        /// The result value of the prompt is OK.
        /// </summary>
        OK     = 1,
        /// <summary>
        /// The result value of the prompt is Cancel.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The result value of the prompt is Yes.
        /// </summary>
        Yes    = 3,
        /// <summary>
        /// The result value of the prompt is No.
        /// </summary>
        No     = 4,
        /// <summary>
        /// The result value of the prompt is Retry. 
        /// </summary>
        Retry  = 5
    }
}
