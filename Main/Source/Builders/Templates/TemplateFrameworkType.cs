using System;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public enum TemplateFrameworkType
    {
        None     = 0,
        DotNet10 = 1,
        DotNet11 = 2,
        DotNet20 = 3,
        DotNet30 = 4,
        DotNet35 = 5,
        DotNet40 = 6
    }
}
