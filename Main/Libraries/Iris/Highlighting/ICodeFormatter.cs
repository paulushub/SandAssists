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

using System.IO;
using System.Xml;

namespace Iris.Highlighting {
	/// <summary>
	/// Defines the interface for Iris code formatters. They are normally driven by an <see cref="IScanner" />.
	/// </summary>
	public interface ICodeFormatter {
		/// <summary>
		/// Gets the suggested file extension in case the caller wants to save formatted output to a file.
		/// </summary>
		/// <value>The suggested file extension.</value>
		string SuggestedFileExtension { get; }

		/// <summary>
		/// Changes the current <see cref="HighlightMode">mode</see> for text being written.
		/// </summary>
		/// <param name="mode"></param>
		void ChangeHighlightMode(HighlightMode mode);

		/// <summary>
		/// Finish working on a chunk of text to be highlighted. You can reuse the <see cref="ICodeFormatter">formatter</see>.
		/// </summary>
		void Finish();

		/// <summary>
		/// Provides a mechanism for the <see cref="ICodeFormatter">formatter</see> to output an error message in a format-specific way.
		/// </summary>
		void ReportError(string errorMessage);

		/// <summary>
		/// Start working on new input.
		/// </summary>
		void Start(TextWriter writer, Syntax syntax);

		/// <summary>
		/// Start working on new input with output going to a <see cref="XmlWriter" />.
		/// </summary>
		void Start(XmlWriter writer, Syntax syntax);

		/// <summary>
		/// Writes new line
		/// </summary>
		void WriteNewLine();

		/// <summary>
		/// Writes text to the output. Text will be highlighted according to the current <see cref="HighlightMode">mode</see>.
		/// </summary>
		/// <param name="buffer">Buffer containing text to be written</param>
		/// <param name="posFirst">First position in buffer to be written, inclusive.</param>
		/// <param name="posLast">Last position in buffer to be written, inclusive.</param>
		void WriteText(string buffer, int posFirst, int posLast);
	}
}