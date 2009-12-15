namespace ICSharpCode.SharpDevelop.Gui
{
    partial class FontSelectionPanel
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
            this.fontListLabel = new System.Windows.Forms.Label();
            this.fontListComboBox = new System.Windows.Forms.ComboBox();
            this.fontSizeLabel = new System.Windows.Forms.Label();
            this.fontSizeComboBox = new System.Windows.Forms.ComboBox();
            this.fontPreviewLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fontListLabel
            // 
            this.fontListLabel.Location = new System.Drawing.Point(8, 3);
            this.fontListLabel.Name = "fontListLabel";
            this.fontListLabel.Size = new System.Drawing.Size(246, 18);
            this.fontListLabel.TabIndex = 0;
            this.fontListLabel.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.TextfontLabel}";
            this.fontListLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // fontListComboBox
            // 
            this.fontListComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.fontListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontListComboBox.FormattingEnabled = true;
            this.fontListComboBox.Location = new System.Drawing.Point(8, 24);
            this.fontListComboBox.Name = "fontListComboBox";
            this.fontListComboBox.Size = new System.Drawing.Size(246, 21);
            this.fontListComboBox.TabIndex = 1;
            // 
            // fontSizeLabel
            // 
            this.fontSizeLabel.Location = new System.Drawing.Point(260, 3);
            this.fontSizeLabel.Name = "fontSizeLabel";
            this.fontSizeLabel.Size = new System.Drawing.Size(83, 18);
            this.fontSizeLabel.TabIndex = 2;
            this.fontSizeLabel.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.FontSizeLabel}";
            // 
            // fontSizeComboBox
            // 
            this.fontSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSizeComboBox.FormattingEnabled = true;
            this.fontSizeComboBox.Location = new System.Drawing.Point(260, 24);
            this.fontSizeComboBox.Name = "fontSizeComboBox";
            this.fontSizeComboBox.Size = new System.Drawing.Size(83, 21);
            this.fontSizeComboBox.TabIndex = 3;
            // 
            // fontPreviewLabel
            // 
            this.fontPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontPreviewLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.fontPreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fontPreviewLabel.Location = new System.Drawing.Point(8, 51);
            this.fontPreviewLabel.Name = "fontPreviewLabel";
            this.fontPreviewLabel.Size = new System.Drawing.Size(335, 50);
            this.fontPreviewLabel.TabIndex = 4;
            this.fontPreviewLabel.Text = "AaBbCcXxYyZz";
            this.fontPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FontSelectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fontPreviewLabel);
            this.Controls.Add(this.fontSizeComboBox);
            this.Controls.Add(this.fontSizeLabel);
            this.Controls.Add(this.fontListComboBox);
            this.Controls.Add(this.fontListLabel);
            this.Name = "FontSelectionPanel";
            this.Size = new System.Drawing.Size(352, 104);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label fontListLabel;
        private System.Windows.Forms.ComboBox fontListComboBox;
        private System.Windows.Forms.Label fontSizeLabel;
        private System.Windows.Forms.ComboBox fontSizeComboBox;
        private System.Windows.Forms.Label fontPreviewLabel;
    }
}
