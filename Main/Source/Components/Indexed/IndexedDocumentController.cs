using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.ReflectionData;

namespace Sandcastle.Components.Indexed
{
    public sealed class IndexedDocumentController : IDisposable
    {
        #region Private Fields

        private Type           _thisType;
        private BuildAssembler _assembler;
        private CopyFromIndexComponent _component;

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
                // get the name of the index
                string name = indexNode.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(name))
                    throw new BuildComponentException("Each index must have a unique name.");

                if (String.Equals(name, "reflection",
                    StringComparison.OrdinalIgnoreCase))
                {
                    this.ProcessReflectionIndex(indexNode, context, globalData);
                }
                else if (String.Equals(name, "comments",
                    StringComparison.OrdinalIgnoreCase))
                {
                    this.ProcessCommentsIndex(indexNode, context, globalData);
                }
                else
                {
                    this.ProcessIndex(indexNode, context, globalData);
                }
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

            MessageWriter writer = _assembler.MessageWriter;
            if (writer != null)
            {
                writer.Write(_thisType, level, message);
            }
        }

        #endregion

        #region Private Methods

        #region ProcessReflectionIndex Method

        private void ProcessReflectionIndex(XPathNavigator indexNode, CustomContext context,
            IDictionary<string, object> globalData)
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
            memorySource = new MemoryIndexedDocumentSource(_component, 
                keyXPath, valueXPath, context, cache);

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
                    recurse = Convert.ToBoolean(recurseValue);

                bool warnOverride = true;
                string warningValue = dataNode.GetAttribute("warnOverride", String.Empty);
                if (!String.IsNullOrEmpty(warningValue))
                    warnOverride = Convert.ToBoolean(warningValue);

                // get the files				
                string files = dataNode.GetAttribute("files", String.Empty);
                if (String.IsNullOrEmpty(files))
                    WriteMessage(MessageLevel.Error, "Each data element must have a files attribute specifying which files to index.");
                // if ((files == null) || (files.Length == 0)) throw new BuildComponentException("When instantiating a CopyFromDirectory component, you must specify a directory path using the files attribute.");
                files = Environment.ExpandEnvironmentVariables(files);

                WriteMessage(MessageLevel.Info, String.Format(
                    "Searching for files that match '{0}'.", files));

                DataSource dataSource = null;
                XPathNavigator nodeDataSource = dataNode.SelectSingleNode("source");
                if (nodeDataSource != null)
                {
                    dataSource = new DataSource(false, nodeDataSource);
                    if (dataSource.IsValid)
                    {
                        // Currently, database is supported for systems only...
                        if (!dataSource.IsSystem && !dataSource.IsDatabase)
                        {
                            dataSource = null;
                        }
                    }
                    else
                    {
                        dataSource = null;
                    }
                }      

                if (dataSource != null && dataSource.Exists)
                {
                    if (databaseSource == null)
                    {
                        databaseSource = new DatabaseIndexedDocumentSource(
                            _component, keyXPath, valueXPath, context, cache, true);
                    }

                    if (!databaseSource.IsInitialized)
                    {
                        databaseSource.Initialize(dataSource.OutputDir, false);
                    }
                    else
                    {
                        DatabaseIndexedDocument document = 
                            new DatabaseIndexedDocument(true, false,
                                dataSource.OutputDir);

                        if (document.Exists)
                        {
                            databaseSource.AddDocument(document);
                        }
                        else
                        {
                            memorySource.AddDocuments(baseValue,
                                files, recurse, false, warnOverride);
                        }
                    }    
                }
                else
                {
                    memorySource.AddDocuments(baseValue, files, recurse,
                        true, warnOverride);
                }
            }

            WriteMessage(MessageLevel.Info, String.Format(
                "Indexed {0} elements in {1} files.", 
                memorySource.Count, memorySource.DocumentCount));

            globalData.Add(name, this);
            _documentSources[name] = new IndexedDocumentSources(_component,
                databaseSource, memorySource);
        }

        #endregion

        #region ProcessCommentsIndex Method

        private void ProcessCommentsIndex(XPathNavigator indexNode, CustomContext context,
            IDictionary<string, object> globalData)
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

            HashSet<string> sourceDirs = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            // create the index
            memorySource = new MemoryIndexedDocumentSource(_component, 
                keyXPath, valueXPath, context, cache);

            // Search for the persistent data sources for entries...
            XPathNodeIterator sourcesNodes = indexNode.Select("sources");
            foreach (XPathNavigator sourcesNode in sourcesNodes)
            {
                DataSources dataSources = new DataSources(false, sourcesNode);
                if (dataSources.IsValid)
                {
                    // Currently, database is supported for systems only...
                    if (!dataSources.IsSystem && !dataSources.IsDatabase)
                    {
                        dataSources = null;
                    }
                }
                else
                {
                    dataSources = null;
                }

                if (dataSources != null && dataSources.Exists)
                {
                    if (databaseSource == null)
                    {
                        databaseSource = new DatabaseIndexedDocumentSource(
                            _component, keyXPath, valueXPath, context, cache, true);
                    }

                    if (!databaseSource.IsInitialized)
                    {
                        sourceDirs.UnionWith(dataSources.Sources);

                        databaseSource.Initialize(dataSources.OutputDir, false);
                    }
                    else
                    {
                        DatabaseIndexedDocument document = 
                            new DatabaseIndexedDocument(true, false,
                                dataSources.OutputDir);

                        if (document.Exists)
                        {
                            sourceDirs.UnionWith(dataSources.Sources);

                            databaseSource.AddDocument(document);
                        }
                    }    
                }
            }

            // search the data directories for entries
            XPathNodeIterator dataNodes = indexNode.Select("data");
            foreach (XPathNavigator dataNode in dataNodes)
            {
                string baseValue = dataNode.GetAttribute("base", String.Empty);
                if (!String.IsNullOrEmpty(baseValue))
                    baseValue = Environment.ExpandEnvironmentVariables(baseValue);

                bool isSystem = false;
                string systemValue = dataNode.GetAttribute("system", String.Empty);
                if (!String.IsNullOrEmpty(systemValue))
                    isSystem = Convert.ToBoolean(systemValue);

                bool recurse = false;
                string recurseValue = dataNode.GetAttribute("recurse", String.Empty);
                if (!String.IsNullOrEmpty(recurseValue))
                    recurse = Convert.ToBoolean(recurseValue);

                bool warnOverride = true;
                string warningValue = dataNode.GetAttribute("warnOverride", String.Empty);
                if (!String.IsNullOrEmpty(warningValue))
                    warnOverride = Convert.ToBoolean(warningValue);

                // get the files				
                string files = dataNode.GetAttribute("files", String.Empty);
                if (String.IsNullOrEmpty(files))
                    WriteMessage(MessageLevel.Error, "Each data element must have a files attribute specifying which files to index.");
                // if ((files == null) || (files.Length == 0)) throw new BuildComponentException("When instantiating a CopyFromDirectory component, you must specify a directory path using the files attribute.");
                files = Environment.ExpandEnvironmentVariables(files);

                WriteMessage(MessageLevel.Info, String.Format(
                    "Searching for files that match '{0}'.", files));

                if (isSystem && sourceDirs.Count != 0 &&
                    Directory.Exists(baseValue))
                {
                    // For consistent, we make sure all directory paths
                    // end with backslash...
                    string sourceDir = String.Copy(baseValue);
                    if (!sourceDir.EndsWith("\\"))
                    {
                        sourceDir += "\\";
                    }

                    // If included already in the persistent sources, we
                    // will not load it into memory...
                    if (sourceDirs.Contains(sourceDir))
                    {
                        continue;
                    }
                }

                memorySource.AddDocuments(baseValue, files, recurse,
                    true, warnOverride);
            }

            WriteMessage(MessageLevel.Info, String.Format(
                "Indexed {0} elements in {1} files.", 
                memorySource.Count, memorySource.DocumentCount));

            globalData.Add(name, this);
            _documentSources[name] = new IndexedDocumentSources(_component,
                databaseSource, memorySource);
        }

        #endregion

        #region ProcessIndex Method

        private void ProcessIndex(XPathNavigator indexNode, CustomContext context,
            IDictionary<string, object> globalData)
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
            memorySource = new MemoryIndexedDocumentSource(_component, 
                keyXPath, valueXPath, context, cache);

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
                    recurse = Convert.ToBoolean(recurseValue);

                bool warnOverride = true;
                string warningValue = dataNode.GetAttribute("warnOverride", String.Empty);
                if (!String.IsNullOrEmpty(warningValue))
                    warnOverride = Convert.ToBoolean(warningValue);

                // get the files				
                string files = dataNode.GetAttribute("files", String.Empty);
                if (String.IsNullOrEmpty(files))
                    WriteMessage(MessageLevel.Error, "Each data element must have a files attribute specifying which files to index.");
                // if ((files == null) || (files.Length == 0)) throw new BuildComponentException("When instantiating a CopyFromDirectory component, you must specify a directory path using the files attribute.");
                files = Environment.ExpandEnvironmentVariables(files);

                WriteMessage(MessageLevel.Info, String.Format(
                    "Searching for files that match '{0}'.", files));

                memorySource.AddDocuments(baseValue, files, recurse,
                    false, warnOverride);
            }

            WriteMessage(MessageLevel.Info, String.Format(
                "Indexed {0} elements in {1} files.", 
                memorySource.Count, memorySource.DocumentCount));

            globalData.Add(name, this);
            _documentSources[name] = new IndexedDocumentSources(_component,
                databaseSource, memorySource);
        }

        #endregion

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
                foreach (IndexedDocumentSources sources in 
                    _documentSources.Values)
                {
                    sources.Dispose();
                }

                _documentSources = null;
            }
        }

        #endregion
    }
}
