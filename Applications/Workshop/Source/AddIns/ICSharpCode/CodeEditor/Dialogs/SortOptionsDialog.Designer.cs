namespace ICSharpCode.SharpDevelop.Gui
{
	partial class SortOptionsDialog
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.descendingRadioButton = new System.Windows.Forms.RadioButton();
            this.ascendingRadioButton = new System.Windows.Forms.RadioButton();
            this.removeDupesCheckBox = new System.Windows.Forms.CheckBox();
            this.caseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.ignoreWhiteSpacesCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.descendingRadioButton);
            this.groupBox.Controls.Add(this.ascendingRadioButton);
            this.groupBox.Location = new System.Drawing.Point(8, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(320, 44);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.SortDirectionGroupBo" +
                "x}";
            // 
            // descendingRadioButton
            // 
            this.descendingRadioButton.Location = new System.Drawing.Point(165, 16);
            this.descendingRadioButton.Name = "descendingRadioButton";
            this.descendingRadioButton.Size = new System.Drawing.Size(143, 20);
            this.descendingRadioButton.TabIndex = 1;
            this.descendingRadioButton.TabStop = true;
            this.descendingRadioButton.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.SortDirectionGroupBo" +
                "x.Descending}";
            this.descendingRadioButton.UseVisualStyleBackColor = true;
            // 
            // ascendingRadioButton
            // 
            this.ascendingRadioButton.Location = new System.Drawing.Point(8, 16);
            this.ascendingRadioButton.Name = "ascendingRadioButton";
            this.ascendingRadioButton.Size = new System.Drawing.Size(143, 20);
            this.ascendingRadioButton.TabIndex = 0;
            this.ascendingRadioButton.TabStop = true;
            this.ascendingRadioButton.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.SortDirectionGroupBo" +
                "x.Ascending}";
            this.ascendingRadioButton.UseVisualStyleBackColor = true;
            // 
            // removeDupesCheckBox
            // 
            this.removeDupesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.removeDupesCheckBox.Location = new System.Drawing.Point(8, 59);
            this.removeDupesCheckBox.Name = "removeDupesCheckBox";
            this.removeDupesCheckBox.Size = new System.Drawing.Size(320, 20);
            this.removeDupesCheckBox.TabIndex = 1;
            this.removeDupesCheckBox.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.RemoveDuplicateLines" +
                "CheckBox}";
            this.removeDupesCheckBox.UseVisualStyleBackColor = true;
            // 
            // caseSensitiveCheckBox
            // 
            this.caseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.caseSensitiveCheckBox.Checked = true;
            this.caseSensitiveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caseSensitiveCheckBox.Location = new System.Drawing.Point(8, 82);
            this.caseSensitiveCheckBox.Name = "caseSensitiveCheckBox";
            this.caseSensitiveCheckBox.Size = new System.Drawing.Size(320, 20);
            this.caseSensitiveCheckBox.TabIndex = 2;
            this.caseSensitiveCheckBox.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.CaseSensitiveCheckBo" +
                "x}";
            this.caseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // ignoreWhiteSpacesCheckBox
            // 
            this.ignoreWhiteSpacesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreWhiteSpacesCheckBox.Location = new System.Drawing.Point(8, 106);
            this.ignoreWhiteSpacesCheckBox.Name = "ignoreWhiteSpacesCheckBox";
            this.ignoreWhiteSpacesCheckBox.Size = new System.Drawing.Size(320, 20);
            this.ignoreWhiteSpacesCheckBox.TabIndex = 3;
            this.ignoreWhiteSpacesCheckBox.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.IgnoreTrailingWhites" +
                "pacesCheckBox}";
            this.ignoreWhiteSpacesCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(173, 134);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 25);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "${res:Global.OKButtonText}";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(253, 134);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SortOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 168);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ignoreWhiteSpacesCheckBox);
            this.Controls.Add(this.caseSensitiveCheckBox);
            this.Controls.Add(this.removeDupesCheckBox);
            this.Controls.Add(this.groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SortOptionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.SortOptionsDialog.DialogName}";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RadioButton descendingRadioButton;
        private System.Windows.Forms.RadioButton ascendingRadioButton;
        private System.Windows.Forms.CheckBox removeDupesCheckBox;
        private System.Windows.Forms.CheckBox caseSensitiveCheckBox;
        private System.Windows.Forms.CheckBox ignoreWhiteSpacesCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
	}
}