using System;
using System.Runtime.Serialization;

namespace Sandcastle.Builders
{
    /// <summary>
    /// Summary description for BuilderException.
    /// </summary>
    [Serializable]
    public class BuilderException : ApplicationException
    {
        #region Constructors and Destructors

        // Default constructor
        public BuilderException()
            : base()
        {
        }

        // Constructor with exception message
        public BuilderException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public BuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected BuilderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
