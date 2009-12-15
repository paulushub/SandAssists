/*
 * Created by SharpDevelop.
 * User: Paul Selormey
 * Date: 11/21/2009
 * Time: 8:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace Sandcastle.Workshop.Tools
{
    public class ToolCommand1 : AbstractMenuCommand
    {
        public override void Run()
        {
            // Here an example that shows how to access the current text document:
            
            ITextEditorControlProvider tecp = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorControlProvider;
            if (tecp == null) {
                // active content is not a text editor control
                return;
            }
            // Get the active text area from the control:
            TextArea textArea = tecp.TextEditorControl.ActiveTextAreaControl.TextArea;
            if (!textArea.SelectionManager.HasSomethingSelected)
                return;
            // get the selected text:
            string text = textArea.SelectionManager.SelectedText;
            // reverse the text:
            StringBuilder b = new StringBuilder(text.Length);
            for (int i = text.Length - 1; i >= 0; i--)
                b.Append(text[i]);
            string newText = b.ToString();
            // ensure caret is at start of selection
            textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
            // deselect text
            textArea.SelectionManager.ClearSelection();
            // replace the selected text with the new text:
            // Replace() takes the arguments: start offset to replace, length of the text to remove, new text
            textArea.Document.Replace(textArea.Caret.Offset,
                                      text.Length,
                                      newText);
            // Redraw:
            textArea.Refresh();
        }
    }
}
