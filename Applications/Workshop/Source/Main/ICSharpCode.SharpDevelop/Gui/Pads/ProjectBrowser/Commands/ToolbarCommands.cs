// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ShowPropertiesForNode : AbstractMenuCommand
	{
		public override void Run()
		{
			BaseNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			node.ShowProperties();
		}
	}
	
	public class ToggleShowAll : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return BaseNode.ShowAll;
			}
			set {
				ProjectBrowserPad.Instance.ProjectBrowserControl.ShowAll = value;
			}
		}
	}
	
	public class RefreshProjectBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
}
