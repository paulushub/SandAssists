using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    public sealed class StepCleanup : BuildStep
    {
        #region Private Fields

        private IList<string> _listPaths;

        #endregion

        #region Constructors and Destructor

        public StepCleanup()
        {
            _listPaths = new List<string>();
        }

        public StepCleanup(string workingDir)
            : this(workingDir, String.Empty)
        {
        }

        public StepCleanup(string workingDir, string deletePath)
            : base(workingDir)
        {
            _listPaths = new List<string>();
            if (String.IsNullOrEmpty(deletePath) == false)
            {
                deletePath = this.ExpandPath(deletePath);

                _listPaths.Add(deletePath);
            }
        }

        public StepCleanup(string workingDir, IList<string> deletePaths)
            : base(workingDir)
        {
            BuildExceptions.NotNull(deletePaths, "deletePaths");

            _listPaths = deletePaths;
        }

        #endregion

        #region Public Properties

        public IList<string> Paths
        {
            get
            {
                return _listPaths;
            }
        }

        #endregion

        #region Public Methods

        public void Add(string deletePath)
        {
            BuildExceptions.NotNullNotEmpty(deletePath, "deletePath");

            if (_listPaths == null)
            {
                _listPaths = new List<string>();
            }
            if (String.IsNullOrEmpty(deletePath) == false)
            {
                deletePath = this.ExpandPath(deletePath);

                _listPaths.Add(deletePath);
            }
        }

        public void Add(IList<string> deletePaths)
        {
            BuildExceptions.NotNull(deletePaths, "deletePaths");

            int itemCount = deletePaths.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(deletePaths[i]);
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            if (_listPaths == null || _listPaths.Count == 0)
            {
                return false;
            }

            BuildLogger logger = context.Logger;

            try
            {
                int itemCount = _listPaths.Count;
                int pathDeleted = 0;

                if (logger != null)
                {
                    logger.WriteLine("Cleaning up...",
                        BuildLoggerLevel.Started);
                }

                for (int i = 0; i < itemCount; i++)
                {
                    string pathItem = _listPaths[i];
                    if (String.IsNullOrEmpty(pathItem))
                    {
                        continue;
                    }

                    if (File.Exists(pathItem))
                    {
                        // It is a file...
                        File.SetAttributes(pathItem, FileAttributes.Normal);
                        File.Delete(pathItem);
                        if (logger != null)
                        {
                            logger.WriteLine(pathItem, BuildLoggerLevel.Info);
                        }
                        pathDeleted++;
                    }
                    else if (Directory.Exists(pathItem))
                    {
                        // It is a directory...
                        DirectoryUtils.DeleteDirectory(pathItem, true);

                        if (logger != null)
                        {
                            logger.WriteLine(pathItem, BuildLoggerLevel.Info);
                        }
                        pathDeleted++;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(String.Format("Path does not exists - {0}",
                                pathItem), BuildLoggerLevel.Info);
                        }
                    }
                }

                if (logger != null)
                {
                    logger.WriteLine("Cleaning up.",
                        BuildLoggerLevel.Ended);
                }

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
