using System;
using System.Runtime.Serialization;

namespace Sandcastle.Controls
{
    /// <summary>
    /// Summary description for ControlException.
    /// </summary>
    [Serializable]
    public class ControlException : ApplicationException
    {
        #region Constructors and Destructors

        // Default constructor
        public ControlException()
            : base()
        {
        }

        // Constructor with exception message
        public ControlException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public ControlException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected ControlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
