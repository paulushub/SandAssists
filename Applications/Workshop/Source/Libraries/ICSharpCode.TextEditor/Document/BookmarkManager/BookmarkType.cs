using System;

namespace ICSharpCode.TextEditor.Document
{
    [Serializable]
    public enum BookmarkType
    {
        None         = 0,
        TextMark     = 1,
        BreakMark    = 2,
        ClassMark    = 3,
        MethodMark   = 4,
        PropertyMark = 5,
        FieldMark    = 6,
        EventMark    = 7,
        CustomMark   = 8
    }
}
