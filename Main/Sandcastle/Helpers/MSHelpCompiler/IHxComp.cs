using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sandcastle.MSHelpCompiler
{
    [ComImport, Guid("314111B5-A502-11D2-BBCA-00C04F8EC294"), TypeLibType((short) 0x10c0)]
    internal interface IHxComp
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(1)]
        void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(2)]
        int AdviseCompilerMessageCallback([In, MarshalAs(UnmanagedType.Interface)] IHxCompError pHxCompError);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(3)]
        void Compile([In, MarshalAs(UnmanagedType.BStr)] string ProjectFile, [In, MarshalAs(UnmanagedType.BStr)] string ProjectRoot, [In, MarshalAs(UnmanagedType.BStr)] string OutputFile, [In] int dwFlags);
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(4)]
        string Decompile([In, MarshalAs(UnmanagedType.BStr)] string CompiledFile, [In, MarshalAs(UnmanagedType.BStr)] string ProjectRoot, [In] int dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(5)]
        void UnadviseCompilerMessageCallback([In] int pdwCookie);
        [DispId(100)]
        short LangId { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(100)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(100)] set; }
    }
}

