using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Indexed
{
    public sealed class IndexedDocumentSources : IndexedDocumentSource
    {
        #region Private Fields

        private MemoryIndexedDocumentSource   _memorySource;
        private DatabaseIndexedDocumentSource _databaseSource;

        #endregion

        #region Constructors and Destructor

        public IndexedDocumentSources(CopyFromIndexComponent component,
            DatabaseIndexedDocumentSource database,
            MemoryIndexedDocumentSource memory)
            : base(component)
        {
            _memorySource   = memory;
            _databaseSource = database;
        }

        #endregion

        #region Public Properties

        public MemoryIndexedDocumentSource MemorySource
        {
            get
            {
                return _memorySource;
            }
        }

        public DatabaseIndexedDocumentSource DatabaseSource
        {
            get
            {
                return _databaseSource;
            }
        }

        #endregion

        #region Public Methods

        public override XPathNavigator GetContent(string key)
        {
            if (_memorySource != null)
            {
                XPathNavigator navigator = _memorySource.GetContent(key);

                if (navigator != null)
                {
                    return navigator;
                }
            }
            if (_databaseSource != null)
            {
                XPathNavigator navigator = _databaseSource.GetContent(key);

                if (navigator != null)
                {
                    return navigator;
                }
            }

            return null;
        }

        public override void AddDocument(string file, bool cacheIt,
            bool warnOverride)
        {
            if (_memorySource != null)
            {
                _memorySource.AddDocument(file, cacheIt, warnOverride);
            }
        }

        public override void AddDocuments(string wildcardPath,
            bool cacheIt, bool warnOverride)
        {
            if (_memorySource != null)
            {
                _memorySource.AddDocuments(wildcardPath, cacheIt, warnOverride);
            }
        }

        public override void AddDocuments(string baseDirectory,
            string wildcardPath, bool recurse, bool cacheIt, bool warnOverride)
        {
            if (_memorySource != null)
            {
                _memorySource.AddDocuments(baseDirectory, wildcardPath,
                    recurse, cacheIt, warnOverride);
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_memorySource != null)
            {
                _memorySource.Dispose();
                _memorySource = null;
            }
            if (_databaseSource != null)
            {
                _databaseSource.Dispose();
                _databaseSource = null;
            }
        }

        #endregion
    }
}
