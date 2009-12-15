﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3648 $</version>
// </file>

using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RunToCursorCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			CodeEditorControl textEditor = this.Owner as CodeEditorControl;
			WindowsDebugger winDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (textEditor == null || winDebugger == null) return;
			
			Breakpoint breakpoint = winDebugger.DebuggerCore.AddBreakpoint(textEditor.FileName, null, textEditor.ActiveTextAreaControl.Caret.Line + 1, textEditor.ActiveTextAreaControl.Caret.Column, true);
			breakpoint.Hit += delegate { breakpoint.Remove(); };
			winDebugger.DebuggedProcess.Paused += delegate { breakpoint.Remove(); };
			if (!winDebugger.IsProcessRunning) {
				winDebugger.Continue();
			}
		}
	}
}