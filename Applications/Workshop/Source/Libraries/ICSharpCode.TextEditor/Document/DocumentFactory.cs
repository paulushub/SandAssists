// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3205 $</version>
// </file>

using System;
using System.Text;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This interface represents a container which holds a text sequence and
	/// all necessary information about it. It is used as the base for a text editor.
	/// </summary>
	public class DocumentFactory
	{
		/// <remarks>
		/// Creates a new <see cref="ITextDocument"/> object. Only create
		/// <see cref="ITextDocument"/> with this method.
		/// </remarks>
		public ITextDocument CreateDocument()
		{
			TextDocument doc = new TextDocument();
			doc.TextBufferStrategy  = new GapTextBufferStrategy();
			doc.FormattingStrategy  = new DefaultFormattingStrategy();
			doc.LineManager         = new LineManager(doc, null);
			doc.FoldingManager      = new FoldingManager(doc, doc.LineManager);
			doc.FoldingManager.FoldingStrategy       = null; //new ParserFoldingStrategy();
			doc.MarkerStrategy      = new MarkerStrategy(doc);
			doc.BookmarkManager     = new BookmarkManager(doc, doc.LineManager);
			return doc;
		}
		
		/// <summary>
		/// Creates a new document and loads the given file
		/// </summary>
		public ITextDocument CreateFromTextBuffer(ITextBufferStrategy textBuffer)
		{
			TextDocument doc = (TextDocument)CreateDocument();
			doc.TextContent = textBuffer.GetText(0, textBuffer.Length);
			doc.TextBufferStrategy = textBuffer;
			return doc;
		}
		
		/// <summary>
		/// Creates a new document and loads the given file
		/// </summary>
		public ITextDocument CreateFromFile(string fileName)
		{
			ITextDocument document = CreateDocument();
			document.TextContent = Util.FileReader.ReadFileContent(fileName, Encoding.Default);
			return document;
		}
	}
}
