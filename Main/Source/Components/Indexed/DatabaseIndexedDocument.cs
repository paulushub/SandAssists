using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

using Sandcastle.ReflectionData;

namespace Sandcastle.Components.Indexed
{
    public sealed class DatabaseIndexedDocument : IndexedDocument
    {
        #region Private Fields

        private bool      _isSystem;
        private bool      _isExisted;
        private bool      _isInitialized;

        private string    _indexedDataDir;

        private XmlReaderSettings _settings;

        private PersistentDictionary<string, string> _indexedDocument;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocument(bool isSystem)
        {
            _isSystem = isSystem;

            //string assemblyPath = Path.GetDirectoryName(
            //    Assembly.GetExecutingAssembly().Location);

            //string workingDir = Path.Combine(assemblyPath, "Data");

            //this.Initialize(workingDir, createNotFound);
        }

        public DatabaseIndexedDocument(bool isSystem, bool createNotFound,
            string workingDir)
        {
            _isSystem = isSystem;

            this.Initialize(workingDir, createNotFound);
        }

        #endregion

        #region Public Properties

        public bool Exists
        {
            get
            {
                return _isExisted;
            }
        }

        public bool IsSystem
        {
            get
            {
                return _isSystem;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(string workingDir, bool createNotFound)
        {
            if (String.IsNullOrEmpty(workingDir) || _indexedDocument != null)
            {
                return;
            }

            if (Directory.Exists(workingDir))
            {
                _isExisted = PersistentDictionaryFile.Exists(workingDir);
                if (_isExisted)
                {
                    _indexedDataDir  = workingDir;
                    _indexedDocument = new PersistentDictionary<string, string>(_indexedDataDir);

                    _isInitialized   = true;
                }
                else
                {
                    if (createNotFound)
                    {
                        _indexedDataDir = workingDir;
                       _indexedDocument = new PersistentDictionary<string, string>(_indexedDataDir);
                        
                        _isInitialized  = true;
                    }
                }
            }
            else
            {
                if (createNotFound)
                {
                    Directory.CreateDirectory(workingDir);

                    _indexedDataDir  = workingDir;
                    _indexedDocument = new PersistentDictionary<string, string>(_indexedDataDir);
                    
                    _isInitialized   = true;
                }
            }
        }

        public void Uninitialize()
        {
            _isInitialized = false;
        }

        public override XPathNavigator GetContent(string key)
        {
            XPathNavigator navigator = null;

            if (_indexedDocument != null)
            {
                string innerXml = null;
                if (_indexedDocument.ContainsKey(key))
                {
                    innerXml = _indexedDocument[key];
                }

                if (String.IsNullOrEmpty(innerXml))
                {
                    return null;
                }

                if (_settings == null)
                {
                    _settings = new XmlReaderSettings();
                    _settings.ConformanceLevel = ConformanceLevel.Fragment;
                }

                StringReader textReader = new StringReader(innerXml);
                using (XmlReader reader = XmlReader.Create(textReader, _settings))
                {
                    XPathDocument document = new XPathDocument(reader);
                    navigator = document.CreateNavigator();
                }
            }

            return navigator;
        }

        public void AddDocument(DatabaseIndexedBuilder builder, string file)
        {
            BuildComponentExceptions.NotNull(builder, "builder");
            BuildComponentExceptions.NotNull(file,   "file");

            // load the document
            XPathDocument document = new XPathDocument(file);

            // search for value nodes
            XPathNodeIterator valueNodes =
                document.CreateNavigator().Select(builder.ValueExpression);

            // get the key string for each value node and record it in the index
            foreach (XPathNavigator valueNode in valueNodes)
            {
                XPathNavigator keyNode = valueNode.SelectSingleNode(
                    builder.KeyExpression);
                if (keyNode == null)
                {
                    continue;
                }

                // The outer container interferes with the processing, so
                // we use the inner XML of the node...
                _indexedDocument[keyNode.Value] = valueNode.InnerXml;
            }
        }

        public void AddDocument(IndexedDocumentSource source, string file)
        {
            BuildComponentExceptions.NotNull(source, "source");
            BuildComponentExceptions.NotNull(file,   "file");

            // load the document
            try
            {
                XPathDocument document = new XPathDocument(file);

                // search for value nodes
                XPathNodeIterator valueNodes =
                    document.CreateNavigator().Select(source.ValueExpression);

                // get the key string for each value node and record it in the index
                foreach (XPathNavigator valueNode in valueNodes)
                {
                    XPathNavigator keyNode = valueNode.SelectSingleNode(
                        source.KeyExpression);
                    if (keyNode == null)
                    {
                        continue;
                    }

                    // The outer container interferes with the processing, so
                    // we use the inner XML of the node...
                    _indexedDocument[keyNode.Value] = valueNode.InnerXml;
                }
            }
            catch (IOException e)
            {
                source.WriteMessage(MessageLevel.Error,
                    String.Format("An access error occurred while attempting to load the file '{0}'. The error message is: {1}", file, e.Message));
            }
            catch (XmlException e)
            {
                source.WriteMessage(MessageLevel.Error,
                    String.Format("The indexed document '{0}' is not a valid XML document. The error message is: {1}", file, e.Message));
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_indexedDocument != null)
            {
                try
                {
                    // Save the system reflection database, if newly created...
                    _indexedDocument.Dispose();
                    _indexedDocument = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_indexedDataDir) && 
                            Directory.Exists(_indexedDataDir))
                        {
                            PersistentDictionaryFile.DeleteFiles(_indexedDataDir);
                        }
                    }
                }
                catch
                {                  	
                }
            }
        }

        #endregion
    }
}
