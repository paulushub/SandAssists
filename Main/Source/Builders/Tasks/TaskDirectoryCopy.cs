using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.Tasks
{
    public class TaskDirectoryCopy : BuildTask
    {
        #region Public Fields

        private const string KeyOverride        = "Override";
        private const string KeyRecursive       = "Recursive";
        private const string KeyIncludeHidden   = "IncludeHidden";
        private const string KeyIncludeSecurity = "IncludeSecurity";

        private const string KeySource          = "DirSource";
        private const string KeyDestination     = "DirDest";

        #endregion

        #region Private Fields

        private bool _isInitialized;
        private bool _isOverride;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        private IList<BuildTaskItem> _taskItems;

        #endregion

        #region Constructors and Destructor

        public TaskDirectoryCopy()
        {
            _isOverride  = true;
            _isRecursive = true;
        }

        public TaskDirectoryCopy(BuildEngine engine)
            : base(engine)
        {
            _isOverride  = true;
            _isRecursive = true;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "DirectoryCopy";
            }
        }

        public override string Description
        {
            get
            {
                return "Copies a file or a directory and its contents to a new location.";
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

        public bool OverrideFiles
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
                TaskException.ThrowTaskNotInitialized("TaskDirectoryCopy");
            }

            if (_taskItems == null || _taskItems.Count == 0)
            {
                throw new TaskException("The items defining the directories is not specified.");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskDirectoryCopy");
            }

            // make a copy of the current stage...
            bool recursive = _isRecursive;

            try
            {
                int itemCount = _taskItems.Count;
                int dirCopied = 0;

                logger.WriteLine("Copying directories...", HelpLoggerLevel.Started);

                DirectoryCopier dirCopier = new DirectoryCopier();
                dirCopier.Overwrite       = _isOverride;
                dirCopier.Recursive       = _isRecursive;
                dirCopier.IncludeHidden   = _includeHidden;
                dirCopier.IncludeSecurity = _includeSecurity;

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

                    dirCopier.Copy(dirSource, dirDest, logger);

                    dirCopied++;
                }

                logger.WriteLine("Copying directories.", HelpLoggerLevel.Ended);

                return (dirCopied > 0);
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
