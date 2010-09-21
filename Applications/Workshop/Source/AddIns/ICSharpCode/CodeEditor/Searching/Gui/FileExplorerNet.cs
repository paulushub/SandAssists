using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.CodeEditor.Searching
{
    public partial class FileExplorerNet : UserControl
    {
        #region Private Fields

        private const string DummyNode = "_DUMMYNODE";

        private bool _ignoreSelection;
        private ImageList _imageList;
        private IconListManager _iconListManager;

        private Dictionary<string, string> _mapDriveLabels;
        
        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerNet"/> class.
        /// </summary>
        public FileExplorerNet()
        {
            InitializeComponent();

            _mapDriveLabels = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the selected text in the treeview.
        /// </summary>
        /// <value>The selected text.</value>
        public string SelectedText
        {
            get
            {
                return treExplorer.SelectedNode.Text;
            }
        }

        /// <summary>
        /// Gets the selected node in the treeview.
        /// </summary>
        /// <value>The selected node.</value>
        public TreeNode SelectedNode
        {
            get
            {
                return treExplorer.SelectedNode;
            }
        }

        public TreeView TreeView
        {
            get
            {
                return treExplorer;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void LoadTreeView()
        {
            Initialize();
        }

        #endregion

        #region Private TreeView Events

        /// <summary>
        /// Handles the BeforeExpand event of the tvExplorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewExpanding(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level > 0)
            {
                e.Node.ImageIndex = _imageList.Images.IndexOfKey("FolderOpened");
            }

            if (_ignoreSelection)
            {
                return;
            }

            treExplorer.BeginUpdate();
            this.treExplorer.Cursor = Cursors.WaitCursor;

            UpdateNode(e.Node, true);

            this.treExplorer.Cursor = Cursors.Default;
            treExplorer.EndUpdate();
        }

        /// <summary>
        /// Handles the MouseDown event of the tvExplorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeViewHitTestInfo hitInfo = treExplorer.HitTest(e.Location);
                if (hitInfo != null && hitInfo.Node != null)
                {
                    _ignoreSelection = true;
                    treExplorer.SelectedNode = hitInfo.Node;
                    _ignoreSelection = false;
                }
                TreeNode selectedNode = treExplorer.SelectedNode;

                if (selectedNode != null && selectedNode.Tag != null)
                {
                    ShellContextMenu ctxMnu = new ShellContextMenu();

                    if ((File.Exists(selectedNode.Tag.ToString())))
                    {
                        ctxMnu.ShowContextMenu(new FileInfo[] { new FileInfo(selectedNode.Tag.ToString()) },
                            PointToScreen(new Point(e.X, e.Y)));
                    }
                    else if (Directory.Exists(selectedNode.Tag.ToString()))
                    {
                        ctxMnu.ShowContextMenu(new DirectoryInfo[] { new DirectoryInfo(selectedNode.Tag.ToString()) },
                            PointToScreen(new Point(e.X, e.Y)));
                    }

                    UpdateNode(treExplorer.SelectedNode, true);
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the treExplorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    UpdateNode(treExplorer.SelectedNode, true);
                    break;
                case Keys.Enter:
                    treExplorer.SelectedNode.Expand();
                    treExplorer.Focus();
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            treExplorer.BeforeExpand += OnTreeViewExpanding;
            treExplorer.MouseDown    += new MouseEventHandler(this.OnTreeViewMouseDown);
            treExplorer.KeyDown      += new KeyEventHandler(this.OnTreeViewKeyDown);

            _imageList = new ImageList();
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.ImageSize  = new System.Drawing.Size(16, 16);

            _iconListManager = new IconListManager(_imageList, IconReader.IconSize.Small);
            
            treExplorer.ImageList  = _imageList;
            treExplorer.ImageIndex = -1;

            LoadDrives();
        }

        /// <summary>
        /// Loads all logical drives into the treeview.
        /// </summary>
        private void LoadDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            TreeNode[] driveNodes = new TreeNode[drives.Length];
            int imageIndex;
            treExplorer.BeginUpdate();

            for (int i = 0; i < drives.Length; i++)
            {
                DriveInfo driveInfo = drives[i];
                string drive = driveInfo.Name;
                _imageList.Images.Add(drive, IconReader.GetDriveIcon(drive));
            }

            if (!_imageList.Images.Keys.Contains("FolderClosed"))
            {
                _imageList.Images.Add("FolderClosed",
                    IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed));
            }

            if (!_imageList.Images.Keys.Contains("FolderOpened"))
            {
                _imageList.Images.Add("FolderOpened",
                    IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Open));
            }

            for (int i = 0; i < drives.Length; i++)
            {
                DriveInfo driveInfo = drives[i];
                string drive = driveInfo.Name;
                TreeNode node = null;
                if (driveInfo.IsReady)
                {
                    string volumeLabel = driveInfo.VolumeLabel;
                    if (!String.IsNullOrEmpty(volumeLabel))
                    {
                        node = new TreeNode(drive.Substring(0, 2));
                        //node = new TreeNode(String.Format("{0} ({1})", 
                        //    volumeLabel, drive.Substring(0, 2)));
                    }
                }
                if (node == null)
                {
                    node = new TreeNode(drive.Substring(0, 2));
                }

                driveNodes[i] = node;
                imageIndex = _imageList.Images.IndexOfKey(drive);

                node.Tag                = drive;
                node.ImageIndex         = imageIndex;
                node.SelectedImageIndex = imageIndex;
                try
                {
                    if (driveInfo.IsReady)
                    {
                        if (Directory.GetDirectories(drive) != null)
                            node.Nodes.Add(DummyNode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            treExplorer.Nodes.AddRange(driveNodes);
            treExplorer.EndUpdate();   
        }

        /// <summary>
        /// Updates the node.
        /// </summary>
        /// <param name="node">The node to be updated</param>
        /// <param name="reload">if set to <c>true</c> the node will be reloaded to get the latest changes from the filesystem.</param>
        private void UpdateNode(TreeNode node, bool reload)
        {
            treExplorer.Focus();

            if (node == null || node.Tag == null)
                return;

            TreeNode dummyNode = (from TreeNode childNode in node.Nodes
                                  where childNode.Text == DummyNode
                                  select childNode).SingleOrDefault();
            if (dummyNode == null)
            {
                return;
            }
            if (reload)
            {
                node.Nodes.Clear();
            }

            if (node.Nodes.Count >= 0)
            {
                string tag = node.Tag.ToString();
                if (tag == null || File.Exists(tag))
                    return;
                DirectoryInfo dirInfo = new DirectoryInfo(tag);

                DirectoryInfo[] folderInfo = null;
                string[] files = new string[0];
                try
                {
                    folderInfo = dirInfo.GetDirectories();
                }
                catch (Exception ex)    // Crazy exception handling
                {
                    MessageBox.Show(ex.Message);
                }

                if (folderInfo == null)
                    return;

                List<TreeNode> contents =
                    new List<TreeNode>(folderInfo.Length);
                int imageIndex;

                imageIndex = _imageList.Images.IndexOfKey("FolderClosed");
                
                // Adding all folders
                for (int i = 0; i < folderInfo.Length; i++)
                {
                    DirectoryInfo nextInfo = folderInfo[i];
                    FileAttributes attributes = nextInfo.Attributes;
                    if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }

                    string nextFolder = nextInfo.FullName;
                    string[] dirs = nextFolder.Split(Path.DirectorySeparatorChar);
                    TreeNode dirNode = new TreeNode(dirs[dirs.Length - 1], imageIndex, imageIndex);
                    dirNode.Tag = nextFolder;
                    contents.Add(dirNode);

                    dirNode.Nodes.Add(DummyNode);
                }

                node.Nodes.AddRange(contents.ToArray());
            }
        }

        #endregion
    }
}