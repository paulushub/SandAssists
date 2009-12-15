namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class LoadSavePanel
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.useRecycleBinCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lineTerminatorStyleComboBox = new System.Windows.Forms.ComboBox();
            this.label = new System.Windows.Forms.Label();
            this.createBackupCopyCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.autoLoadExternalChangesCheckBox = new System.Windows.Forms.CheckBox();
            this.detectExternalChangesCheckBox = new System.Windows.Forms.CheckBox();
            this.loadUserDataCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.useRecycleBinCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(8, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 51);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delete";
            // 
            // useRecycleBinCheckBox
            // 
            this.useRecycleBinCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useRecycleBinCheckBox.Location = new System.Drawing.Point(8, 18);
            this.useRecycleBinCheckBox.Name = "useRecycleBinCheckBox";
            this.useRecycleBinCheckBox.Size = new System.Drawing.Size(370, 24);
            this.useRecycleBinCheckBox.TabIndex = 0;
            this.useRecycleBinCheckBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.UseRecycleBin}";
            this.useRecycleBinCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lineTerminatorStyleComboBox);
            this.groupBox2.Controls.Add(this.label);
            this.groupBox2.Controls.Add(this.createBackupCopyCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(8, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 92);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.SaveLabel}";
            // 
            // lineTerminatorStyleComboBox
            // 
            this.lineTerminatorStyleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lineTerminatorStyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineTerminatorStyleComboBox.Location = new System.Drawing.Point(49, 60);
            this.lineTerminatorStyleComboBox.Name = "lineTerminatorStyleComboBox";
            this.lineTerminatorStyleComboBox.Size = new System.Drawing.Size(308, 20);
            this.lineTerminatorStyleComboBox.TabIndex = 2;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(8, 40);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(370, 18);
            this.label.TabIndex = 1;
            this.label.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.LineTerminatorStyleGroupBox}";
            this.label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // createBackupCopyCheckBox
            // 
            this.createBackupCopyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.createBackupCopyCheckBox.Location = new System.Drawing.Point(8, 18);
            this.createBackupCopyCheckBox.Name = "createBackupCopyCheckBox";
            this.createBackupCopyCheckBox.Size = new System.Drawing.Size(370, 18);
            this.createBackupCopyCheckBox.TabIndex = 0;
            this.createBackupCopyCheckBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.CreateBackupCopyCheckBox}";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.autoLoadExternalChangesCheckBox);
            this.groupBox.Controls.Add(this.detectExternalChangesCheckBox);
            this.groupBox.Controls.Add(this.loadUserDataCheckBox);
            this.groupBox.Location = new System.Drawing.Point(8, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(386, 88);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadLabel}";
            // 
            // autoLoadExternalChangesCheckBox
            // 
            this.autoLoadExternalChangesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.autoLoadExternalChangesCheckBox.Location = new System.Drawing.Point(31, 62);
            this.autoLoadExternalChangesCheckBox.Name = "autoLoadExternalChangesCheckBox";
            this.autoLoadExternalChangesCheckBox.Size = new System.Drawing.Size(336, 18);
            this.autoLoadExternalChangesCheckBox.TabIndex = 2;
            this.autoLoadExternalChangesCheckBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.AutoLoadExternalChanges}";
            this.autoLoadExternalChangesCheckBox.UseVisualStyleBackColor = true;
            // 
            // detectExternalChangesCheckBox
            // 
            this.detectExternalChangesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detectExternalChangesCheckBox.Location = new System.Drawing.Point(8, 40);
            this.detectExternalChangesCheckBox.Name = "detectExternalChangesCheckBox";
            this.detectExternalChangesCheckBox.Size = new System.Drawing.Size(359, 18);
            this.detectExternalChangesCheckBox.TabIndex = 1;
            this.detectExternalChangesCheckBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.DetectExternalChanges}";
            this.detectExternalChangesCheckBox.UseVisualStyleBackColor = true;
            // 
            // loadUserDataCheckBox
            // 
            this.loadUserDataCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loadUserDataCheckBox.Location = new System.Drawing.Point(8, 18);
            this.loadUserDataCheckBox.Name = "loadUserDataCheckBox";
            this.loadUserDataCheckBox.Size = new System.Drawing.Size(359, 18);
            this.loadUserDataCheckBox.TabIndex = 0;
            this.loadUserDataCheckBox.Text = "${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadUserDataCheckBox}";
            // 
            // LoadSavePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox);
            this.Name = "LoadSavePanel";
            this.Size = new System.Drawing.Size(404, 380);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox useRecycleBinCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox lineTerminatorStyleComboBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.CheckBox createBackupCopyCheckBox;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.CheckBox autoLoadExternalChangesCheckBox;
        private System.Windows.Forms.CheckBox detectExternalChangesCheckBox;
        private System.Windows.Forms.CheckBox loadUserDataCheckBox;
    }
}
