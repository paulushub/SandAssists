// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3806 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
    public sealed class OptionsCommand : AbstractMenuCommand
	{
		public static void ShowTabbedOptions(string dialogTitle, AddInTreeNode node)
		{
			TabbedOptions o = new TabbedOptions(dialogTitle, node);

            o.ShowDialog(WorkbenchSingleton.MainForm);
			o.Dispose();
		}
		
		public override void Run()
		{
			using (TreeViewOptions optionsDialog = new TreeViewOptions(
                AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"))) 
            {
				optionsDialog.Owner = WorkbenchSingleton.MainForm;
                if (optionsDialog.ShowDialog(
                    WorkbenchSingleton.MainForm) == DialogResult.OK)
                {
                    PropertyService.Save();
                }
            }
		}
	}

    public sealed class ToggleFullscreenCommand : AbstractMenuCommand
	{
        private ToolBarCommand fullscreenButton;

		public override void Run()
		{
            Workbench workBench = ((Workbench)WorkbenchSingleton.Workbench);

            MenuCommand menuCmd = (MenuCommand)this.Owner;
            if (fullscreenButton != null && menuCmd != null)
            {
                if (workBench.FullScreen)
                {
                    menuCmd.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

                    workBench.MainMenuStrip.Items.Remove(fullscreenButton);

                    // Make sure all is unchecked...
                    menuCmd.Checked = false;
                    fullscreenButton.Checked = false;
                }
                else
                {
                    menuCmd.DisplayStyle = ToolStripItemDisplayStyle.Text;

                    fullscreenButton.Checked = true;
                    workBench.MainMenuStrip.Items.Add(fullscreenButton);
                }
            }

            workBench.FullScreen = !workBench.FullScreen;
		}

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            MenuCommand menuCmd = (MenuCommand)this.Owner;
            menuCmd.CheckOnClick = true;

            if (fullscreenButton == null)
            {
                fullscreenButton = new ToolBarCommand(menuCmd.Text, menuCmd.Image,
                    OnButtonClicked);
                fullscreenButton.ToolTipText = menuCmd.Text;
                fullscreenButton.CheckOnClick = true;
            }
        }

        private void OnButtonClicked(object sender, EventArgs args)
        {
            this.Run();
        }   
	}

    public sealed class StatusBarCommand : AbstractCheckableMenuCommand
    {
        public override bool IsChecked
        {
            get
            {
                return PropertyService.Get(
                    "ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
            }
        }

        public override void Run()
        {
            bool statusBarVisible = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
            PropertyService.Set("ICSharpCode.SharpDevelop.Gui.StatusBarVisible",
                !statusBarVisible);
        }
    }
}
