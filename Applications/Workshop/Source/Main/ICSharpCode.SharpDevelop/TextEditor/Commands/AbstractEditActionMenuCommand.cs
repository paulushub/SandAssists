// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3537 $</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Bookmarks;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.TextEditor.Commands
{
    public abstract class AbstractEditActionMenuCommand : AbstractMenuCommand
    {
        public abstract IEditAction EditAction
        {
            get;
        }

        public override void Run()
        {
            IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (viewContent == null || !(viewContent is ITextEditorControlProvider))
            {
                return;
            }
            TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;
            EditAction.Execute(textEditor.ActiveTextAreaControl.TextArea);
        }
    }
}
