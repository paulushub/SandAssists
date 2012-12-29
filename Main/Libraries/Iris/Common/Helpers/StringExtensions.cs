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
using System.Globalization;

namespace Common.Helpers {
	/// <summary>
	/// Provides utility extension methods for System.string
	/// </summary>
	public static class StringExtensions {
		#region Public members

		public static readonly Char[] MagicRegexChars = new Char[] {'^', '$', '.', '(', ')', '{', '\\', '[', '?', '+', '*', '|'};

		public static bool HasMagicRegexChars(string s) {
			ArgumentValidator.ThrowIfNull(s, "s");

			return -1 != s.IndexOfAny(MagicRegexChars);
		}

		public static char LastChar(string s) {
			if (string.IsNullOrEmpty(s)) {
				throw (new InvalidOperationException(Fi("s must not be null or empty")));
			}

			return s[s.Length - 1];
		}

		/// <summary>
		/// Performs a <see cref="StringComparison.OrdinalIgnoreCase"/> comparison between two <see cref="String.Trim()">trimmed</see> strings.
		/// </summary>
		/// <returns>True if strings match in an ordinal case insensive comparison, otherwise false.</returns>
		public static bool NormalEquals(string s, string stringToCompare) {
			ArgumentValidator.ThrowIfNull(s, "s");

			if (null == stringToCompare) {
				return false;
			}

			return s.Trim().Equals(stringToCompare.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		public static string Or(string s, string alternative) {
			if (string.IsNullOrEmpty(s)) {
				return alternative;
			}

			return s;
		}

		public static string Reverse(string s) {
			ArgumentValidator.ThrowIfNull(s, "s");

			Char[] chars = s.ToCharArray();
			Array.Reverse(chars);

			return new string(chars);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// <para>This should <b>not</b> be used for any strings that are displayed to the user. It is meant for log
		/// messages, exception messages, and other types of information that do not make it into the UI, or wouldn't
		/// make sense to users anyway ;).</para>
		/// </remarks>
		public static string Fi(string format, params object[] args) {
			ArgumentValidator.ThrowIfNull(format, "format");

			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		/// <summary>
		/// Returns a trimmed, invariant upper case version of this <see cref="string" />.
		/// </summary>
		public static string Normalize(string s) {
			ArgumentValidator.ThrowIfNull(s, "s");

			return s.Trim().ToUpperInvariant();
		}

		public static string ToNullIfEmpty(string s) {
			ArgumentValidator.ThrowIfNull(s, "s");

			if (0 == s.Length) {
				return null;
			} else {
				return s;
			}
		}

		/// <summary>
		/// Translates this string using current culture.
		/// </summary>
		public static string Translate(string s, params object[] args) {
			ArgumentValidator.ThrowIfNull(s, "s");

			return String.Format(CultureInfo.CurrentCulture, s, args);
		}

		#endregion
	}
}