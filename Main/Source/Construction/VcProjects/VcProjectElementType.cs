using System;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public enum VcProjectElementType
    {
        None              = 0,
        Root              = 1,
        Platforms         = 2,
        Platform          = 3,
        ToolFiles         = 4,
        ToolFile          = 5,
        DefaultToolFile   = 6,
        PublishingData    = 7,
        PublishingItem    = 8,
        Configurations    = 9,
        Configuration     = 10,
        References        = 11,
        ActiveXReference  = 12,
        AssemblyReference = 13,
        ProjectReference  = 14,
        Files             = 15,
        File              = 16,
        Filter            = 17,
        Globals           = 18,
        Global            = 19,
        Tool              = 20,
        DeploymentTool    = 21,
        DebuggerTool      = 22,
        Comment           = 23
    }
}
