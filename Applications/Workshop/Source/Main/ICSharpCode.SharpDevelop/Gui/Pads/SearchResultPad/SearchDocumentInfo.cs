// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2650 $</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Searching
{
	public class SearchDocumentInfo
	{
		ITextDocument           document;
		ITextBufferStrategy textBuffer;
		string              fileName;
		int                 currentOffset;
		TextAreaControl     textAreaControl;
		
		public ITextBufferStrategy TextBuffer {
			get {
				return textBuffer;
			}
			set {
				textBuffer = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public ITextDocument Document {
			get {
				return document;
			}
		}
		
		public int CurrentOffset {
			get {
				if (textAreaControl != null) {
					return textAreaControl.Caret.Offset;
				}
				return currentOffset;
			}
			set {
				if (textAreaControl != null) {
					textAreaControl.Caret.Position = document.OffsetToPosition(value + 1);
				} else {
					currentOffset = value;
				}
			}
		}
		
		int endOffset;
		public int EndOffset {
			get {
//				if (document != null) {
//					return SearchReplaceUtilities.CalcCurrentOffset(document);
//				}
				return endOffset;
			}
		}
		
		public void Replace(int offset, int length, string pattern)
		{
			if (document != null) {
				document.Replace(offset, length, pattern);
			} else {
				textBuffer.Replace(offset, length, pattern);
			}
			
			if (offset <= CurrentOffset) {
				CurrentOffset = CurrentOffset - length + pattern.Length;
			}
		}
		
		public ITextDocument CreateDocument()
		{
			if (document != null) {
				return document;
			}
			return new DocumentFactory().CreateFromTextBuffer(textBuffer);
		}		
		
		public override bool Equals(object obj)
		{
			SearchDocumentInfo info = obj as SearchDocumentInfo;
			if (info == null) {
				return false;
			}
			return this.fileName == info.fileName && 
				this.textAreaControl == info.textAreaControl;
		}
		
		public override int GetHashCode()
		{
			return fileName.GetHashCode();
		}
		
		public SearchDocumentInfo(ITextDocument document, string fileName, int currentOffset)
		{
			this.document   = document;
			this.textBuffer = document.TextBufferStrategy;
			this.fileName   = fileName;
			this.endOffset  = this.currentOffset = currentOffset;
		}
		
		public SearchDocumentInfo(ITextDocument document, string fileName, TextAreaControl textAreaControl)
		{
			this.document   = document;
			this.textBuffer = document.TextBufferStrategy;
			this.fileName   = fileName;
			this.textAreaControl = textAreaControl;
			this.endOffset = this.CurrentOffset;
		}
		
		public SearchDocumentInfo(ITextBufferStrategy textBuffer, string fileName, int currentOffset)
		{
			this.textBuffer    = textBuffer;
			this.fileName      = fileName;
			this.endOffset = this.currentOffset = currentOffset;
		}
	}
}
