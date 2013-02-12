using System;

namespace Sandcastle.Construction
{
    [Serializable]
    public enum ProjectElementType
    {
        None                    = 0,
        Choose                  = 1,
        Comment                 = 2,
        Extensions              = 3,
        Import                  = 4,
        ImportGroup             = 5,
        ItemDefinition          = 6,
        ItemDefinitionGroup     = 7,
        Item                    = 8,
        ItemGroup               = 9,
        Metadata                = 10,
        OnError                 = 11,
        Otherwise               = 12,
        Output                  = 13,
        Property                = 14,
        PropertyGroup           = 15,
        Root                    = 16,
        Target                  = 17,
        Task                    = 18,
        UsingTaskBody           = 19,
        UsingTask               = 20,
        UsingTaskParameter      = 21,
        When                    = 22,
        UsingTaskParameterGroup = 23
    }
}
