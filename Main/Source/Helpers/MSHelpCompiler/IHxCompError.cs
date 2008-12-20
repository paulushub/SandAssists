using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Sandcastle.MSHelpCompiler
{
    [ComImport, TypeLibType((short) 0x10c0), Guid("314111F9-A502-11D2-BBCA-00C04F8EC294")]
    internal interface IHxCompError
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)]
        void ReportError([In, MarshalAs(UnmanagedType.BStr)] string TaskItemString, [In, MarshalAs(UnmanagedType.BStr)] string Filename, [In] int nLineNum, [In] int nCharNum, [In] HxCompErrorSeverity Severity, [In, MarshalAs(UnmanagedType.BStr)] string DescriptionString);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)]
        void ReportMessage([In] HxCompErrorSeverity Severity, [In, MarshalAs(UnmanagedType.BStr)] string DescriptionString);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)]
        HxCompStatus QueryStatus();
    }
}

