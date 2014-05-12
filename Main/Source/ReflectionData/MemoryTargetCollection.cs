using System;
using System.Collections.Generic;

using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{  
    public sealed class MemoryTargetCollection : TargetCollection
    {
        #region Private Fields

        private MemoryTargetStorage _targetsStorage;

        #endregion

        #region Constructors and Destructor

        public MemoryTargetCollection()
        {
            _targetsStorage = new MemoryTargetStorage();
        }

        #endregion

        #region Public Properties

        // read the collection

        public override int Count
        {
            get
            {
                return _targetsStorage.Count;
            }
        }

        public override Target this[string id]
        {
            get
            {
                Target result = _targetsStorage[id];

                return result;
            }
        }


        public MemoryTargetStorage Storage
        {
            get
            {
                return _targetsStorage;
            }
        }

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            return _targetsStorage.Contains(id);
        }

        #endregion
    }
}