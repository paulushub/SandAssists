// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.TextEditor.Commands
{
    public sealed class ToggleCommentRegion : AbstractMenuCommand
	{ 
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			
			TextEditorControl textarea = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);
		}
	}

    public sealed class CommentRegion : AbstractMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                return false; //TODO-PAUL: This feature is not yet implemented!
                //return base.IsEnabled;
            }
            set
            {
                base.IsEnabled = value;
            }
        }

		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) 
            {
				return;
			}
			
            //TextEditorControl textarea = ((ITextEditorControlProvider)viewContent).TextEditorControl;
            //new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);
		}
	}

    public sealed class UnCommentRegion : AbstractMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                return false; //TODO-PAUL: This feature is not yet implemented!
                //return base.IsEnabled;
            }
            set
            {
                base.IsEnabled = value;
            }
        }

        public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) 
            {
				return;
			}
			
            //TextEditorControl textarea = ((ITextEditorControlProvider)viewContent).TextEditorControl;
            //new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);
		}
	}
}
