namespace ICSharpCode.SharpDevelop.Gui
{
    partial class HighlightingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                nodeTree.Nodes.Clear();
                optionPanel.Controls.Clear();

                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.nodeTree = new ICSharpCode.Core.WinForms.TreeViewVista();
            this.optionPanel = new System.Windows.Forms.Panel();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.bottomSeparator = new System.Windows.Forms.Label();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(8, 9);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.nodeTree);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.optionPanel);
            this.splitContainer.Size = new System.Drawing.Size(596, 413);
            this.splitContainer.SplitterDistance = 220;
            this.splitContainer.TabIndex = 0;
            // 
            // nodeTree
            // 
            this.nodeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeTree.FullRowSelect = true;
            this.nodeTree.HideSelection = false;
            this.nodeTree.Location = new System.Drawing.Point(0, 0);
            this.nodeTree.Name = "nodeTree";
            this.nodeTree.ShowRootLines = false;
            this.nodeTree.Size = new System.Drawing.Size(220, 413);
            this.nodeTree.TabIndex = 0;
            // 
            // optionPanel
            // 
            this.optionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionPanel.Location = new System.Drawing.Point(0, 0);
            this.optionPanel.Name = "optionPanel";
            this.optionPanel.Size = new System.Drawing.Size(372, 413);
            this.optionPanel.TabIndex = 0;
            // 
            // acceptBtn
            // 
            this.acceptBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.acceptBtn.Location = new System.Drawing.Point(448, 436);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 25);
            this.acceptBtn.TabIndex = 1;
            this.acceptBtn.Text = "${res:Global.OKButtonText}";
            this.acceptBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(529, 436);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 25);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "${res:Global.CancelButtonText}";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // bottomSeparator
            // 
            this.bottomSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bottomSeparator.Location = new System.Drawing.Point(8, 428);
            this.bottomSeparator.Name = "bottomSeparator";
            this.bottomSeparator.Size = new System.Drawing.Size(596, 2);
            this.bottomSeparator.TabIndex = 3;
            // 
            // HighlightingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 470);
            this.Controls.Add(this.bottomSeparator);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.splitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HighlightingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.HighlightingEditor.EditDlg.Title}";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button acceptBtn;
        private ICSharpCode.Core.WinForms.TreeViewVista nodeTree;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Panel optionPanel;
        private System.Windows.Forms.Label bottomSeparator;
    }
}