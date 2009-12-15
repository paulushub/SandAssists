// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>

using System;
using Aga.Controls.Tree;
using Debugger.AddIn.TreeModel;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Debugger.AddIn
{
	public class AddWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
            ToolBarCommand toolstripItem = (ToolBarCommand)this.Owner;
            ToolBarStrip toolStrip = toolstripItem.Owner as ToolBarStrip;
            if (toolStrip == null)
            {
                return;
            }
            WatchPad pad = toolStrip.Owner as WatchPad;

            if (pad != null)
            {
				TreeViewAdv ctrl = (TreeViewAdv)pad.Control;
				
				string input = MessageService.ShowInputBox(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.AddWatch}"),
				                                           StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.EnterExpression}"),
				                                           "");
				if (!String.IsNullOrEmpty(input)) {
                    ctrl.BeginUpdate();
					TextNode text = new TextNode(input);
                    TreeViewVarNode node = new TreeViewVarNode(pad.Process, ctrl, text);
					
					pad.Watches.Add(text);
                    ctrl.Root.Children.Add(node);
                    ctrl.EndUpdate();
				}

                pad.RefreshPad();
			}
		}
	}
	
	public class RemoveWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
            ToolBarCommand toolstripItem = (ToolBarCommand)this.Owner;
            ToolBarStrip toolStrip = toolstripItem.Owner as ToolBarStrip;
            if (toolStrip == null)
            {
                return;
            }
            WatchPad pad = toolStrip.Owner as WatchPad;

            if (pad != null)
            {
                TreeViewAdv treeView = (TreeViewAdv)pad.Control;

                TreeNodeAdv node = treeView.SelectedNode;
				
				if (node == null)
					return;

                while (node.Parent != treeView.Root)
				{
					node = node.Parent;
				}
				
				pad.Watches.RemoveAt(node.Index);
                treeView.Root.Children.Remove(node);

                pad.RefreshPad();
			}
		}
	}
	
	public class RefreshWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
            ToolBarCommand toolstripItem = (ToolBarCommand)this.Owner;
            ToolBarStrip toolStrip = toolstripItem.Owner as ToolBarStrip;
            if (toolStrip == null)
            {
                return;
            }
            WatchPad pad = toolStrip.Owner as WatchPad;

            if (pad != null)
            {
                pad.RefreshPad();
			}
		}
	}
	
	public class ClearWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
            ToolBarCommand toolstripItem = (ToolBarCommand)this.Owner;
            ToolBarStrip toolStrip = toolstripItem.Owner as ToolBarStrip;
            if (toolStrip == null)
            {
                return;
            }
            WatchPad pad = toolStrip.Owner as WatchPad;

            if (pad != null)
            {
                TreeViewAdv treeView = (TreeViewAdv)pad.Control;

                treeView.BeginUpdate();
				pad.Watches.Clear();
                treeView.Root.Children.Clear();
                treeView.EndUpdate();
			}
		}
	}
}
