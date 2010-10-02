using System;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public abstract class SnippetReader : IDisposable
    {
        #region Private Fields

        private int            _tabSize;
        private Type           _componentType;
        private MessageHandler _messageHandler;

        #endregion

        #region Constructors and Destructor

        protected SnippetReader(int tabSize, Type componentType, 
            MessageHandler messageHandler)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException("componentType",
                    "The component type cannot be null (or Nothing).");
            }
            if (messageHandler == null)
            {
                throw new ArgumentNullException("messageHandler",
                    "The message handler cannot be null (or Nothing).");
            }

            _tabSize        = tabSize;
            _componentType  = componentType;
            _messageHandler = messageHandler;
        }

        ~SnippetReader()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public int TabSize
        {
            get
            {
                return _tabSize;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Read(string dataSource, SnippetProvider provider);
        public abstract void Read(IList<string> dataSources, SnippetProvider provider);

        #endregion

        #region Protected Methods

        protected virtual void WriteMessage(MessageLevel level, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            if (level != MessageLevel.Ignore && _messageHandler != null)
            {
                _messageHandler(_componentType, level, message);
            }
        }

        protected virtual void WriteMessage(MessageLevel level, Exception ex)
        {
            this.WriteMessage(level, ex.ToString());
            //this.WriteMessage(level, String.Format("Exception({0}) - {1}",
            //    ex.GetType().FullName, ex.Message));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _componentType  = null;
            _messageHandler = null;
        }

        #endregion
    }
}
