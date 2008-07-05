using System;
using System.Runtime.Serialization;

namespace Sandcastle.Builders.Tasks
{
    /// <summary>
    /// Summary description for TaskException.
    /// </summary>
    [Serializable]
    public class TaskException : HelpException
    {
        #region Constructors and Destructors

        // Default constructor
        public TaskException()
            : base()
        {
        }

        // Constructor with exception message
        public TaskException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public TaskException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected TaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Static Methods

        public static void ThrowNoContext(string taskName)
        {

        }

        public static void ThrowTaskNotInitialized(string taskName)
        {

        }

        public static void ThrowTaskNoLogger(string taskName)
        {

        }

        #endregion
    }
}
