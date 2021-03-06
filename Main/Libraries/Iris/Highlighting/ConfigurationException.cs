﻿/*
Copyright (c) 2007 Gustavo G. Duarte (http://duartes.org/gustavo)

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

using System;
using System.Runtime.Serialization;
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Thrown when Iris detects a problem in its configuration, which is stored in the <b>IrisCatalog.xml</b> file.
	/// </summary>
	[Serializable]
	public class ConfigurationException : CognizantException {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		public ConfigurationException() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		public ConfigurationException(string message) : base(ExceptionInformation.NoRetry, message) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		public ConfigurationException(string message, Exception inner) : base(ExceptionInformation.NoRetry, message, inner) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}