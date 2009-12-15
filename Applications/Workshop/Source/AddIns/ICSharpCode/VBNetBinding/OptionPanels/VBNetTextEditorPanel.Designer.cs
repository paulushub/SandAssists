namespace VBNetBinding.OptionPanels
{
    partial class VBNetTextEditorPanel
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
            this.CreatedObject2 = new System.Windows.Forms.GroupBox();
            this.enableEndConstructsCheckBox = new System.Windows.Forms.CheckBox();
            this.enableCasingCheckBox = new System.Windows.Forms.CheckBox();
            this.CreatedObject2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CreatedObject2
            // 
            this.CreatedObject2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject2.Controls.Add(this.enableEndConstructsCheckBox);
            this.CreatedObject2.Controls.Add(this.enableCasingCheckBox);
            this.CreatedObject2.Location = new System.Drawing.Point(8, 8);
            this.CreatedObject2.Name = "CreatedObject2";
            this.CreatedObject2.Size = new System.Drawing.Size(386, 396);
            this.CreatedObject2.TabIndex = 1;
            this.CreatedObject2.TabStop = false;
            this.CreatedObject2.Text = "${res:Dialog.Options.IDEOptions.TextEditor.VB.FormattingGroupBox}";
            // 
            // enableEndConstructsCheckBox
            // 
            this.enableEndConstructsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.enableEndConstructsCheckBox.Location = new System.Drawing.Point(8, 16);
            this.enableEndConstructsCheckBox.Name = "enableEndConstructsCheckBox";
            this.enableEndConstructsCheckBox.Size = new System.Drawing.Size(370, 20);
            this.enableEndConstructsCheckBox.TabIndex = 0;
            this.enableEndConstructsCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.VB.EnableEndConstructsCheckBox}";
            // 
            // enableCasingCheckBox
            // 
            this.enableCasingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.enableCasingCheckBox.Location = new System.Drawing.Point(8, 38);
            this.enableCasingCheckBox.Name = "enableCasingCheckBox";
            this.enableCasingCheckBox.Size = new System.Drawing.Size(370, 20);
            this.enableCasingCheckBox.TabIndex = 1;
            this.enableCasingCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.VB.EnableCasingCheckBox}";
            // 
            // VBNetTextEditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CreatedObject2);
            this.Name = "VBNetTextEditorPanel";
            this.Size = new System.Drawing.Size(404, 412);
            this.CreatedObject2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CreatedObject2;
        private System.Windows.Forms.CheckBox enableEndConstructsCheckBox;
        private System.Windows.Forms.CheckBox enableCasingCheckBox;
    }
}
