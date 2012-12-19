using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    internal sealed class DatabaseIndexedDocument : IDisposable
    {
        #region Private Fields

        private bool      _isSystem;
        private bool      _isComments;

        private string    _dataDir;

        // search pattern for index values
        private XPathExpression _valueExpression;

        // search pattern for the index keys (relative to the index value node)
        private XPathExpression _keyExpression;

        private XmlReaderSettings _settings;

        private PersistentDictionary<string, string> _plusTree;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedDocument(bool isSystem, bool isComments, 
            string workingDir)
        {
            _isSystem   = isSystem;
            _isComments = isComments;
            _dataDir    = workingDir;

            string keyXPath       = null;
            string valueXPath     = null;
            CustomContext context = new CustomContext();

            // The following are the usual key/value in the configuration file...
            if (_isComments)
            {
                // <index name="comments" value="/doc/members/member" key="@name" cache="100">
                keyXPath   = "@name";
                valueXPath = "/doc/members/member";
            }
            else
            {
                //  <index name="reflection" value="/reflection/apis/api" key="@id" cache="10">
                keyXPath   = "@id";
                valueXPath = "/reflection/apis/api";
            }

            _keyExpression = XPathExpression.Compile(keyXPath);
            _keyExpression.SetContext(context);

            _valueExpression = XPathExpression.Compile(valueXPath);
            _valueExpression.SetContext(context);

            if (PersistentDictionaryFile.Exists(_dataDir))
            {
                PersistentDictionaryFile.DeleteFiles(_dataDir);
            }

            _plusTree = new PersistentDictionary<string, string>(_dataDir);
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
                if (!String.IsNullOrEmpty(_dataDir) && Directory.Exists(_dataDir))
                {
                    return PersistentDictionaryFile.Exists(_dataDir);
                }

                return false;
            }
        }

        public bool IsSystem
        {
            get
            {
                return _isSystem;
            }
        }

        public bool IsComments
        {
            get
            {
                return _isComments;
            }
        }

        public XPathExpression ValueExpression
        {
            get
            {
                return _valueExpression;
            }
        }

        public XPathExpression KeyExpression
        {
            get
            {
                return _keyExpression;
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

        public void AddDocument(string file)
        {
            BuildExceptions.NotNull(file,   "file");

            // load the document
            XPathDocument document = new XPathDocument(file);

            // search for value nodes
            XPathNodeIterator valueNodes =
                document.CreateNavigator().Select(_valueExpression);

            // get the key string for each value node and record it in the index
            foreach (XPathNavigator valueNode in valueNodes)
            {
                XPathNavigator keyNode = valueNode.SelectSingleNode(_keyExpression);
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

        #region CustomContext Class

        private sealed class CustomContext : XsltContext
        {
            // variable control
            private Dictionary<string, IXsltContextVariable> variables;

            public CustomContext()
                : base()
            {
                variables = new Dictionary<string, IXsltContextVariable>();
            }

            public string this[string variable]
            {
                get
                {
                    return (variables[variable].Evaluate(this).ToString());
                }
                set
                {
                    variables[variable] = new CustomVariable(value);
                }
            }

            public bool ClearVariable(string name)
            {
                return (variables.Remove(name));
            }

            public void ClearVariables()
            {
                variables.Clear();
            }

            // Implementation of XsltContext methods

            public override IXsltContextVariable ResolveVariable(
                string prefix, string name)
            {
                return (variables[name]);
            }

            public override IXsltContextFunction ResolveFunction(
                string prefix, string name, XPathResultType[] argumentTypes)
            {
                throw new NotImplementedException();
            }

            public override int CompareDocument(string baseUri, string nextBaseUri)
            {
                return (0);
            }

            public override bool Whitespace
            {
                get
                {
                    return (true);
                }
            }

            public override bool PreserveWhitespace(XPathNavigator node)
            {
                return (true);
            }
        }

        private sealed class CustomVariable : IXsltContextVariable
        {
            private string value;

            public CustomVariable(string value)
            {
                this.value = value;
            }

            public bool IsLocal
            {
                get
                {
                    return (false);
                }
            }

            public bool IsParam
            {
                get
                {
                    return (false);
                }
            }

            public XPathResultType VariableType
            {
                get
                {
                    return (XPathResultType.String);
                }
            }

            public Object Evaluate(XsltContext context)
            {
                return (value);
            }
        }

        #endregion
    }
}
