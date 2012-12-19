using System;

namespace Sandcastle.ReflectionData
{
    [Serializable]
    public enum DataSourceType
    {
        None        = 0,
        Blend       = 1,
        Portable    = 2,
        Framework   = 3,
        Silverlight = 4,
        ScriptSharp = 5,
        Compact     = 6,
        FSharp      = 7
    }
}
