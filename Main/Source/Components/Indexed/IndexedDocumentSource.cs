using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Indexed
{
    public abstract class IndexedDocumentSource : IDisposable
    {
        #region Private Fields

        private Type           _thisType;
        private BuildComponent _component;
        private BuildAssembler _assembler;

        // search pattern for index values
        private XPathExpression _valueExpression;

        // search pattern for the index keys (relative to the index value node)
        private XPathExpression _keyExpression;

        #endregion

        #region Constructors and Destructor

        protected IndexedDocumentSource(BuildComponent component)
        {
            BuildComponentExceptions.NotNull(component, "component");

            _component = component;
            _thisType  = this.GetType();
            _assembler = component.BuildAssembler;
        }

        public IndexedDocumentSource(CopyFromIndexComponent component, 
            string keyXPath, string valueXPath, XmlNamespaceManager context,
            int cacheSize) : this(component)
        {       
            try
            {
                _keyExpression = XPathExpression.Compile(keyXPath);
            }
            catch (XPathException)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The key expression '{0}' is not a valid XPath expression.", keyXPath));
            }
            _keyExpression.SetContext(context);

            try
            {
                _valueExpression = XPathExpression.Compile(valueXPath);
            }
            catch (XPathException)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The value expression '{0}' is not a valid XPath expression.", valueXPath));
            }
            _valueExpression.SetContext(context);
        }

        ~IndexedDocumentSource()
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

        public abstract XPathNavigator GetContent(string key);

        public abstract void AddDocument(string file, bool cacheIt, 
            bool warnOverride);

        public abstract void AddDocuments(string wildcardPath,
            bool cacheIt, bool warnOverride);

        public abstract void AddDocuments(string baseDirectory,
            string wildcardPath, bool recurse, bool cacheIt, bool warnOverride);

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

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
