using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Indexed;

namespace Sandcastle.Components.Copiers
{
    public abstract class CopyComponentEx : CopyComponent, IDisposable
    {
        #region Private Fields

        private Type          _thisType;
        private MessageWriter _messageWriter;

        #endregion

        #region Constructors and Destructor

        protected CopyComponentEx(XPathNavigator configuration, Dictionary<string, object> data)
            : base(configuration, data)
        {
            IndexedDocumentController cached = null;

            foreach (KeyValuePair<string, object> item in data)
            {
                cached = item.Value as IndexedDocumentController;
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
                    BuildAssembler assembler = copyComponent.BuildAssembler;

                    if (assembler != null)
                    {
                        _messageWriter = assembler.MessageWriter;
                    }
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

        protected MessageWriter MessageWriter
        {
            get
            {
                return _messageWriter;
            }
        }

        #endregion

        #region Protected Methods

        protected void WriteMessage(MessageLevel level, string message)
        {
            if (_messageWriter == null || level == MessageLevel.Ignore) 
                return;

            _messageWriter.Write(_thisType, level, message);
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
