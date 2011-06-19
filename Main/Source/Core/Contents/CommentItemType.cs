using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public enum CommentItemType
    {
        None       = 0,
        Project    = 1,
        Namespace  = 2,
        ClassType  = 3,
        StructType = 4,
        EnumType   = 5,
        Method     = 6,
        Property   = 7,
        Field      = 8,
        Event      = 9,
        Delegate   = 10
    }
}
