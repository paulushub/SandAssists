using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace Sandcastle.Components
{
    /// <summary>
    /// Summary description for BuilderComponentException.
    /// </summary>
    [Serializable]
    public class BuildComponentException : ConfigurationErrorsException
    {
        #region Constructors and Destructors

        // Default constructor
        public BuildComponentException()
            : base()
        {
        }

        // Constructor with exception message
        public BuildComponentException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public BuildComponentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected BuildComponentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
