using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Indexed
{
    public sealed class DatabaseIndexedDocumentSource : IndexedDocumentSource
    {
        #region Private Fields

        private DatabaseIndexedDocument      _document;
        private DatabaseIndexedDocumentCache _cache;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocumentSource(CopyFromIndexComponent component, 
            string keyXPath, string valueXPath, XmlNamespaceManager context,
            int cacheSize, bool isSystem)
            : base(component, keyXPath, valueXPath, context, cacheSize)
        {
            _document = new DatabaseIndexedDocument(isSystem, false);
            _cache    = new DatabaseIndexedDocumentCache(100);
        }

        #endregion

        #region Public Properties

        public bool Exists
        {
            get
            {
                if (_document != null)
                {
                    return _document.Exists;
                }

                return false;
            }
        }

        public bool IsSystem
        {
            get
            {
                if (_document != null)
                {
                    return _document.IsSystem;
                }

                return false;
            }
        }

        public DatabaseIndexedDocumentCache Cache
        {
            get
            {
                return _cache;
            }
        }

        public DatabaseIndexedDocument Document
        {
            get
            {
                return _document;
            }
        }

        #endregion

        #region Public Methods

        public override XPathNavigator GetContent(string key)
        {
            XPathNavigator navigator = _cache[key];
            if (navigator != null)
            {
                return navigator;
            }

            if (_document != null)
            {
                navigator = _document.GetContent(key);
                if (navigator != null)
                {
                    _cache.Add(key, navigator);
                }
            }

            return navigator;
        }

        public override void AddDocument(string file,
            bool cacheIt, bool warnOverride)
        {
            if (_document != null)
            {
                _document.AddDocument(this, file);
            }
        }

        public override void AddDocuments(string wildcardPath,
            bool cacheIt, bool warnOverride)
        {
            string directoryPart = Path.GetDirectoryName(wildcardPath);
            if (String.IsNullOrEmpty(directoryPart))
                directoryPart = Environment.CurrentDirectory;

            directoryPart   = Path.GetFullPath(directoryPart);
            string filePart = Path.GetFileName(wildcardPath);

            string[] files = Directory.GetFiles(directoryPart, filePart);

            foreach (string file in files)
            {
                AddDocument(file, cacheIt, warnOverride);
            }
        }

        public override void AddDocuments(string baseDirectory,
            string wildcardPath, bool recurse, bool cacheIt, bool warnOverride)
        {
            string path;
            if (String.IsNullOrEmpty(baseDirectory))
            {
                path = wildcardPath;
            }
            else
            {
                path = Path.Combine(baseDirectory, wildcardPath);
            }

            AddDocuments(path, cacheIt, warnOverride);

            if (recurse)
            {
                string[] subDirectories = Directory.GetDirectories(baseDirectory);
                foreach (string subDirectory in subDirectories)
                    this.AddDocuments(subDirectory, wildcardPath, recurse, 
                        cacheIt, warnOverride);
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_document != null)
            {
                _document.Dispose();
                _document = null;
            }
        }

        #endregion
    }
}
