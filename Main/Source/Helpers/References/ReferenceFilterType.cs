using System;

namespace Sandcastle.References
{
    [Serializable]
    public enum ReferenceFilterType
    {
        None      = 0,
        Member    = 1,
        Type      = 2,
        Namespace = 3,
        Root      = 4
    }
}
