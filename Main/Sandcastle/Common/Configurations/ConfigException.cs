using System;
using System.Runtime.Serialization;

namespace Sandcastle.Configurations
{
    /// <summary>
    /// Summary description for ConfigException.
    /// </summary>
    [Serializable]
    public class ConfigException : ApplicationException
    {
        #region Constructors and Destructors

        // Default constructor
        public ConfigException()
            : base()
        {
        }

        // Constructor with exception message
        public ConfigException(string message)
            : base(message)
        {
        }

        // Constructor with message and inner exception
        public ConfigException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Protected constructor to de-serialize data
        protected ConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
