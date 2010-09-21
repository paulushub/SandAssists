using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildConfigurationType
    {
        None             = 0,
        Sandcastle       = 1,
        SandcastleAssist = 2,
        Custom           = 3
    }
}
