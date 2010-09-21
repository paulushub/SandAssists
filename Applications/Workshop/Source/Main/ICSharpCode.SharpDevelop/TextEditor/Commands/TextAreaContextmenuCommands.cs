// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4081 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Commands
{
	public sealed class ShowBufferOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			OptionsCommand.ShowTabbedOptions(ResourceService.GetString("Dialog.Options.BufferOptions"),
			                                 AddInTree.GetTreeNode("/SharpDevelop/ViewContent/TextEditor/OptionsDialog"));
		}
	}            	
	
	public sealed class HighlightingTypeBuilder : ISubmenuBuilder
	{
		private TextEditorControl  control;
        private ToolStripItem[] menuCommands;
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			control = (TextEditorControl)owner;

            List<ToolStripItem> menuItems = new List<ToolStripItem>();

            string activeHighlight = control.Document.HighlightingStrategy.Name;
			
            IDictionary<string, object> highlightDefs = HighlightingManager.Manager.HighlightingDefinitions;
            foreach (KeyValuePair<string, object> entry in highlightDefs)
            {
				MenuCheckBox item = new MenuCheckBox(entry.Key);
				item.Click    += new EventHandler(ChangeSyntax);
                item.Checked   = (activeHighlight == entry.Key);
				menuItems.Add(item);
			}

			menuCommands = menuItems.ToArray();
			
            return menuCommands;
		}
		
		private void ChangeSyntax(object sender, EventArgs e)
		{
			if (control != null) 
            {
				MenuCheckBox item = (MenuCheckBox)sender;
				foreach (MenuCheckBox i in menuCommands) 
                {
					i.Checked = false;
				}
				item.Checked = true;
				try 
                {
                    control.SetHighlighting(item.Text);
                }
                catch (HighlightingDefinitionInvalidException ex)
                {
					MessageBox.Show(ex.ToString(), "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				control.Refresh();
			}
		}
	}
}
