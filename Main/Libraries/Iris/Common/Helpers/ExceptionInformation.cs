/*
Copyright (c) 2006 Gustavo G. Duarte (http://duartes.org/gustavo)

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
using System.Collections.Generic;
using System.Text;

namespace Common.Helpers {
	/// <summary>
	/// Encapsulates information about an exception.
	/// </summary>
	[Serializable]
	public class ExceptionInformation {
		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionInformation"/> class.
		/// </summary>
		public ExceptionInformation(bool isBug, bool mayRetry) {
			m_isBug = isBug;
			m_mayRetry = mayRetry;
		}

		/// <summary>
		/// Gets a value indicating whether the exception was likely caused by a bug.
		/// </summary>
		public bool IsBug {
			get { return m_isBug; }
		}

		/// <summary>
		/// Gets a value indicating whether the operation that led to the exception may be retried without changes.
		/// </summary>
		public bool MayRetry {
			get { return m_mayRetry; }
		}
		
		/// <summary>
		/// Ready made <see cref="ExceptionInformation" /> for exceptions caused by bugs.
		/// </summary>
		public static readonly ExceptionInformation Bug = new ExceptionInformation(true, false);
		
		/// <summary>
		/// Ready made <see cref="ExceptionInformation" /> for exceptions caused by an action that should not be retried without modification.
		/// </summary>
		public static readonly ExceptionInformation NoRetry = new ExceptionInformation(false, false);
		
		private readonly bool m_mayRetry;
		private readonly bool m_isBug;
	}
}
