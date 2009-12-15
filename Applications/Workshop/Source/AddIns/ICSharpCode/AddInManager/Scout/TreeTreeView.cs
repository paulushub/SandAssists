// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
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
	public class TreeTreeView : Panel
	{
        private TreeViewVista treeView;
		
		public TreeTreeView()
		{   
            treeView = new TreeViewVista();
            treeView.ShowLines = false;
            treeView.HideSelection = false;

            treeView.BeforeExpand += new TreeViewCancelEventHandler(
                OnTreeViewBeforeExpand);
            treeView.BeforeCollapse += new TreeViewCancelEventHandler(
                OnTreeViewBeforeCollapse);

			PopulateTreeView();
			
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Assembly"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenAssembly"));
            treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.VFolderClosed"));
            treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.VFolderOpen"));
            treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.HtmlElements.InputFileElement"));
			
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

        private void OnTreeViewBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                return;
            }

            e.Node.ImageIndex = 3;
            e.Node.SelectedImageIndex = 3;
        }

        private void OnTreeViewBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                return;
            }

            e.Node.ImageIndex = 4;
            e.Node.SelectedImageIndex = 4;
        }

        private void PopulateTreeView()
		{
			TreeNode rootNode = new TreeNode("AddInTree");
			rootNode.Expand();
			
			treeView.Nodes.Add(rootNode);
			
			for (int i = 0; i < AddInTree.AddIns.Count; i++) {
				GetExtensions(AddInTree.AddIns[i], rootNode);
			}

            rootNode.ImageIndex = rootNode.SelectedImageIndex = 0;
		}

        private void GetExtensions(AddIn ai, TreeNode treeNode)
		{
			foreach (ExtensionPath ext in ai.Paths.Values) 
            {
				string[] name = ext.Name.Split('/');
				TreeNode currentNode = treeNode;
				if (name.Length < 1) {
					continue;
				}
				for (int i = 1; i < name.Length; ++i) {
					bool found = false;
					foreach (TreeNode n in currentNode.Nodes) {
						if (n.Text == name[i]) {
							currentNode = n;
							found = true;
							break;
						}
					}
					if (found) {
						if (i == name.Length - 1 && currentNode.Tag == null)
							currentNode.Tag = ext;
					} else {
						TreeNode newNode = new TreeNode(name[i]);
						newNode.ImageIndex = 5;
                        newNode.SelectedImageIndex = 5;
						if (i == name.Length - 1) {
							newNode.Tag = ext;
						}
                        currentNode.ImageIndex = 3;
                        currentNode.SelectedImageIndex = 3;
						currentNode.Nodes.Add(newNode);
						currentNode = newNode;
					}
				}
			}
		}
	}
}
