﻿using System;

namespace TestLibrary
{
    [Flags]
    [Serializable]
    public enum AEnum
    {
        /// <summary>
        /// Enumeration member A.
        /// </summary>
        A = 0,
        B = 1,
        /// <summary>
        /// Enumeration member C.
        /// </summary>
        C = 2
    }
}
