using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ICSharpCode.Core.WinForms
{
    /// <summary>
    /// This is used to get the icons that are associated with files.
    /// </summary>
    /// <remarks>
    /// See Q319350 in the MSDN. 
    /// </remarks>
    public static class IconExtractor
    {
        // Constants that we need in the function call
        private const int SHGFI_ICON      = 0x100;
        private const int SHGFI_SMALLICON = 0x1; // 'Small icon
        private const int SHGFI_LARGEICON = 0x0; // 'Large icon

        // This structure will contain information about the file
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            // Handle to the icon representing the file
            public IntPtr hIcon;
            // Index of the icon within the image list
            public int iIcon;
            // Various attributes of the file
            public uint dwAttributes;
            // Path to the file
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szDisplayName;
            // File type
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        // The signature of SHGetFileInfo (located in Shell32.dll)
        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        public static Icon Extract(string filePath, bool largeIcon)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return null;
            }

            SHFILEINFO shinfo = new SHFILEINFO();

            if (largeIcon)
            {
                // Get a handle to the large icon
                IntPtr hImgLarge = SHGetFileInfo(filePath, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                if (hImgLarge == IntPtr.Zero)
                {
                    return null;
                }
                // Get the large icon from the handle
                return Icon.FromHandle(shinfo.hIcon);
            }
            else
            {
                // Get a handle to the small icon
                IntPtr hImgSmall = SHGetFileInfo(filePath, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                if (hImgSmall == IntPtr.Zero)
                {
                    return null;
                }
               // Get the small icon from the handle
               return Icon.FromHandle(shinfo.hIcon);
            }
        }
    }
}
