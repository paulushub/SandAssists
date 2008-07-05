using System;

namespace Sandcastle.Configurations
{
    [Serializable]
    [Flags]
    public enum ConfigTargetType
    {
        None        = 0,
        HtmlHelp    = 1,
        MsdnHelp    = 2,
        WebHtml     = 4,
        WebAsp      = 8,
        WebAjax     = 16,
        WebAspAjax  = 32,
        WebServices = 64,
    }
}
