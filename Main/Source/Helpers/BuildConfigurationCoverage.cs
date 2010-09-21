using System;

namespace Sandcastle
{
    [Flags]
    [Serializable]
    public enum BuildConfigurationCoverage
    {
        None    = 0,
        Include = 1,
        Full    = 2,
        Both    = Include | Full
    }
}
