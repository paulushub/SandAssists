using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.Utilities
{
    public static class PathUtils
    {
        private const int MAX_PATH = 260;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        private static extern bool PathRelativePathTo(
            [MarshalAsAttribute(UnmanagedType.LPWStr)] StringBuilder pszPath,
            [MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszFrom,
            uint dwAttrFrom,
            [MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszTo, uint dwAttrTo);

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="from">Contains the directory that defines the start of 
        /// the relative path.</param>
        /// <param name="to">Contains the path that defines the endpoint of the 
        /// relative path.</param>
        /// <returns>The relative path from the start directory to the end 
        /// path.</returns>
        public static string GetRelativePath(string from, string to)
        {
            if (from != null)
            {
                from = from.Trim();
            }
            if (to != null)
            {
                to = to.Trim();
            }

            string result = String.Empty;
            if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(to))
            {
                return result;
            }

            StringBuilder path  = new StringBuilder(MAX_PATH);
            uint fromAttributes = DirectoryUtils.IsDirectory(from) ? FILE_ATTRIBUTE_DIRECTORY : 0;
            uint toAttributes   = DirectoryUtils.IsDirectory(to) ? FILE_ATTRIBUTE_DIRECTORY : 0;

            if (PathRelativePathTo(path, from, fromAttributes, to, toAttributes))
            {
                result = path.ToString();
            }

            return result;
        }
    }
}
