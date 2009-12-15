namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class TaskListOptions
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.changeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.taskListView = new System.Windows.Forms.ListBox();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.changeButton);
            this.groupBox.Controls.Add(this.addButton);
            this.groupBox.Controls.Add(this.removeButton);
            this.groupBox.Controls.Add(this.label2);
            this.groupBox.Controls.Add(this.nameTextBox);
            this.groupBox.Controls.Add(this.label);
            this.groupBox.Controls.Add(this.taskListView);
            this.groupBox.Location = new System.Drawing.Point(8, 9);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(386, 291);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "${res:Dialog.Options.IDEOptions.TaskListOptions.CommentTagsGroupBox}";
            // 
            // changeButton
            // 
            this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changeButton.Location = new System.Drawing.Point(241, 125);
            this.changeButton.Name = "changeButton";
            this.changeButton.Size = new System.Drawing.Size(132, 25);
            this.changeButton.TabIndex = 5;
            this.changeButton.Text = "${res:Global.ChangeButtonText}";
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(241, 93);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(132, 25);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "${res:Global.AddButtonText}";
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(241, 156);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(132, 25);
            this.removeButton.TabIndex = 6;
            this.removeButton.Text = "${res:Global.RemoveButtonText}";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(241, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "${res:Dialog.Options.IDEOptions.TaskListOptions.NameLabel}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(241, 63);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(132, 20);
            this.nameTextBox.TabIndex = 3;
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(12, 17);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(218, 20);
            this.label.TabIndex = 0;
            this.label.Text = "${res:Dialog.Options.IDEOptions.TaskListOptions.TokenListLabel}";
            this.label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // taskListView
            // 
            this.taskListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskListView.Location = new System.Drawing.Point(12, 40);
            this.taskListView.Name = "taskListView";
            this.taskListView.Size = new System.Drawing.Size(218, 238);
            this.taskListView.TabIndex = 1;
            // 
            // columnHeader
            // 
            this.columnHeader.Name = "columnHeader";
            this.columnHeader.Width = 200;
            // 
            // TaskListOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "TaskListOptions";
            this.Size = new System.Drawing.Size(404, 412);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button changeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListBox taskListView;
        private System.Windows.Forms.ColumnHeader columnHeader;
    }
}
