// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2659 $</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions
{
    public sealed class ShiftCaretRight : CaretRight
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftCaretLeft : CaretLeft
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftCaretUp : CaretUp
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftCaretDown : CaretDown
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftWordRight : WordRight
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftWordLeft : WordLeft
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftHome : Home
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftEnd : End
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftMoveToStart : MoveToStart
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftMoveToEnd : MoveToEnd
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftMovePageUp : MovePageUp
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class ShiftMovePageDown : MovePageDown
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldCaretPos  = textArea.Caret.Position;
			base.Execute(textArea);
			textArea.AutoClearSelection = false;
			textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
		}
	}

    public sealed class SelectWholeDocument : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.AutoClearSelection = false;
			TextLocation startPoint = new TextLocation(0, 0);
			TextLocation endPoint   = textArea.Document.OffsetToPosition(textArea.Document.TextLength);
			if (textArea.SelectionManager.HasSomethingSelected) {
				if (textArea.SelectionManager.SelectionCollection[0].StartPosition == startPoint &&
				    textArea.SelectionManager.SelectionCollection[0].EndPosition   == endPoint) {
					return;
				}
			}
			textArea.Caret.Position = textArea.SelectionManager.NextValidPosition(endPoint.Y);
			textArea.SelectionManager.ExtendSelection(startPoint, endPoint);
			// after a SelectWholeDocument selection, the caret is placed correctly,
			// but it is not positioned internally.  The effect is when the cursor
			// is moved up or down a line, the caret will take on the column that
			// it was in before the SelectWholeDocument
			textArea.SetDesiredColumn();
		}
	}

    public sealed class ClearAllSelections : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.SelectionManager.ClearSelection();
		}
	}
}
