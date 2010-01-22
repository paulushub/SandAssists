using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Utilities
{
    public static class DirectoryUtils
    {      
        // String test = GetRelativePath(
        // @"E:\Source_Code\Code\ProjectsGroup1\Project1", 
        // @"E:\Source_Code\Code\ProjecstGroup2\Project2"); 
        // This will generate something like ..\..\ProjectGroup2\Project2

        public static string GetRelativePath(string mainDirPath,
            string absoluteFilePath)
        {
            string[] firstPathParts = mainDirPath.Trim(
                Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] secondPathParts = absoluteFilePath.Trim(
                Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            int sameCounter = 0;
            for (int i = 0; i < Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
            {
                if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
                {
                    break;
                } 
                sameCounter++;
            }

            if (sameCounter == 0)
            {
                return absoluteFilePath;
            }

            string newPath = String.Empty;
            for (int i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                {
                    newPath += Path.DirectorySeparatorChar;
                }
                newPath += "..";
            }
            if (newPath.Length == 0)
            {
                newPath = ".";
            }
            for (int i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }

            return newPath;
        }
    }
}
