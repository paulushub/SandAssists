using System;

namespace Sandcastle.References
{
    [Serializable]
    public enum ReferenceVersionType
    {
        None            = 0,
        Assembly        = 1,
        AssemblyAndFile = 2,
        Advanced        = 3
    }
}
