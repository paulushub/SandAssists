// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	/// <summary>
    /// A line inside a <see cref="IRefactorDocument"/>.
	/// </summary>
	public interface IRefactorDocumentLine
	{
		int Offset { get; }
		int Length { get; }
		string Text { get; }
    }

    /// <summary>
	/// A document representing a source code file for refactoring.
	/// Line and column counting starts at 1.
	/// Offset counting starts at 0.
	/// </summary>
	public interface IRefactorDocument
	{
		int TextLength { get; }
		int TotalNumberOfLines { get; }
		IRefactorDocumentLine GetLine(int lineNumber);
		
		int PositionToOffset(int line, int column);
		
		void Insert(int offset, string text);
		void Remove(int offset, int length);
		char GetCharAt(int offset);
		
		/// <summary>
		/// Make the document combine the following actions into a single
		/// action for undo purposes.
		/// </summary>
		void StartUndoableAction();
		
		/// <summary>
		/// Ends the undoable action started with <see cref="StartUndoableAction"/>.
		/// </summary>
		void EndUndoableAction();
		
		void UpdateView();
	}
}
