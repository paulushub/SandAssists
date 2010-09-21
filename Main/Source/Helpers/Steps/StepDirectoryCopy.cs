using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    public sealed class StepDirectoryCopy : BuildStep
    {
        #region Private Fields

        private bool _isRecursive;
        private bool _isOverwrite;

        private List<string> _listSourceDir;
        private List<string> _listDestDir;

        #endregion

        #region Constructors and Destructor

        public StepDirectoryCopy()
        {
            _isOverwrite   = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();

            this.LogTitle   = "Copying directories";
        }

        public StepDirectoryCopy(string workingDir)
            : base(workingDir)
        {
            _isOverwrite   = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();

            this.LogTitle   = "Copying directories";
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (_listSourceDir == null || _listSourceDir.Count == 0)
                {
                    return false;
                }
                if (_listDestDir == null || _listDestDir.Count == 0)
                {
                    return false;
                }

                return (_listSourceDir.Count == _listDestDir.Count);
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

        public IList<string> SourceDirectories
        {
            get
            {
                if (_listSourceDir != null)
                {
                    _listSourceDir.AsReadOnly();
                }

                return null;
            }
        }

        public IList<string> DestinationDirectories
        {
            get
            {
                if (_listDestDir != null)
                {
                    _listDestDir.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Add(string sourceDir, string destDir)
        {
            if (_listSourceDir == null || _listDestDir == null)
            {
                _listSourceDir = new List<string>();
                _listDestDir   = new List<string>();
            }
            if (String.IsNullOrEmpty(sourceDir) == false &&
                String.IsNullOrEmpty(destDir) == false)
            {
                sourceDir = this.ExpandPath(sourceDir);
                destDir   = this.ExpandPath(destDir);

                _listSourceDir.Add(sourceDir);
                _listDestDir.Add(destDir);
            }
        }

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            int itemCount = _listSourceDir.Count;
            if (itemCount == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine("There is no directory specified to copy.",
                        BuildLoggerLevel.Warn);
                }

                return true;
            }

            try
            {
                int dirCopied = 0;

                BuildDirCopier dirCopier = new BuildDirCopier();
                dirCopier.Overwrite = _isOverwrite;
                dirCopier.Recursive = _isRecursive;

                string baseDir = context.BaseDirectory;

                for (int i = 0; i < itemCount; i++)
                {
                    string dirSource = _listSourceDir[i];
                    string dirDest   = _listDestDir[i];
                    if (Directory.Exists(dirSource) == false)
                    {
                        logger.WriteLine(String.Format("Path does not exists - {0}",
                            dirSource), BuildLoggerLevel.Info);

                        continue;
                    }

                    if (logger != null)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Copying - ");
                        if (dirSource.StartsWith(baseDir,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Append(PathUtils.GetRelativePath(
                                baseDir, dirSource));
                        }
                        else
                        {
                            builder.Append(dirSource);
                        }
                        builder.Append(" -> ");
                        if (dirDest.StartsWith(baseDir,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Append(PathUtils.GetRelativePath(
                                baseDir, dirDest)); 
                        }
                        else
                        {
                            builder.Append(PathUtils.GetRelativePath(
                                dirSource, dirDest));
                        }
                        logger.WriteLine(builder.ToString(),
                            BuildLoggerLevel.Info);
                    }

                    int fileCopies = dirCopier.Copy(dirSource, dirDest);

                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "Completed Copying - Total of {0} files copied.", fileCopies),
                            BuildLoggerLevel.Info);
                    }

                    dirCopied++;
                }

                return (dirCopied > 0);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
