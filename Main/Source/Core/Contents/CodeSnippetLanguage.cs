using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public enum CodeSnippetLanguage
    {
        None                 = 0,
        CSharp               = 1,
        VisualBasic          = 2,
        VBScript             = 3,
        VisualBasicAndCSharp = 4,
        ManagedCPlusPlus     = 5,
        JSharp               = 6,
        JScript              = 7,
        XmlLang              = 8,
        Html                 = 9,
        Xaml                 = 10,
        FSharp               = 11
    }
}
