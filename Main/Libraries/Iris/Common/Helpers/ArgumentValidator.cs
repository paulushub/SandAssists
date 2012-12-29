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
using System.IO;

namespace Common.Helpers {
	/// <summary>
	/// Offers helpful methods for argument validation
	/// </summary>
	public static class ArgumentValidator {
		#region Public members

		public static void ThrowIfDoesNotExist(FileSystemInfo fileSytemObject, string argumentName) {
			ThrowIfNull(fileSytemObject, "fileSytemObject");
			ThrowIfNullOrEmpty(argumentName, "argumentName");

			if (!fileSytemObject.Exists) {
				throw new FileNotFoundException(StringExtensions.Translate("'{0}' not found", fileSytemObject.FullName));
			}
		}

		/// <summary>
		/// Throws a <see cref="ArgumentNullException" /> if <paramref name="argumentToValidate"/> is null.
		/// </summary>
		public static void ThrowIfNull(object argumentToValidate, string argumentName) {
			if (null == argumentName) {
				throw new ArgumentNullException("argumentName");
			}

			if (null == argumentToValidate) {
				throw new ArgumentNullException(argumentName);
			}
		}

		/// <summary>
		/// Throws a <see cref="ArgumentException" /> if <paramref name="argumentToValidate"/> is null or empty.
		/// </summary>
		public static void ThrowIfNullOrEmpty(string argumentToValidate, string argumentName) {
			ThrowIfNull(argumentToValidate, argumentName);

			if (0 == argumentToValidate.Length) {
				throw new ArgumentException(argumentName);
			}
		}

		public static void ThrowIfTrue(bool condition, string msg) {
			ThrowIfNullOrEmpty(msg, "msg");

			if (condition) {
				throw new ArgumentException(msg);
			}
		}

		#endregion
	}
}