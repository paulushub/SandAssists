namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class EditStandardHeaderPanel
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
            this.headerTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.headerChooser = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // headerTextBox
            // 
            this.headerTextBox.AcceptsReturn = true;
            this.headerTextBox.AcceptsTab = true;
            this.headerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerTextBox.Location = new System.Drawing.Point(8, 79);
            this.headerTextBox.Multiline = true;
            this.headerTextBox.Name = "headerTextBox";
            this.headerTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerTextBox.Size = new System.Drawing.Size(386, 289);
            this.headerTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(386, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "${res:Dialog.Options.IDEOptions.EditStandardHeaderPanel.LanguageLabel}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(8, 56);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(386, 18);
            this.label.TabIndex = 7;
            this.label.Text = "${res:Dialog.Options.IDEOptions.EditStandardHeaderPanel.HeaderLabel}";
            this.label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // headerChooser
            // 
            this.headerChooser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.headerChooser.Location = new System.Drawing.Point(8, 31);
            this.headerChooser.Name = "headerChooser";
            this.headerChooser.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerChooser.Size = new System.Drawing.Size(386, 21);
            this.headerChooser.TabIndex = 6;
            // 
            // EditStandardHeaderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.headerTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.headerChooser);
            this.Name = "EditStandardHeaderPanel";
            this.Size = new System.Drawing.Size(404, 380);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox headerTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ComboBox headerChooser;
    }
}
