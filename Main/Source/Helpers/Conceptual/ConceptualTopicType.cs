using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public enum ConceptualTopicType
    {
        None                         = 0,
        Conceptual                   = 1,
        ErrorMessage                 = 2,
        Glossary                     = 3,
        HowTo                        = 4,
        Orientation                  = 5,
        CodeEntity                   = 6,
        ReferenceWithSyntax          = 7,
        ReferenceWithoutSyntax       = 8,
        Sample                       = 9,
        Troubleshooting              = 10,
        UIReference                  = 11,
        Walkthrough                  = 12,
        WhitePaper                   = 13,
        XmlReference                 = 14,
        SDKTechOverviewArchitecture  = 15,
        SDKTechOverviewCodeDirectory = 16,
        SDKTechOverviewOrientation   = 17,
        SDKTechOverviewScenarios     = 18,
        SDKTechOverviewTechSummary   = 19,
    }
}
