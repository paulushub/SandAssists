using System;

namespace Sandcastle.Configurations
{
    [Serializable]
    [Flags]
    public enum ConfigSourceType
    {
        None       = 0,
        Reference  = 1,
        Conceptual = 2,
        XmlSchema  = 4
    }
}
