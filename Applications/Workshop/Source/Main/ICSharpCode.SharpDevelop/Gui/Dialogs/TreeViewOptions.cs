// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The option container panel is <c>(408, 384)</c> in size, and panels are
    /// recommended to be of size <c>(404, 380)</c>.
    /// </remarks>
    public partial class TreeViewOptions : Form
    {
        private string dialogTitle;
        protected List<IDialogPanel> OptionPanels = new List<IDialogPanel>();

        //protected Font plainFont = null;
        //protected Font boldFont = null;

        public TreeViewOptions()
        {
            InitializeComponent();

            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");
            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
        }

        public TreeViewOptions(AddInTreeNode node)
            : this()
        {
            dialogTitle = StringParser.Parse(
                "${res:Dialog.Options.TreeViewOptions.DialogName}");
            this.Text = dialogTitle;

            this.Initialize();

            //plainFont = new Font(optionsTreeView.Font, FontStyle.Regular);
            //boldFont = new Font(optionsTreeView.Font, FontStyle.Bold);

            //InitImageList();

            if (node != null)
            {
                AddNodes(optionsTreeView.Nodes, 
                    node.BuildChildItems<IDialogPanelDescriptor>(this));
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        protected void Initialize()
        {
            //this.Owner = WorkbenchSingleton.MainForm;
            //this.Icon = null;

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.TreeViewOptionsDialog.xfrm"));

            this.okButton.Click += new EventHandler(AcceptEvent);

            //this.optionsTreeView.Click += new EventHandler(HandleClick);
            this.optionsTreeView.AfterSelect += new TreeViewEventHandler(AfterSelectNode);
            this.optionsTreeView.BeforeSelect += new TreeViewCancelEventHandler(BeforeSelectNode);
            //this.optionsTreeView.BeforeExpand += new TreeViewCancelEventHandler(BeforeExpandNode);
            //this.optionsTreeView.BeforeExpand += new TreeViewCancelEventHandler(ShowOpenFolderIcon);
            //this.optionsTreeView.BeforeCollapse += new TreeViewCancelEventHandler(ShowClosedFolderIcon);
            //this.optionsTreeView.MouseDown += new MouseEventHandler(TreeMouseDown);
        }

        protected void AcceptEvent(object sender, EventArgs e)
        {
            foreach (IDialogPanel pane in OptionPanels)
            {
                if (!pane.ReceiveDialogMessage(DialogMessage.OK))
                {
                    return;
                }
            }
            DialogResult = DialogResult.OK;
        }

        protected void ResetImageIndex(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    ResetImageIndex(node.Nodes);
                }
                else
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 3;
                }
            }
        }

        //protected bool b = true;

        //protected void BeforeExpandNode(object sender, TreeViewCancelEventArgs e)
        //{
        //    if (!b)
        //    {
        //        return;
        //    }
        //    b = false;
        //    //optionsTreeView.BeginUpdate();
        //    // search first leaf node (leaf nodes have no children)
        //    TreeNode node = e.Node.FirstNode;
        //    while (node.Nodes.Count > 0)
        //    {
        //        node = node.FirstNode;
        //    }
        //    //optionsTreeView.CollapseAll();
        //    node.EnsureVisible();
        //    //node.ImageIndex = 3;
        //    //optionsTreeView.EndUpdate();
        //    SetOptionPanelTo(node);
        //    b = true;
        //}

        protected void BeforeSelectNode(object sender, TreeViewCancelEventArgs e)
        {
            //ResetImageIndex(optionsTreeView.Nodes);
            //if (b)
            //{
            //    CollapseOrExpandNode(e.Node);
            //}
            TreeNode selectedNode = e.Node;
            if (selectedNode.Parent == null)
            {
                IDialogPanelDescriptor selDescriptor = 
                    selectedNode.Tag as IDialogPanelDescriptor;
                if (selDescriptor == null || selDescriptor.DialogPanel == null)
                {
                    if (selectedNode.Nodes.Count > 0)
                    {
                        e.Cancel = true;
                        optionsTreeView.SelectedNode = selectedNode.FirstNode;
                    }
                }
            }
        }

        protected void AfterSelectNode(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            if (selectedNode.Parent == null)
            {                   
                if (selectedNode.Nodes.Count > 0)
                {
                    selectedNode = selectedNode.FirstNode;
                }
            }
            //SetOptionPanelTo(optionsTreeView.SelectedNode);
            SetOptionPanelTo(selectedNode);
        }

        //protected void HandleClick(object sender, EventArgs e)
        //{
        //    if (optionsTreeView.GetNodeAt(optionsTreeView.PointToClient(
        //        Control.MousePosition)) == optionsTreeView.SelectedNode && b)
        //    {
        //        //CollapseOrExpandNode(optionsTreeView.SelectedNode);
        //    }
        //}

        //private void CollapseOrExpandNode(TreeNode node)
        //{
        //    if (node.Nodes.Count > 0)
        //    {  // only folders
        //        if (node.IsExpanded)
        //        {
        //            node.Collapse();
        //        }
        //        else
        //        {
        //            node.Expand();
        //        }
        //    }
        //}

        protected void SetOptionPanelTo(TreeNode node)
        {
            IDialogPanelDescriptor descriptor = node.Tag as IDialogPanelDescriptor;
            if (descriptor != null && descriptor.DialogPanel != null && 
                descriptor.DialogPanel.Control != null)
            {
                if (!OptionPanels.Contains(descriptor.DialogPanel))
                {
                    descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
                    OptionPanels.Add(descriptor.DialogPanel);
                }

                descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
                optionControlPanel.Controls.Clear();
                RightToLeftConverter.ConvertRecursive(descriptor.DialogPanel.Control);
                optionControlPanel.Controls.Add(descriptor.DialogPanel.Control);

                List<string> listNames = new List<string>();
                listNames.Add(node.Text);
                TreeNode nodeParent = node.Parent;
                while (nodeParent != null)
                {
                    listNames.Add(nodeParent.Text);
                    nodeParent = nodeParent.Parent;
                }

                int itemCount = listNames.Count;
                string captionText = dialogTitle + " - " + listNames[itemCount - 1];
                for (int i = itemCount - 2; i >= 0; i--)
                {
                    captionText += " > " + listNames[i];
                }
                this.Text = captionText;
            }
        }

        //private void TreeMouseDown(object sender, MouseEventArgs e)
        //{
        //    TreeNode node = optionsTreeView.GetNodeAt(
        //        optionsTreeView.PointToClient(Control.MousePosition));
        //    if (node != null)
        //    {
        //        if (node.Nodes.Count == 0)
        //            optionsTreeView.SelectedNode = node;
        //    }
        //}

        protected void AddNodes(TreeNodeCollection nodes, IEnumerable<IDialogPanelDescriptor> dialogPanelDescriptors)
        {
            optionsTreeView.BeginUpdate();
            nodes.Clear();
            foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors)
            {
                TreeNode newNode = new TreeNode(descriptor.Label);
                newNode.Tag = descriptor;
                //newNode.NodeFont = plainFont;
                nodes.Add(newNode);
                if (descriptor.ChildDialogPanelDescriptors != null)
                {
                    AddNodes(newNode.Nodes, descriptor.ChildDialogPanelDescriptors);
                }
            }
            optionsTreeView.EndUpdate();

            // If there are items, select the first one...
            if (nodes.Count > 0)
            {
                TreeNode firstNode = nodes[0];
                firstNode.Expand();
                optionsTreeView.SelectedNode = firstNode;
            }
        }

        //protected void InitImageList()
        //{
        //    ImageList imglist = new ImageList();
        //    imglist.ColorDepth = ColorDepth.Depth32Bit;
        //    imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
        //    imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
        //    imglist.Images.Add(new Bitmap(1, 1));
        //    imglist.Images.Add(IconService.GetBitmap("Icons.16x16.SelectionArrow"));

        //    optionsTreeView.ImageList = imglist;
        //}

        //protected void ShowOpenFolderIcon(object sender, TreeViewCancelEventArgs e)
        //{
        //    if (e.Node.Nodes.Count > 0)
        //    {
        //        e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
        //    }
        //}

        //protected void ShowClosedFolderIcon(object sender, TreeViewCancelEventArgs e)
        //{
        //    if (e.Node.Nodes.Count > 0)
        //    {
        //        e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
        //    }
        //}

        //public class GradientHeaderPanel : Label
        //{
        //    public GradientHeaderPanel(int fontSize)
        //        : this()
        //    {
        //        this.Font = WinFormsResourceService.LoadFont("Tahoma", fontSize);
        //    }

        //    public GradientHeaderPanel()
        //        : base()
        //    {


        //        ResizeRedraw = true;
        //        Text = String.Empty;
        //    }

        //    protected override void OnPaintBackground(PaintEventArgs pe)
        //    {
        //        base.OnPaintBackground(pe);
        //        Graphics g = pe.Graphics;

        //        using (Brush brush = new LinearGradientBrush(
        //            new Point(0, 0), new Point(Width, Height),
        //            SystemColors.Window, SystemColors.Control))
        //        {
        //            g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
        //        }
        //    }
        //}
    }
}
