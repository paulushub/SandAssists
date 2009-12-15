namespace CSharpBinding.OptionPanels
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
            this.checkForOverflowCheckBox = new System.Windows.Forms.CheckBox();
            this.noCorlibCheckBox = new System.Windows.Forms.CheckBox();
            this.advancedOutputGroupBox = new System.Windows.Forms.GroupBox();
            this.debugInfoLabel = new System.Windows.Forms.Label();
            this.debugInfoComboBox = new System.Windows.Forms.ComboBox();
            this.dllBaseAddressTextBox = new System.Windows.Forms.TextBox();
            this.baseIntermediateOutputPathLabel = new System.Windows.Forms.Label();
            this.baseIntermediateOutputPathTextBox = new System.Windows.Forms.TextBox();
            this.dllBaseAddressLabel = new System.Windows.Forms.Label();
            this.baseIntermediateOutputPathBrowseButton = new System.Windows.Forms.Button();
            this.intermediateOutputPathLabel = new System.Windows.Forms.Label();
            this.fileAlignmentComboBox = new System.Windows.Forms.ComboBox();
            this.intermediateOutputPathTextBox = new System.Windows.Forms.TextBox();
            this.intermediateOutputPathBrowseButton = new System.Windows.Forms.Button();
            this.fileAlignmentLabel = new System.Windows.Forms.Label();
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
            this.generalGroupBox.Controls.Add(this.checkForOverflowCheckBox);
            this.generalGroupBox.Controls.Add(this.noCorlibCheckBox);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 8);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(382, 64);
            this.generalGroupBox.TabIndex = 5;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.General}";
            this.generalGroupBox.UseCompatibleTextRendering = true;
            // 
            // checkForOverflowCheckBox
            // 
            this.checkForOverflowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkForOverflowCheckBox.Location = new System.Drawing.Point(8, 18);
            this.checkForOverflowCheckBox.Name = "checkForOverflowCheckBox";
            this.checkForOverflowCheckBox.Size = new System.Drawing.Size(335, 18);
            this.checkForOverflowCheckBox.TabIndex = 4;
            this.checkForOverflowCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.CheckForOverflow}";
            this.checkForOverflowCheckBox.UseCompatibleTextRendering = true;
            // 
            // noCorlibCheckBox
            // 
            this.noCorlibCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.noCorlibCheckBox.Location = new System.Drawing.Point(8, 38);
            this.noCorlibCheckBox.Name = "noCorlibCheckBox";
            this.noCorlibCheckBox.Size = new System.Drawing.Size(335, 18);
            this.noCorlibCheckBox.TabIndex = 5;
            this.noCorlibCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.NoCorlib}";
            this.noCorlibCheckBox.UseCompatibleTextRendering = true;
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
            this.advancedOutputGroupBox.Controls.Add(this.fileAlignmentComboBox);
            this.advancedOutputGroupBox.Controls.Add(this.intermediateOutputPathTextBox);
            this.advancedOutputGroupBox.Controls.Add(this.intermediateOutputPathBrowseButton);
            this.advancedOutputGroupBox.Controls.Add(this.fileAlignmentLabel);
            this.advancedOutputGroupBox.Location = new System.Drawing.Point(8, 78);
            this.advancedOutputGroupBox.Name = "advancedOutputGroupBox";
            this.advancedOutputGroupBox.Size = new System.Drawing.Size(382, 187);
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
            this.dllBaseAddressTextBox.Location = new System.Drawing.Point(190, 68);
            this.dllBaseAddressTextBox.Name = "dllBaseAddressTextBox";
            this.dllBaseAddressTextBox.Size = new System.Drawing.Size(184, 19);
            this.dllBaseAddressTextBox.TabIndex = 8;
            // 
            // baseIntermediateOutputPathLabel
            // 
            this.baseIntermediateOutputPathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.baseIntermediateOutputPathLabel.Location = new System.Drawing.Point(8, 93);
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
            this.baseIntermediateOutputPathTextBox.Location = new System.Drawing.Point(8, 112);
            this.baseIntermediateOutputPathTextBox.Name = "baseIntermediateOutputPathTextBox";
            this.baseIntermediateOutputPathTextBox.Size = new System.Drawing.Size(332, 19);
            this.baseIntermediateOutputPathTextBox.TabIndex = 1;
            // 
            // dllBaseAddressLabel
            // 
            this.dllBaseAddressLabel.Location = new System.Drawing.Point(8, 68);
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
            this.baseIntermediateOutputPathBrowseButton.Location = new System.Drawing.Point(344, 110);
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
            this.intermediateOutputPathLabel.Location = new System.Drawing.Point(8, 135);
            this.intermediateOutputPathLabel.Name = "intermediateOutputPathLabel";
            this.intermediateOutputPathLabel.Size = new System.Drawing.Size(363, 18);
            this.intermediateOutputPathLabel.TabIndex = 0;
            this.intermediateOutputPathLabel.Text = "${res:Dialog.ProjectOptions.Build.IntermediateOutputPath}";
            this.intermediateOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.intermediateOutputPathLabel.UseCompatibleTextRendering = true;
            // 
            // fileAlignmentComboBox
            // 
            this.fileAlignmentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileAlignmentComboBox.FormattingEnabled = true;
            this.fileAlignmentComboBox.Location = new System.Drawing.Point(190, 42);
            this.fileAlignmentComboBox.Name = "fileAlignmentComboBox";
            this.fileAlignmentComboBox.Size = new System.Drawing.Size(184, 20);
            this.fileAlignmentComboBox.TabIndex = 6;
            // 
            // intermediateOutputPathTextBox
            // 
            this.intermediateOutputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.intermediateOutputPathTextBox.Location = new System.Drawing.Point(8, 156);
            this.intermediateOutputPathTextBox.Name = "intermediateOutputPathTextBox";
            this.intermediateOutputPathTextBox.Size = new System.Drawing.Size(332, 19);
            this.intermediateOutputPathTextBox.TabIndex = 1;
            // 
            // intermediateOutputPathBrowseButton
            // 
            this.intermediateOutputPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.intermediateOutputPathBrowseButton.Location = new System.Drawing.Point(344, 154);
            this.intermediateOutputPathBrowseButton.Name = "intermediateOutputPathBrowseButton";
            this.intermediateOutputPathBrowseButton.Size = new System.Drawing.Size(30, 23);
            this.intermediateOutputPathBrowseButton.TabIndex = 2;
            this.intermediateOutputPathBrowseButton.Text = "...";
            this.intermediateOutputPathBrowseButton.UseCompatibleTextRendering = true;
            // 
            // fileAlignmentLabel
            // 
            this.fileAlignmentLabel.Location = new System.Drawing.Point(8, 42);
            this.fileAlignmentLabel.Name = "fileAlignmentLabel";
            this.fileAlignmentLabel.Size = new System.Drawing.Size(177, 18);
            this.fileAlignmentLabel.TabIndex = 5;
            this.fileAlignmentLabel.Text = "${res:Dialog.ProjectOptions.Build.FileAlignment}";
            this.fileAlignmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileAlignmentLabel.UseCompatibleTextRendering = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(234, 271);
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
            this.cancelButton.Location = new System.Drawing.Point(315, 271);
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
            this.ClientSize = new System.Drawing.Size(398, 302);
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
        private System.Windows.Forms.CheckBox checkForOverflowCheckBox;
        private System.Windows.Forms.CheckBox noCorlibCheckBox;
        private System.Windows.Forms.GroupBox advancedOutputGroupBox;
        private System.Windows.Forms.TextBox dllBaseAddressTextBox;
        private System.Windows.Forms.Label dllBaseAddressLabel;
        private System.Windows.Forms.ComboBox fileAlignmentComboBox;
        private System.Windows.Forms.Label fileAlignmentLabel;
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