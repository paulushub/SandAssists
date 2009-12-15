namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    partial class FontsColorsPanel
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
            this.CreatedObject7 = new System.Windows.Forms.GroupBox();
            this.fontPreviewLabel = new System.Windows.Forms.Label();
            this.fontSizeLabel = new System.Windows.Forms.Label();
            this.fontSizeComboBox = new System.Windows.Forms.ComboBox();
            this.fontListComboBox = new System.Windows.Forms.ComboBox();
            this.CreatedObject9 = new System.Windows.Forms.Label();
            this.CreatedObject7.SuspendLayout();
            this.SuspendLayout();
            // 
            // CreatedObject7
            // 
            this.CreatedObject7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject7.Controls.Add(this.fontPreviewLabel);
            this.CreatedObject7.Controls.Add(this.fontSizeLabel);
            this.CreatedObject7.Controls.Add(this.fontSizeComboBox);
            this.CreatedObject7.Controls.Add(this.fontListComboBox);
            this.CreatedObject7.Controls.Add(this.CreatedObject9);
            this.CreatedObject7.Location = new System.Drawing.Point(8, 8);
            this.CreatedObject7.Name = "CreatedObject7";
            this.CreatedObject7.Size = new System.Drawing.Size(386, 130);
            this.CreatedObject7.TabIndex = 5;
            this.CreatedObject7.TabStop = false;
            this.CreatedObject7.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox}";
            // 
            // fontPreviewLabel
            // 
            this.fontPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPreviewLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.fontPreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fontPreviewLabel.Location = new System.Drawing.Point(8, 67);
            this.fontPreviewLabel.Name = "fontPreviewLabel";
            this.fontPreviewLabel.Size = new System.Drawing.Size(366, 54);
            this.fontPreviewLabel.TabIndex = 4;
            this.fontPreviewLabel.Text = "AaBbCcXxYyZz";
            this.fontPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fontSizeLabel
            // 
            this.fontSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSizeLabel.Location = new System.Drawing.Point(289, 16);
            this.fontSizeLabel.Name = "fontSizeLabel";
            this.fontSizeLabel.Size = new System.Drawing.Size(81, 20);
            this.fontSizeLabel.TabIndex = 2;
            this.fontSizeLabel.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.FontSizeLabel}";
            this.fontSizeLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // fontSizeComboBox
            // 
            this.fontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSizeComboBox.FormattingEnabled = true;
            this.fontSizeComboBox.Location = new System.Drawing.Point(289, 39);
            this.fontSizeComboBox.Name = "fontSizeComboBox";
            this.fontSizeComboBox.Size = new System.Drawing.Size(85, 21);
            this.fontSizeComboBox.TabIndex = 3;
            // 
            // fontListComboBox
            // 
            this.fontListComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.fontListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontListComboBox.FormattingEnabled = true;
            this.fontListComboBox.Location = new System.Drawing.Point(8, 39);
            this.fontListComboBox.Name = "fontListComboBox";
            this.fontListComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fontListComboBox.Size = new System.Drawing.Size(275, 21);
            this.fontListComboBox.TabIndex = 1;
            // 
            // CreatedObject9
            // 
            this.CreatedObject9.Location = new System.Drawing.Point(8, 16);
            this.CreatedObject9.Name = "CreatedObject9";
            this.CreatedObject9.Size = new System.Drawing.Size(275, 20);
            this.CreatedObject9.TabIndex = 0;
            this.CreatedObject9.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.TextfontLabel}";
            this.CreatedObject9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // FontsColorsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CreatedObject7);
            this.Name = "FontsColorsPanel";
            this.CreatedObject7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CreatedObject7;
        private System.Windows.Forms.Label fontPreviewLabel;
        private System.Windows.Forms.Label fontSizeLabel;
        private System.Windows.Forms.ComboBox fontSizeComboBox;
        private System.Windows.Forms.ComboBox fontListComboBox;
        private System.Windows.Forms.Label CreatedObject9;
    }
}
