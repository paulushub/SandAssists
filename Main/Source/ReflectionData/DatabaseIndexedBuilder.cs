using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseIndexedBuilder : IDisposable
    {
        #region Private Fields

        // search pattern for index values
        private XPathExpression _valueExpression;

        // search pattern for the index keys (relative to the index value node)
        private XPathExpression _keyExpression;

        private DatabaseIndexedDocument _document;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedBuilder()
        {
            _document             = new DatabaseIndexedDocument(true);

            // The following are the usual key/value in the configuration file...
            string keyXPath       = "@id";
            string valueXPath     = "/reflection/apis/api";
            CustomContext context = new CustomContext();

            _keyExpression        = XPathExpression.Compile(keyXPath);
            _keyExpression.SetContext(context);

            _valueExpression      = XPathExpression.Compile(valueXPath);
            _valueExpression.SetContext(context);
        }

        ~DatabaseIndexedBuilder()
        {
            this.Dispose(false);
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

        public void AddDocument(string file, bool cacheIt)
        {
            if (_document != null)
            {
                _document.AddDocument(this, file);
            }
        }

        public void AddDocuments(string wildcardPath, bool cacheIt)
        {
            string directoryPart = Path.GetDirectoryName(wildcardPath);
            if (String.IsNullOrEmpty(directoryPart))
                directoryPart = Environment.CurrentDirectory;

            directoryPart = Path.GetFullPath(directoryPart);
            string filePart = Path.GetFileName(wildcardPath);

            string[] files = Directory.GetFiles(directoryPart, filePart);

            foreach (string file in files)
            {
                AddDocument(file, cacheIt);
            }
        }

        public void AddDocuments(string baseDirectory,
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

        public void AddDocuments()
        {   
            string baseDirectory = @"%DXROOT%\Data\Reflection";
            baseDirectory = Environment.ExpandEnvironmentVariables(baseDirectory);

            this.AddDocuments(baseDirectory, "*.xml", true, true);
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
            if (_document != null)
            {
                _document.Dispose();
                _document = null;
            }
        }

        #endregion
    }

    #region CustomContext Class

    internal sealed class CustomContext : XsltContext
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

    internal sealed class CustomVariable : IXsltContextVariable
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
