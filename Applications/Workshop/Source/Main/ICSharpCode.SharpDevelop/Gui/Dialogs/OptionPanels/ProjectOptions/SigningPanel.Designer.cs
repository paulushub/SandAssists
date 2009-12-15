namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class SigningPanel
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
            this.clickOnceGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.signingGroupBox = new System.Windows.Forms.GroupBox();
            this.strongNameSignPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.delaySignOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.keyFileComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.changePasswordButton = new System.Windows.Forms.Button();
            this.signAssemblyCheckBox = new System.Windows.Forms.CheckBox();
            this.clickOnceGroupBox.SuspendLayout();
            this.signingGroupBox.SuspendLayout();
            this.strongNameSignPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // clickOnceGroupBox
            // 
            this.clickOnceGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clickOnceGroupBox.Controls.Add(this.label2);
            this.clickOnceGroupBox.Location = new System.Drawing.Point(8, 177);
            this.clickOnceGroupBox.Name = "clickOnceGroupBox";
            this.clickOnceGroupBox.Size = new System.Drawing.Size(444, 44);
            this.clickOnceGroupBox.TabIndex = 4;
            this.clickOnceGroupBox.TabStop = false;
            this.clickOnceGroupBox.Text = "${res:Dialog.ProjectOptions.Signing.ClickOnce}";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(428, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "${res:Dialog.ProjectOptions.Signing.ClickOnceNotSupported}";
            // 
            // signingGroupBox
            // 
            this.signingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.signingGroupBox.Controls.Add(this.strongNameSignPanel);
            this.signingGroupBox.Controls.Add(this.signAssemblyCheckBox);
            this.signingGroupBox.Location = new System.Drawing.Point(8, 9);
            this.signingGroupBox.Name = "signingGroupBox";
            this.signingGroupBox.Size = new System.Drawing.Size(444, 159);
            this.signingGroupBox.TabIndex = 3;
            this.signingGroupBox.TabStop = false;
            this.signingGroupBox.Text = "${res:Dialog.ProjectOptions.Signing.StrongName}";
            // 
            // strongNameSignPanel
            // 
            this.strongNameSignPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.strongNameSignPanel.Controls.Add(this.label1);
            this.strongNameSignPanel.Controls.Add(this.delaySignOnlyCheckBox);
            this.strongNameSignPanel.Controls.Add(this.keyFileComboBox);
            this.strongNameSignPanel.Controls.Add(this.label3);
            this.strongNameSignPanel.Controls.Add(this.changePasswordButton);
            this.strongNameSignPanel.Location = new System.Drawing.Point(7, 39);
            this.strongNameSignPanel.Name = "strongNameSignPanel";
            this.strongNameSignPanel.Size = new System.Drawing.Size(429, 108);
            this.strongNameSignPanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(0, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(430, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "${res:Dialog.ProjectOptions.Signing.DelaySignWarning}";
            // 
            // delaySignOnlyCheckBox
            // 
            this.delaySignOnlyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.delaySignOnlyCheckBox.Location = new System.Drawing.Point(0, 54);
            this.delaySignOnlyCheckBox.Name = "delaySignOnlyCheckBox";
            this.delaySignOnlyCheckBox.Size = new System.Drawing.Size(430, 20);
            this.delaySignOnlyCheckBox.TabIndex = 8;
            this.delaySignOnlyCheckBox.Text = "${res:Dialog.ProjectOptions.Signing.DelaySignOnly}";
            // 
            // keyFileComboBox
            // 
            this.keyFileComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keyFileComboBox.FormattingEnabled = true;
            this.keyFileComboBox.Location = new System.Drawing.Point(0, 25);
            this.keyFileComboBox.Name = "keyFileComboBox";
            this.keyFileComboBox.Size = new System.Drawing.Size(286, 21);
            this.keyFileComboBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(0, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(430, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "${res:Dialog.ProjectOptions.Signing.ChooseKeyFile}";
            // 
            // changePasswordButton
            // 
            this.changePasswordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changePasswordButton.Location = new System.Drawing.Point(292, 24);
            this.changePasswordButton.Name = "changePasswordButton";
            this.changePasswordButton.Size = new System.Drawing.Size(138, 24);
            this.changePasswordButton.TabIndex = 2;
            this.changePasswordButton.Text = "${res:Dialog.ProjectOptions.Signing.ChangePassword}";
            this.changePasswordButton.UseVisualStyleBackColor = true;
            // 
            // signAssemblyCheckBox
            // 
            this.signAssemblyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.signAssemblyCheckBox.Location = new System.Drawing.Point(8, 17);
            this.signAssemblyCheckBox.Name = "signAssemblyCheckBox";
            this.signAssemblyCheckBox.Size = new System.Drawing.Size(428, 20);
            this.signAssemblyCheckBox.TabIndex = 0;
            this.signAssemblyCheckBox.Text = "${res:Dialog.ProjectOptions.Signing.SignAssembly}";
            // 
            // SigningPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clickOnceGroupBox);
            this.Controls.Add(this.signingGroupBox);
            this.Name = "SigningPanel";
            this.clickOnceGroupBox.ResumeLayout(false);
            this.signingGroupBox.ResumeLayout(false);
            this.strongNameSignPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox clickOnceGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox signingGroupBox;
        private System.Windows.Forms.Panel strongNameSignPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox delaySignOnlyCheckBox;
        private System.Windows.Forms.ComboBox keyFileComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button changePasswordButton;
        private System.Windows.Forms.CheckBox signAssemblyCheckBox;
    }
}
