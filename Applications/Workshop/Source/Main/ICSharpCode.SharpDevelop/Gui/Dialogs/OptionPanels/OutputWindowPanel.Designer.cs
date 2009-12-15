namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class OutputWindowPanel
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
            this.FontGroupBox = new System.Windows.Forms.GroupBox();
            this.formatGroupBox = new System.Windows.Forms.GroupBox();
            this.wordWrapCheckBox = new System.Windows.Forms.CheckBox();
            this.fontSelectionPanel = new ICSharpCode.SharpDevelop.Gui.FontSelectionPanel();
            this.FontGroupBox.SuspendLayout();
            this.formatGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FontGroupBox
            // 
            this.FontGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FontGroupBox.Controls.Add(this.fontSelectionPanel);
            this.FontGroupBox.Location = new System.Drawing.Point(8, 59);
            this.FontGroupBox.Name = "FontGroupBox";
            this.FontGroupBox.Size = new System.Drawing.Size(386, 126);
            this.FontGroupBox.TabIndex = 4;
            this.FontGroupBox.TabStop = false;
            this.FontGroupBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox}";
            // 
            // formatGroupBox
            // 
            this.formatGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.formatGroupBox.Controls.Add(this.wordWrapCheckBox);
            this.formatGroupBox.Location = new System.Drawing.Point(8, 8);
            this.formatGroupBox.Name = "formatGroupBox";
            this.formatGroupBox.Size = new System.Drawing.Size(386, 44);
            this.formatGroupBox.TabIndex = 3;
            this.formatGroupBox.TabStop = false;
            this.formatGroupBox.Text = "${res:Dialog.Options.IDEOptions.OutputPanel.Format}";
            // 
            // wordWrapCheckBox
            // 
            this.wordWrapCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wordWrapCheckBox.Checked = true;
            this.wordWrapCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wordWrapCheckBox.Location = new System.Drawing.Point(76, 19);
            this.wordWrapCheckBox.Name = "wordWrapCheckBox";
            this.wordWrapCheckBox.Size = new System.Drawing.Size(261, 16);
            this.wordWrapCheckBox.TabIndex = 0;
            this.wordWrapCheckBox.Text = "${res:Dialog.Options.IDEOptions.OutputPanel.WordWrap}";
            // 
            // fontSelectionPanel
            // 
            this.fontSelectionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSelectionPanel.Location = new System.Drawing.Point(16, 14);
            this.fontSelectionPanel.Name = "fontSelectionPanel";
            this.fontSelectionPanel.Size = new System.Drawing.Size(352, 101);
            this.fontSelectionPanel.TabIndex = 0;
            // 
            // OutputWindowPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FontGroupBox);
            this.Controls.Add(this.formatGroupBox);
            this.Name = "OutputWindowPanel";
            this.Size = new System.Drawing.Size(404, 380);
            this.FontGroupBox.ResumeLayout(false);
            this.formatGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox FontGroupBox;
        private System.Windows.Forms.GroupBox formatGroupBox;
        private System.Windows.Forms.CheckBox wordWrapCheckBox;
        private FontSelectionPanel fontSelectionPanel;
    }
}
