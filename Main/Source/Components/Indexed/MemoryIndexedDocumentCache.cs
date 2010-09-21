using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Indexed
{
    public sealed class MemoryIndexedDocumentCache
    {
        #region Private Fields

        // a simple caching mechanism

        private int _cacheSize;

        // an improved cache

        // this cache keeps track of the order that files are loaded in, and always unloads the oldest one
        // this is better, but a document that is often accessed gets no "points", so it will eventualy be
        // thrown out even if it is used regularly

        private Dictionary<string, WeakReference> _cacheStorage;

        private Queue<string> _cacheQueue;

        private MemoryIndexedDocumentSource _documentSource;

        #endregion

        #region Constructors and Destructor

        public MemoryIndexedDocumentCache(int cacheSize, 
            MemoryIndexedDocumentSource source)
        {
            BuildComponentExceptions.NotNull(source, "source");

            if (cacheSize < 0)
                throw new ArgumentOutOfRangeException("cacheSize");

            _cacheSize      = cacheSize;
            _documentSource = source;

            // set up the cache
            _cacheStorage = new Dictionary<string, WeakReference>(
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
        public MemoryIndexedDocument this[string file]
        {
            get
            {
                // now look for that file in the cache
                WeakReference weakRef;
                if (!_cacheStorage.TryGetValue(file, out weakRef))
                {
                    return null;
                }

                // Obtain an instance of a data object from the cache of
                // of weak reference objects.
                MemoryIndexedDocument document = 
                    weakRef.Target as MemoryIndexedDocument;

                if (document == null)
                {
                    // Object was reclaimed, so generate a new one.
                    document = new MemoryIndexedDocument(_documentSource, file);
                    weakRef.Target = document;
                }
                else
                {
                    // Object was obtained with the weak reference.
                }

                return document;
            }
        }

        #endregion

        #region Public Methods

        public bool IsCached(string file)
        {
            if (String.IsNullOrEmpty(file))
            {
                return false;
            }

            return _cacheStorage.ContainsKey(file);
        }

        public void Add(string file, MemoryIndexedDocument document)
        {
            if (document == null)
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
            _cacheStorage.Add(file, new WeakReference(document, false));
            _cacheQueue.Enqueue(file);

            document = null;
        }

        #endregion
    }
}
