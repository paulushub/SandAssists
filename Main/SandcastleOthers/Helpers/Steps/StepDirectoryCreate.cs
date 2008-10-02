using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepDirectoryCreate : BuildStep
    {
        #region Private Fields

        private List<string> _listDirs;

        #endregion

        #region Constructors and Destructor

        public StepDirectoryCreate()
        {
            _listDirs    = new List<string>();
        }

        public StepDirectoryCreate(string workingDir)
            : this(workingDir, null)
        {
        }

        public StepDirectoryCreate(string workingDir, string createDir)
            : base(workingDir)
        {
            _listDirs    = new List<string>();
            if (String.IsNullOrEmpty(createDir) == false)
            {
                createDir = this.ExpandPath(createDir);

                _listDirs.Add(createDir);
            }
        }

        public StepDirectoryCreate(StepDirectoryCreate source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

        public void Add(string createDir)
        {   
            if (_listDirs == null)
            {
                _listDirs = new List<string>();
            }
            if (String.IsNullOrEmpty(createDir) == false)
            {
                createDir = this.ExpandPath(createDir);

                _listDirs.Add(createDir);
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
                int itemCount  = _listDirs.Count;
                int dirCreated = 0;

                if (logger != null)
                {
                    logger.WriteLine("Creating directories...", 
                        BuildLoggerLevel.Started);
                }

                for (int i = 0; i < itemCount; i++)
                {
                    string dirPath = _listDirs[i];
                    if (String.IsNullOrEmpty(dirPath))
                    {
                        continue;
                    }

                    if (Directory.Exists(dirPath))
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(String.Format("Path exists - {0}", 
                                dirPath), BuildLoggerLevel.Info);
                        }

                        continue;
                    }

                    DirectoryInfo dirInfo = Directory.CreateDirectory(dirPath);

                    if (dirInfo != null && logger != null)
                    {
                        logger.WriteLine(dirPath, BuildLoggerLevel.Info);
                    }

                    dirCreated++;
                }

                if (logger != null)
                {
                    logger.WriteLine("Creating directories.", 
                        BuildLoggerLevel.Ended);
                }

                //return (dirCreated > 0); // could step the step...

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
            StepDirectoryCreate buildStep = new StepDirectoryCreate(this);
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
