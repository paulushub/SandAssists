namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class DebugPanel
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
            this.startActionGroupBox = new System.Windows.Forms.GroupBox();
            this.startBrowserInURLTextBox = new System.Windows.Forms.TextBox();
            this.startExternalProgramBrowseButton = new System.Windows.Forms.Button();
            this.startExternalProgramTextBox = new System.Windows.Forms.TextBox();
            this.startBrowserInURLRadioButton = new System.Windows.Forms.RadioButton();
            this.startExternalProgramRadioButton = new System.Windows.Forms.RadioButton();
            this.startProjectRadioButton = new System.Windows.Forms.RadioButton();
            this.startOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.commandLineArgumentsTextBox = new System.Windows.Forms.TextBox();
            this.workingDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.workingDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.startActionGroupBox.SuspendLayout();
            this.startOptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // startActionGroupBox
            // 
            this.startActionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startActionGroupBox.Controls.Add(this.startBrowserInURLTextBox);
            this.startActionGroupBox.Controls.Add(this.startExternalProgramBrowseButton);
            this.startActionGroupBox.Controls.Add(this.startExternalProgramTextBox);
            this.startActionGroupBox.Controls.Add(this.startBrowserInURLRadioButton);
            this.startActionGroupBox.Controls.Add(this.startExternalProgramRadioButton);
            this.startActionGroupBox.Controls.Add(this.startProjectRadioButton);
            this.startActionGroupBox.Location = new System.Drawing.Point(8, 9);
            this.startActionGroupBox.Name = "startActionGroupBox";
            this.startActionGroupBox.Size = new System.Drawing.Size(444, 104);
            this.startActionGroupBox.TabIndex = 10;
            this.startActionGroupBox.TabStop = false;
            this.startActionGroupBox.Text = "${res:Dialog.ProjectOptions.DebugOptions.StartAction}";
            // 
            // startBrowserInURLTextBox
            // 
            this.startBrowserInURLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startBrowserInURLTextBox.Location = new System.Drawing.Point(156, 74);
            this.startBrowserInURLTextBox.Name = "startBrowserInURLTextBox";
            this.startBrowserInURLTextBox.Size = new System.Drawing.Size(280, 20);
            this.startBrowserInURLTextBox.TabIndex = 8;
            // 
            // startExternalProgramBrowseButton
            // 
            this.startExternalProgramBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startExternalProgramBrowseButton.Location = new System.Drawing.Point(406, 42);
            this.startExternalProgramBrowseButton.Name = "startExternalProgramBrowseButton";
            this.startExternalProgramBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.startExternalProgramBrowseButton.TabIndex = 7;
            this.startExternalProgramBrowseButton.Text = "...";
            this.startExternalProgramBrowseButton.UseVisualStyleBackColor = true;
            // 
            // startExternalProgramTextBox
            // 
            this.startExternalProgramTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startExternalProgramTextBox.Location = new System.Drawing.Point(156, 46);
            this.startExternalProgramTextBox.Name = "startExternalProgramTextBox";
            this.startExternalProgramTextBox.Size = new System.Drawing.Size(244, 20);
            this.startExternalProgramTextBox.TabIndex = 3;
            // 
            // startBrowserInURLRadioButton
            // 
            this.startBrowserInURLRadioButton.Location = new System.Drawing.Point(8, 74);
            this.startBrowserInURLRadioButton.Name = "startBrowserInURLRadioButton";
            this.startBrowserInURLRadioButton.Size = new System.Drawing.Size(148, 20);
            this.startBrowserInURLRadioButton.TabIndex = 2;
            this.startBrowserInURLRadioButton.Text = "${res:Dialog.ProjectOptions.DebugOptions.StartBrowser}";
            // 
            // startExternalProgramRadioButton
            // 
            this.startExternalProgramRadioButton.Location = new System.Drawing.Point(8, 46);
            this.startExternalProgramRadioButton.Name = "startExternalProgramRadioButton";
            this.startExternalProgramRadioButton.Size = new System.Drawing.Size(148, 20);
            this.startExternalProgramRadioButton.TabIndex = 1;
            this.startExternalProgramRadioButton.Text = "${res:Dialog.ProjectOptions.DebugOptions.StartProgram}";
            // 
            // startProjectRadioButton
            // 
            this.startProjectRadioButton.Location = new System.Drawing.Point(8, 17);
            this.startProjectRadioButton.Name = "startProjectRadioButton";
            this.startProjectRadioButton.Size = new System.Drawing.Size(148, 20);
            this.startProjectRadioButton.TabIndex = 0;
            this.startProjectRadioButton.Text = "${res:Dialog.ProjectOptions.DebugOptions.StartProject}";
            // 
            // startOptionsGroupBox
            // 
            this.startOptionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startOptionsGroupBox.Controls.Add(this.label2);
            this.startOptionsGroupBox.Controls.Add(this.label1);
            this.startOptionsGroupBox.Controls.Add(this.commandLineArgumentsTextBox);
            this.startOptionsGroupBox.Controls.Add(this.workingDirectoryBrowseButton);
            this.startOptionsGroupBox.Controls.Add(this.workingDirectoryTextBox);
            this.startOptionsGroupBox.Location = new System.Drawing.Point(8, 128);
            this.startOptionsGroupBox.Name = "startOptionsGroupBox";
            this.startOptionsGroupBox.Size = new System.Drawing.Size(444, 82);
            this.startOptionsGroupBox.TabIndex = 11;
            this.startOptionsGroupBox.TabStop = false;
            this.startOptionsGroupBox.Text = "${res:Dialog.ProjectOptions.DebugOptions.StartOptions}";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 22);
            this.label2.TabIndex = 12;
            this.label2.Text = "${res:Dialog.ProjectOptions.DebugOptions.WorkingDir}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 22);
            this.label1.TabIndex = 11;
            this.label1.Text = "${res:Dialog.ProjectOptions.DebugOptions.Arguments}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // commandLineArgumentsTextBox
            // 
            this.commandLineArgumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commandLineArgumentsTextBox.Location = new System.Drawing.Point(156, 22);
            this.commandLineArgumentsTextBox.Name = "commandLineArgumentsTextBox";
            this.commandLineArgumentsTextBox.Size = new System.Drawing.Size(280, 20);
            this.commandLineArgumentsTextBox.TabIndex = 10;
            // 
            // workingDirectoryBrowseButton
            // 
            this.workingDirectoryBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.workingDirectoryBrowseButton.Location = new System.Drawing.Point(406, 47);
            this.workingDirectoryBrowseButton.Name = "workingDirectoryBrowseButton";
            this.workingDirectoryBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.workingDirectoryBrowseButton.TabIndex = 7;
            this.workingDirectoryBrowseButton.Text = "...";
            this.workingDirectoryBrowseButton.UseVisualStyleBackColor = true;
            // 
            // workingDirectoryTextBox
            // 
            this.workingDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workingDirectoryTextBox.Location = new System.Drawing.Point(156, 49);
            this.workingDirectoryTextBox.Name = "workingDirectoryTextBox";
            this.workingDirectoryTextBox.Size = new System.Drawing.Size(244, 20);
            this.workingDirectoryTextBox.TabIndex = 3;
            // 
            // DebugPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.startActionGroupBox);
            this.Controls.Add(this.startOptionsGroupBox);
            this.Name = "DebugPanel";
            this.startActionGroupBox.ResumeLayout(false);
            this.startActionGroupBox.PerformLayout();
            this.startOptionsGroupBox.ResumeLayout(false);
            this.startOptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox startActionGroupBox;
        private System.Windows.Forms.TextBox startBrowserInURLTextBox;
        private System.Windows.Forms.Button startExternalProgramBrowseButton;
        private System.Windows.Forms.TextBox startExternalProgramTextBox;
        private System.Windows.Forms.RadioButton startBrowserInURLRadioButton;
        private System.Windows.Forms.RadioButton startExternalProgramRadioButton;
        private System.Windows.Forms.RadioButton startProjectRadioButton;
        private System.Windows.Forms.GroupBox startOptionsGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox commandLineArgumentsTextBox;
        private System.Windows.Forms.Button workingDirectoryBrowseButton;
        private System.Windows.Forms.TextBox workingDirectoryTextBox;
    }
}
