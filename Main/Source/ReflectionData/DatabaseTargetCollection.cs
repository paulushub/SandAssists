using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetCollection : TargetCollection
    {
        #region Private Fields

        private DatabaseTargetTextStorage _targetsStorage;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetCollection(int tn)
        {
            _targetsStorage = new DatabaseTargetTextStorage(true, false);
        }

        #endregion

        #region Public Properties

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

        public DatabaseTargetTextStorage Storage
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
