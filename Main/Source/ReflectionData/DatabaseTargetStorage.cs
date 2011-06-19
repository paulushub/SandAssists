using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandcastle.ReflectionData
{
    public abstract class DatabaseTargetStorage : TargetStorage
    {
        #region Constructors and Destructor

        public DatabaseTargetStorage(bool isSystem, bool createNotFound)
        {
        }

        public DatabaseTargetStorage(bool isSystem, bool createNotFound, 
            string workingDir)
        {
        }

        #endregion

        #region Public Properties

        public abstract DatabaseTargetCache Cache
        {
            get;
        }

        #endregion
    }
}
