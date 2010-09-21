using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Indexed
{
    public sealed class DatabaseIndexedDocumentCache
    {
        #region Private Fields

        // a simple caching mechanism

        private int _cacheSize;

        // an improved cache

        // this cache keeps track of the order that files are loaded in, and always unloads the oldest one
        // this is better, but a document that is often accessed gets no "points", so it will eventualy be
        // thrown out even if it is used regularly

        private Dictionary<string, XPathNavigator> _cacheStorage;

        private Queue<string> _cacheQueue;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocumentCache(int cacheSize)
        {
            if (cacheSize < 0)
                throw new ArgumentOutOfRangeException("cacheSize");

            _cacheSize = cacheSize;

            // set up the cache
            _cacheStorage = new Dictionary<string, XPathNavigator>(
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
        public XPathNavigator this[string id]
        {
            get
            {
                // now look for that id in the cache
                XPathNavigator navigator;
                if (!_cacheStorage.TryGetValue(id, out navigator))
                {
                    return null;
                }

                return navigator;
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

        public void Add(string id, XPathNavigator navigator)
        {
            if (navigator == null)
            {
                return;
            }

            // if the cache is full, remove a document
            if (_cacheStorage.Count >= _cacheSize)
            {
                string fileToUnload = _cacheQueue.Dequeue();
                _cacheStorage.Remove(fileToUnload);
            }

            // add it to the cache
            _cacheStorage.Add(id, navigator);
            _cacheQueue.Enqueue(id);
        }

        #endregion
    }
}
