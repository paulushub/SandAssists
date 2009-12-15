using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepDirectoryMove : BuildStep
    {
        #region Private Fields

        private bool _isOverride;

        private List<string> _listSourceDir;
        private List<string> _listDestDir;

        #endregion

        #region Constructors and Destructor

        public StepDirectoryMove()
        {
            _isOverride    = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();

            this.Message   = "Moving directories";
        }

        public StepDirectoryMove(string workingDir)
            : base(workingDir)
        {
            _isOverride    = true;
            _listDestDir   = new List<string>();
            _listSourceDir = new List<string>();

            this.Message   = "Moving directories";
        }

        #endregion

        #region Public Properties

        public bool OverrideDirectory
        {
            get
            {
                return _isOverride;
            }

            set
            {
                _isOverride = value;
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

        protected override bool MainExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            try
            {
                int itemCount = _listSourceDir.Count;
                int dirMoved  = 0;

                //if (logger != null)
                //{
                //    logger.WriteLine("Moving directories...", 
                //        BuildLoggerLevel.Started);
                //}

                for (int i = 0; i < itemCount; i++)
                {
                    string dirSource = _listSourceDir[i];
                    string dirDest   = _listDestDir[i];

                    if (logger != null)
                    {
                        if (String.IsNullOrEmpty(dirSource) == false)
                        {
                            logger.WriteLine("Moving From:..." + dirSource,
                                BuildLoggerLevel.Info);
                        }
                        if (String.IsNullOrEmpty(dirDest) == false)
                        {
                            logger.WriteLine("Moving To:..." + dirDest,
                                BuildLoggerLevel.Info);
                        }
                    }

                    if (Directory.Exists(dirSource) == false)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(String.Format(
                                "Path does not exists - {0}",
                                dirSource), BuildLoggerLevel.Info);
                        }

                        continue;
                    }

                    if (Directory.Exists(dirDest) == true)
                    {
                        if (_isOverride)
                        {
                            BuildDirHandler.DeleteDirectory(dirDest, true);
                        }
                        else
                        {
                            if (logger != null)
                            {
                                logger.WriteLine(String.Format("Path exists - {0}",
                                    dirDest), BuildLoggerLevel.Info);
                            }

                            continue;
                        }
                    }

                    Directory.Move(dirSource, dirDest);

                    dirMoved++;
                }

                //if (logger != null)
                //{
                //    logger.WriteLine("Moving directories.", 
                //        BuildLoggerLevel.Ended);
                //}

                //return (dirMoved > 0);  // could stop the steps...

                return true;
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
