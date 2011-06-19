using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseIndexedDocument
    {
        #region Private Fields

        private bool      _isSystem;
        private bool      _isExisted;

        private string    _dataDir;

        private XmlReaderSettings _settings;

        private PersistentDictionary<string, string> _plusTree;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocument(bool isSystem)
        {
            _isSystem = isSystem;

            _settings = new XmlReaderSettings();
            _settings.ConformanceLevel = ConformanceLevel.Fragment;            

            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir    = Path.Combine(assemblyPath, "Data");
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            if (_isSystem)
            {
                _dataDir = Path.Combine(workingDir, "RefT26106211");
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());
                _dataDir = Path.Combine(workingDir, tempFile);
            }

            _isExisted = PersistentDictionaryFile.Exists(_dataDir);
            if (_isExisted)
            {
                _plusTree = new PersistentDictionary<string, string>(_dataDir);
            }
            else
            {
                _plusTree = new PersistentDictionary<string, string>(_dataDir);
            }
        }

        ~DatabaseIndexedDocument()
        {
            this.Dispose(false);
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

        #endregion

        #region Public Methods

        public XPathNavigator GetContent(string key)
        {
            XPathNavigator navigator = null;

            if (_plusTree != null && _plusTree.ContainsKey(key))
            {
                string innerXml = _plusTree[key];

                if (String.IsNullOrEmpty(innerXml))
                {
                    return null;
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
            BuildExceptions.NotNull(builder, "builder");
            BuildExceptions.NotNull(file,   "file");

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
                _plusTree[keyNode.Value] = valueNode.InnerXml;
            }
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
            if (_plusTree != null)
            {
                try
                {
                    // Save the system reflection database, if newly created...
                    if (_isSystem)
                    {
                        if (!_isExisted)
                        {
                        }
                    }

                    _plusTree.Dispose();
                    _plusTree = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_dataDir) &&
                            Directory.Exists(_dataDir))
                        {
                            PersistentDictionaryFile.DeleteFiles(_dataDir);
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
