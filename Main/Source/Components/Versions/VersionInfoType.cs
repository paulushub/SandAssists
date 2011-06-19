using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandcastle.Components.Versions
{
    [Serializable]
    public enum VersionInfoType
    {
        None            = 0,
        Assembly        = 1,
        AssemblyAndFile = 2,
        Advanced        = 3
    }
}
