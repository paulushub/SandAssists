using System;
using System.Runtime.Serialization;

namespace Sandcastle
{
    /// <summary>
    /// Summary description for BuildException.
    /// </summary>
    [Serializable]
    public class BuildException : ApplicationException
    {
        #region Constructors and Destructors

        // Default constructor
        public BuildException()
            : base()
        {
        }

        // Constructor with exception message
        public BuildException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public BuildException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected BuildException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
