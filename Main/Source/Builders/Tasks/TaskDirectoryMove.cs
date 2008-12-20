using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Tasks
{
    public class TaskDirectoryMove : BuildTask
    {
        #region Public Fields

        private const string KeySource      = "DirSource";
        private const string KeyDestination = "DirDest";

        #endregion

        #region Private Fields

        private bool _isInitialized;
        private bool _isOverride;

        private IList<BuildTaskItem> _taskItems;

        #endregion

        #region Constructors and Destructor

        public TaskDirectoryMove()
        {
            _isOverride = true;
        }

        public TaskDirectoryMove(BuildEngine engine)
            : base(engine)
        {
            _isOverride = true;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "DirectoryMove";
            }
        }

        public override string Description
        {
            get
            {
                return "Moves a file or a directory and its contents to a new location.";
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public override IList<BuildTaskItem> Items
        {
            get
            {
                return _taskItems;
            }
        }

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

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {
            _taskItems     = taskItems;
            _isInitialized = (_taskItems != null && _taskItems.Count > 0);
        }

        public override void Uninitialize()
        {
            _isInitialized = false;
            _taskItems     = null;
        }

        public override bool Execute()
        {
            if (!_isInitialized)
            {
                TaskException.ThrowTaskNotInitialized("TaskDirectoryMove");
            }

            if (_taskItems == null || _taskItems.Count == 0)
            {
                throw new TaskException("The items defining the directories is not specified.");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskDirectoryMove");
            }

            try
            {
                int itemCount = _taskItems.Count;
                int dirMoved  = 0;

                logger.WriteLine("Moving directories.", HelpLoggerLevel.Started);

                for (int i = 0; i < itemCount; i++)
                {
                    BuildTaskItem taskItem = _taskItems[i];
                    if (taskItem == null || taskItem.Count < 2)
                    {
                        continue;
                    }

                    string dirSource = taskItem[KeySource];
                    string dirDest   = taskItem[KeyDestination];
                    if (String.IsNullOrEmpty(dirSource) || 
                        String.IsNullOrEmpty(dirDest))
                    {
                        continue;
                    }
                    if (Directory.Exists(dirSource) == false)
                    {
                        logger.WriteLine(String.Format("Path does not exists - {0}",
                            dirSource), HelpLoggerLevel.Info);

                        continue;
                    }

                    if (Directory.Exists(dirDest) == true)
                    {
                        if (_isOverride)
                        {
                            Directory.Delete(dirDest, true);
                        }
                        else
                        {
                            logger.WriteLine(String.Format("Path exists - {0}",
                                dirDest), HelpLoggerLevel.Info);

                            continue;
                        }
                    }

                    Directory.Move(dirSource, dirDest);

                    dirMoved++;
                }

                logger.WriteLine("Moving directories.", HelpLoggerLevel.Ended);

                return (dirMoved > 0);
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex, HelpLoggerLevel.Error);

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
