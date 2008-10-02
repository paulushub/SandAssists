using System;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum BuildLoggerLevel
    {
        /// <summary>
        /// 
        /// </summary>
        None      = 0,
        /// <summary>
        /// 
        /// </summary>
        Started   = 1,
        /// <summary>
        /// 
        /// </summary>
        Ended     = 2,
        /// <summary>
        /// 
        /// </summary>
        Copyright = 3,
        /// <summary>
        /// 
        /// </summary>
        Info      = 4,
        /// <summary>
        /// 
        /// </summary>
        Warn      = 5,
        /// <summary>
        /// 
        /// </summary>
        Error     = 6,
    }
}
