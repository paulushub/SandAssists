namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class BuildEventsPanel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.runPostBuildEventComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.postBuildEventTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.postBuildEventBrowseButton = new System.Windows.Forms.Button();
            this.preBuildEventTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.preBuildEventBrowseButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.runPostBuildEventComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.postBuildEventTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.postBuildEventBrowseButton);
            this.groupBox1.Controls.Add(this.preBuildEventTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.preBuildEventBrowseButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 437);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "${res:Dialog.ProjectOptions.BuildEvents}";
            // 
            // runPostBuildEventComboBox
            // 
            this.runPostBuildEventComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.runPostBuildEventComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.runPostBuildEventComboBox.FormattingEnabled = true;
            this.runPostBuildEventComboBox.Location = new System.Drawing.Point(8, 406);
            this.runPostBuildEventComboBox.Name = "runPostBuildEventComboBox";
            this.runPostBuildEventComboBox.Size = new System.Drawing.Size(264, 21);
            this.runPostBuildEventComboBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(8, 382);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(428, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "${res:Dialog.ProjectOptions.BuildEvents.RunPostBuild}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // postBuildEventTextBox
            // 
            this.postBuildEventTextBox.AcceptsReturn = true;
            this.postBuildEventTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuildEventTextBox.Location = new System.Drawing.Point(8, 223);
            this.postBuildEventTextBox.Multiline = true;
            this.postBuildEventTextBox.Name = "postBuildEventTextBox";
            this.postBuildEventTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.postBuildEventTextBox.Size = new System.Drawing.Size(430, 123);
            this.postBuildEventTextBox.TabIndex = 5;
            this.postBuildEventTextBox.WordWrap = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(8, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(424, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "${res:Dialog.ProjectOptions.BuildEvents.PostBuild}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // postBuildEventBrowseButton
            // 
            this.postBuildEventBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuildEventBrowseButton.Enabled = false;
            this.postBuildEventBrowseButton.Location = new System.Drawing.Point(297, 353);
            this.postBuildEventBrowseButton.Name = "postBuildEventBrowseButton";
            this.postBuildEventBrowseButton.Size = new System.Drawing.Size(141, 25);
            this.postBuildEventBrowseButton.TabIndex = 6;
            this.postBuildEventBrowseButton.Text = "Edit Post-build...";
            this.postBuildEventBrowseButton.UseVisualStyleBackColor = true;
            // 
            // preBuildEventTextBox
            // 
            this.preBuildEventTextBox.AcceptsReturn = true;
            this.preBuildEventTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuildEventTextBox.Location = new System.Drawing.Point(8, 41);
            this.preBuildEventTextBox.Multiline = true;
            this.preBuildEventTextBox.Name = "preBuildEventTextBox";
            this.preBuildEventTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.preBuildEventTextBox.Size = new System.Drawing.Size(430, 123);
            this.preBuildEventTextBox.TabIndex = 2;
            this.preBuildEventTextBox.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(424, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "${res:Dialog.ProjectOptions.BuildEvents.PreBuild}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // preBuildEventBrowseButton
            // 
            this.preBuildEventBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuildEventBrowseButton.Enabled = false;
            this.preBuildEventBrowseButton.Location = new System.Drawing.Point(297, 171);
            this.preBuildEventBrowseButton.Name = "preBuildEventBrowseButton";
            this.preBuildEventBrowseButton.Size = new System.Drawing.Size(141, 25);
            this.preBuildEventBrowseButton.TabIndex = 3;
            this.preBuildEventBrowseButton.Text = "Edit Pre-build...";
            this.preBuildEventBrowseButton.UseVisualStyleBackColor = true;
            // 
            // BuildEventsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "BuildEventsPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox runPostBuildEventComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox postBuildEventTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button postBuildEventBrowseButton;
        private System.Windows.Forms.TextBox preBuildEventTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button preBuildEventBrowseButton;
    }
}
