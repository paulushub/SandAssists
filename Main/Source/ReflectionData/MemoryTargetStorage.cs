using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class MemoryTargetStorage : TargetStorage
    {
        #region Private Fields

        private Dictionary<string, Target> _targetsIndex;

        #endregion

        #region Constructors and Destructor

        public MemoryTargetStorage()
        {
            _targetsIndex = new Dictionary<string, Target>();
        }

        #endregion

        #region Public Properties

        public override int Count
        {
            get
            {
                return _targetsIndex.Count;
            }
        }

        public override Target this[string id]
        {
            get
            {
                Target result;
                _targetsIndex.TryGetValue(id, out result);

                return result;
            }
        }

        public IEnumerable<string> Ids
        {
            get
            {
                return _targetsIndex.Keys;
            }
        }

        public IEnumerable<Target> Targets
        {
            get
            {
                return _targetsIndex.Values;
            }
        }

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            return _targetsIndex.ContainsKey(id);
        }

        public override void Add(Target target)
        {
            _targetsIndex[target.id] = target;
        }

        public override void Clear()
        {
            _targetsIndex.Clear();
        }

        #endregion
    }
}
