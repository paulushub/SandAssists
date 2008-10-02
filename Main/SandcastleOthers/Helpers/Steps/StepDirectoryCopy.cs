using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepDirectoryCopy : BuildStep
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
            _isOverwrite    = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();
        }

        public StepDirectoryCopy(string workingDir)
            : base(workingDir)
        {
            _isOverwrite    = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();
        }

        public StepDirectoryCopy(StepDirectoryCopy source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

        protected override bool MainExecute(BuildEngine engine)
        {
            BuildLogger logger = engine.Logger;

            try
            {
                BuildDirCopier dirCopier = new BuildDirCopier();
                dirCopier.Overwrite = _isOverwrite;
                dirCopier.Recursive = _isRecursive;

                int itemCount = _listSourceDir.Count;
                int dirCopied  = 0;

                if (logger != null)
                {
                    logger.WriteLine("Copying directories.", 
                        BuildLoggerLevel.Started);
                }

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
                        logger.WriteLine("Copying:..." + dirSource,
                            BuildLoggerLevel.Info);
                    }

                    int fileCopies = dirCopier.Copy(dirSource, dirDest);

                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "Total of {0} files copied.", fileCopies),
                            BuildLoggerLevel.Info);
                    }

                    dirCopied++;
                }

                if (logger != null)
                {
                    logger.WriteLine("Copying directories.", 
                        BuildLoggerLevel.Ended);
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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepDirectoryCopy buildStep = new StepDirectoryCopy(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
