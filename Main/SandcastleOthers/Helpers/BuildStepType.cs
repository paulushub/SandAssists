using System;

namespace Sandcastle
{
    [Serializable]
    public enum BuildStepType
    {
        None        = 0,
        CloseViewer = 1,
        StartViewer = 2,
        Assembler   = 3,
        Compilation = 4,
        Custom      = 10,
    }
}
