using System;

namespace Sandcastle.Components.Codes
{                
    [Serializable]
    public enum CodeLanguage
    {
        None   = 0,
        CSharp = 1,
        VisualBasic = 2,
        ManagedCPlusPlus = 3,
        JSharp = 4,
        VBScript = 5,
        JScript = 6,
        xmlLang = 7,
        html = 8,
        visualbasicANDcsharp = 9,
        XAML = 10,
        Other = 11
    }
}
