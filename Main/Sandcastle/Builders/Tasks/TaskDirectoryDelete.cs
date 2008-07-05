using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Tasks
{
    public class TaskDirectoryDelete : BuildTask
    {
        #region Public Fields

        public const string KeyDirectory = "Directory";

        #endregion

        #region Private Fields

        private bool                 _isRecursive;
        private bool                 _isInitialized;
        private IList<string>        _listPath;
        private IList<BuildTaskItem> _taskItems;

        #endregion

        #region Constructors and Destructor

        public TaskDirectoryDelete()
        {
            _isRecursive = true;
            _listPath    = new List<string>();
        }

        public TaskDirectoryDelete(BuildEngine engine, 
            string directory, bool recursive) : base(engine)
        {
            _isRecursive = recursive;
            _listPath    = new List<string>();
            if (!String.IsNullOrEmpty(directory))
            {
                _listPath.Add(directory);
            }
        }

        public TaskDirectoryDelete(BuildEngine engine,
            IList<string> directories, bool recursive)
            : base(engine)
        {
            if (directories == null)
            {
                throw new ArgumentNullException("directories",
                    "The directories parameter cannot be null (or Nothing).");
            }

            _isRecursive = recursive;
            _listPath    = directories;
        }

        public TaskDirectoryDelete(BuildEngine engine)
            : base(engine)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "DirectoryDelete";
            }
        }

        public override string Description
        {
            get
            {
                return "Deletes all the specified directories";
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

        public IList<string> Directories
        {
            get
            {
                if (_listPath == null)
                {
                    _listPath = new List<string>();
                }

                return _listPath;
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

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {
            if (taskItems != null && taskItems.Count > 0)
            {
                int itemCount  = taskItems.Count;
                string dirPath = null;

                _listPath = new List<string>(itemCount);
                
                for (int i = 0; i < itemCount; i++)
                {
                    BuildTaskItem item = taskItems[i];
                    
                    if (item != null && item.Count != 0)
                    {
                        dirPath = item[KeyDirectory];
                        if (String.IsNullOrEmpty(dirPath) == false)
                        {
                            _listPath.Add(dirPath);
                        }
                    }
                }
            }

            _taskItems     = taskItems;
            _isInitialized = (_listPath != null && _listPath.Count > 0);
        }

        public override void Uninitialize()
        {
            _isInitialized = false;
            _listPath      = null;
            _taskItems     = null;
        }

        public override bool Execute()
        {
            if (!_isInitialized)
            {
                TaskException.ThrowTaskNotInitialized("TaskDirectoryDelete");
            }

            if (_listPath == null || _listPath.Count == 0)
            {
                throw new TaskException("The path to be created is not specified.");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskDirectoryDelete");
            }

            // make a copy of the current stage...
            bool recursive = _isRecursive;

            try
            {
                int itemCount  = _listPath.Count;
                int dirDeleted = 0;

                logger.WriteLine("Deleting directories.", HelpLoggerLevel.Started);

                for (int i = 0; i < itemCount; i++)
                {
                    string dirPath = _listPath[i];
                    if (String.IsNullOrEmpty(dirPath))
                    {
                        continue;
                    }

                    if (Directory.Exists(dirPath) == false)
                    {
                        logger.WriteLine(String.Format("Path does not exists - {0}", 
                            dirPath), HelpLoggerLevel.Info);

                        continue;
                    }

                    Directory.Delete(dirPath, recursive);

                    logger.WriteLine(dirPath, HelpLoggerLevel.Info);
                    dirDeleted++;
                }

                logger.WriteLine("Deleting directories.", HelpLoggerLevel.Ended);

                return (dirDeleted > 0);
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
