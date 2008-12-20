using System;
using System.Runtime.Serialization;

namespace Sandcastle
{
    /// <summary>
    /// This represents errors that occur in documentation operations.
    /// </summary>
    [Serializable]
    public class BuildException : ApplicationException
    {
        #region Constructors and Destructors

        /// <overloads>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class
        /// with no params.
        /// </summary>
        public BuildException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class, 
        /// with the exception message.
        /// </summary>
        /// <param name="message">
        /// A string setting the message of the exception.
        /// </param>
        public BuildException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class, 
        /// with the exception message and inner exception.
        /// </summary>
        /// <param name="message">
        /// A string setting the message of the exception.
        /// </param>
        /// <param name="inner">A reference to the inner exception.</param>
        public BuildException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class,
        /// for deserialization of the exception class.
        /// </summary>
        /// <param name="info">Represents the SerializationInfo of the exception.</param>
        /// <param name="context">Represents the context information of the exception.</param>
        protected BuildException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
