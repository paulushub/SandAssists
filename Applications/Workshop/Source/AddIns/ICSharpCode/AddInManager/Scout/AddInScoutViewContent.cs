// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace AddInScout
{
	public class AddInScoutViewContent : AbstractViewContent
	{
		Control control = null;

        AddInDetailsPanel addInDetailsPanel = new AddInDetailsPanel();
        CodonListPanel codonListPanel = new CodonListPanel();
		
		public AddInScoutViewContent() : base()
		{
			this.TitleName = "Add-In Explorer";
			
			Panel p = new Panel();
			p.Dock = DockStyle.Fill;
			//p.BorderStyle = BorderStyle.FixedSingle;
			
			Panel RightPanel = new Panel();
			RightPanel.Dock = DockStyle.Fill;
			p.Controls.Add(RightPanel);
			
			codonListPanel.Dock = DockStyle.Fill;
			codonListPanel.CurrentAddinChanged += new EventHandler(CodonListPanelCurrentAddinChanged);
			RightPanel.Controls.Add(codonListPanel);
			
			Splitter hs = new Splitter();
			hs.Dock = DockStyle.Top;
			RightPanel.Controls.Add(hs);
			
			addInDetailsPanel.Dock = DockStyle.Top;
			addInDetailsPanel.Height = 175;
			RightPanel.Controls.Add(addInDetailsPanel);
			
			Splitter s1 = new Splitter();
			s1.Dock = DockStyle.Left;
			p.Controls.Add(s1);
			
			AddinTreeView addinTreeView = new AddinTreeView();
			addinTreeView.Dock = DockStyle.Fill;
			addinTreeView.TreeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			TreeTreeView treeTreeView = new TreeTreeView();
			treeTreeView.Dock = DockStyle.Fill;
			treeTreeView.TreeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			TabControl tab = new TabControl();
			tab.Width = 300;
			tab.Dock = DockStyle.Left;
			
			TabPage tabPage2 = new TabPage("Tree");
			tabPage2.Dock = DockStyle.Left;
			tabPage2.Controls.Add(treeTreeView);
			tab.TabPages.Add(tabPage2);
			
			TabPage tabPage = new TabPage("AddIns");
			tabPage.Dock = DockStyle.Left;
			tabPage.Controls.Add(addinTreeView);
			tab.TabPages.Add(tabPage);
			
			p.Controls.Add(tab);
            p.Padding = new Padding(3);
            p.BorderStyle = BorderStyle.Fixed3D;
			
			this.control = p;
			this.TitleName = "Add-In Explorer";
		}

        public override Control Control
        {
            get
            {
                return control;
            }
        }

        public override bool IsViewOnly
        {
            get
            {
                return true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (control != null)
            {
                control.Dispose();
                control = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();
            SetIcon();
        }

        protected virtual void SetIcon()
        {
            if (this.WorkbenchWindow != null)
            {
                System.Drawing.Icon icon = WinFormsResourceService.GetIcon(
                    "Icons.16x16.AddIns");
                if (icon != null)
                {
                    this.WorkbenchWindow.Icon = icon;
                }
            }
        }
		
		void CodonListPanelCurrentAddinChanged(object sender, EventArgs e)
		{
			addInDetailsPanel.ShowAddInDetails(codonListPanel.CurrentAddIn);
		}
		
		public void tvSelectHandler(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null) {
				codonListPanel.ClearList();
				return;
			}
			
			TreeNode tn = e.Node;
			
			object selItem = e.Node.Tag;

            AddIn addIn = selItem as AddIn;
            if (addIn != null)
            {
				addInDetailsPanel.ShowAddInDetails(addIn);
				if (tn.FirstNode != null) {
					codonListPanel.ListCodons((ExtensionPath)tn.FirstNode.Tag);
				} else {
					codonListPanel.ClearList();
				}
			} else {
                ExtensionPath ext = (ExtensionPath)selItem;
				addIn = tn.Parent.Tag as AddIn;
				if (addIn == null) {
					codonListPanel.ListCodons(ext.Name);
				} else {
					addInDetailsPanel.ShowAddInDetails(addIn);
					codonListPanel.ListCodons(ext);
				}
			}
		}
	}
}
