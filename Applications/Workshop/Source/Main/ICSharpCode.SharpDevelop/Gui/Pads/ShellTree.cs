using System;
using System.IO;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
    sealed class ShellTree : TreeView
    {
        public string NodePath
        {
            get
            {
                return (string)SelectedNode.Tag;
            }
            set
            {
                PopulateShellTree(value);
            }
        }

        public ShellTree()
        {
            Sorted = true;
            TreeNode rootNode = Nodes.Add(Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
            rootNode.ImageIndex = 6;
            rootNode.SelectedImageIndex = 6;
            rootNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            TreeNode myFilesNode = rootNode.Nodes.Add(ResourceService.GetString("MainWindow.Windows.FileScout.MyDocuments"));
            myFilesNode.ImageIndex = 7;
            myFilesNode.SelectedImageIndex = 7;
            try
            {
                myFilesNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            catch (Exception)
            {
                myFilesNode.Tag = "C:\\";
            }

            myFilesNode.Nodes.Add("");

            TreeNode computerNode = rootNode.Nodes.Add(ResourceService.GetString("MainWindow.Windows.FileScout.MyComputer"));
            computerNode.ImageIndex = 8;
            computerNode.SelectedImageIndex = 8;
            try
            {
                computerNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            catch (Exception)
            {
                computerNode.Tag = "C:\\";
            }

            foreach (DriveInfo info in DriveInfo.GetDrives())
            {
                DriveObject drive = new DriveObject(info);

                TreeNode node = new TreeNode(drive.ToString());
                node.Nodes.Add(new TreeNode(""));
                node.Tag = drive.Drive.Substring(0, 2);
                computerNode.Nodes.Add(node);

                switch (info.DriveType)
                {
                    case DriveType.Removable:
                        node.ImageIndex = node.SelectedImageIndex = 2;
                        break;
                    case DriveType.Fixed:
                        node.ImageIndex = node.SelectedImageIndex = 3;
                        break;
                    case DriveType.CDRom:
                        node.ImageIndex = node.SelectedImageIndex = 4;
                        break;
                    case DriveType.Network:
                        node.ImageIndex = node.SelectedImageIndex = 5;
                        break;
                    default:
                        node.ImageIndex = node.SelectedImageIndex = 3;
                        break;
                }
            }

            foreach (string directory in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)))
            {
                TreeNode node = rootNode.Nodes.Add(Path.GetFileName(directory));
                node.Tag = directory;
                node.ImageIndex = node.SelectedImageIndex = 0;
                node.Nodes.Add(new TreeNode(""));
            }

            rootNode.Expand();
            computerNode.Expand();

            InitializeComponent();
        }

        int getNodeLevel(TreeNode node)
        {
            TreeNode parent = node;
            int depth = 0;

            while (true)
            {
                parent = parent.Parent;
                if (parent == null)
                {
                    return depth;
                }
                depth++;
            }
        }

        void InitializeComponent()
        {
            BeforeSelect += new TreeViewCancelEventHandler(SetClosedIcon);
            AfterSelect += new TreeViewEventHandler(SetOpenedIcon);
        }

        void SetClosedIcon(object sender, TreeViewCancelEventArgs e) // Set icon as closed
        {
            if (SelectedNode != null)
            {
                if (getNodeLevel(SelectedNode) > 2)
                {
                    SelectedNode.ImageIndex = SelectedNode.SelectedImageIndex = 0;
                }
            }
        }

        void SetOpenedIcon(object sender, TreeViewEventArgs e) // Set icon as opened
        {
            if (getNodeLevel(e.Node) > 2)
            {
                if (e.Node.Parent != null && e.Node.Parent.Parent != null)
                {
                    e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
                }
            }
        }

        void PopulateShellTree(string path)
        {
            string[] pathlist = path.Split(new char[] { Path.DirectorySeparatorChar });
            TreeNodeCollection curnode = Nodes;

            foreach (string dir in pathlist)
            {

                foreach (TreeNode childnode in curnode)
                {
                    if (((string)childnode.Tag).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        SelectedNode = childnode;

                        PopulateSubDirectory(childnode, 2);
                        childnode.Expand();

                        curnode = childnode.Nodes;
                        break;
                    }
                }
            }
        }

        void PopulateSubDirectory(TreeNode curNode, int depth)
        {
            if (--depth < 0)
            {
                return;
            }

            if (curNode.Nodes.Count == 1 && curNode.Nodes[0].Text.Equals(""))
            {

                string[] directories = null;
                curNode.Nodes.Clear();
                try
                {
                    directories = Directory.GetDirectories(curNode.Tag.ToString() + Path.DirectorySeparatorChar);
                }
                catch (Exception)
                {
                    return;
                }


                foreach (string fulldir in directories)
                {
                    try
                    {
                        string dir = System.IO.Path.GetFileName(fulldir);

                        FileAttributes attr = File.GetAttributes(fulldir);
                        if ((attr & FileAttributes.Hidden) == 0)
                        {
                            TreeNode node = curNode.Nodes.Add(dir);
                            node.Tag = curNode.Tag.ToString() + Path.DirectorySeparatorChar + dir;
                            node.ImageIndex = node.SelectedImageIndex = 0;

                            node.Nodes.Add(""); // Add dummy child node to make node expandable

                            PopulateSubDirectory(node, depth);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                foreach (TreeNode node in curNode.Nodes)
                {
                    PopulateSubDirectory(node, depth); // Populate sub directory
                }
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // do not populate if the "My Cpmputer" node is expaned
                if (e.Node.Parent != null && e.Node.Parent.Parent != null)
                {
                    PopulateSubDirectory(e.Node, 2);
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    PopulateSubDirectory(e.Node, 1);
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception excpt)
            {

                MessageService.ShowError(excpt, "Device error");
                e.Cancel = true;
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
