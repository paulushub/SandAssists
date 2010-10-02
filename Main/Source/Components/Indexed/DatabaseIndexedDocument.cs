using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using BplusDotNet;

using Microsoft.Ddue.Tools;

using Sandcastle.ReflectionData;

namespace Sandcastle.Components.Indexed
{
    public sealed class DatabaseIndexedDocument : IndexedDocument
    {
        #region Private Fields

        private bool      _isSystem;
        private bool      _isExisted;

        private string    _treeFileName;
        private string    _blockFileName;

        private BplusTree _plusTree;

        private XmlReaderSettings _settings;

        private TargetDictionary  _targetIds;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocument(bool isSystem, bool createNotFound)
        {
            _isSystem = isSystem;

            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir = Path.Combine(assemblyPath, "Data");

            this.Initialize(workingDir, createNotFound);
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

        #endregion

        #region Public Methods

        public override XPathNavigator GetContent(string key)
        {
            XPathNavigator navigator = null;

            if (_plusTree != null)
            {
                string innerXml = null;
                if (_targetIds != null && _targetIds.Contains(key))
                {
                    innerXml = _plusTree[key];
                }
                else if (_plusTree.ContainsKey(key))
                {
                    innerXml = _plusTree[key];
                }

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
                _plusTree[keyNode.Value] = valueNode.InnerXml;
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
                    _plusTree[keyNode.Value] = valueNode.InnerXml;
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

        #region Private Methods

        private void Initialize(string workingDir, bool createNotFound)
        {
            _settings = new XmlReaderSettings();
            _settings.ConformanceLevel = ConformanceLevel.Fragment;

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            if (_isSystem)
            {
                _treeFileName  = Path.Combine(workingDir, "RefT2.6.10621.1.dat");
                _blockFileName = Path.Combine(workingDir, "RefB2.6.10621.1.dat");
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());
                _treeFileName  = Path.Combine(workingDir, tempFile + "Tree.dat");
                _blockFileName = Path.Combine(workingDir, tempFile + "Block.dat");
            }

            if (File.Exists(_treeFileName) && File.Exists(_blockFileName))
            {
                _isExisted = true;
                _plusTree  = hBplusTree.ReOpen(_treeFileName, _blockFileName);
            }
            else
            {
                if (createNotFound)
                {
                    _plusTree = hBplusTree.Initialize(_treeFileName,
                        _blockFileName, 64);
                }
            }

            _targetIds = TargetDictionary.Dictionary;
            if (_targetIds != null)
            {
                if (!_targetIds.Exists || _targetIds.Count == 0)
                {
                    _targetIds = null;
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
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
                            _plusTree.Commit();
                        }
                    }

                    _plusTree.Shutdown();
                    _plusTree = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_treeFileName) &&
                            File.Exists(_treeFileName))
                        {
                            File.Delete(_treeFileName);
                        }
                        if (!String.IsNullOrEmpty(_blockFileName) &&
                            File.Exists(_blockFileName))
                        {
                            File.Delete(_blockFileName);
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
