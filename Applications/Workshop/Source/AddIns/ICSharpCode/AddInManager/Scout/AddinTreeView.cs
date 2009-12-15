// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace AddInScout
{
	/// <summary>
	/// Description of AddinTreeView.
	/// </summary>
	public class AddinTreeView : Panel
	{
        private TreeViewVista treeView;
		
		public AddinTreeView()
		{
            treeView = new TreeViewVista();
            treeView.ShowLines = false;
            treeView.HideSelection = false;

//			treeView.BorderStyle = BorderStyle.;
//			treeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			PopulateTreeView();
			
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
            treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.AddIns"));
            treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.HtmlElements.InputFileElement"));
            //treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
            //treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			
			treeView.Dock = DockStyle.Fill;
			Controls.Add(treeView);
		}

        public TreeView TreeView
        {
            get
            {
                return treeView;
            }
        }
		
		private void PopulateTreeView()
		{
			TreeNode rootNode = new TreeNode("Addins");
			rootNode.Expand();
			
			treeView.Nodes.Add(rootNode);
			
			for (int i = 0; i < AddInTree.AddIns.Count; i++) {
				TreeNode newNode = new TreeNode(AddInTree.AddIns[i].Properties["name"]);
				newNode.ImageIndex = 1;
				newNode.SelectedImageIndex = 1;
				newNode.Tag = AddInTree.AddIns[i];
				GetExtensions(AddInTree.AddIns[i], newNode);
				rootNode.Nodes.Add(newNode);
			}

            rootNode.ImageIndex = rootNode.SelectedImageIndex = 0;
		}

        private void GetExtensions(AddIn ai, TreeNode treeNode)
		{
			if (!ai.Enabled)
				return;
			foreach (ExtensionPath ext in ai.Paths.Values) 
            {
				TreeNode newNode = new TreeNode(ext.Name);
				newNode.ImageIndex = 2;
				newNode.SelectedImageIndex = 2;
				newNode.Tag = ext;
				treeNode.Nodes.Add(newNode);
			}
		}
	}
}
