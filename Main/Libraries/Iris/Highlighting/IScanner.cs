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

namespace Iris.Highlighting {
	/// <summary>
	/// Defines the interface that a scanner (tokenizer) must implement to work with Iris
	/// </summary>
	public interface IScanner {
		#region Public members

		/// <summary>
		/// The id for the <see cref="Syntax" /> highlighted by this scanner
		/// </summary>
		string SyntaxId { get; }

		/// <summary>
		/// Scans the input by reading from a <see cref="TextReader" />. Used for inputs up to 2GB.
		/// </summary>
		void Scan(TextReader input, ICodeFormatter formatter);

		/// <summary>
		/// Scans the input by reading from a <see cref="string" />
		/// </summary>
		void Scan(string input, ICodeFormatter formatter);

		#endregion
	}
}