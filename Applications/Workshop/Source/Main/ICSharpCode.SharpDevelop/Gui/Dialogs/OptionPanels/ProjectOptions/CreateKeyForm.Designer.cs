namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class CreateKeyForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblKeyName = new System.Windows.Forms.Label();
            this.keyFileTextBox = new System.Windows.Forms.TextBox();
            this.usePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.passwordPanel = new System.Windows.Forms.Panel();
            this.confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.lblEnterPassword = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.passwordPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblKeyName
            // 
            this.lblKeyName.Location = new System.Drawing.Point(8, 4);
            this.lblKeyName.Name = "lblKeyName";
            this.lblKeyName.Size = new System.Drawing.Size(394, 18);
            this.lblKeyName.TabIndex = 0;
            this.lblKeyName.Text = "${res:Dialog.ProjectOptions.Signing.CreateKey.KeyName}";
            this.lblKeyName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // keyFileTextBox
            // 
            this.keyFileTextBox.Location = new System.Drawing.Point(8, 26);
            this.keyFileTextBox.Name = "keyFileTextBox";
            this.keyFileTextBox.Size = new System.Drawing.Size(394, 19);
            this.keyFileTextBox.TabIndex = 1;
            // 
            // usePasswordCheckBox
            // 
            this.usePasswordCheckBox.Location = new System.Drawing.Point(8, 55);
            this.usePasswordCheckBox.Name = "usePasswordCheckBox";
            this.usePasswordCheckBox.Size = new System.Drawing.Size(394, 18);
            this.usePasswordCheckBox.TabIndex = 2;
            this.usePasswordCheckBox.Text = "${res:Dialog.ProjectOptions.Signing.CreateKey.UsePassword}";
            this.usePasswordCheckBox.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.usePasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // passwordPanel
            // 
            this.passwordPanel.Controls.Add(this.confirmPasswordTextBox);
            this.passwordPanel.Controls.Add(this.lblConfirmPassword);
            this.passwordPanel.Controls.Add(this.passwordTextBox);
            this.passwordPanel.Controls.Add(this.lblEnterPassword);
            this.passwordPanel.Enabled = false;
            this.passwordPanel.Location = new System.Drawing.Point(8, 75);
            this.passwordPanel.Name = "passwordPanel";
            this.passwordPanel.Size = new System.Drawing.Size(400, 92);
            this.passwordPanel.TabIndex = 3;
            // 
            // confirmPasswordTextBox
            // 
            this.confirmPasswordTextBox.Location = new System.Drawing.Point(22, 69);
            this.confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            this.confirmPasswordTextBox.Size = new System.Drawing.Size(372, 19);
            this.confirmPasswordTextBox.TabIndex = 3;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.Location = new System.Drawing.Point(20, 47);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(374, 18);
            this.lblConfirmPassword.TabIndex = 2;
            this.lblConfirmPassword.Text = "${res:Dialog.ProjectOptions.Signing.CreateKey.ConfirmPassword}";
            this.lblConfirmPassword.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(22, 25);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(372, 19);
            this.passwordTextBox.TabIndex = 1;
            // 
            // lblEnterPassword
            // 
            this.lblEnterPassword.Location = new System.Drawing.Point(20, 3);
            this.lblEnterPassword.Name = "lblEnterPassword";
            this.lblEnterPassword.Size = new System.Drawing.Size(363, 18);
            this.lblEnterPassword.TabIndex = 0;
            this.lblEnterPassword.Text = "${res:Dialog.ProjectOptions.Signing.CreateKey.EnterPassword}";
            this.lblEnterPassword.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(246, 173);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "${res:Global.OKButtonText}";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(327, 173);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // CreateKeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 202);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.passwordPanel);
            this.Controls.Add(this.usePasswordCheckBox);
            this.Controls.Add(this.keyFileTextBox);
            this.Controls.Add(this.lblKeyName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateKeyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.ProjectOptions.Signing.CreateKey.Title}";
            this.passwordPanel.ResumeLayout(false);
            this.passwordPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblKeyName;
        private System.Windows.Forms.TextBox keyFileTextBox;
        private System.Windows.Forms.CheckBox usePasswordCheckBox;
        private System.Windows.Forms.Panel passwordPanel;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label lblEnterPassword;
        private System.Windows.Forms.TextBox confirmPasswordTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}