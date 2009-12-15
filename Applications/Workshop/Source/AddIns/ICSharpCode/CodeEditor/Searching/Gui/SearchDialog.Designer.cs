namespace ICSharpCode.TextEditor.Searching
{
    partial class SearchDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDialog));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.useButton = new System.Windows.Forms.ToolStripLabel();
            this.useComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.searchHelp = new System.Windows.Forms.ToolStripButton();
            this.searchPanel = new ICSharpCode.TextEditor.Searching.SearchPanel();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchButton,
            this.toolStripSeparator1,
            this.replaceButton,
            this.toolStripSeparator2,
            this.useButton,
            this.useComboBox,
            this.searchHelp});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(434, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // searchButton
            // 
            this.searchButton.Image = ((System.Drawing.Image)(resources.GetObject("searchButton.Image")));
            this.searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchButton.Margin = new System.Windows.Forms.Padding(4, 1, 4, 2);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(50, 22);
            this.searchButton.Text = "Find";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // replaceButton
            // 
            this.replaceButton.Image = ((System.Drawing.Image)(resources.GetObject("replaceButton.Image")));
            this.replaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceButton.Margin = new System.Windows.Forms.Padding(4, 1, 4, 2);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(68, 22);
            this.replaceButton.Text = "Replace";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // useButton
            // 
            this.useButton.Margin = new System.Windows.Forms.Padding(4, 1, 4, 2);
            this.useButton.Name = "useButton";
            this.useButton.Size = new System.Drawing.Size(29, 22);
            this.useButton.Text = "Use:";
            // 
            // useComboBox
            // 
            this.useComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.useComboBox.Name = "useComboBox";
            this.useComboBox.Size = new System.Drawing.Size(180, 25);
            // 
            // searchHelp
            // 
            this.searchHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchHelp.Image = ((System.Drawing.Image)(resources.GetObject("searchHelp.Image")));
            this.searchHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchHelp.Margin = new System.Windows.Forms.Padding(0, 1, 4, 2);
            this.searchHelp.Name = "searchHelp";
            this.searchHelp.Size = new System.Drawing.Size(52, 22);
            this.searchHelp.Text = "Help";
            // 
            // searchPanel
            // 
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPanel.Location = new System.Drawing.Point(0, 25);
            this.searchPanel.MinimumSize = new System.Drawing.Size(432, 260);
            this.searchPanel.Mode = ICSharpCode.TextEditor.Searching.SearchAndReplaceMode.Search;
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(434, 264);
            this.searchPanel.TabIndex = 1;
            // 
            // SearchDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 289);
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(450, 323);
            this.Name = "SearchDialog";
            this.ShowInTaskbar = false;
            this.Text = "SearchDialog";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton replaceButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel useButton;
        private System.Windows.Forms.ToolStripComboBox useComboBox;
        private SearchPanel searchPanel;
        private System.Windows.Forms.ToolStripButton searchHelp;
    }
}