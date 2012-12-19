using System;

namespace Microsoft.Ddue.Tools
{
    public enum MessageLevel
    {
        Ignore = 0,     // don't show at all
        Info   = 1,		// informational message
        Warn   = 2,		// a minor problem occurred
        Error  = 3	    // a major problem occurred
    }

    #region MessageWriter Class

    public abstract class MessageWriter : MarshalByRefObject
    {
        #region Private Fields

        private int _ignores;
        private int _warns;
        private int _errors;

        #endregion

        #region Constructors and Destructor

        protected MessageWriter()
        {
        }

        #endregion

        #region Public Properties

        public virtual int Ignores
        {
            get
            {
                return _ignores;
            }
        }

        public virtual int Warns
        {
            get
            {
                return _warns;
            }
        }

        public virtual int Errors
        {
            get
            {
                return _errors;
            }
        }

        public virtual bool ErrorOccurred
        {
            get
            {
                return (_errors > 0);
            }
        }

        #endregion

        #region Public Methods

        public virtual void Write(Type type, MessageLevel level, 
            string message)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type",
                    "The type is required and cannot be null (or Nothing).");
            }

            // If there is error, we will ignore other messages except errors...
            if (_errors > 0 && level != MessageLevel.Error)
            {
                return;
            }

            switch (level)
            {
                case MessageLevel.Ignore:
                    _ignores++;
                    break;
                case MessageLevel.Info:
                    break;
                case MessageLevel.Warn:
                    _warns++;
                    break;
                case MessageLevel.Error:
                    _errors++;
                    break;
            }

            this.OnWrite(type, level, message);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region Protected Methods

        protected abstract void OnWrite(Type type, MessageLevel level,
            string message);

        #endregion
    }

    #endregion

    #region ConsoleMessageWriter Class

    public sealed class ConsoleMessageWriter : MessageWriter
    {
        public ConsoleMessageWriter()
        {
        }

        protected override void OnWrite(Type type, MessageLevel level,
            string message)
        {
            WriteMessage(level, String.Format("{0}: {1}", type.Name, message));
        }

        public static void WriteMessage(MessageLevel level, string message)
        {
            Console.WriteLine("{0}: {1}", level, message);
        }

        public static void WriteMessage(MessageLevel level, string format, 
            params object[] arg)
        {
            Console.Write("{0}: ", level);
            Console.WriteLine(format, arg);
        }
    }

    #endregion
}
