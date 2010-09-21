using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Sandcastle.Utilities
{
    /// <summary>
    /// Copies a file or a directory and its contents to a new location. 
    /// </summary>
    [Serializable]
    public sealed class DirectoryCopier : BuildObject<DirectoryCopier>
    {
        #region Private Fields

        private int  _copiedCount;
        private bool _isOverwrite;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        #endregion

        #region Constructors and Destructor

        public DirectoryCopier()
        {
            _isOverwrite      = true;
            _isRecursive     = true;
        }

        public DirectoryCopier(DirectoryCopier source)
            : base(source)
        {
            _copiedCount     = source._copiedCount;
            _isOverwrite      = source._isOverwrite;
            _isRecursive     = source._isRecursive;
            _includeHidden   = source._includeHidden;
            _includeSecurity = source._includeSecurity;
        }

        #endregion

        #region Public Properties

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

        public int Copy(string sourceDir, string targetDir, BuildLogger logger)
        {
            _copiedCount = 0;

            if (sourceDir == null)
            {
                throw new ArgumentNullException("sourceDir");
            }
            if (sourceDir.Length == 0)
            {
                throw new ArgumentException("sourceDir");
            }
            if (!Directory.Exists(sourceDir))
            {
                throw new InvalidOperationException();
            }

            if (targetDir == null)
            {
                throw new ArgumentNullException("targetDir");
            }
            if (targetDir.Length == 0)
            {
                throw new ArgumentException("targetDir");
            }
            
            if (String.Equals(sourceDir, targetDir, 
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
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

        private void CopyFiles(DirectoryInfo source, DirectoryInfo target, 
            BuildLogger logger)
        {
            FileInfo[] listInfo = source.GetFiles();

            int fileCount = (listInfo == null) ? 0 : listInfo.Length;

            string targetDirName = target.ToString();
            string filePath;
            bool isLogging = (logger != null && logger.Logging &&
                logger.Verbosity == BuildLoggerVerbosity.Detailed);
            
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

                if (isLogging)
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

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override DirectoryCopier Clone()
        {
            DirectoryCopier copier = new DirectoryCopier(this);

            return copier;
        }

        #endregion
    }
}
