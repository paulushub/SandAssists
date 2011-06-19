using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public enum CommentPartType
    {
        None          = 0,
        Overloads     = 1,
        Summary       = 2,
        Remarks       = 3,
        Value         = 4,
        Returns       = 5,
        Parameter     = 6,
        TypeParameter = 7,
        Example       = 8,
        Exception     = 9,
        Enumeration   = 10
    }
}
