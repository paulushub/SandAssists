// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.TextEditor.Commands;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
    public sealed class ToggleBookmark : AbstractEditActionMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

                if (viewContent == null || !(viewContent is ITextEditorControlProvider))
                {
                    return false;
                }

                return true;
            }
        }

        public override IEditAction EditAction
        {
			get {
				return new ICSharpCode.TextEditor.Actions.ToggleBookmark();
			}
		}
	}

    public sealed class PrevBookmark : AbstractEditActionMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

                if (viewContent == null || !(viewContent is ITextEditorControlProvider))
                {
                    return false;
                }
                TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;

                BookmarkManager bookmarks = textEditor.Document.BookmarkManager;

                return bookmarks.ContainsMarks(BookmarkManager.AcceptOnlyTextMarks);
            }
        }
		
		public override IEditAction EditAction {
			get {                                  
				return new GotoPrevBookmark(BookmarkManager.AcceptOnlyTextMarks);
			}
		}
	}

    public sealed class NextBookmark : AbstractEditActionMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

                if (viewContent == null || !(viewContent is ITextEditorControlProvider))
                {
                    return false;
                }
                TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;

                BookmarkManager bookmarks = textEditor.Document.BookmarkManager;

                return bookmarks.ContainsMarks(BookmarkManager.AcceptOnlyTextMarks);
            }
        }

        public override IEditAction EditAction
        {
			get {
				return new GotoNextBookmark(BookmarkManager.AcceptOnlyTextMarks);
			}
		}
	}

    public sealed class ClearBookmarks : AbstractEditActionMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

                if (viewContent == null || !(viewContent is ITextEditorControlProvider))
                {
                    return false;
                }
                TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;

                BookmarkManager bookmarks = textEditor.Document.BookmarkManager;

                return bookmarks.ContainsMarks(BookmarkManager.AcceptOnlyTextMarks);
            }
        }

        public override IEditAction EditAction
        {
			get {
				return new ClearAllBookmarks(BookmarkManager.AcceptOnlyTextMarks);
			}
		}
	}

    public sealed class BookmarksCommand : AbstractCheckableMenuCommand
    {
        public override bool IsChecked
        {
            get
            {
                BookmarkPad pad = BookmarkPad.Instance;
                if (pad != null)
                {
                    return pad.IsVisible;
                }

                return false;
            }
        }

        public override void Run()
        {
            BookmarkPad pad = BookmarkPad.Instance;
            if (pad != null)
            {
                pad.BringToFront();
            }
            else
            {
                foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection)
                {
                    if (String.Equals(padContent.Category, "Tools",
                        StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(padContent.Class, "ICSharpCode.SharpDevelop.Bookmarks.BookmarkPad",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        padContent.BringPadToFront();
                        break;
                    }
                }
            }

            base.Run();
        }
    }
}
