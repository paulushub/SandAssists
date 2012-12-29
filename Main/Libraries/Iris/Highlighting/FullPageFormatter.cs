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
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Outputs a fully W3C-compliant XHTML page containing highlighted text (including &lt;html&gt;, &lt;head&gt;, and friends).
	/// </summary>
	/// <remarks>
	/// <para>The produced output validates correctly as HTML 4.1 transitional, XHTML transitional, and XHTML strict. However, currently there's
	/// no way to choose anything other than HTML 4.1. If you care about this, let us know as it's a 10-minute job.</para>
	/// </remarks>
	public class FullPageFormatter : XhtmlFormatter {
		private readonly string m_pageTitle;

		/// <summary>
		/// Initializes a new instance of the <see cref="FullPageFormatter"/> class.
		/// </summary>
		/// <param name="pageTitle">The page title. May be empty, but not null.</param>
		public FullPageFormatter(string pageTitle) {
			ArgumentValidator.ThrowIfNull(pageTitle, "pageTitle");

			this.m_pageTitle = pageTitle;
		}

		/// <summary>
		/// Start work on a chunk of text. Internally, an <see cref="XmlWriter"/> is created atop the <paramref name="writer"/>.
		/// </summary>
		public override void Start(TextWriter writer, Syntax syntax) {
			ArgumentValidator.ThrowIfNull(writer, "writer");

			XmlWriterSettings settings = m_xmlWriterSettings.Clone();
			settings.OmitXmlDeclaration = false;

			this.Start(XmlWriter.Create(writer, settings), syntax);
		}

		/// <summary>
		/// Prepares for highlighting of the specified <see cref="Syntax"/>
		/// </summary>
		protected override void Start(Syntax syntax) {
			// SHOULD: expose XHTML strict option - we actually pass XHTML strict validation with no errors!
#if FALSE
				this.m_writer.WriteDocType("html", "-//W3C//DTD XHTML 1.0 Strict//EN", null, null);
				this.m_writer.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
#endif
			this.m_writer.WriteDocType("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
			this.m_writer.WriteStartElement("html");

			this.m_writer.WriteStartElement("head");

			this.m_writer.WriteElementString("title", this.m_pageTitle);

			this.EmitStyleTag(syntax.Id);

			this.m_writer.WriteEndElement();

			this.m_writer.WriteStartElement("body");

			this.m_options = XhtmlOptions.Defaults;
			base.Start(syntax);
		}
	}
}