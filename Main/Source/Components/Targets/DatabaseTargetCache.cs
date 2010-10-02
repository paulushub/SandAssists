using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Targets
{
    public sealed class DatabaseTargetCache
    {
        #region Private Fields

        // a simple caching mechanism

        private int _cacheSize;

        // an improved cache

        // this cache keeps track of the order that files are loaded in, and always unloads the oldest one
        // this is better, but a document that is often accessed gets no "points", so it will eventualy be
        // thrown out even if it is used regularly

        private Dictionary<string, Target> _cacheStorage;

        private Queue<string> _cacheQueue;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetCache(int cacheSize)
        {
            if (cacheSize < 0)
                throw new ArgumentOutOfRangeException("cacheSize");

            _cacheSize = cacheSize;

            // Limit the initial memory size...
            if (cacheSize > 50)
            {
                cacheSize = 50;
            }

            // set up the cache
            _cacheStorage = new Dictionary<string, Target>(
                cacheSize, StringComparer.OrdinalIgnoreCase);

            _cacheQueue = new Queue<string>(cacheSize);
        }

        #endregion

        #region Public Properties

        // Returns the number of items in the cache.
        public int Count
        {
            get
            {
                return _cacheStorage.Count;
            }
        }

        // Accesses a data object from the cache.
        // If the object was reclaimed for garbage collection,
        // create a new data object at that index location.
        public Target this[string id]
        {
            get
            {
                // now look for that id in the cache
                if (_cacheStorage.ContainsKey(id))
                {
                    return _cacheStorage[id];
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public bool IsCached(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            return _cacheStorage.ContainsKey(id);
        }

        public void Add(Target target)
        {
            if (target == null)
            {
                return;
            }

            // if the cache is full, remove a document
            if (_cacheStorage.Count >= _cacheSize)
            {
                string fileToUnload = _cacheQueue.Dequeue();
                _cacheStorage.Remove(fileToUnload);
            }
             
            string id = target.id;
            // add it to the cache
            _cacheStorage.Add(id, target);
            _cacheQueue.Enqueue(id);
        }

        #endregion
    }
}
