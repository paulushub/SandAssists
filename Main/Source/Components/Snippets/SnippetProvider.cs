using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public abstract class SnippetProvider : IDisposable
    {
        #region Private Fields

        private Type          _componentType;
        private MessageWriter _messageWriter;

        #endregion

        #region Constructors and Destructor

        protected SnippetProvider(Type componentType, MessageWriter messageWriter)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException("componentType",
                    "The component type cannot be null (or Nothing).");
            }
            if (messageWriter == null)
            {
                throw new ArgumentNullException("messageWriter",
                    "The message writer cannot be null (or Nothing).");
            }

            _componentType = componentType;
            _messageWriter = messageWriter;
        }

        ~SnippetProvider()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public abstract int Count
        {
            get;
        }

        public abstract bool IsMemory
        {
            get;
        }

        public abstract SnippetStorage Storage
        {
            get;
        }

        public abstract IList<SnippetItem> this[SnippetInfo info]
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract bool StartRegister(bool clearExisting);
        public abstract void FinishRegister();

        public abstract void Register(Snippet snippet);
        public abstract void Register(SnippetInfo Info, SnippetItem item);

        public virtual void Register(string snippetGroup, string snippetId,
            string snippetLang, string snippetText)
        {
            if (snippetGroup == null)
            {
                throw new ArgumentNullException("snippetGroup",
                    "The example identifier cannot be null (or Nothing).");
            }
            if (snippetGroup.Length == 0)
            {
                throw new ArgumentException("The example identifier cannot be empty",
                    "snippetGroup");
            }
            if (snippetId == null)
            {
                throw new ArgumentNullException("snippetId",
                    "The snippet identifier cannot be null (or Nothing).");
            }
            if (snippetId.Length == 0)
            {
                throw new ArgumentException("The snippet identifier cannot be empty",
                    "snippetId");
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void WriteMessage(MessageLevel level, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            if (level != MessageLevel.Ignore && _messageWriter != null)
            {
                _messageWriter.Write(_componentType, level, message);
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
        }

        #endregion
    }
}
