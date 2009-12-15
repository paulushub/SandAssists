// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3771 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Commands
{
    public abstract class SideBarCommand : AbstractMenuCommand
	{
        protected SideBarCommand()
        {   
        }

        public SharpDevelopSideBar SideBar
        {
            get
            {
                MenuCommand menuItem = this.Owner as MenuCommand;
                if (menuItem != null)
                {
                    return menuItem.Caller as SharpDevelopSideBar;
                }

                ToolBarCommand toolItem = this.Owner as ToolBarCommand;
                if (toolItem != null)
                {
                    return toolItem.Caller as SharpDevelopSideBar;
                }

                return null;
            }
        }
	}

    public sealed class SideBarRenameTabItem : SideBarCommand
    {
        public override void Run()
        {
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            SideTabItem item = sideBar.ActiveTab.ChoosedItem;
            if (item != null)
            {
                sideBar.StartRenamingOf(item);
            }
        }
    }

    public sealed class SideBarDeleteTabItem : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            SideTabItem item = sideBar.ActiveTab.ChoosedItem;
			if (item != null && MessageBox.Show(StringParser.Parse(ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabItemQuestion"), new string[,] { {"TabItem", item.Name}}),
			                    ResourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.ActiveTab.Items.Remove(item);
				sideBar.Refresh();
			}
		}
	}

    public sealed class SideBarAddTabHeader : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            SideTab tab = new SideTab(sideBar, "New Tab");
			sideBar.Tabs.Add(tab);
			sideBar.StartRenamingOf(tab);
			sideBar.DoAddTab = true;
			sideBar.Refresh();
		} 
	}

    public sealed class SideBarMoveTabUp : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.GetTabIndexAt(
                sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index > 0) 
            {
				SideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}

    public sealed class SideBarMoveTabDown : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.GetTabIndexAt(
                sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) 
            {
				SideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
			
		} 
	}

    public sealed class SideBarMoveActiveTabUp : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index > 0) 
            {
				SideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}

    public sealed class SideBarMoveActiveMoveTabDown : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) 
            {
				SideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
		} 
	}

    public sealed class SideBarDeleteTabHeader : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            SideTab selectedSideTab = sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (MessageBox.Show(StringParser.Parse(ResourceService.GetString("SideBarComponent.ContextMenu.DeleteTabHeaderQuestion"), new string[,] { {"TabHeader", selectedSideTab.DisplayName}}),
			                    ResourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.DeleteSideTab(selectedSideTab);
				sideBar.Refresh();
			}
		} 
	}

    public sealed class SideBarRenameTabHeader : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            sideBar.StartRenamingOf(sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y));
		} 
	}

    public sealed class SideBarMoveActiveItemUp : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.ChoosedItem);
			if (index > 0) {
				sideBar.ActiveTab.Exchange(index -1, index);
				sideBar.Refresh();
			}
		}
	}

    public sealed class SideBarMoveActiveItemDown : SideBarCommand
	{
		public override void Run()
		{
            SharpDevelopSideBar sideBar = this.SideBar;
            if (sideBar == null)
            {
                return;
            }

            int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.ChoosedItem);
			if (index >= 0 && index < sideBar.ActiveTab.Items.Count - 1) {
				sideBar.ActiveTab.Exchange(index, index + 1);
				sideBar.Refresh();
			}
		} 
	}
}
