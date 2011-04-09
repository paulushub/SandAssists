using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public enum TocItemSourceType
    {
        None          = 0,
        Topic         = 1,
        Group         = 2,
        Namespace     = 3,
        NamespaceRoot = 4
    }
}
