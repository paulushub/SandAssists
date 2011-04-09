using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public enum ConceptualItemType
    {
        None    = 0,
        Topic   = 1,
        Related = 2,
        Marker  = 3,
        Html    = 4,
        Custom  = 5
    }
}
