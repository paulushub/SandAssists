using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildLoggerVerbosity
    {
        Quiet      = 0,
        Minimal    = 1,
        Normal     = 2,
        Detailed   = 3,
        Diagnostic = 4
    }
}
