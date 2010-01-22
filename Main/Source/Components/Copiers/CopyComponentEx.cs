using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Copiers
{
    public abstract class CopyComponentEx : CopyComponent, IDisposable
    {
        #region Private Fields

        private Type           _thisType;
        private BuildAssembler _assembler;

        #endregion

        #region Constructors and Destructor

        protected CopyComponentEx(XPathNavigator configuration, Dictionary<string, object> data)
            : base(configuration, data)
        {
            IndexedDocumentCache cached = null;

            foreach (KeyValuePair<string, object> item in data)
            {
                cached = item.Value as IndexedDocumentCache;
                if (cached != null)
                {
                    break;
                }
            }

            if (cached !=  null)
            {
                BuildComponent copyComponent = cached.Component;
                if (copyComponent != null)
                {
                    _assembler = copyComponent.BuildAssembler;
                }
            }

            _thisType = this.GetType();
        }

        ~CopyComponentEx()
        {
            this.Dispose(false);
        }

        #endregion

        #region Protected Properties

        protected BuildAssembler Assembler
        {
            get
            {
                return _assembler;
            }
        }

        #endregion

        #region Protected Methods

        protected void WriteMessage(MessageLevel level, string message)
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
