using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Indexed
{
    public sealed class IndexedDocumentController : IDisposable
    {
        #region Private Fields

        private Type           _thisType;
        private BuildComponent _component;
        private BuildAssembler _assembler;

        private Dictionary<string, IndexedDocumentSources> _documentSources;

        #endregion

        #region Constructors and Destructor

        public IndexedDocumentController(CopyFromIndexComponent component,
            XPathNavigator configuration, CustomContext context, 
            IDictionary<string, object> globalData)
        {
            BuildComponentExceptions.NotNull(component, "component");

            _component = component;
            _thisType  = this.GetType();
            _assembler = component.BuildAssembler;

            _documentSources = new Dictionary<string, IndexedDocumentSources>(
                StringComparer.OrdinalIgnoreCase);

            // set up the indices
            XPathNodeIterator indexNodes = configuration.Select("index");
            foreach (XPathNavigator indexNode in indexNodes)
            {   
                MemoryIndexedDocumentSource memorySource     = null;
                DatabaseIndexedDocumentSource databaseSource = null;

                // get the name of the index
                string name = indexNode.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(name))
                    throw new BuildComponentException("Each index must have a unique name.");

                // get the xpath for value nodes
                string valueXPath = indexNode.GetAttribute("value", String.Empty);
                if (String.IsNullOrEmpty(valueXPath))
                    WriteMessage(MessageLevel.Error, "Each index element must have a value attribute containing an XPath that describes index entries.");

                // get the xpath for keys (relative to value nodes)
                string keyXPath = indexNode.GetAttribute("key", String.Empty);
                if (String.IsNullOrEmpty(keyXPath))
                    WriteMessage(MessageLevel.Error, "Each index element must have a key attribute containing an XPath (relative to the value XPath) that evaluates to the entry key.");

                // get the cache size
                int cache = 10;
                string cacheValue = indexNode.GetAttribute("cache", String.Empty);
                if (!String.IsNullOrEmpty(cacheValue))
                    cache = Convert.ToInt32(cacheValue);

                // create the index
                memorySource   = new MemoryIndexedDocumentSource(
                    component, keyXPath, valueXPath, context, cache);

                // search the data directories for entries
                XPathNodeIterator dataNodes = indexNode.Select("data");
                foreach (XPathNavigator dataNode in dataNodes)
                {
                    string baseValue = dataNode.GetAttribute("base", String.Empty);
                    if (!String.IsNullOrEmpty(baseValue))
                        baseValue = Environment.ExpandEnvironmentVariables(baseValue);

                    bool recurse = false;
                    string recurseValue = dataNode.GetAttribute("recurse", String.Empty);
                    if (!String.IsNullOrEmpty(recurseValue))
                        recurse = (bool)Convert.ToBoolean(recurseValue);

                    // get the files				
                    string files = dataNode.GetAttribute("files", String.Empty);
                    if (String.IsNullOrEmpty(files))
                        WriteMessage(MessageLevel.Error, "Each data element must have a files attribute specifying which files to index.");
                    // if ((files == null) || (files.Length == 0)) throw new BuildComponentException("When instantiating a CopyFromDirectory component, you must specify a directory path using the files attribute.");
                    files = Environment.ExpandEnvironmentVariables(files);

                    WriteMessage(MessageLevel.Info, String.Format(
                        "Searching for files that match '{0}'.", files));

                    if (String.Equals(name, "reflection", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(baseValue) &&
                            baseValue.EndsWith(@"Sandcastle\Data\Reflection", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Assert(databaseSource == null);

                            databaseSource = new DatabaseIndexedDocumentSource(
                                component, keyXPath, valueXPath, context, cache, true);
                            if (!databaseSource.Exists)
                            {
                                memorySource.AddDocuments(baseValue, 
                                    files, recurse, false); 
                            }
                        }
                        else
                        {
                            memorySource.AddDocuments(baseValue, files, recurse, true);
                        }
                    }
                    else
                    {
                        memorySource.AddDocuments(baseValue, files, recurse, false);
                    }
                }
                WriteMessage(MessageLevel.Info, String.Format(
                    "Indexed {0} elements in {1} files.", 
                    memorySource.Count, memorySource.DocumentCount));

                globalData.Add(name, this);
                _documentSources[name] = new IndexedDocumentSources(component,
                    databaseSource, memorySource);
            }
        }

        ~IndexedDocumentController()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public BuildComponent Component
        {
            get
            {
                return _component;
            }
        }

        public IDictionary<string, IndexedDocumentSources> Sources
        {
            get
            {
                return _documentSources;
            }
        }

        public IndexedDocumentSource this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                {
                    return null;
                }

                IndexedDocumentSources documentSource = null;
                if (_documentSources.TryGetValue(name, out documentSource))
                {
                    return documentSource;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void WriteMessage(MessageLevel level, string message)
        {
            if (_assembler == null || level == MessageLevel.Ignore)
                return;

            MessageHandler handler = _assembler.MessageHandler;
            if (handler != null)
                handler(_thisType, level, message);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_documentSources != null && _documentSources.Count != 0)
            {
                ICollection<IndexedDocumentSources> collSources = 
                    _documentSources.Values;
                foreach (IndexedDocumentSources sources in collSources)
                {
                    sources.Dispose();
                }

                _documentSources = null;
            }
        }

        #endregion
    }
}
