using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildSystem
    {
        None    = 0,
        Console = 1,
        MSBuild = 2,
        NAnt    = 3,
        Custom  = 4
    }
}
