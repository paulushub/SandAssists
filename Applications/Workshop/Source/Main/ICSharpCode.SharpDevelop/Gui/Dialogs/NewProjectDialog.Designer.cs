// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 2952 $</version>
// </file>
namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	partial class NewProjectDialog
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.categoryTreeView = new ICSharpCode.Core.WinForms.TreeViewVista();
            this.label1 = new System.Windows.Forms.Label();
            this.templateListView = new System.Windows.Forms.ListView();
            this.largeIconsRadioButton = new System.Windows.Forms.RadioButton();
            this.smallIconsRadioButton = new System.Windows.Forms.RadioButton();
            this.targetFrameworkComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bottomPanel = new ICSharpCode.SharpDevelop.Gui.PanelWithSizeGrip();
            this.label5 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.createInLabel = new System.Windows.Forms.Label();
            this.createDirectoryForSolutionCheckBox = new System.Windows.Forms.CheckBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.solutionNameTextBox = new System.Windows.Forms.TextBox();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.solutionNameLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.categoryTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.templateListView);
            this.splitContainer1.Panel2.Controls.Add(this.largeIconsRadioButton);
            this.splitContainer1.Panel2.Controls.Add(this.smallIconsRadioButton);
            this.splitContainer1.Panel2.Controls.Add(this.targetFrameworkComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(668, 282);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 0;
            // 
            // categoryTreeView
            // 
            this.categoryTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.categoryTreeView.HideSelection = false;
            this.categoryTreeView.Location = new System.Drawing.Point(12, 33);
            this.categoryTreeView.Name = "categoryTreeView";
            this.categoryTreeView.Size = new System.Drawing.Size(226, 245);
            this.categoryTreeView.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(10, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "${res:Dialog.NewProject.ProjectTypeLabelText}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // templateListView
            // 
            this.templateListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateListView.HideSelection = false;
            this.templateListView.Location = new System.Drawing.Point(0, 33);
            this.templateListView.Name = "templateListView";
            this.templateListView.Size = new System.Drawing.Size(414, 245);
            this.templateListView.TabIndex = 4;
            this.templateListView.UseCompatibleStateImageBehavior = false;
            // 
            // largeIconsRadioButton
            // 
            this.largeIconsRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.largeIconsRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.largeIconsRadioButton.Location = new System.Drawing.Point(390, 4);
            this.largeIconsRadioButton.Name = "largeIconsRadioButton";
            this.largeIconsRadioButton.Size = new System.Drawing.Size(24, 26);
            this.largeIconsRadioButton.TabIndex = 2;
            // 
            // smallIconsRadioButton
            // 
            this.smallIconsRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.smallIconsRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.smallIconsRadioButton.Location = new System.Drawing.Point(361, 4);
            this.smallIconsRadioButton.Name = "smallIconsRadioButton";
            this.smallIconsRadioButton.Size = new System.Drawing.Size(24, 26);
            this.smallIconsRadioButton.TabIndex = 1;
            // 
            // targetFrameworkComboBox
            // 
            this.targetFrameworkComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFrameworkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetFrameworkComboBox.Location = new System.Drawing.Point(160, 7);
            this.targetFrameworkComboBox.Name = "targetFrameworkComboBox";
            this.targetFrameworkComboBox.Size = new System.Drawing.Size(195, 21);
            this.targetFrameworkComboBox.TabIndex = 0;
            this.targetFrameworkComboBox.SelectedIndexChanged += new System.EventHandler(this.TargetFrameworkComboBoxSelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(0, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "${res:Dialog.NewProject.TemplateLabelText}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.label5);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.openButton);
            this.bottomPanel.Controls.Add(this.createInLabel);
            this.bottomPanel.Controls.Add(this.createDirectoryForSolutionCheckBox);
            this.bottomPanel.Controls.Add(this.browseButton);
            this.bottomPanel.Controls.Add(this.solutionNameTextBox);
            this.bottomPanel.Controls.Add(this.locationTextBox);
            this.bottomPanel.Controls.Add(this.solutionNameLabel);
            this.bottomPanel.Controls.Add(this.label4);
            this.bottomPanel.Controls.Add(this.nameTextBox);
            this.bottomPanel.Controls.Add(this.label3);
            this.bottomPanel.Controls.Add(this.descriptionLabel);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 282);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(668, 176);
            this.bottomPanel.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(12, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(646, 2);
            this.label5.TabIndex = 12;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(581, 141);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Enabled = false;
            this.openButton.Location = new System.Drawing.Point(500, 142);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 25);
            this.openButton.TabIndex = 10;
            this.openButton.Text = "${res:Global.OKButtonText}";
            // 
            // createInLabel
            // 
            this.createInLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.createInLabel.Location = new System.Drawing.Point(12, 106);
            this.createInLabel.Name = "createInLabel";
            this.createInLabel.Size = new System.Drawing.Size(642, 21);
            this.createInLabel.TabIndex = 9;
            this.createInLabel.Text = "label6";
            this.createInLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // createDirectoryForSolutionCheckBox
            // 
            this.createDirectoryForSolutionCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createDirectoryForSolutionCheckBox.Location = new System.Drawing.Point(429, 78);
            this.createDirectoryForSolutionCheckBox.Name = "createDirectoryForSolutionCheckBox";
            this.createDirectoryForSolutionCheckBox.Size = new System.Drawing.Size(222, 24);
            this.createDirectoryForSolutionCheckBox.TabIndex = 8;
            this.createDirectoryForSolutionCheckBox.Text = "${res:Dialog.NewProject.CreateDirectoryForSolution}";
            this.createDirectoryForSolutionCheckBox.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(589, 52);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(67, 25);
            this.browseButton.TabIndex = 5;
            this.browseButton.Text = "${res:Global.BrowseButtonText}";
            // 
            // solutionNameTextBox
            // 
            this.solutionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.solutionNameTextBox.Location = new System.Drawing.Point(120, 80);
            this.solutionNameTextBox.Name = "solutionNameTextBox";
            this.solutionNameTextBox.Size = new System.Drawing.Size(303, 20);
            this.solutionNameTextBox.TabIndex = 7;
            // 
            // locationTextBox
            // 
            this.locationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationTextBox.Location = new System.Drawing.Point(120, 54);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(463, 20);
            this.locationTextBox.TabIndex = 4;
            // 
            // solutionNameLabel
            // 
            this.solutionNameLabel.Location = new System.Drawing.Point(12, 80);
            this.solutionNameLabel.Name = "solutionNameLabel";
            this.solutionNameLabel.Size = new System.Drawing.Size(102, 23);
            this.solutionNameLabel.TabIndex = 6;
            this.solutionNameLabel.Text = "${res:Dialog.NewProject.SolutionName}";
            this.solutionNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "${res:Dialog.NewProject.LocationLabelText}";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(120, 28);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(463, 20);
            this.nameTextBox.TabIndex = 2;
            this.nameTextBox.TextChanged += new System.EventHandler(this.NameTextBoxTextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "${res:Dialog.NewProject.NameLabelText}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.Location = new System.Drawing.Point(12, 2);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.ReadOnly = true;
            this.descriptionLabel.Size = new System.Drawing.Size(644, 20);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "label3";
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.openButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(668, 458);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bottomPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(405, 290);
            this.Name = "NewProjectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.NewProject.DialogName}";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ICSharpCode.SharpDevelop.Gui.PanelWithSizeGrip bottomPanel;
		private System.Windows.Forms.Label solutionNameLabel;
		private System.Windows.Forms.CheckBox createDirectoryForSolutionCheckBox;
		private System.Windows.Forms.ListView templateListView;
		private System.Windows.Forms.TextBox solutionNameTextBox;
        private ICSharpCode.Core.WinForms.TreeViewVista categoryTreeView;
		private System.Windows.Forms.Button openButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label createInLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox locationTextBox;
		private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox descriptionLabel;
		private System.Windows.Forms.ComboBox targetFrameworkComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton smallIconsRadioButton;
		private System.Windows.Forms.RadioButton largeIconsRadioButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label5;
	}
}
