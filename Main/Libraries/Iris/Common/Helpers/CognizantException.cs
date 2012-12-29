using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Common.Helpers;

namespace Common.Helpers {
	/// <summary>
	/// Represents an exception that has information such as why it happened, whether to retry, whether it indicates a bug, and so on.
	/// </summary>
	[Serializable]
	public abstract class CognizantException : Exception {
		protected CognizantException() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CognizantException" /> class with a specified error message.
		/// </summary>
		protected CognizantException(ExceptionInformation exceptionInformation, string message) : base(message) {
			ArgumentValidator.ThrowIfNull(exceptionInformation, "exceptionInformation");
			
			m_exceptionInformation = exceptionInformation;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CognizantException" /> class with serialized data.
		/// </summary>
		protected CognizantException(SerializationInfo info, StreamingContext context) : base(info, context) {
			if (info != null) {
				m_exceptionInformation	= (ExceptionInformation) info.GetValue("m_exceptionInformation", typeof(ExceptionInformation));
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CognizantException" /> class with the specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		protected CognizantException(ExceptionInformation exceptionInformation, string message, Exception innerException)
			: base(message, innerException) {
			ArgumentValidator.ThrowIfNull(exceptionInformation, "exceptionInformation");
			
			m_exceptionInformation = exceptionInformation;
		}

		/// <summary>
		/// Adds <see cref="ExceptionInformation" /> to a serialized <see cref="CognizantException" />.
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData (info, context);

			if (info != null) {
				info.AddValue("m_exceptionInformation", m_exceptionInformation);
			}
		}
		
		private readonly ExceptionInformation m_exceptionInformation;
	}
}
