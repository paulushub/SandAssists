// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1971 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Holds information about the start of a fold in an xml string.
	/// </summary>
	public sealed class XmlFoldStart
	{
		private int line;
		private int col;
		private string prefix;
		private string name;
		private string foldText;
		
		public XmlFoldStart(string prefix, string name, int line, int col)
		{
			this.line     = line;
			this.col      = col;
			this.prefix   = prefix;
			this.name     = name;
            this.foldText = String.Empty;
		}
		
		/// <summary>
		/// The line where the fold should start.  Lines start from 0.
		/// </summary>
		public int Line {
			get {
				return line;
			}
		}
		
		/// <summary>
		/// The column where the fold should start.  Columns start from 0.
		/// </summary>
		public int Column {
			get {
				return col;
			}
		}	
		
		/// <summary>
		/// The name of the xml item with its prefix if it has one.
		/// </summary>
		public string Name {
			get {
				if (prefix.Length > 0) {
					return String.Concat(prefix, ":", name);
				} else {
					return name;
				}
			}
		}
		
		/// <summary>
		/// The text to be displayed when the item is folded.
		/// </summary>
		public string FoldText {
			get {
				return foldText;
			}
			set {
				foldText = value;
			}
		}
	}
	
	/// <summary>
	/// Determines folds for an xml string in the editor.
	/// </summary>
	public sealed class XmlFoldingStrategy : IFoldingStrategy
	{
		/// <summary>
		/// Flag indicating whether attributes should be displayed on folded
		/// elements.
		/// </summary>
		bool showAttributesWhenFolded;
		
		public XmlFoldingStrategy()
		{
		}	
		
		#region IFoldingStrategy 
		
		/// <summary>
		/// Adds folds to the text editor around each start-end element pair.
		/// </summary>
		/// <remarks>
		/// <para>If the xml is not well formed then no folds are created.</para> 
		/// <para>Note that the xml text reader lines and positions start 
		/// from 1 and the SharpDevelop text editor line information starts
		/// from 0.</para>
		/// </remarks>
		public List<FoldMarker> GenerateFoldMarkers(ITextDocument document, string fileName, object parseInformation)
		{
			showAttributesWhenFolded = XmlEditorAddInOptions.ShowAttributesWhenFolded;
			
			List<FoldMarker> foldMarkers = new List<FoldMarker>();
			Stack stack = new Stack();
			
			try {
				string xml = document.TextContent;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = null;
                settings.ProhibitDtd = false;
                XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (!reader.IsEmptyElement) {
								XmlFoldStart newFoldStart = CreateElementFoldStart(reader);
								stack.Push(newFoldStart);
							}
							break;
						
						case XmlNodeType.EndElement:
							XmlFoldStart foldStart = (XmlFoldStart)stack.Pop();
							CreateElementFold(document, foldMarkers, reader, foldStart);
							break;
							
						case XmlNodeType.Comment:
							CreateCommentFold(document, foldMarkers, reader);
							break;
					}
				}
			} catch (Exception) {
				// If the xml is not well formed keep the foldings 
				// that already exist in the document.
				return new List<FoldMarker>(document.FoldingManager.FoldMarker);
			}

			return foldMarkers;
		}
		
		#endregion
		
		/// <summary>
		/// Creates a comment fold if the comment spans more than one line.
		/// </summary>
		/// <remarks>The text displayed when the comment is folded is the first 
		/// line of the comment.</remarks>
		void CreateCommentFold(ITextDocument document, List<FoldMarker> foldMarkers, XmlReader reader)
		{
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;

			if (reader.Value != null && lineInfo != null) {
				string comment = reader.Value.Replace("\r\n", "\n");
				string[] lines = comment.Split('\n');
				if (lines.Length > 1) {
					
					// Take off 5 chars to get the actual comment start (takes
					// into account the <!-- chars.

					int startCol = lineInfo.LinePosition - 5;
					int startLine = lineInfo.LineNumber - 1;
					
					// Add 3 to the end col value to take into account the '-->'
					int endCol = lines[lines.Length - 1].Length + startCol + 3;
					int endLine = startLine + lines.Length - 1;
					string foldText = String.Concat("<!--", lines[0], "-->");
					FoldMarker foldMarker = new FoldMarker(document, startLine, startCol, endLine, endCol, FoldType.TypeBody, foldText);
					foldMarkers.Add(foldMarker);
				}				
			}
		}
		
		/// <summary>
		/// Creates an XmlFoldStart for the start tag of an element.
		/// </summary>
		XmlFoldStart CreateElementFoldStart(XmlReader reader)
		{
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo == null)
            {
                throw new InvalidOperationException();
            }

			// Take off 2 from the line position returned 
			// from the xml since it points to the start
			// of the element name and not the beginning 
			// tag.
			XmlFoldStart newFoldStart = new XmlFoldStart(reader.Prefix, 
                reader.LocalName, lineInfo.LineNumber - 1, lineInfo.LinePosition - 2);
			
			if (showAttributesWhenFolded && reader.HasAttributes) {
				newFoldStart.FoldText = String.Concat("<", newFoldStart.Name, " ", GetAttributeFoldText(reader), ">");
			} else {
				newFoldStart.FoldText = String.Concat("<", newFoldStart.Name, ">");			
			}
			
			return newFoldStart;
		}
		
		/// <summary>
		/// Create an element fold if the start and end tag are on 
		/// different lines.
		/// </summary>
		void CreateElementFold(ITextDocument document, List<FoldMarker> foldMarkers, XmlReader reader, XmlFoldStart foldStart)
		{
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo == null)
            {
                throw new InvalidOperationException();
            }

			int endLine = lineInfo.LineNumber - 1;
			if (endLine > foldStart.Line) {
				int endCol = lineInfo.LinePosition + foldStart.Name.Length;
				FoldMarker foldMarker = new FoldMarker(document, foldStart.Line, foldStart.Column, endLine, endCol, FoldType.TypeBody, foldStart.FoldText);
				foldMarkers.Add(foldMarker);
			}
		}
		
		/// <summary>
		/// Gets the element's attributes as a string on one line that will
		/// be displayed when the element is folded.
		/// </summary>
		/// <remarks>
		/// Currently this puts all attributes from an element on the same
		/// line of the start tag.  It does not cater for elements where attributes
		/// are not on the same line as the start tag.
		/// </remarks>
		string GetAttributeFoldText(XmlReader reader)
		{
			StringBuilder text = new StringBuilder();
			
			for (int i = 0; i < reader.AttributeCount; ++i) {
				reader.MoveToAttribute(i);
				
				text.Append(reader.Name);
				text.Append("=");
				text.Append(reader.QuoteChar.ToString());
				text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
				text.Append(reader.QuoteChar.ToString());
				
				// Append a space if this is not the
				// last attribute.
				if (i < reader.AttributeCount - 1) {
					text.Append(" ");
				}
			}
			
			return text.ToString();
		}
		
		/// <summary>
		/// Xml encode the attribute string since the string returned from
		/// the XmlReader is the plain unencoded string and .NET
		/// does not provide us with an xml encode method.
		/// </summary>
		static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
		{
			StringBuilder encodedValue = new StringBuilder(attributeValue);
			
			encodedValue.Replace("&", "&amp;");
			encodedValue.Replace("<", "&lt;");
			encodedValue.Replace(">", "&gt;");
			
			if (quoteChar == '"') {
				encodedValue.Replace("\"", "&quot;");
			} else {
				encodedValue.Replace("'", "&apos;");
			}
			
			return encodedValue.ToString();
		}
	}
}
