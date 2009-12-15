// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
    public sealed class SelectNextWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			int index = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Workbench.WorkbenchWindowCollection[(index + 1) % WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}

    public sealed class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			int index = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Workbench.WorkbenchWindowCollection[(index + WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count - 1) % WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}

    public sealed class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.CloseAllViews();
		}
	}

    public sealed class MoreWindows : AbstractMenuCommand
    {
        public override void Run()
        {
            WindowsListDialog dlg = new WindowsListDialog();
            dlg.ShowDialog();
        }
    }	
}
