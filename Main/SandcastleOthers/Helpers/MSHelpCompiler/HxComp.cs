using System;
using System.Runtime.InteropServices;

namespace Sandcastle.MSHelpCompiler
{
    [ComImport, CoClass(typeof(HxCompClass)), Guid("314111B5-A502-11D2-BBCA-00C04F8EC294")]
    internal interface HxComp : IHxComp
    {
    }
}

