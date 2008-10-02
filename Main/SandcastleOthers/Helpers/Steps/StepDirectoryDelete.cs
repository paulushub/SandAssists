using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepDirectoryDelete : BuildStep
    {
        #region Private Fields

        private bool         _isRecursive;
        private List<string> _listDirs;

        #endregion

        #region Constructors and Destructor

        public StepDirectoryDelete()
        {
            _isRecursive = true;
            _listDirs    = new List<string>();
        }

        public StepDirectoryDelete(string workingDir)
            : this(workingDir, null)
        {
        }

        public StepDirectoryDelete(string workingDir, string deleteDir)
            : base(workingDir)
        {
            _isRecursive = true;
            _listDirs    = new List<string>();
            if (String.IsNullOrEmpty(deleteDir) == false)
            {
                deleteDir = this.ExpandPath(deleteDir);

                _listDirs.Add(deleteDir);
            }
        }

        public StepDirectoryDelete(StepDirectoryDelete source)
            : base(source)
        {
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

        public IList<string> Directories
        {
            get
            {
                if (_listDirs != null)
                {
                    _listDirs.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Add(string deleteDir)
        {   
            if (_listDirs == null)
            {
                _listDirs = new List<string>();
            }
            if (String.IsNullOrEmpty(deleteDir) == false)
            {
                deleteDir = this.ExpandPath(deleteDir);

                _listDirs.Add(deleteDir);
            }
        }

        protected override bool MainExecute(BuildEngine engine)
        {
            if (_listDirs == null || _listDirs.Count == 0)
            {
                return false;
            }

            BuildLogger logger = engine.Logger;

            try
            {
                // make a copy of the current stage...
                bool recursive = _isRecursive; 

                int itemCount  = _listDirs.Count;
                int dirDeleted = 0;

                if (logger != null)
                {
                    logger.WriteLine("Deleting directories...", 
                        BuildLoggerLevel.Started);
                }

                for (int i = 0; i < itemCount; i++)
                {
                    string dirPath = _listDirs[i];
                    if (String.IsNullOrEmpty(dirPath))
                    {
                        continue;
                    }

                    if (Directory.Exists(dirPath) == false)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(String.Format("Path does not exists - {0}",
                                dirPath), BuildLoggerLevel.Info);
                        }

                        continue;
                    }

                    Directory.Delete(dirPath, recursive);

                    if (logger != null)
                    {
                        logger.WriteLine(dirPath, BuildLoggerLevel.Info);
                    }
                    dirDeleted++;
                }

                if (logger != null)
                {
                    logger.WriteLine("Deleting directories.", 
                        BuildLoggerLevel.Ended);
                }

                //return (dirDeleted > 0); // could stop the steps...

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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepDirectoryDelete buildStep = new StepDirectoryDelete(this);
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
