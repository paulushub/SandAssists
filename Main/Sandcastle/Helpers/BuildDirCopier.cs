using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Sandcastle
{
    /// <summary>
    /// Copies a file or a directory and its contents to a new location. 
    /// </summary>
    [Serializable]
    public sealed class BuildDirCopier : BuildObject<BuildDirCopier>
    {
        #region Private Fields

        private int  _copiedCount;
        private bool _isOverwrite;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// 
        /// </overloads>
        /// <summary>
        /// 
        /// </summary>
        public BuildDirCopier()
        {
            _isOverwrite = true;
            _isRecursive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public BuildDirCopier(BuildDirCopier source)
            : base(source)
        {
            _copiedCount     = source._copiedCount;
            _isOverwrite     = source._isOverwrite;
            _isRecursive     = source._isRecursive;
            _includeHidden   = source._includeHidden;
            _includeSecurity = source._includeSecurity;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Recursive
        {
            get
            {
                return _isRecursive;
            }
            set
            {
                _isRecursive = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Overwrite
        {
            get
            {
                return _isOverwrite;
            }
            set
            {
                _isOverwrite = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool IncludeSecurity
        {
            get
            {
                return _includeSecurity;
            }
            set
            {
                _includeSecurity = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool IncludeHidden
        {
            get
            {
                return _includeHidden;
            }
            set
            {
                _includeHidden = value;
            }
        }

        #endregion

        #region Public Methods
        
        /// <overloads>
        /// 
        /// </overloads>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDir">
        /// The path of the file or directory to copy. 
        /// </param>
        /// <param name="targetDir">
        /// The path to the new location.
        /// </param>
        public int Copy(string sourceDir, string targetDir)
        {
            return this.Copy(sourceDir, targetDir, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public int Copy(string sourceDir, string targetDir, BuildLogger logger)
        {
            _copiedCount = 0;

            BuildExceptions.NotNullNotEmpty(sourceDir, "sourceDir");
            BuildExceptions.NotNullNotEmpty(targetDir, "targetDir");

            if (!Directory.Exists(sourceDir))
            {
                throw new BuildException("The source directory must exist.");
            }            
            if (String.Equals(sourceDir, targetDir, 
                StringComparison.CurrentCultureIgnoreCase))
            {
                throw new BuildException(
                    "The source and destination cannot be the same.");
            }

            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDir);
            DirectoryInfo targetInfo = new DirectoryInfo(targetDir);
            DirectorySecurity dirSecurity = null;
            if (_includeSecurity)
            {
                dirSecurity = sourceInfo.GetAccessControl();
            }
            if (!targetInfo.Exists)
            {
                if (dirSecurity != null)
                {
                    targetInfo.Create(dirSecurity);
                }
                else
                {
                    targetInfo.Create();
                }
                targetInfo.Attributes = sourceInfo.Attributes;
            }
            else
            {
                if (dirSecurity != null)
                {
                    targetInfo.SetAccessControl(dirSecurity);
                }
            }

            Copy(sourceInfo, targetInfo, logger);

            return _copiedCount;
        }

        #endregion    

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="logger"></param>
        private void Copy(DirectoryInfo source, DirectoryInfo target, 
            BuildLogger logger)
        {
            CopyFiles(source, target, logger);

            if (!_isRecursive)
            {
                return;
            }

            DirectoryInfo[] arrSourceInfo = source.GetDirectories();

            int dirCount = (arrSourceInfo == null) ? 0 : arrSourceInfo.Length;

            for (int i = 0; i < dirCount; i++)
            {
                DirectoryInfo sourceInfo = arrSourceInfo[i];
                FileAttributes fileAttr  = sourceInfo.Attributes;
                if (!_includeHidden)
                {
                    if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                }

                DirectoryInfo targetInfo = null;
                if (_includeSecurity)
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name,
                        sourceInfo.GetAccessControl());
                }
                else
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name);
                }
                targetInfo.Attributes = fileAttr;

                Copy(sourceInfo, targetInfo, logger);
            }                   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="logger"></param>
        private void CopyFiles(DirectoryInfo source, DirectoryInfo target, 
            BuildLogger logger)
        {
            FileInfo[] listInfo = source.GetFiles();

            int fileCount = (listInfo == null) ? 0 : listInfo.Length;

            string targetDirName = target.ToString();
            string filePath;
            
            // Handle the copy of each file into it's new directory.
            for (int i = 0; i < fileCount; i++)
            {
                FileInfo fi = listInfo[i];
                FileAttributes fileAttr = fi.Attributes;
                if (!_includeHidden)
                {
                    if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                }

                _copiedCount++;

                filePath = Path.Combine(targetDirName, fi.Name);

                if (logger != null)
                {
                    logger.WriteLine(String.Format(@"Copying {0}\{1}", 
                        target.FullName, fi.Name), BuildLoggerLevel.Info);
                }

                fi.CopyTo(filePath, _isOverwrite);

                File.SetAttributes(filePath, fileAttr);
                // if required to set the security or access control
                if (_includeSecurity)
                {
                    File.SetAccessControl(filePath, fi.GetAccessControl());
                }
            }
        }

        #endregion

        #region ICloneable Members

        public override BuildDirCopier Clone()
        {
            BuildDirCopier copier = new BuildDirCopier(this);

            return copier;
        }

        #endregion
    }
}
