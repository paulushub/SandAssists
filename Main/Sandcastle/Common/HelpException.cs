using System;
using System.Runtime.Serialization;

namespace Sandcastle
{
    /// <summary>
    /// Summary description for HelpException.
    /// </summary>
    [Serializable]
    public class HelpException : ApplicationException
    {
        #region Constructors and Destructors

        // Default constructor
        public HelpException()
            : base()
        {
        }

        // Constructor with exception message
        public HelpException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public HelpException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected HelpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
