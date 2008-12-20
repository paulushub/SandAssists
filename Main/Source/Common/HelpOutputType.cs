using System;

namespace Sandcastle
{
    [Serializable]
    public enum HelpOutputType
    {
        None       = 0,
        ChmHelp    = 1,
        HxsHelp    = 2,
        HtmHelp    = 3,
        AspxHelp   = 4,
        JspHelp    = 5,
        PhpHelp    = 6,
        CustomHelp = 7
    }
}
