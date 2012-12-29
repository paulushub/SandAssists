/*
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
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Provides options to control the <see cref="XhtmlFormatter" /> output.
	/// </summary>
	/// <remarks>
	/// <para>Please see <see cref="XhtmlFormatter" /> for more discussion on outputting XHTML.</para>
	/// </remarks>
	public class XhtmlOptions {
		static XhtmlOptions() {
			Defaults = new XhtmlOptions();
			DefaultsWithoutStyleElement = new XhtmlOptions();
			DefaultsWithoutStyleElement.EmitStyleTag = false;
		}

		#region Public members


		/// <summary>
		/// Instantiates a new <see cref="XhtmlOptions" />.
		/// </summary>
		public XhtmlOptions() {
			this.EmitLineNumbers = true;
			this.EmitStyleTag = true;
			this.MarkNthLineNumber = true;
		}


		/// <summary>
		/// Provides a default ready-made <see cref="XhtmlOptions" /> object.
		/// </summary>
		public static readonly XhtmlOptions Defaults;

		/// <summary>
		/// Provides an <see cref="XhtmlOptions" /> object with <see cref="EmitStyleTag" /> set to false
		/// </summary>
		public static readonly XhtmlOptions DefaultsWithoutStyleElement;

		private CssScheme m_cssScheme;

		/// <summary>
		/// Whether to emit a &lt;style&gt; tag containing the CSS stylesheet used in highlighting
		/// </summary>
		public bool EmitStyleTag { get; set; }

		private int m_nthLineNumber = 10;
		private string m_newLineChars = Environment.NewLine;

//		/// <summary>
//		/// Whether to emit a toolbar atop the highlighted output with functions like selecting all code, etc.
//		/// </summary>
//		public bool EmitToolbar;

		/// <summary>
		/// Whether to emit line numbers along with the highlighted output
		/// </summary>
		public bool EmitLineNumbers { get; set; }

		/// <summary>
		/// Whether to emit every nth line number with the 'nthLineNumber' CSS class
		/// </summary>
		public bool MarkNthLineNumber { get; set; }

		/// <summary>
		/// Every nth number that must be emitted with the 'nthLineNumber' CSS class
		/// </summary>
		public int NthLineNumber {
			get { return m_nthLineNumber; }
			set {
				if (value <= 0) {
					throw new ArgumentException("value", StringExtensions.Fi("Invalid nth line number value: {0}. It must be greater than zero. Set "
						+ "MarkNthLineNumber to false if you wish to turn this feature off.", value));
				}
				this.m_nthLineNumber = value;
			}
		}

		/// <summary>
		/// Specifies the <see cref="Highlighting.CssScheme">CSS scheme</see> for highlighting. Only matters if <see cref="EmitStyleTag" /> is true.
		/// </summary>
		public CssScheme CssScheme {
			get { return this.m_cssScheme ?? CssScheme.DefaultCssScheme; }
			set {
				ArgumentValidator.ThrowIfNull(value, "value");
				this.m_cssScheme = value; 
			}
		}

		/// <summary>
		/// Specifies characters to use for new line
		/// </summary>
		public string NewLineChars {
			get { return this.m_newLineChars; }
			set {
				ArgumentValidator.ThrowIfNull(value, "value");
				this.m_newLineChars = value;
			}
		}

		#endregion
	}
}