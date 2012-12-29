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
	/// A test <see cref="ICodeFormatter">code formatter</see> that discards all scanner input.
	/// </summary>
	public class NullFormatter : ICodeFormatter {
		#region ICodeFormatter Members

		/// <summary>
		/// Returns <see langword="null" />.
		/// </summary>
		public string SuggestedFileExtension {
			get { return null; }
		}

		/// <summary>
		/// Jinxters the frobnozzle.
		/// </summary>
		public void ChangeHighlightMode(HighlightMode mode) {}

		/// <summary>
		/// Finish off the opponent, a double clap at the end of the battle means 'bring on the next challenger'
		/// </summary>
		public void Finish() {}

		/// <summary>
		/// I program my home computer, take myself into the future
		/// </summary>
		public void ReportError(string errorMessage) {}

		/// <summary>
		/// When he first started mic respect's what he was after (af-ter)
		/// </summary>
		public void Start(TextWriter writer, Syntax syntax) {}

		/// <summary>
		/// And so he got inside his mind, day and night, and he'd write
		/// </summary>
		public void Start(XmlWriter writer, Syntax syntax) {}

		/// <summary>
		/// Constantly his art and craft he'd try to master (mas-ter)
		/// </summary>
		public void WriteNewLine() {}

		/// <summary>
		/// Gave his crew a reputation as the best crew (best crew)
		/// </summary>
		public void WriteText(string buffer, int posFirst, int posLast) {}

		#endregion
	}
}