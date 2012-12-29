// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Marshalling.cs" company="Maierhofer Software and the Hunspell Developers">
//   (c) by Maierhofer Software an the Hunspell Developers
// </copyright>
// <summary>
//   The marshal hunspell dll.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NHunspell
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The marshal hunspell dll.
    /// </summary>
    internal static class MarshalHunspellDll
    {
        #region Constants and Fields

        /// <summary>
        /// The hunspell add.
        /// </summary>
        internal static HunspellAddDelegate HunspellAdd;

        /// <summary>
        /// The hunspell add with affix.
        /// </summary>
        internal static HunspellAddWithAffixDelegate HunspellAddWithAffix;

        /// <summary>
        /// The hunspell analyze.
        /// </summary>
        internal static HunspellAnalyzeDelegate HunspellAnalyze;

        /// <summary>
        /// The hunspell free.
        /// </summary>
        internal static HunspellFreeDelegate HunspellFree;

        /// <summary>
        /// The hunspell generate.
        /// </summary>
        internal static HunspellGenerateDelegate HunspellGenerate;

        /// <summary>
        /// The hunspell init.
        /// </summary>
        internal static HunspellInitDelegate HunspellInit;

        /// <summary>
        /// The hunspell spell.
        /// </summary>
        internal static HunspellSpellDelegate HunspellSpell;

        /// <summary>
        /// The hunspell stem.
        /// </summary>
        internal static HunspellStemDelegate HunspellStem;

        /// <summary>
        /// The hunspell suggest.
        /// </summary>
        internal static HunspellSuggestDelegate HunspellSuggest;

        /// <summary>
        /// The hyphen free.
        /// </summary>
        internal static HyphenFreeDelegate HyphenFree;

        /// <summary>
        /// The hyphen hyphenate.
        /// </summary>
        internal static HyphenHyphenateDelegate HyphenHyphenate;

        /// <summary>
        /// The hyphen init.
        /// </summary>
        internal static HyphenInitDelegate HyphenInit;

        /// <summary>
        /// The dll handle.
        /// </summary>
        private static IntPtr dllHandle = IntPtr.Zero;

        /// <summary>
        /// The native dll path.
        /// </summary>
        private static string nativeDLLPath;

        #endregion

        // Hunspell
        #region Delegates

        /// <summary>
        /// The hunspell add delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate bool HunspellAddDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hunspell add with affix delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="example">
        /// The example.
        /// </param>
        internal delegate bool HunspellAddWithAffixDelegate(
            IntPtr handle, 
            [MarshalAs(UnmanagedType.LPWStr)] string word, 
            [MarshalAs(UnmanagedType.LPWStr)] string example);

        /// <summary>
        /// The hunspell analyze delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate IntPtr HunspellAnalyzeDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hunspell free delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        internal delegate void HunspellFreeDelegate(IntPtr handle);

        /// <summary>
        /// The hunspell generate delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="word2">
        /// The word 2.
        /// </param>
        internal delegate IntPtr HunspellGenerateDelegate(
            IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word, [MarshalAs(UnmanagedType.LPWStr)] string word2
            );

        /// <summary>
        /// The hunspell init delegate.
        /// </summary>
        /// <param name="affixData">
        /// The affix data.
        /// </param>
        /// <param name="affixDataSize">
        /// The affix data size.
        /// </param>
        /// <param name="dictionaryData">
        /// The dictionary data.
        /// </param>
        /// <param name="dictionaryDataSize">
        /// The dictionary data size.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        internal delegate IntPtr HunspellInitDelegate(
            [MarshalAs(UnmanagedType.LPArray)] byte[] affixData, 
            IntPtr affixDataSize, 
            [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, 
            IntPtr dictionaryDataSize, 
            string key);

        /// <summary>
        /// The hunspell spell delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate bool HunspellSpellDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hunspell stem delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate IntPtr HunspellStemDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hunspell suggest delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate IntPtr HunspellSuggestDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hyphen free delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        internal delegate void HyphenFreeDelegate(IntPtr handle);

        /// <summary>
        /// The hyphen hyphenate delegate.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        internal delegate IntPtr HyphenHyphenateDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        /// <summary>
        /// The hyphen init delegate.
        /// </summary>
        /// <param name="dictData">
        /// The dict data.
        /// </param>
        /// <param name="dictDataSize">
        /// The dict data size.
        /// </param>
        internal delegate IntPtr HyphenInitDelegate(
            [MarshalAs(UnmanagedType.LPArray)] byte[] dictData, IntPtr dictDataSize);

        #endregion

        #region Enums

        /// <summary>
        /// The processo r_ architecture.
        /// </summary>
        internal enum PROCESSOR_ARCHITECTURE : ushort
        {
            /// <summary>
            /// The intel.
            /// </summary>
            Intel = 0, 

            /// <summary>
            /// The mips.
            /// </summary>
            MIPS = 1, 

            /// <summary>
            /// The alpha.
            /// </summary>
            Alpha = 2, 

            /// <summary>
            /// The ppc.
            /// </summary>
            PPC = 3, 

            /// <summary>
            /// The shx.
            /// </summary>
            SHX = 4, 

            /// <summary>
            /// The arm.
            /// </summary>
            ARM = 5, 

            /// <summary>
            /// The i a 64.
            /// </summary>
            IA64 = 6, 

            /// <summary>
            /// The alpha 64.
            /// </summary>
            Alpha64 = 7, 

            /// <summary>
            /// The amd 64.
            /// </summary>
            Amd64 = 9, 

            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown = 0xFFFF
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets NativeDLLPath.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal static string NativeDLLPath
        {
            get
            {
                if (nativeDLLPath == null)
                {
                    nativeDLLPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, 
                        AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty);
                }

                return nativeDLLPath;
            }

            set
            {
                if (dllHandle != IntPtr.Zero)
                {
                    throw new InvalidOperationException("Native Library is already loaded");
                }

                nativeDLLPath = value;
            }
        }

        internal static string AssemblyPath
        {
            get
            {
                string assemblyPath = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                return assemblyPath;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get proc address.
        /// </summary>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <param name="procName">
        /// The proc name.
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// The get system info.
        /// </summary>
        /// <param name="lpSystemInfo">
        /// The lp system info.
        /// </param>
        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        /// <summary>
        /// The load library.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(string fileName);

        /// <summary>
        /// The load native hunspell dll.
        /// </summary>
        /// <exception cref="DllNotFoundException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        internal static void LoadNativeHunspellDll()
        {
            if (dllHandle != IntPtr.Zero)
            {
                return; // Already loaded
            }

            try
            {
                // Initialze the dynamic marshall Infrastructure to call the 32Bit (x86) or the 64Bit (x64) Dll respectively 
                var info = new SYSTEM_INFO();
                GetSystemInfo(ref info);

                // Load the correct DLL according to the processor architecture
                switch (info.wProcessorArchitecture)
                {
                    case PROCESSOR_ARCHITECTURE.Intel:
                        string pathx86 = NativeDLLPath;
                        if (pathx86 != string.Empty && !pathx86.EndsWith("\\"))
                        {
                            pathx86 += "\\";
                        }

                        pathx86 += Resources.HunspellX86DllName;

                        if (!File.Exists(pathx86))
                        {
                            pathx86 = AssemblyPath;
                            if (pathx86 != string.Empty && !pathx86.EndsWith("\\"))
                            {
                                pathx86 += "\\";
                            }

                            pathx86 += Resources.HunspellX86DllName;
                        }

                        dllHandle = LoadLibrary(pathx86);
                        if (dllHandle == IntPtr.Zero)
                        {
                            throw new DllNotFoundException(
                                string.Format(Resources.HunspellX86DllNotFoundMessage, pathx86));
                        }

                        break;

                    case PROCESSOR_ARCHITECTURE.Amd64:
                        string pathx64 = NativeDLLPath;
                        if (pathx64 != string.Empty && !pathx64.EndsWith("\\"))
                        {
                            pathx64 += "\\";
                        }

                        pathx64 += Resources.HunspellX64DllName;
                        if (!File.Exists(pathx64))
                        {
                            pathx64 = AssemblyPath;
                            if (pathx64 != string.Empty && !pathx64.EndsWith("\\"))
                            {
                                pathx64 += "\\";
                            }

                            pathx64 += Resources.HunspellX64DllName;
                        }

                        dllHandle = LoadLibrary(pathx64);
                        if (dllHandle == IntPtr.Zero)
                        {
                            throw new DllNotFoundException(
                                string.Format(Resources.HunspellX64DllNotFoundMessage, pathx64));
                        }

                        break;

                    default:
                        throw new NotSupportedException(
                            Resources.HunspellNotAvailabeForProcessorArchitectureMessage + info.wProcessorArchitecture);
                }

                HunspellInit = (HunspellInitDelegate)GetDelegate("HunspellInit", typeof(HunspellInitDelegate));
                HunspellFree = (HunspellFreeDelegate)GetDelegate("HunspellFree", typeof(HunspellFreeDelegate));

                HunspellAdd = (HunspellAddDelegate)GetDelegate("HunspellAdd", typeof(HunspellAddDelegate));
                HunspellAddWithAffix =
                    (HunspellAddWithAffixDelegate)
                    GetDelegate("HunspellAddWithAffix", typeof(HunspellAddWithAffixDelegate));

                HunspellSpell = (HunspellSpellDelegate)GetDelegate("HunspellSpell", typeof(HunspellSpellDelegate));
                HunspellSuggest =
                    (HunspellSuggestDelegate)GetDelegate("HunspellSuggest", typeof(HunspellSuggestDelegate));

                HunspellAnalyze =
                    (HunspellAnalyzeDelegate)GetDelegate("HunspellAnalyze", typeof(HunspellAnalyzeDelegate));
                HunspellStem = (HunspellStemDelegate)GetDelegate("HunspellStem", typeof(HunspellStemDelegate));
                HunspellGenerate =
                    (HunspellGenerateDelegate)GetDelegate("HunspellGenerate", typeof(HunspellGenerateDelegate));

                HyphenInit = (HyphenInitDelegate)GetDelegate("HyphenInit", typeof(HyphenInitDelegate));
                HyphenFree = (HyphenFreeDelegate)GetDelegate("HyphenFree", typeof(HyphenFreeDelegate));
                HyphenHyphenate =
                    (HyphenHyphenateDelegate)GetDelegate("HyphenHyphenate", typeof(HyphenHyphenateDelegate));
            }
            catch
            {
                if (dllHandle != IntPtr.Zero)
                {
                    FreeLibrary(dllHandle);
                }

                throw;
            }
        }

        /// <summary>
        /// The free library.
        /// </summary>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <returns>
        /// The free library.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The get delegate.
        /// </summary>
        /// <param name="procName">
        /// The proc name.
        /// </param>
        /// <param name="delegateType">
        /// The delegate type.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="EntryPointNotFoundException">
        /// </exception>
        private static Delegate GetDelegate(string procName, Type delegateType)
        {
            IntPtr procAdress = GetProcAddress(dllHandle, procName);
            if (procAdress == IntPtr.Zero)
            {
                throw new EntryPointNotFoundException("Function: " + procName);
            }

            return Marshal.GetDelegateForFunctionPointer(procAdress, delegateType);
        }

        #endregion

        /// <summary>
        /// The syste m_ info.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            /// <summary>
            /// The w processor architecture.
            /// </summary>
            internal PROCESSOR_ARCHITECTURE wProcessorArchitecture;

            /// <summary>
            /// The w reserved.
            /// </summary>
            internal ushort wReserved;

            /// <summary>
            /// The dw page size.
            /// </summary>
            internal uint dwPageSize;

            /// <summary>
            /// The lp minimum application address.
            /// </summary>
            internal IntPtr lpMinimumApplicationAddress;

            /// <summary>
            /// The lp maximum application address.
            /// </summary>
            internal IntPtr lpMaximumApplicationAddress;

            /// <summary>
            /// The dw active processor mask.
            /// </summary>
            internal IntPtr dwActiveProcessorMask;

            /// <summary>
            /// The dw number of processors.
            /// </summary>
            internal uint dwNumberOfProcessors;

            /// <summary>
            /// The dw processor type.
            /// </summary>
            internal uint dwProcessorType;

            /// <summary>
            /// The dw allocation granularity.
            /// </summary>
            internal uint dwAllocationGranularity;

            /// <summary>
            /// The dw processor level.
            /// </summary>
            internal ushort dwProcessorLevel;

            /// <summary>
            /// The dw processor revision.
            /// </summary>
            internal ushort dwProcessorRevision;
        }
    }
}