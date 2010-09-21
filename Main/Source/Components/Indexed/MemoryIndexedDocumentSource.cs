using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Indexed
{
    public sealed class MemoryIndexedDocumentSource : IndexedDocumentSource
    {
        #region Private Fields

        private int _documentCount;

        // a index mapping keys to the files that contain them
        private Dictionary<string, string> _keyFileIndex;

        // an improved cache
        private MemoryIndexedDocumentCache _documentCache;

        #endregion

        #region Constructors and Destructor

        public MemoryIndexedDocumentSource(CopyFromIndexComponent component, 
            string keyXPath, string valueXPath, XmlNamespaceManager context,
            int cacheSize)
            : base(component, keyXPath, valueXPath, context, cacheSize)
        {
            _documentCache = new MemoryIndexedDocumentCache(cacheSize, this);
            _keyFileIndex  = new Dictionary<string, string>();
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                return (_keyFileIndex.Count);
            }
        }

        public int DocumentCount
        {
            get
            {
                return _documentCount;
            }
        }

        public MemoryIndexedDocumentCache Cache
        {
            get
            {
                return _documentCache;
            }
        }

        #endregion

        #region Public Methods

        public override XPathNavigator GetContent(string key)
        {
            XPathNavigator navigator = null;

            MemoryIndexedDocument document = GetDocument(key);
            if (document != null)
            {
                navigator = document.GetContent(key);
                // Remove any strong reference to the document by setting it null
                document  = null;
            }

            return navigator;
        }

        public override void AddDocument(string file, bool cacheIt)
        {    
            // load the document
            MemoryIndexedDocument document = 
                new MemoryIndexedDocument(this, file);

            // record the keys
            //string[] keys = document.GetKeys();
            ICollection<string> keys = document.GetKeys();
            foreach (string key in keys)
            {
                if (_keyFileIndex.ContainsKey(key))
                {
                    this.WriteMessage(MessageLevel.Warn, 
                        String.Format("Entries for the key '{0}' occur in both '{1}' and '{2}'. The last entry will be used.", 
                        key, _keyFileIndex[key], file));
                }
                _keyFileIndex[key] = file;  
            }  
   
            if (cacheIt)
            {
                this.CacheDocument(file, document);
            }

            document = null;
        }

        public override void AddDocuments(string wildcardPath, bool cacheIt)
        {
            string directoryPart = Path.GetDirectoryName(wildcardPath);
            if (String.IsNullOrEmpty(directoryPart)) 
                directoryPart = Environment.CurrentDirectory;

            directoryPart   = Path.GetFullPath(directoryPart);
            string filePart = Path.GetFileName(wildcardPath);

            string[] files  = Directory.GetFiles(directoryPart, filePart);
            
            foreach (string file in files)
            {
                AddDocument(file, cacheIt);
            }

            _documentCount += files.Length;
        }

        public override void AddDocuments(string baseDirectory, 
            string wildcardPath, bool recurse, bool cacheIt)
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

            AddDocuments(path, cacheIt);

            if (recurse)
            {
                string[] subDirectories = Directory.GetDirectories(baseDirectory);
                foreach (string subDirectory in subDirectories)
                    AddDocuments(subDirectory, wildcardPath, recurse, cacheIt);
            }
        }

        #endregion

        #region Private Methods

        private MemoryIndexedDocument GetDocument(string key)
        {           
            // look up the file corresponding to the key
            string file;
            if (_keyFileIndex.TryGetValue(key, out file))
            {       
                // now look for that file in the cache
                MemoryIndexedDocument document = _documentCache[file];
                if (document == null)
                {   
                    // not in the cache, so load it
                    document = new MemoryIndexedDocument(this, file);

                    this.CacheDocument(file, document);
                }

                return document;
            }
            else
            {
                // there is no such key
                return null;
            } 
        }

        private void CacheDocument(string file, MemoryIndexedDocument document)
        {
            if (_documentCache == null)
            {
                return;
            }

            // add it to the cache
            _documentCache.Add(file, document);
        }

        #endregion
    }
}
