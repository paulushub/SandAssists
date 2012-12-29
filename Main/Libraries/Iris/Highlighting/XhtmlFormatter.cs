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
using System.Collections.Generic;
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Formats highlighted output into XHTML fragments.
	/// </summary>
	/// <remarks>
	/// <para>
	///		<see cref="XhtmlFormatter" /> outputs highlighted text in XHTML. Each bit of the output text
	///		is enclosed in a span tag, which in turn has a class attribute declaring a CSS class that corresponds to the <see cref="HighlightMode" /> 
	///		for the text. The actual
	///		formatting is done completely via CSS. The XHTML produced for the output is fully compliant with W3C XHTML strict, 
	///		XHTML transitional, and HTML 4 (but there's a caveat below).
	/// </para>
	/// <para>
	///		You must decide how to deliver the CSS stylesheet required by the XHTML-formatted output. The <see cref="CssScheme" /> object can
	///		help you here. For any given scheme (eg, irisDefault, irisDark), <see cref="CssScheme" /> knows how to produce the CSS required for highlighting a given
	///		input. You have three main options:
	///		<list type="number">
	///			<item>Save the CSS stylesheet to a file in your server, and include a &lt;link&gt; tag in your XHTML document to reference the file</item>
	///			<item>Output the required CSS to a &lt;style&gt; tag in the &lt;head&gt; of your XHTML document</item>
	///			<item>Output a &lt;style&gt; tag just before the highlighted output. <b>This violates the XHTML standard</b> because &lt;style&gt;
	///			is supposed to be inside the document's &lt;head&gt;. Nevertheless it's a pragmatic approach often taken.</item>
	///		</list>
	/// </para>
	/// <para>
	///		Since the <see cref="XhtmlFormatter" /> can't touch your document's &lt;head&gt;, the only thing it can do out-of-the-box is #3 above.
	///		It does that by default, so in practice things "just work", but every time you do it a purist cries somewhere. You can turn off the
	///		auto-emitted &lt;style&gt; tag by using an <see cref="XhtmlOptions" /> object with <see cref="XhtmlOptions.EmitStyleTag" /> set to 
	///		<see langword="false" />. You can then serve the CSS as a file or use the <see cref="CssScheme" /> to emit the style within &lt;head&gt;.
	/// </para>
	/// </remarks>
	public class XhtmlFormatter : XmlFormatter {
		#region Public members

		/// <summary>
		/// Initializes a new instance of the <see cref="XhtmlFormatter"/> class with the specified options.
		/// </summary>
		public XhtmlFormatter(XhtmlOptions options) {
			ArgumentValidator.ThrowIfNull(options, "options");

			this.m_options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XhtmlFormatter" /> using default options.
		/// </summary>
		public XhtmlFormatter() {
			this.m_options = new XhtmlOptions();
		}

		/// <summary>
		/// Gets the suggested file extension in case the caller wants to save formatted output to a file.
		/// </summary>
		/// <value>The suggested file extension.</value>
		public override string SuggestedFileExtension {
			get { return ".html"; }
		}

		/// <summary>
		/// Finishes formatting a chunk of code.
		/// </summary>
		public override void Finish() {
			if (HighlightMode.Normal != this.m_mode && this.m_hasStartedMode) {
				this.m_writer.WriteEndElement();
			}

			this.m_writer.WriteRaw(this.m_options.NewLineChars);

			this.m_writer.WriteEndElement(); // closes the <span> for the normal mode
			this.m_writer.WriteEndElement(); // closes the <pre> that contains highlighted output
			this.m_writer.WriteRaw(this.m_options.NewLineChars);
			this.m_writer.WriteRaw("</td></tr><tr><td");
			if (this.m_options.EmitLineNumbers) {
				WriteLineNumbers();
			} else {
				this.m_writer.WriteRaw(">");
			}

			this.m_writer.WriteRaw("</td></tr></tbody></table>");
			this.m_writer.Close();
		}

		/// <summary>
		/// Writes the characters in <see cref="XhtmlOptions.NewLineChars" /> and increments the line count.
		/// </summary>
		public override void WriteNewLine() {
			this.m_cntLines++;

			this.m_writer.WriteString(this.m_options.NewLineChars);
		}

		private void WriteLineNumbers() {
			this.m_writer.WriteRaw(" class='output lineNumbers'>");
			this.m_writer.WriteRaw(this.m_options.NewLineChars);
			this.m_writer.WriteRaw("<pre class='lineNumbers'><span class='lineNumbers'>");

			for (int i = 1; i <= this.m_cntLines; i++) {
				if (i != 1) {
					this.m_writer.WriteRaw(this.m_options.NewLineChars);
				}

				if (this.m_options.MarkNthLineNumber && 0 == i%this.m_options.NthLineNumber) {
					this.m_writer.WriteRaw("<span class='nthLineNumber'>");
					this.m_writer.WriteRaw(i.ToString());
					this.m_writer.WriteRaw("</span>");
				} else {
					this.m_writer.WriteRaw(i.ToString());
				}
			}

			// the last new line is important for IE. Otherwise, the <pre> block ends and the last number 
			// ends up misaligned.
			this.m_writer.WriteRaw(this.m_options.NewLineChars);
			this.m_writer.WriteRaw("</span></pre>");
		}

		#endregion

		#region Protected members


		/// <summary>
		/// The <see cref="XhtmlOptions" /> controlling the output for this <see cref="XhtmlFormatter" />
		/// </summary>
		protected XhtmlOptions m_options;

		/// <summary>
		/// Prepares for highlighting of the specified <see cref="Syntax"/>
		/// </summary>
		protected override void Start(Syntax syntax) {
			if (this.m_options.EmitStyleTag) {
				this.EmitStyleTag(syntax.Id);
			}

			this.m_classNames = CssScheme.LongCssClassNamesForModes;

			this.m_writer.WriteRaw("<table cellpadding='0' cellspacing='0' class='irisContainer' style='border-collapse: collapse; border-spacing:0'");

			this.m_writer.WriteRaw("><tbody><tr><td style='margin: 0; padding:0'></td>");
			this.m_writer.WriteRaw(this.m_options.NewLineChars);

//			if (this.m_options.EmitToolbar) {
//				this.m_writer.WriteRaw("<td class='toolBar'> <span class='irisLogo'>color by iris</span> <span>select</span> | <span>view plain</span> </td> </tr>	<tr> <td></td>");
//			}

			this.m_writer.WriteRaw("<td rowspan='2' class='highlighted output'>");
			this.m_writer.WriteStartElement("pre");
			this.m_writer.WriteAttributeString("class", syntax.Id + " " + "highlighted");

			this.m_writer.WriteStartElement("span");
			this.m_writer.WriteAttributeString("class", this.m_classNames[(int) HighlightMode.Normal]);

			this.m_cntLines = 1;
		}

		/// <summary>
		/// Emits a &lt;style&gt; tag containing the CSS style sheet for the given syntax and the current <see cref="XhtmlOptions.CssScheme" />.
		/// </summary>
		/// <param name="syntaxId"></param>
		protected void EmitStyleTag(string syntaxId) {
			this.m_writer.WriteRaw("<style type='text/css'>");
			string styleSheet = this.m_options.CssScheme.GetCssStyleSheetFor(syntaxId, this.m_options.NewLineChars);
			this.m_writer.WriteRaw(styleSheet);
			this.m_writer.WriteRaw("</style>");
		}

		/// <summary>
		/// Called when a <see cref="HighlightMode">mode</see> starts.
		/// </summary>
		/// <param name="mode"></param>
		protected override void WriteModeStart(HighlightMode mode) {
			this.m_writer.WriteStartElement("span");
			this.m_writer.WriteAttributeString("class", this.m_classNames[(int)mode]);
		}

		#endregion

		#region Internal and Private members

		private int m_cntLines;
		private string[] m_classNames;

		#endregion
	}
}
