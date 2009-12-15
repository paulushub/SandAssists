namespace VBNetBinding.OptionPanels
{
    partial class AdvancedBuildSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.removeOverflowCheckBox = new System.Windows.Forms.CheckBox();
            this.advancedOutputGroupBox = new System.Windows.Forms.GroupBox();
            this.debugInfoLabel = new System.Windows.Forms.Label();
            this.debugInfoComboBox = new System.Windows.Forms.ComboBox();
            this.dllBaseAddressTextBox = new System.Windows.Forms.TextBox();
            this.baseIntermediateOutputPathLabel = new System.Windows.Forms.Label();
            this.baseIntermediateOutputPathTextBox = new System.Windows.Forms.TextBox();
            this.dllBaseAddressLabel = new System.Windows.Forms.Label();
            this.baseIntermediateOutputPathBrowseButton = new System.Windows.Forms.Button();
            this.intermediateOutputPathLabel = new System.Windows.Forms.Label();
            this.intermediateOutputPathTextBox = new System.Windows.Forms.TextBox();
            this.intermediateOutputPathBrowseButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.generalGroupBox.SuspendLayout();
            this.advancedOutputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.removeOverflowCheckBox);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 8);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(382, 47);
            this.generalGroupBox.TabIndex = 5;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.General}";
            this.generalGroupBox.UseCompatibleTextRendering = true;
            // 
            // removeOverflowCheckBox
            // 
            this.removeOverflowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.removeOverflowCheckBox.Location = new System.Drawing.Point(8, 18);
            this.removeOverflowCheckBox.Name = "removeOverflowCheckBox";
            this.removeOverflowCheckBox.Size = new System.Drawing.Size(366, 18);
            this.removeOverflowCheckBox.TabIndex = 4;
            this.removeOverflowCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.RemoveOverflowChecks}";
            this.removeOverflowCheckBox.UseCompatibleTextRendering = true;
            // 
            // advancedOutputGroupBox
            // 
            this.advancedOutputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedOutputGroupBox.Controls.Add(this.debugInfoLabel);
            this.advancedOutputGroupBox.Controls.Add(this.debugInfoComboBox);
            this.advancedOutputGroupBox.Controls.Add(this.dllBaseAddressTextBox);
            this.advancedOutputGroupBox.Controls.Add(this.baseIntermediateOutputPathLabel);
            this.advancedOutputGroupBox.Controls.Add(this.baseIntermediateOutputPathTextBox);
            this.advancedOutputGroupBox.Controls.Add(this.dllBaseAddressLabel);
            this.advancedOutputGroupBox.Controls.Add(this.baseIntermediateOutputPathBrowseButton);
            this.advancedOutputGroupBox.Controls.Add(this.intermediateOutputPathLabel);
            this.advancedOutputGroupBox.Controls.Add(this.intermediateOutputPathTextBox);
            this.advancedOutputGroupBox.Controls.Add(this.intermediateOutputPathBrowseButton);
            this.advancedOutputGroupBox.Location = new System.Drawing.Point(8, 61);
            this.advancedOutputGroupBox.Name = "advancedOutputGroupBox";
            this.advancedOutputGroupBox.Size = new System.Drawing.Size(382, 162);
            this.advancedOutputGroupBox.TabIndex = 10;
            this.advancedOutputGroupBox.TabStop = false;
            this.advancedOutputGroupBox.Text = "${res:Dialog.ProjectOptions.Build.Output}";
            this.advancedOutputGroupBox.UseCompatibleTextRendering = true;
            // 
            // debugInfoLabel
            // 
            this.debugInfoLabel.Location = new System.Drawing.Point(7, 16);
            this.debugInfoLabel.Name = "debugInfoLabel";
            this.debugInfoLabel.Size = new System.Drawing.Size(177, 16);
            this.debugInfoLabel.TabIndex = 10;
            this.debugInfoLabel.Text = "${res:Dialog.ProjectOptions.Build.DebugInfo}";
            this.debugInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.debugInfoLabel.UseCompatibleTextRendering = true;
            // 
            // debugInfoComboBox
            // 
            this.debugInfoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.debugInfoComboBox.Location = new System.Drawing.Point(190, 16);
            this.debugInfoComboBox.Name = "debugInfoComboBox";
            this.debugInfoComboBox.Size = new System.Drawing.Size(184, 20);
            this.debugInfoComboBox.TabIndex = 11;
            // 
            // dllBaseAddressTextBox
            // 
            this.dllBaseAddressTextBox.Location = new System.Drawing.Point(190, 42);
            this.dllBaseAddressTextBox.Name = "dllBaseAddressTextBox";
            this.dllBaseAddressTextBox.Size = new System.Drawing.Size(184, 19);
            this.dllBaseAddressTextBox.TabIndex = 8;
            // 
            // baseIntermediateOutputPathLabel
            // 
            this.baseIntermediateOutputPathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.baseIntermediateOutputPathLabel.Location = new System.Drawing.Point(8, 68);
            this.baseIntermediateOutputPathLabel.Name = "baseIntermediateOutputPathLabel";
            this.baseIntermediateOutputPathLabel.Size = new System.Drawing.Size(335, 18);
            this.baseIntermediateOutputPathLabel.TabIndex = 0;
            this.baseIntermediateOutputPathLabel.Text = "${res:Dialog.ProjectOptions.Build.BaseIntermediateOutputPath}";
            this.baseIntermediateOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.baseIntermediateOutputPathLabel.UseCompatibleTextRendering = true;
            // 
            // baseIntermediateOutputPathTextBox
            // 
            this.baseIntermediateOutputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.baseIntermediateOutputPathTextBox.Location = new System.Drawing.Point(8, 89);
            this.baseIntermediateOutputPathTextBox.Name = "baseIntermediateOutputPathTextBox";
            this.baseIntermediateOutputPathTextBox.Size = new System.Drawing.Size(332, 19);
            this.baseIntermediateOutputPathTextBox.TabIndex = 1;
            // 
            // dllBaseAddressLabel
            // 
            this.dllBaseAddressLabel.Location = new System.Drawing.Point(8, 42);
            this.dllBaseAddressLabel.Name = "dllBaseAddressLabel";
            this.dllBaseAddressLabel.Size = new System.Drawing.Size(177, 18);
            this.dllBaseAddressLabel.TabIndex = 7;
            this.dllBaseAddressLabel.Text = "${res:Dialog.ProjectOptions.Build.DLLBaseAddress}";
            this.dllBaseAddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.dllBaseAddressLabel.UseCompatibleTextRendering = true;
            // 
            // baseIntermediateOutputPathBrowseButton
            // 
            this.baseIntermediateOutputPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.baseIntermediateOutputPathBrowseButton.Location = new System.Drawing.Point(344, 87);
            this.baseIntermediateOutputPathBrowseButton.Name = "baseIntermediateOutputPathBrowseButton";
            this.baseIntermediateOutputPathBrowseButton.Size = new System.Drawing.Size(30, 23);
            this.baseIntermediateOutputPathBrowseButton.TabIndex = 2;
            this.baseIntermediateOutputPathBrowseButton.Text = "...";
            this.baseIntermediateOutputPathBrowseButton.UseCompatibleTextRendering = true;
            // 
            // intermediateOutputPathLabel
            // 
            this.intermediateOutputPathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.intermediateOutputPathLabel.Location = new System.Drawing.Point(8, 112);
            this.intermediateOutputPathLabel.Name = "intermediateOutputPathLabel";
            this.intermediateOutputPathLabel.Size = new System.Drawing.Size(363, 18);
            this.intermediateOutputPathLabel.TabIndex = 0;
            this.intermediateOutputPathLabel.Text = "${res:Dialog.ProjectOptions.Build.IntermediateOutputPath}";
            this.intermediateOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.intermediateOutputPathLabel.UseCompatibleTextRendering = true;
            // 
            // intermediateOutputPathTextBox
            // 
            this.intermediateOutputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.intermediateOutputPathTextBox.Location = new System.Drawing.Point(8, 133);
            this.intermediateOutputPathTextBox.Name = "intermediateOutputPathTextBox";
            this.intermediateOutputPathTextBox.Size = new System.Drawing.Size(332, 19);
            this.intermediateOutputPathTextBox.TabIndex = 1;
            // 
            // intermediateOutputPathBrowseButton
            // 
            this.intermediateOutputPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.intermediateOutputPathBrowseButton.Location = new System.Drawing.Point(344, 131);
            this.intermediateOutputPathBrowseButton.Name = "intermediateOutputPathBrowseButton";
            this.intermediateOutputPathBrowseButton.Size = new System.Drawing.Size(30, 23);
            this.intermediateOutputPathBrowseButton.TabIndex = 2;
            this.intermediateOutputPathBrowseButton.Text = "...";
            this.intermediateOutputPathBrowseButton.UseCompatibleTextRendering = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(234, 230);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "${res:Global.OKButtonText}";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OnAcceptSettings);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(315, 230);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.OnRejectSettings);
            // 
            // AdvancedBuildSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 261);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.advancedOutputGroupBox);
            this.Controls.Add(this.generalGroupBox);
            this.Name = "AdvancedBuildSettings";
            this.Text = "${res:Dialog.ProjectOptions.Build.Advanced}";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.generalGroupBox.ResumeLayout(false);
            this.advancedOutputGroupBox.ResumeLayout(false);
            this.advancedOutputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox removeOverflowCheckBox;
        private System.Windows.Forms.GroupBox advancedOutputGroupBox;
        private System.Windows.Forms.TextBox dllBaseAddressTextBox;
        private System.Windows.Forms.Label dllBaseAddressLabel;
        private System.Windows.Forms.Label baseIntermediateOutputPathLabel;
        private System.Windows.Forms.TextBox baseIntermediateOutputPathTextBox;
        private System.Windows.Forms.Button baseIntermediateOutputPathBrowseButton;
        private System.Windows.Forms.Label intermediateOutputPathLabel;
        private System.Windows.Forms.TextBox intermediateOutputPathTextBox;
        private System.Windows.Forms.Button intermediateOutputPathBrowseButton;
        private System.Windows.Forms.Label debugInfoLabel;
        private System.Windows.Forms.ComboBox debugInfoComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}