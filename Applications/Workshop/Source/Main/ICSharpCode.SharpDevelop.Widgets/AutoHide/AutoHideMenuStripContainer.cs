// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 1974 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.AutoHide
{
	/// <summary>
	/// AutoHideMenuStripContainer can be used instead of MenuStrip to get a menu
	/// which is automaticaly hiden and shown. It is especially useful in fullscreen.
	/// </summary>
	public class AutoHideMenuStripContainer: AutoHideContainer
	{
		protected bool dropDownOpened;
		
		Padding? defaultPadding;
		
		protected override void Reformat()
		{
			if (defaultPadding == null) {
				defaultPadding = ((MenuStrip)control).Padding;
			}
			((MenuStrip)control).Padding = AutoHide ? Padding.Empty : (Padding)defaultPadding;
			base.Reformat();
		}
		
		public AutoHideMenuStripContainer(MenuStrip menuStrip):base(menuStrip)
		{
			menuStrip.AutoSize = false;
			menuStrip.ItemAdded += OnMenuItemAdded;
            ToolStripItemCollection listItem = menuStrip.Items;
            int itemCount = listItem.Count;

			for (int i = 0; i < itemCount; i++) 
            {
                ToolStripMenuItem menuItem = listItem[i] as ToolStripMenuItem;
                if (menuItem != null)
                {
                    AddEventHandlersForItem(menuItem);
                }
			}
		}
		
		void OnMenuItemAdded(object sender, EventArgs e)
		{
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                AddEventHandlersForItem(menuItem);
            }
		}
		
		void AddEventHandlersForItem(ToolStripMenuItem menuItem)
		{
            if (menuItem == null)
            {
                return;
            }
			menuItem.DropDownOpened += delegate { dropDownOpened = true; };
			menuItem.DropDownClosed += delegate { dropDownOpened = false; if (!mouseIn) ShowOverlay = false; };
		}
		
		protected override void OnControlMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			if (!dropDownOpened) 
                this.ShowOverlay = false;
		}
	}
}
