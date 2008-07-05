using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Tasks
{
    public class TaskDirectoryCreate : BuildTask
    {
        #region Public Fields

        public const string KeyDirectory = "Directory";

        #endregion

        #region Private Fields

        private bool                 _isInitialized;
        private IList<string>        _listPath;
        private IList<BuildTaskItem> _taskItems;

        #endregion

        #region Constructors and Destructor

        public TaskDirectoryCreate()
        {
            _listPath = new List<string>();
        }

        public TaskDirectoryCreate(BuildEngine engine, string directory)
            : base(engine)
        {
            _listPath = new List<string>();
            if (!String.IsNullOrEmpty(directory))
            {
                _listPath.Add(directory);
            }
        }

        public TaskDirectoryCreate(BuildEngine engine, IList<string> directories)
            : base(engine)
        {
            if (directories == null)
            {
                throw new ArgumentNullException("directories",
                    "The directories parameter cannot be null (or Nothing).");
            }

            _listPath = directories;
        }

        public TaskDirectoryCreate(BuildEngine engine)
            : base(engine)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "DirectoryCreate";
            }
        }

        public override string Description
        {
            get
            {
                return "Creates all the directories in a specified path";
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
                TaskException.ThrowTaskNotInitialized("TaskDirectoryCreate");
            }

            if (_listPath == null || _listPath.Count == 0)
            {
                throw new TaskException("The path to be created is not specified.");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskDirectoryCreate");
            }

            try
            {
                int itemCount  = _listPath.Count;
                int dirCreated = 0;

                logger.WriteLine("Creating directories.", HelpLoggerLevel.Started);

                for (int i = 0; i < itemCount; i++)
                {
                    string dirPath = _listPath[i];
                    if (String.IsNullOrEmpty(dirPath))
                    {
                        continue;
                    }

                    if (Directory.Exists(dirPath))
                    {
                        logger.WriteLine(String.Format("Path exists - {0}", dirPath),
                            HelpLoggerLevel.Info);

                        continue;
                    }

                    Directory.CreateDirectory(dirPath);

                    logger.WriteLine(dirPath, HelpLoggerLevel.Info);
                    dirCreated++;
                }

                logger.WriteLine("Creating directories.", HelpLoggerLevel.Ended);

                return (dirCreated > 0);
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
