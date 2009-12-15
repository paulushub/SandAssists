namespace ICSharpCode.FormsDesigner.Gui
{
    partial class ConfigureSideBarDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureSideBarDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.componentListView = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.categoryListView = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tbbCategories = new System.Windows.Forms.ToolStripLabel();
            this.tbbAddCategory = new System.Windows.Forms.ToolStripButton();
            this.tbbRemoveCategory = new System.Windows.Forms.ToolStripButton();
            this.tbbRenameCategory = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbComponents = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tbbAddComponents = new System.Windows.Forms.ToolStripButton();
            this.tbbRemoveComponents = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(599, 364);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(518, 364);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "${res:Global.OKButtonText}";
            this.okButton.Click += new System.EventHandler(this.okButtonClick);
            // 
            // componentListView
            // 
            this.componentListView.CheckBoxes = true;
            this.componentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader2,
            this.columnHeader3});
            this.componentListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentListView.FullRowSelect = true;
            this.componentListView.GridLines = true;
            this.componentListView.HideSelection = false;
            this.componentListView.Location = new System.Drawing.Point(0, 0);
            this.componentListView.Name = "componentListView";
            this.componentListView.Size = new System.Drawing.Size(450, 329);
            this.componentListView.TabIndex = 2;
            this.componentListView.UseCompatibleStateImageBehavior = false;
            this.componentListView.View = System.Windows.Forms.View.Details;
            this.componentListView.SelectedIndexChanged += new System.EventHandler(this.componentListViewSelectedIndexChanged);
            // 
            // columnHeader
            // 
            this.columnHeader.Name = "columnHeader";
            this.columnHeader.Text = "${res:Global.Name}";
            this.columnHeader.Width = 123;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Namespace}" +
                "";
            this.columnHeader2.Width = 145;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Name = "columnHeader3";
            this.columnHeader3.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Assembly}";
            this.columnHeader3.Width = 174;
            // 
            // categoryListView
            // 
            this.categoryListView.CheckBoxes = true;
            this.categoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.categoryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryListView.FullRowSelect = true;
            this.categoryListView.HideSelection = false;
            this.categoryListView.Location = new System.Drawing.Point(0, 0);
            this.categoryListView.MultiSelect = false;
            this.categoryListView.Name = "categoryListView";
            this.categoryListView.Size = new System.Drawing.Size(214, 329);
            this.categoryListView.TabIndex = 0;
            this.categoryListView.UseCompatibleStateImageBehavior = false;
            this.categoryListView.View = System.Windows.Forms.View.Details;
            this.categoryListView.SelectedIndexChanged += new System.EventHandler(this.categoryListViewSelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Name = "columnHeader5";
            this.columnHeader5.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Categories" +
                "}";
            this.columnHeader5.Width = 205;
            // 
            // toolStrip
            // 
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbCategories,
            this.tbbAddCategory,
            this.tbbRemoveCategory,
            this.tbbRenameCategory,
            this.toolStripSeparator1,
            this.tbbComponents,
            this.tbbAddComponents,
            this.tbbRemoveComponents});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(682, 25);
            this.toolStrip.TabIndex = 12;
            this.toolStrip.Text = "toolStrip";
            // 
            // tbbCategories
            // 
            this.tbbCategories.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbbCategories.Name = "tbbCategories";
            this.tbbCategories.Size = new System.Drawing.Size(73, 22);
            this.tbbCategories.Text = "Categories:";
            // 
            // tbbAddCategory
            // 
            this.tbbAddCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbAddCategory.Image = ((System.Drawing.Image)(resources.GetObject("tbbAddCategory.Image")));
            this.tbbAddCategory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbAddCategory.Margin = new System.Windows.Forms.Padding(4, 1, 4, 2);
            this.tbbAddCategory.Name = "tbbAddCategory";
            this.tbbAddCategory.Size = new System.Drawing.Size(29, 22);
            this.tbbAddCategory.Text = "Add";
            this.tbbAddCategory.Click += new System.EventHandler(this.newCategoryButtonClick);
            // 
            // tbbRemoveCategory
            // 
            this.tbbRemoveCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbRemoveCategory.Image = ((System.Drawing.Image)(resources.GetObject("tbbRemoveCategory.Image")));
            this.tbbRemoveCategory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbRemoveCategory.Margin = new System.Windows.Forms.Padding(0, 1, 4, 2);
            this.tbbRemoveCategory.Name = "tbbRemoveCategory";
            this.tbbRemoveCategory.Size = new System.Drawing.Size(50, 22);
            this.tbbRemoveCategory.Text = "Remove";
            this.tbbRemoveCategory.Click += new System.EventHandler(this.removeCategoryButtonClick);
            // 
            // tbbRenameCategory
            // 
            this.tbbRenameCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbRenameCategory.Image = ((System.Drawing.Image)(resources.GetObject("tbbRenameCategory.Image")));
            this.tbbRenameCategory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbRenameCategory.Name = "tbbRenameCategory";
            this.tbbRenameCategory.Size = new System.Drawing.Size(50, 22);
            this.tbbRenameCategory.Text = "Rename";
            this.tbbRenameCategory.Click += new System.EventHandler(this.renameCategoryButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbbComponents
            // 
            this.tbbComponents.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbbComponents.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.tbbComponents.Name = "tbbComponents";
            this.tbbComponents.Size = new System.Drawing.Size(81, 22);
            this.tbbComponents.Text = "Components:";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(6, 28);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.categoryListView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.componentListView);
            this.splitContainer.Size = new System.Drawing.Size(668, 329);
            this.splitContainer.SplitterDistance = 214;
            this.splitContainer.TabIndex = 13;
            // 
            // tbbAddComponents
            // 
            this.tbbAddComponents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbAddComponents.Image = ((System.Drawing.Image)(resources.GetObject("tbbAddComponents.Image")));
            this.tbbAddComponents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbAddComponents.Margin = new System.Windows.Forms.Padding(4, 1, 4, 2);
            this.tbbAddComponents.Name = "tbbAddComponents";
            this.tbbAddComponents.Size = new System.Drawing.Size(29, 22);
            this.tbbAddComponents.Text = "Add";
            this.tbbAddComponents.Click += new System.EventHandler(this.addComponentsButtonClick);
            // 
            // tbbRemoveComponents
            // 
            this.tbbRemoveComponents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbRemoveComponents.Image = ((System.Drawing.Image)(resources.GetObject("tbbRemoveComponents.Image")));
            this.tbbRemoveComponents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbRemoveComponents.Name = "tbbRemoveComponents";
            this.tbbRemoveComponents.Size = new System.Drawing.Size(50, 22);
            this.tbbRemoveComponents.Text = "Remove";
            this.tbbRemoveComponents.Click += new System.EventHandler(this.removeComponentsButtonClick);
            // 
            // ConfigureSideBarDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 394);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureSideBarDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.DialogName" +
                "}";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ListView componentListView;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView categoryListView;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel tbbCategories;
        private System.Windows.Forms.ToolStripButton tbbAddCategory;
        private System.Windows.Forms.ToolStripButton tbbRemoveCategory;
        private System.Windows.Forms.ToolStripButton tbbRenameCategory;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tbbComponents;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripButton tbbAddComponents;
        private System.Windows.Forms.ToolStripButton tbbRemoveComponents;
    }
}