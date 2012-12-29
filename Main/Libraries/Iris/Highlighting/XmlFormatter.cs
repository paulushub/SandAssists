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
using System.IO;
using System.Xml;
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Highlights text using XML tags to indicate the <see cref="HighlightMode">mode</see> for each fragment of text.
	/// </summary>
	public class XmlFormatter : ICodeFormatter {
		static XmlFormatter() {
			m_xmlWriterSettings = new XmlWriterSettings();
			m_xmlWriterSettings.OmitXmlDeclaration = true;
			m_xmlWriterSettings.Indent = false;
			m_xmlWriterSettings.CheckCharacters = true;
			m_xmlWriterSettings.NewLineChars = Environment.NewLine;
			m_xmlWriterSettings.ConformanceLevel = ConformanceLevel.Auto;
			m_xmlWriterSettings.NewLineHandling = NewLineHandling.None;

			List<string> modeNames = new List<string>(40);

			for (HighlightMode mode = HighlightMode.IrisBug; mode <= HighlightMode.Todo; mode++) {
				modeNames.Add(mode.ToString());
			}

			m_modeNames = modeNames.ToArray();
		}

		#region ICodeFormatter Members

		/// <summary>
		/// Gets the suggested file extension in case the caller wants to save formatted output to a file.
		/// </summary>
		/// <value>The suggested file extension.</value>
		public virtual string SuggestedFileExtension {
			get { return ".xml"; }
		}

		/// <summary>
		/// Changes the current <see cref="HighlightMode">mode</see> for text being written.
		/// </summary>
		/// <param name="mode"></param>
		public void ChangeHighlightMode(HighlightMode mode) {
			if (this.m_hasStartedMode) {
				if (mode != this.m_mode) {
					if (this.m_mode != HighlightMode.Normal) {
						this.WriteModeEnd();
					}

					if (mode != HighlightMode.Normal) {
						this.WriteModeStart(mode);
					}
				}

				this.m_mode = mode;
			} else {
				this.m_mode = mode;
				this.m_hasStartedMode = true;

				if (mode != HighlightMode.Normal) {
					this.WriteModeStart(mode);
				}
			}
		}

		/// <summary>
		/// Finish working on a chunk of text to be highlighted. You can reuse the <see cref="ICodeFormatter">formatter</see>.
		/// </summary>
		public virtual void Finish() {
			this.m_writer.WriteEndElement();
			this.m_writer.Close();
		}

		/// <summary>
		/// Writes an error message as an XML comment
		/// </summary>
		public void ReportError(string errorMessage) {
			ArgumentValidator.ThrowIfNullOrEmpty(errorMessage, "errorMessage");

			this.m_writer.WriteComment(errorMessage);
		}

		/// <summary>
		/// Start work on a chunk of text. Internally, an <see cref="XmlWriter" /> is created atop the <paramref name="writer"/>.
		/// </summary>
		public virtual void Start(TextWriter writer, Syntax syntax) {
			ArgumentValidator.ThrowIfNull(writer, "writer");

			this.Start(XmlWriter.Create(writer, m_xmlWriterSettings), syntax);
		}

		/// <summary>
		/// Start work on a chunk of text with output to a <see cref="XmlWriter"/>.
		/// </summary>
		public void Start(XmlWriter writer, Syntax syntax) {
			ArgumentValidator.ThrowIfNull(writer, "writer");
			ArgumentValidator.ThrowIfNull(syntax, "syntax");

			this.m_writer = writer;
			this.m_mode = HighlightMode.Unknown;
			this.m_hasStartedMode = false;
			this.m_charBuf = new char[CharBufferSize];

			this.Start(syntax);
		}

		/// <summary>
		/// Writes new line
		/// </summary>
		public virtual void WriteNewLine() {
			this.m_writer.WriteString(this.m_writer.Settings.NewLineChars);
		}

		/// <summary>
		/// Writes text to the output. Text will be highlighted according to the current <see cref="HighlightMode">mode</see>.
		/// </summary>
		public void WriteText(string buffer, int posFirst, int posLast) {
			ArgumentValidator.ThrowIfNull(buffer, "buffer");

			if (posFirst < 0) {
				throw new ArgumentException(StringExtensions.Fi("idxFirst is negative: {0}", posFirst));
			}

			if (buffer.Length - 1 < posLast) {
				throw new ArgumentException(StringExtensions.Fi("idxLast is {0}, but last valid string index is {1}", posLast, buffer.Length - 1));
			}

			if (posLast < posFirst) {
				throw new ArgumentException(StringExtensions.Fi("idxFirst is {0}, which is greater than idxLast at {1}", posFirst, posLast));
			}

			// Notice that we use a char[] buffer to write the XML. This is because XmlWriter doesn't have a method for writing
			// part of a string (ie, accepting a string, index and count). Since this (WriteText) is called _very_ frequently, we don't want
			// to create a new string or char[] object every time, as it'll waste memory and speed. So we re-use our single char[] buffer every time.
			int posLastWritten = posFirst - 1;

			while (posLastWritten < posLast) {
				int cntToCopy = posLast - posLastWritten;

				if (CharBufferSize < cntToCopy) {
					cntToCopy = CharBufferSize;
				}

				buffer.CopyTo(posLastWritten + 1, this.m_charBuf, 0, cntToCopy);

				// MUST: need to figure out error reporting and handling.
				this.m_writer.WriteChars(this.m_charBuf, 0, cntToCopy);

				posLastWritten += cntToCopy;
			}
		}

		#endregion

		#region Protected members

		/// <summary>
		/// Ready-made <see cref="XmlWriterSettings" />
		/// </summary>
		protected static readonly XmlWriterSettings m_xmlWriterSettings;

		/// <summary>
		/// <see cref="XmlWriter" /> that will receive all of the output
		/// </summary>
		protected XmlWriter m_writer;

		/// <summary>
		/// Prepares for highlighting of the specified <see cref="Syntax" />
		/// </summary>
		protected virtual void Start(Syntax syntax) {
			this.m_writer.WriteStartElement("code");
			this.m_writer.WriteAttributeString("syntax", syntax.ToString());
		}

		/// <summary>
		/// Called when a <see cref="HighlightMode">mode</see> ends.
		/// </summary>
		protected virtual void WriteModeEnd() {
			this.m_writer.WriteEndElement();
		}

		/// <summary>
		/// Called when a <see cref="HighlightMode">mode</see> starts.
		/// </summary>
		protected virtual void WriteModeStart(HighlightMode mode) {
			this.m_writer.WriteStartElement(m_modeNames[(int) mode]);
		}

		#endregion

		#region Internal and Private members

		private const int CharBufferSize = 512;
		private static readonly string[] m_modeNames;
		private char[] m_charBuf;

		/// <summary>
		/// Whether any <see cref="HighlightMode">modes</see> have been started in this formatter.
		/// </summary>
		protected bool m_hasStartedMode;

		/// <summary>
		/// The current <see cref="HighlightMode" /> for the formatter
		/// </summary>
		protected HighlightMode m_mode;

		#endregion
	}
}