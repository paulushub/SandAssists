using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildState
    {
        None      = 0,
        Started   = 1,
        Running   = 2,
        Finished  = 3,
        Error     = 4,
        Cancelled = 5
    }
}
