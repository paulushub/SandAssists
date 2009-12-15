// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2659 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Use this class to pass a text editor document to the refactoring API.
	/// </summary>
	public sealed class RefactorDocument : IRefactorDocument
	{
        private ITextDocument doc;
		
		public RefactorDocument(ITextDocument doc)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");

			this.doc = doc;
		}

        private sealed class RefactorDocumentLine : IRefactorDocumentLine
		{
			private ITextDocument doc;
            private LineSegment line;
			
			public RefactorDocumentLine(ITextDocument doc, LineSegment line)
			{
				this.doc = doc;
				this.line = line;
			}
			
			public int Offset {
				get {
					return line.Offset;
				}
			}
			
			public int Length {
				get {
					return line.Length;
				}
			}
			
			public string Text {
				get {
					return doc.GetText(line.Offset, line.Length);
				}
			}
		}
		
		public int TextLength {
			get {
				return doc.TextLength;
			}
		}

        public IRefactorDocumentLine GetLine(int lineNumber)
		{
			return new RefactorDocumentLine(doc, doc.GetLineSegment(lineNumber - 1));
		}
		
		public int PositionToOffset(int line, int column)
		{
			return doc.PositionToOffset(new TextLocation(column - 1, line - 1));
		}
		
		public void Insert(int offset, string text)
		{
			doc.Insert(offset, text);
		}
		
		public void Remove(int offset, int length)
		{
			doc.Remove(offset, length);
		}
		
		public char GetCharAt(int offset)
		{
			return doc.GetCharAt(offset);
		}
		
		public void StartUndoableAction()
		{
			doc.UndoStack.StartUndoGroup();
		}
		
		public void EndUndoableAction()
		{
			doc.UndoStack.EndUndoGroup();
		}
		
		public void UpdateView()
		{
			doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			doc.CommitUpdate();
		}
		
		public int TotalNumberOfLines {
			get {
				return doc.TotalNumberOfLines;
			}
		}
	}
}
