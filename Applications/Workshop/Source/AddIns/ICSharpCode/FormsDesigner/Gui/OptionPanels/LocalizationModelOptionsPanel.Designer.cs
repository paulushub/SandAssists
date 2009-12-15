namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
    partial class LocalizationModelOptionsPanel
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

        #region Windows Forms Designer code

        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox modelGroupBox;
            this.assignmentRadioButton = new System.Windows.Forms.RadioButton();
            this.reflectionRadioButton = new System.Windows.Forms.RadioButton();
            this.keepModelCheckBox = new System.Windows.Forms.CheckBox();
            modelGroupBox = new System.Windows.Forms.GroupBox();
            modelGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // assignmentRadioButton
            // 
            this.assignmentRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.assignmentRadioButton.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.assignmentRadioButton.Location = new System.Drawing.Point(8, 54);
            this.assignmentRadioButton.Name = "assignmentRadioButton";
            this.assignmentRadioButton.Size = new System.Drawing.Size(370, 38);
            this.assignmentRadioButton.TabIndex = 1;
            this.assignmentRadioButton.TabStop = true;
            this.assignmentRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
                "ionsPanel.AssignmentRadioButton}";
            this.assignmentRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.assignmentRadioButton.UseVisualStyleBackColor = true;
            // 
            // reflectionRadioButton
            // 
            this.reflectionRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reflectionRadioButton.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.reflectionRadioButton.Location = new System.Drawing.Point(8, 16);
            this.reflectionRadioButton.Name = "reflectionRadioButton";
            this.reflectionRadioButton.Size = new System.Drawing.Size(370, 38);
            this.reflectionRadioButton.TabIndex = 0;
            this.reflectionRadioButton.TabStop = true;
            this.reflectionRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
                "ionsPanel.ReflectionRadioButton}";
            this.reflectionRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.reflectionRadioButton.UseVisualStyleBackColor = true;
            // 
            // keepModelCheckBox
            // 
            this.keepModelCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keepModelCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.keepModelCheckBox.Location = new System.Drawing.Point(8, 115);
            this.keepModelCheckBox.Name = "keepModelCheckBox";
            this.keepModelCheckBox.Size = new System.Drawing.Size(386, 20);
            this.keepModelCheckBox.TabIndex = 1;
            this.keepModelCheckBox.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
                "ionsPanel.KeepModelCheckBox}";
            this.keepModelCheckBox.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.keepModelCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelGroupBox
            // 
            modelGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            modelGroupBox.Controls.Add(this.assignmentRadioButton);
            modelGroupBox.Controls.Add(this.reflectionRadioButton);
            modelGroupBox.Location = new System.Drawing.Point(8, 8);
            modelGroupBox.Name = "modelGroupBox";
            modelGroupBox.Size = new System.Drawing.Size(386, 100);
            modelGroupBox.TabIndex = 0;
            modelGroupBox.TabStop = false;
            modelGroupBox.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
                "ionsPanel.DefaultLocalizationModel}";
            // 
            // LocalizationModelOptionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.keepModelCheckBox);
            this.Controls.Add(modelGroupBox);
            this.Name = "LocalizationModelOptionsPanel";
            modelGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox keepModelCheckBox;
        private System.Windows.Forms.RadioButton reflectionRadioButton;
        private System.Windows.Forms.RadioButton assignmentRadioButton;
    }
}
