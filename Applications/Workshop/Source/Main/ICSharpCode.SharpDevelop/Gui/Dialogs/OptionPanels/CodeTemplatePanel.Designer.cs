namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class CodeTemplatePanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.templateTextBox = new System.Windows.Forms.TextBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.templateListView = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.removeGroupButton = new System.Windows.Forms.Button();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.groupComboBox = new System.Windows.Forms.ComboBox();
            this.extensionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // templateTextBox
            // 
            this.templateTextBox.AcceptsReturn = true;
            this.templateTextBox.AcceptsTab = true;
            this.templateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateTextBox.Location = new System.Drawing.Point(8, 196);
            this.templateTextBox.Multiline = true;
            this.templateTextBox.Name = "templateTextBox";
            this.templateTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.templateTextBox.Size = new System.Drawing.Size(386, 202);
            this.templateTextBox.TabIndex = 0;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(319, 138);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 25);
            this.removeButton.TabIndex = 16;
            this.removeButton.Text = "${res:Global.RemoveButtonText}";
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Location = new System.Drawing.Point(319, 103);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 25);
            this.editButton.TabIndex = 15;
            this.editButton.Text = "${res:Global.EditButtonText}";
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(319, 68);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 25);
            this.addButton.TabIndex = 14;
            this.addButton.Text = "${res:Global.AddButtonText}";
            // 
            // templateListView
            // 
            this.templateListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader2});
            this.templateListView.FullRowSelect = true;
            this.templateListView.GridLines = true;
            this.templateListView.HideSelection = false;
            this.templateListView.Location = new System.Drawing.Point(8, 68);
            this.templateListView.Name = "templateListView";
            this.templateListView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.templateListView.Size = new System.Drawing.Size(305, 121);
            this.templateListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.templateListView.TabIndex = 13;
            this.templateListView.UseCompatibleStateImageBehavior = false;
            this.templateListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Name = "columnHeader";
            this.columnHeader.Text = "${res:Dialog.Options.CodeTemplate.Template}";
            this.columnHeader.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "${res:Dialog.Options.CodeTemplate.Description}";
            this.columnHeader2.Width = 140;
            // 
            // removeGroupButton
            // 
            this.removeGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeGroupButton.Location = new System.Drawing.Point(244, 35);
            this.removeGroupButton.Name = "removeGroupButton";
            this.removeGroupButton.Size = new System.Drawing.Size(116, 25);
            this.removeGroupButton.TabIndex = 12;
            this.removeGroupButton.Text = "${res:Dialog.Options.CodeTemplate.RemoveGroupLabel}";
            // 
            // addGroupButton
            // 
            this.addGroupButton.Location = new System.Drawing.Point(122, 35);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(116, 25);
            this.addGroupButton.TabIndex = 11;
            this.addGroupButton.Text = "${res:Dialog.Options.CodeTemplate.AddGroupLabel}";
            // 
            // groupComboBox
            // 
            this.groupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupComboBox.Location = new System.Drawing.Point(88, 9);
            this.groupComboBox.Name = "groupComboBox";
            this.groupComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupComboBox.Size = new System.Drawing.Size(306, 21);
            this.groupComboBox.TabIndex = 10;
            // 
            // extensionLabel
            // 
            this.extensionLabel.Location = new System.Drawing.Point(8, 9);
            this.extensionLabel.Name = "extensionLabel";
            this.extensionLabel.Size = new System.Drawing.Size(80, 20);
            this.extensionLabel.TabIndex = 9;
            this.extensionLabel.Text = "${res:Dialog.Options.CodeTemplate.ExtensionsLabel}";
            this.extensionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CodeTemplatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.templateTextBox);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.templateListView);
            this.Controls.Add(this.removeGroupButton);
            this.Controls.Add(this.addGroupButton);
            this.Controls.Add(this.groupComboBox);
            this.Controls.Add(this.extensionLabel);
            this.Name = "CodeTemplatePanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox templateTextBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListView templateListView;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button removeGroupButton;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.ComboBox groupComboBox;
        private System.Windows.Forms.Label extensionLabel;
    }
}
