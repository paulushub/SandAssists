// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 2229 $</version>
// </file>

using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Core;
using ICSharpCode.CodeEditor.HighlightingEditor.Nodes;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class HighlightingDialog : Form
    {
        private NodeOptionPanel currentPanel;

        public HighlightingDialog()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);
        }

        public HighlightingDialog(TreeNode topNode)
            : this()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.EditDialog.xfrm"));
            //acceptBtn = (Button)ControlDictionary["acceptBtn"];
            //nodeTree = (TreeView)ControlDictionary["nodeTree"];
            //propPanel = (Panel)ControlDictionary["propPanel"];
            //optionPanel = (Panel)ControlDictionary["optionPanel"];

            // Form editor does not work properly with the custom control

            //this.ClientSize = new Size(660, 530);
            this.acceptBtn.Click += new EventHandler(acceptClick);
            this.cancelBtn.Click += new EventHandler(cancelClick);

            this.nodeTree.AfterSelect  += new System.Windows.Forms.TreeViewEventHandler(this.NodeTreeAfterSelect);
            this.nodeTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.NodeTreeBeforeSelect);

            this.nodeTree.Nodes.Add(topNode);
            this.nodeTree.ExpandAll();

            this.nodeTree.SelectedNode = topNode;
        }

        private void cancelClick(object sender, EventArgs e)
        {
            if (currentPanel != null)
            {
                currentPanel.ParentNode.UpdateNodeText();
                optionPanel.Controls.Clear();
            }
            nodeTree.Nodes.Clear();
            DialogResult = DialogResult.Cancel;
        }

        private void acceptClick(object sender, EventArgs e)
        {
            if (currentPanel != null)
            {
                if (!currentPanel.ValidateSettings())
                {
                    return;
                }
                currentPanel.StoreSettings();
                currentPanel.ParentNode.UpdateNodeText();
                optionPanel.Controls.Clear();
            }
            nodeTree.Nodes.Clear();
            DialogResult = DialogResult.OK;
        }

        private void NodeTreeBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (currentPanel != null)
            {
                if (!currentPanel.ValidateSettings())
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void NodeTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (currentPanel != null)
            {
                currentPanel.StoreSettings();
                currentPanel.ParentNode.UpdateNodeText();
            }

            optionPanel.Controls.Clear();
            NodeOptionPanel control = ((AbstractNode)e.Node).OptionPanel;
            if (control != null)
            {
                optionPanel.Controls.Add(control);
                currentPanel = control;
                currentPanel.LoadSettings();
            }
        }
    }
}
