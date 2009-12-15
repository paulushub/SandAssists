namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class ApplicationPanel
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
            this.applicationManifestComboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.win32ResourceFileBrowseButton = new System.Windows.Forms.Button();
            this.win32ResourceFileTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.applicationIconBrowseButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.outputNameTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.projectFileTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.projectFolderTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.applicationIconPictureBox = new System.Windows.Forms.PictureBox();
            this.applicationIconTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.startupObjectComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.outputTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rootNamespaceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.assemblyNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.convertProjectToMSBuild35Button = new System.Windows.Forms.Button();
            this.targetFrameworkLabel = new System.Windows.Forms.Label();
            this.targetFrameworkComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationIconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // applicationManifestComboBox
            // 
            this.applicationManifestComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationManifestComboBox.Location = new System.Drawing.Point(132, 187);
            this.applicationManifestComboBox.Name = "applicationManifestComboBox";
            this.applicationManifestComboBox.Size = new System.Drawing.Size(320, 21);
            this.applicationManifestComboBox.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 186);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 20);
            this.label10.TabIndex = 14;
            this.label10.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.Manifest}";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // win32ResourceFileBrowseButton
            // 
            this.win32ResourceFileBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.win32ResourceFileBrowseButton.Location = new System.Drawing.Point(422, 211);
            this.win32ResourceFileBrowseButton.Name = "win32ResourceFileBrowseButton";
            this.win32ResourceFileBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.win32ResourceFileBrowseButton.TabIndex = 18;
            this.win32ResourceFileBrowseButton.Text = "...";
            this.win32ResourceFileBrowseButton.UseVisualStyleBackColor = true;
            // 
            // win32ResourceFileTextBox
            // 
            this.win32ResourceFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.win32ResourceFileTextBox.Location = new System.Drawing.Point(132, 213);
            this.win32ResourceFileTextBox.Name = "win32ResourceFileTextBox";
            this.win32ResourceFileTextBox.Size = new System.Drawing.Size(284, 20);
            this.win32ResourceFileTextBox.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(8, 213);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 20);
            this.label9.TabIndex = 16;
            this.label9.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.Win32Resource}";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applicationIconBrowseButton
            // 
            this.applicationIconBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationIconBrowseButton.Location = new System.Drawing.Point(386, 152);
            this.applicationIconBrowseButton.Name = "applicationIconBrowseButton";
            this.applicationIconBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.applicationIconBrowseButton.TabIndex = 13;
            this.applicationIconBrowseButton.Text = "...";
            this.applicationIconBrowseButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.outputNameTextBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.projectFileTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.projectFolderTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(8, 240);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 107);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.ProjectInformation}";
            // 
            // outputNameTextBox
            // 
            this.outputNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputNameTextBox.Location = new System.Drawing.Point(124, 75);
            this.outputNameTextBox.Name = "outputNameTextBox";
            this.outputNameTextBox.ReadOnly = true;
            this.outputNameTextBox.Size = new System.Drawing.Size(312, 20);
            this.outputNameTextBox.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 20);
            this.label8.TabIndex = 4;
            this.label8.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.OutputName}";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // projectFileTextBox
            // 
            this.projectFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectFileTextBox.Location = new System.Drawing.Point(124, 46);
            this.projectFileTextBox.Name = "projectFileTextBox";
            this.projectFileTextBox.Size = new System.Drawing.Size(312, 20);
            this.projectFileTextBox.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 20);
            this.label7.TabIndex = 2;
            this.label7.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.ProjectFile}";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // projectFolderTextBox
            // 
            this.projectFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectFolderTextBox.Location = new System.Drawing.Point(124, 17);
            this.projectFolderTextBox.Name = "projectFolderTextBox";
            this.projectFolderTextBox.ReadOnly = true;
            this.projectFolderTextBox.Size = new System.Drawing.Size(312, 20);
            this.projectFolderTextBox.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.ProjectFolder}";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applicationIconPictureBox
            // 
            this.applicationIconPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationIconPictureBox.Location = new System.Drawing.Point(420, 149);
            this.applicationIconPictureBox.Name = "applicationIconPictureBox";
            this.applicationIconPictureBox.Size = new System.Drawing.Size(32, 35);
            this.applicationIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.applicationIconPictureBox.TabIndex = 36;
            this.applicationIconPictureBox.TabStop = false;
            // 
            // applicationIconTextBox
            // 
            this.applicationIconTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationIconTextBox.Location = new System.Drawing.Point(132, 155);
            this.applicationIconTextBox.Name = "applicationIconTextBox";
            this.applicationIconTextBox.Size = new System.Drawing.Size(248, 20);
            this.applicationIconTextBox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.ApplicationIcon}";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startupObjectComboBox
            // 
            this.startupObjectComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startupObjectComboBox.FormattingEnabled = true;
            this.startupObjectComboBox.Location = new System.Drawing.Point(132, 123);
            this.startupObjectComboBox.Name = "startupObjectComboBox";
            this.startupObjectComboBox.Size = new System.Drawing.Size(320, 21);
            this.startupObjectComboBox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(8, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.StartupObject}";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // outputTypeComboBox
            // 
            this.outputTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputTypeComboBox.FormattingEnabled = true;
            this.outputTypeComboBox.Location = new System.Drawing.Point(132, 95);
            this.outputTypeComboBox.Name = "outputTypeComboBox";
            this.outputTypeComboBox.Size = new System.Drawing.Size(172, 21);
            this.outputTypeComboBox.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.OutputType}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // rootNamespaceTextBox
            // 
            this.rootNamespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rootNamespaceTextBox.Location = new System.Drawing.Point(132, 37);
            this.rootNamespaceTextBox.Name = "rootNamespaceTextBox";
            this.rootNamespaceTextBox.Size = new System.Drawing.Size(320, 20);
            this.rootNamespaceTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.RootNamespace}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // assemblyNameTextBox
            // 
            this.assemblyNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.assemblyNameTextBox.Location = new System.Drawing.Point(132, 9);
            this.assemblyNameTextBox.Name = "assemblyNameTextBox";
            this.assemblyNameTextBox.Size = new System.Drawing.Size(320, 20);
            this.assemblyNameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "${res:Dialog.ProjectOptions.ApplicationSettings.AssemblyName}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // convertProjectToMSBuild35Button
            // 
            this.convertProjectToMSBuild35Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.convertProjectToMSBuild35Button.Location = new System.Drawing.Point(308, 63);
            this.convertProjectToMSBuild35Button.Name = "convertProjectToMSBuild35Button";
            this.convertProjectToMSBuild35Button.Size = new System.Drawing.Size(144, 26);
            this.convertProjectToMSBuild35Button.TabIndex = 6;
            this.convertProjectToMSBuild35Button.Text = "Convert to MSBuild 3.5...";
            this.convertProjectToMSBuild35Button.UseVisualStyleBackColor = true;
            // 
            // targetFrameworkLabel
            // 
            this.targetFrameworkLabel.Location = new System.Drawing.Point(8, 66);
            this.targetFrameworkLabel.Name = "targetFrameworkLabel";
            this.targetFrameworkLabel.Size = new System.Drawing.Size(118, 20);
            this.targetFrameworkLabel.TabIndex = 4;
            this.targetFrameworkLabel.Text = "${res:Dialog.ProjectOptions.Build.TargetFramework}";
            this.targetFrameworkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetFrameworkLabel.UseCompatibleTextRendering = true;
            // 
            // targetFrameworkComboBox
            // 
            this.targetFrameworkComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFrameworkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetFrameworkComboBox.Location = new System.Drawing.Point(132, 66);
            this.targetFrameworkComboBox.Name = "targetFrameworkComboBox";
            this.targetFrameworkComboBox.Size = new System.Drawing.Size(172, 21);
            this.targetFrameworkComboBox.TabIndex = 5;
            // 
            // ApplicationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.convertProjectToMSBuild35Button);
            this.Controls.Add(this.targetFrameworkLabel);
            this.Controls.Add(this.targetFrameworkComboBox);
            this.Controls.Add(this.applicationManifestComboBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.win32ResourceFileBrowseButton);
            this.Controls.Add(this.win32ResourceFileTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.applicationIconBrowseButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.applicationIconPictureBox);
            this.Controls.Add(this.applicationIconTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.startupObjectComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.outputTypeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rootNamespaceTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.assemblyNameTextBox);
            this.Controls.Add(this.label1);
            this.Name = "ApplicationPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationIconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox applicationManifestComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button win32ResourceFileBrowseButton;
        private System.Windows.Forms.TextBox win32ResourceFileTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button applicationIconBrowseButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox outputNameTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox projectFileTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox projectFolderTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox applicationIconPictureBox;
        private System.Windows.Forms.TextBox applicationIconTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox startupObjectComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox outputTypeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox rootNamespaceTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox assemblyNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button convertProjectToMSBuild35Button;
        private System.Windows.Forms.Label targetFrameworkLabel;
        private System.Windows.Forms.ComboBox targetFrameworkComboBox;
    }
}
