using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildComponentType
    {
        None             = 0,
        Sandcastle       = 1,
        SandcastleAssist = 2,
        Custom           = 3
    }
}
