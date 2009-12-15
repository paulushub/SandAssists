// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3559 $</version>
// </file>

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands.TabStrip
{
    public sealed class CloseFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow window = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                window = command.Caller as IWorkbenchWindow;
            }

			if (window != null) {
				window.CloseWindow(false);
			}
		}
	}

    public sealed class CloseAllButThisFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow thisWindow = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                thisWindow = command.Caller as IWorkbenchWindow;
            }
            else
            {
                return;
            }

			foreach (IWorkbenchWindow window in WorkbenchSingleton.Workbench.WorkbenchWindowCollection.ToArray()) {
				if (window != thisWindow) {
					if (!window.CloseWindow(false))
						break;
				}
			}
		}
	}

    public sealed class SaveFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow window = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                window = command.Caller as IWorkbenchWindow;
            }

            if (window != null)
            {
				SaveFile.Save(window);
			}
		}
	}

    public sealed class SaveFileAsTab : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow window = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                window = command.Caller as IWorkbenchWindow;
            }

            if (window != null)
            {
				SaveFileAs.Save(window);
			}
		}
	}
	
	/// <summary>
	/// Copies the path to the clipboard.
	/// </summary>
    public sealed class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow window = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                window = command.Caller as IWorkbenchWindow;
            }

            if (window == null)
            {
                return;
            }
			ClipboardWrapper.SetText(window.ActiveViewContent.PrimaryFileName ?? "");
		}
	}
	
	/// <summary>
	/// Opens the containing folder in the clipboard.
	/// </summary>
    public sealed class OpenFolderContainingFile : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow window = null;

            MenuCommand command = this.Owner as MenuCommand;
            if (command != null)
            {
                window = command.Caller as IWorkbenchWindow;
            }

            if (window == null)
            {
                return;
            }
            ICSharpCode.SharpDevelop.Project.Commands.OpenFolderContainingFile.OpenContainingFolderInExplorer(
                window.ActiveViewContent.PrimaryFileName);
        }
	}
}
