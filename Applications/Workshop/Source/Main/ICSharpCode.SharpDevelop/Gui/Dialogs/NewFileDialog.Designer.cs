namespace ICSharpCode.SharpDevelop.Gui
{
	partial class NewFileDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.bottomPanel = new ICSharpCode.SharpDevelop.Gui.PanelWithSizeGrip();
            this.label3 = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.TextBox();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.categoryTreeView = new ICSharpCode.Core.WinForms.TreeViewVista();
            this.propertySplitContainer = new System.Windows.Forms.SplitContainer();
            this.templateListView = new System.Windows.Forms.ListView();
            this.propertyGrid = new ICSharpCode.Core.WinForms.PropertyGridEx();
            this.label2 = new System.Windows.Forms.Label();
            this.smallIconsRadioButton = new System.Windows.Forms.RadioButton();
            this.largeIconsRadioButton = new System.Windows.Forms.RadioButton();
            this.bottomPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.propertySplitContainer.Panel1.SuspendLayout();
            this.propertySplitContainer.Panel2.SuspendLayout();
            this.propertySplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.label3);
            this.bottomPanel.Controls.Add(this.fileNameTextBox);
            this.bottomPanel.Controls.Add(this.descriptionLabel);
            this.bottomPanel.Controls.Add(this.lblSeparator);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.openButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 390);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(674, 68);
            this.bottomPanel.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(11, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "${res:Dialog.NewFile.FileNameLabel}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTextBox.Location = new System.Drawing.Point(91, 37);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(360, 20);
            this.fileNameTextBox.TabIndex = 4;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.Location = new System.Drawing.Point(8, 2);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.ReadOnly = true;
            this.descriptionLabel.Size = new System.Drawing.Size(654, 20);
            this.descriptionLabel.TabIndex = 3;
            // 
            // lblSeparator
            // 
            this.lblSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeparator.Location = new System.Drawing.Point(7, 28);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(656, 2);
            this.lblSeparator.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(568, 35);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(94, 25);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Location = new System.Drawing.Point(469, 35);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(94, 25);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "${res:Global.CreateButtonText}";
            this.openButton.UseVisualStyleBackColor = true;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.label1);
            this.splitContainer.Panel1.Controls.Add(this.categoryTreeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.propertySplitContainer);
            this.splitContainer.Panel2.Controls.Add(this.label2);
            this.splitContainer.Panel2.Controls.Add(this.smallIconsRadioButton);
            this.splitContainer.Panel2.Controls.Add(this.largeIconsRadioButton);
            this.splitContainer.Size = new System.Drawing.Size(674, 390);
            this.splitContainer.SplitterDistance = 220;
            this.splitContainer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "${res:Dialog.NewFile.CategoryText}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // categoryTreeView
            // 
            this.categoryTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.categoryTreeView.HideSelection = false;
            this.categoryTreeView.Location = new System.Drawing.Point(8, 34);
            this.categoryTreeView.Name = "categoryTreeView";
            this.categoryTreeView.Size = new System.Drawing.Size(212, 353);
            this.categoryTreeView.TabIndex = 0;
            // 
            // propertySplitContainer
            // 
            this.propertySplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertySplitContainer.Location = new System.Drawing.Point(0, 34);
            this.propertySplitContainer.Name = "propertySplitContainer";
            this.propertySplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // propertySplitContainer.Panel1
            // 
            this.propertySplitContainer.Panel1.Controls.Add(this.templateListView);
            // 
            // propertySplitContainer.Panel2
            // 
            this.propertySplitContainer.Panel2.Controls.Add(this.propertyGrid);
            this.propertySplitContainer.Panel2Collapsed = true;
            this.propertySplitContainer.Size = new System.Drawing.Size(438, 353);
            this.propertySplitContainer.SplitterDistance = 192;
            this.propertySplitContainer.TabIndex = 4;
            // 
            // templateListView
            // 
            this.templateListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateListView.Location = new System.Drawing.Point(0, 0);
            this.templateListView.Name = "templateListView";
            this.templateListView.Size = new System.Drawing.Size(438, 353);
            this.templateListView.TabIndex = 0;
            this.templateListView.UseCompatibleStateImageBehavior = false;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(150, 46);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(309, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "${res:Dialog.NewFile.TemplateText}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // smallIconsRadioButton
            // 
            this.smallIconsRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.smallIconsRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.smallIconsRadioButton.Location = new System.Drawing.Point(384, 3);
            this.smallIconsRadioButton.Name = "smallIconsRadioButton";
            this.smallIconsRadioButton.Size = new System.Drawing.Size(24, 26);
            this.smallIconsRadioButton.TabIndex = 2;
            this.smallIconsRadioButton.TabStop = true;
            this.smallIconsRadioButton.UseVisualStyleBackColor = true;
            // 
            // largeIconsRadioButton
            // 
            this.largeIconsRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.largeIconsRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.largeIconsRadioButton.Location = new System.Drawing.Point(414, 3);
            this.largeIconsRadioButton.Name = "largeIconsRadioButton";
            this.largeIconsRadioButton.Size = new System.Drawing.Size(24, 26);
            this.largeIconsRadioButton.TabIndex = 1;
            this.largeIconsRadioButton.TabStop = true;
            this.largeIconsRadioButton.UseVisualStyleBackColor = true;
            // 
            // NewFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 458);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.bottomPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.NewFile.DialogName}";
            this.Load += new System.EventHandler(this.OnLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.propertySplitContainer.Panel1.ResumeLayout(false);
            this.propertySplitContainer.Panel2.ResumeLayout(false);
            this.propertySplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private PanelWithSizeGrip bottomPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.TextBox descriptionLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView templateListView;
        private ICSharpCode.Core.WinForms.TreeViewVista categoryTreeView;
        private System.Windows.Forms.RadioButton largeIconsRadioButton;
        private System.Windows.Forms.RadioButton smallIconsRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.SplitContainer propertySplitContainer;
        private ICSharpCode.Core.WinForms.PropertyGridEx propertyGrid;
	}
}