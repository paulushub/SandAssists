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

        public static string ProgramFiles32
        {
            get      
            {
                if (IntPtr.Size == 8 || (!String.IsNullOrEmpty(
                    Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }

                return Environment.GetEnvironmentVariable("ProgramFiles");
            }
        }

        public static string ProgramFiles64
        {
            get
            {
                if (IntPtr.Size == 8 || (!String.IsNullOrEmpty(
                    Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                {
                    return Environment.GetEnvironmentVariable("ProgramFiles(x64)");
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="basePath">
        /// Contains the directory that defines the start of the relative path.
        /// </param>
        /// <param name="absolutePath">
        /// Contains the path that defines the endpoint of the relative path.
        /// </param>
        /// <returns>
        /// The relative path from the start directory to the end path.
        /// </returns>
        public static string GetRelativePath(string basePath, string absolutePath)
        {
            if (basePath != null)
            {
                basePath = basePath.Trim();
            }
            if (absolutePath != null)
            {
                absolutePath = absolutePath.Trim();
            }

            BuildExceptions.NotNullNotEmpty(basePath, "basePath");
            BuildExceptions.NotNullNotEmpty(absolutePath, "absolutePath");

            string result = String.Empty;
            if (String.IsNullOrEmpty(basePath) || String.IsNullOrEmpty(absolutePath))
            {
                return result;
            }

            basePath = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(basePath));

            StringBuilder path  = new StringBuilder(MAX_PATH);
            uint fromAttributes = DirectoryUtils.IsDirectory(basePath) ? FILE_ATTRIBUTE_DIRECTORY : 0;
            uint toAttributes   = DirectoryUtils.IsDirectory(absolutePath) ? FILE_ATTRIBUTE_DIRECTORY : 0;

            if (PathRelativePathTo(path, basePath, fromAttributes, absolutePath, toAttributes))
            {
                result = path.ToString();
            }

            return result;
        }

        public static string GetAbsolutePath(string basePath, string relativePath)
        {
            BuildExceptions.NotNullNotEmpty(basePath, "basePath");
            BuildExceptions.NotNullNotEmpty(relativePath, "relativePath");

            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            basePath = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(basePath));
            // Check the required condition, which works for all cases...
            if (!basePath.EndsWith("\\"))
            {
                basePath += "\\";
            }

            // This is the most reliable so far on tests...
            return Path.GetFullPath(Path.Combine(basePath, relativePath));
        }
    }
}
