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

using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Highlights input according to a syntax definition. This is the central class in Iris.
	/// </summary>
	/// <remarks>
	/// <para>
	///		<see cref="Highlighter" /> is the class responsible for coordinating the transformation of textual input into highlighted output. It relies
	///		on two main components, a <see cref="IScanner">scanner</see> and a <see cref="ICodeFormatter">formatter</see>. The <see cref="IScanner">scanner</see>
	///		decides which <see cref="HighlightMode">mode</see> (eg, <see cref="HighlightMode.String" /> or <see cref="HighlightMode.Constant" />) should apply for 
	///		each character in the input. The <see cref="ICodeFormatter">formatter</see> receives this information from the scanner and produces output in a certain
	///		format (eg, <see cref="XmlFormatter" /> or <see cref="XhtmlFormatter" />). The highlighter itself does no work, it
	///		simply relies on these two objects to get things done.
	/// </para>
	/// <para>
	///		You can reuse Highlighters as many times as you wish. Creating highlighters is not expensive, but if you are highlighting often
	///		you may want to cache your <see cref="Highlighter" />. Each individual instance is <b>not</b> thread safe. Here are some examples:
	/// </para>
	/// <example>
	/// <para>For quick and dirty highlighting, you can use the static <see cref="Highlight(string,string)" /> method. The downside
	/// is that you don't get to set any options.</para>
	/// <code>
	///		string someCsharp = Highlighter.Highlight("cs", "public void Foo() {}");
	///		string someC = Highlighter.Highlight("c", "printf(\"%s\", evilHackerInput)");
	///		string someLua = Highlighter.Highlight("lua", "print \"Oi, mundo! Oi, PUC-RJ!\"");
	/// </code>
	/// </example>
	/// <example>
	/// <para>For better control over settings you may instantiate your own <see cref="Highlighter" />:</para>
	/// <code>
	///		XhtmlFormatter formatter = new XhtmlFormatter(XhtmlOptions.Defaults);
	///		Highlighter csHighlighter = new Highlighter("cs", formatter);
	///		string prettyCSharp = csHighlighter.Highlight("public void Foozboo() {}");
	///
	///		// highlight a file
	///		humoungousCSharpSource = File.OpenText("frozBlaster.cs");
	///		outputFile = File.CreateText("frozBlaster.cs.html");
	///		csHighlighter.Highlight(humongousCSharpSource, outputFile);
	/// </code>
	/// </example>
	/// </remarks>
	public class Highlighter {
		#region Public members

		/// <summary>
		/// Instantiates a new <see cref="Highlighter" /> using the specified syntax id using a default <see cref="XhtmlFormatter" />.
		/// </summary>
		/// <param name="syntaxId">A syntax id that can be resolved via the <see cref="SyntaxCatalog" />.</param>
		public Highlighter(string syntaxId) : this(syntaxId, new XhtmlFormatter()) {}

		/// <summary>
		/// Instantiates a new <see cref="Highlighter" /> using the specified syntax id and formatter.
		/// </summary>
		/// <param name="syntaxIdOrAlias">A syntax id that can be resolved via the <see cref="SyntaxCatalog" />.</param>
		/// <param name="formatter">The <see cref="ICodeFormatter">formatter</see> to be used for output generation.</param>
		public Highlighter(string syntaxIdOrAlias, ICodeFormatter formatter) {
			ArgumentValidator.ThrowIfNullOrEmpty(syntaxIdOrAlias, "syntaxIdOrAlias");
			ArgumentValidator.ThrowIfNull(formatter, "formatter");

			this.Init(SyntaxCatalog.GetSyntaxBy(syntaxIdOrAlias), formatter);
		}

		/// <summary>
		/// Instantiates a new <see cref="Highlighter" /> for the given <see cref="Syntax" /> using the specified formatter
		/// </summary>
		/// <param name="syntax">Any <see cref="Syntax" /> whose syntax definition file is present, regardless of whether it's in the <see cref="SyntaxCatalog" />.</param>
		/// <param name="formatter">The <see cref="ICodeFormatter">formatter</see> to be used for the output generation.</param>
		public Highlighter(Syntax syntax, ICodeFormatter formatter) {
			ArgumentValidator.ThrowIfNull(syntax, "syntax");
			ArgumentValidator.ThrowIfNull(formatter, "formatter");

			this.Init(syntax, formatter);
		}

		/// <summary>
		/// A convenience method to highlight input without instantiating a <see cref="Highlighter" />.
		/// </summary>
		/// <param name="syntaxId">A syntax id that can be resolved via the <see cref="SyntaxCatalog" />.</param>
		/// <param name="inputText">Text to be highlighted</param>
		/// <returns>Highlighted output</returns>
		public static string Highlight(string syntaxId, string inputText) {
			ArgumentValidator.ThrowIfNullOrEmpty(syntaxId, "syntaxId");
			ArgumentValidator.ThrowIfNull(inputText, "inputText");

			return (new Highlighter(syntaxId)).Highlight(inputText);
		}

		/// <summary>
		/// Takes a string input and returns a string containing the highlighted output.
		/// </summary>
		/// <param name="inputText">Text to be highlighted</param>
		/// <returns>Highlighted output</returns>
		public string Highlight(string inputText) {
			ArgumentValidator.ThrowIfNull(inputText, "inputText");

			// todo: find the right multiplier
			StringBuilder output = new StringBuilder(50 + inputText.Length*2);

			TextWriter writer = new StringWriter(output, CultureInfo.InvariantCulture);

			this.DoHighlight(inputText, null, writer);

			return output.ToString();
		}

		/// <summary>
		/// Highlights the given string input, writing the output to a <see cref="TextWriter" />.
		/// </summary>
		/// <param name="inputText">Text to be highlighted</param>
		/// <param name="writer"><see cref="TextWriter" /> to receive output</param>
		public void Highlight(string inputText, TextWriter writer) {
			ArgumentValidator.ThrowIfNull(inputText, "inputText");
			ArgumentValidator.ThrowIfNull(writer, "writer");

			this.DoHighlight(inputText, null, writer);
		}

		/// <summary>
		/// Reads input from a <see cref="TextReader" /> and writes highlighted text to a <see cref="TextWriter" />. Supports files up to 2GB.
		/// </summary>
		/// <param name="input"><see cref="TextReader" /> where input will be read from</param>
		/// <param name="output">Output will be written using this <see cref="TextWriter" /></param>
		public void Highlight(TextReader input, TextWriter output) {
			ArgumentValidator.ThrowIfNull(input, "input");
			ArgumentValidator.ThrowIfNull(output, "output");

			this.DoHighlight(null, input, output);
		}

		/// <summary>
		/// Reads input from a <see cref="TextReader" /> and writes highlighted text to a <see cref="XmlWriter" />. Supports files up to 2GB.
		/// </summary>
		/// <param name="input"><see cref="TextReader" /> where input will be read from</param>
		/// <param name="output">Output will be written using this <see cref="XmlWriter" /></param>
		public void Highlight(TextReader input, XmlWriter output) {
			ArgumentValidator.ThrowIfNull(input, "input");
			ArgumentValidator.ThrowIfNull(output, "output");

			this.m_formatter.Start(output, this.m_syntax);
			this.m_scanner.Scan(input, this.m_formatter);
			this.m_formatter.Finish();
		}

		#endregion

		#region Internal and Private members

		private ICodeFormatter m_formatter;
		private IScanner m_scanner;
		private Syntax m_syntax;

		private void DoHighlight(string inputstring, TextReader inputReader, TextWriter output) {
			this.m_formatter.Start(output, this.m_syntax);

			if (inputstring != null) {
				this.m_scanner.Scan(inputstring, this.m_formatter);
			} else {
				this.m_scanner.Scan(inputReader, this.m_formatter);
			}

			this.m_formatter.Finish();
		}

		private void Init(Syntax syntax, ICodeFormatter formatter) {
			this.m_syntax = syntax;
			this.m_scanner = this.m_syntax.BuildScanner();
			this.m_formatter = formatter;
		}

		#endregion
	}
}