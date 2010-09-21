using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.Utilities
{
    public static class DirectoryUtils
    {
        private const int MAX_PATH = 260;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        private static extern bool PathIsDirectoryEmpty(
            [MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszPath);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        private static extern bool PathIsDirectory(
            [MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszPath);

        /// <summary>
        /// Verifies that a path is a valid directory.
        /// </summary>
        /// <param name="path">The path to verify.</param>
        /// <returns><see langword="true"/> if the path is a valid directory; 
        /// otherwise, <see langword="false"/>.</returns>
        public static bool IsDirectory(string path)
        {
            return PathIsDirectory(path);
        }

        public static bool IsDirectoryEmpty(string directory)
        {      
            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                return false;
            }

            return PathIsDirectoryEmpty(directory);
        }

        public static void DeleteDirectory(string directoryPath, bool recursive)
        {
            if (String.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            if (dirInfo.Exists)
            {
                // It is a directory...
                try
                {
                    dirInfo.Attributes = FileAttributes.Normal;
                    dirInfo.Delete(recursive);
                }
                catch (UnauthorizedAccessException)
                {
                    // One possible cause of this is read-only file, so first
                    // try another method of deleting the directory...
                    foreach (string file in PathSearch.FindFiles(dirInfo, "*.*",
                        recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }

                    dirInfo.Delete(recursive);
                }
            }
        }
    }
}
