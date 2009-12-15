namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class SelectVisualStylePanel
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
            this.useProfessionalStyleCheckBox = new System.Windows.Forms.CheckBox();
            this.showToolBarCheckBox = new System.Windows.Forms.CheckBox();
            this.showStatusBarCheckBox = new System.Windows.Forms.CheckBox();
            this.groupAppearance = new System.Windows.Forms.GroupBox();
            this.preferProjectAmbienceCheckBox = new System.Windows.Forms.CheckBox();
            this.showExtensionsCheckBox = new System.Windows.Forms.CheckBox();
            this.selectAmbienceComboBox = new System.Windows.Forms.ComboBox();
            this.label = new System.Windows.Forms.Label();
            this.groupAppearance.SuspendLayout();
            this.SuspendLayout();
            // 
            // useProfessionalStyleCheckBox
            // 
            this.useProfessionalStyleCheckBox.Location = new System.Drawing.Point(20, 188);
            this.useProfessionalStyleCheckBox.Name = "useProfessionalStyleCheckBox";
            this.useProfessionalStyleCheckBox.Size = new System.Drawing.Size(355, 18);
            this.useProfessionalStyleCheckBox.TabIndex = 8;
            this.useProfessionalStyleCheckBox.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.UseProfessionalStyleCheckBox}";
            this.useProfessionalStyleCheckBox.Visible = false;
            // 
            // showToolBarCheckBox
            // 
            this.showToolBarCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showToolBarCheckBox.Location = new System.Drawing.Point(20, 165);
            this.showToolBarCheckBox.Name = "showToolBarCheckBox";
            this.showToolBarCheckBox.Size = new System.Drawing.Size(355, 18);
            this.showToolBarCheckBox.TabIndex = 7;
            this.showToolBarCheckBox.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.ShowToolBarCheckBox}";
            this.showToolBarCheckBox.Visible = false;
            // 
            // showStatusBarCheckBox
            // 
            this.showStatusBarCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showStatusBarCheckBox.Location = new System.Drawing.Point(20, 141);
            this.showStatusBarCheckBox.Name = "showStatusBarCheckBox";
            this.showStatusBarCheckBox.Size = new System.Drawing.Size(355, 18);
            this.showStatusBarCheckBox.TabIndex = 6;
            this.showStatusBarCheckBox.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.ShowStatusBarCheckBox}";
            this.showStatusBarCheckBox.Visible = false;
            // 
            // groupAppearance
            // 
            this.groupAppearance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAppearance.Controls.Add(this.preferProjectAmbienceCheckBox);
            this.groupAppearance.Controls.Add(this.showExtensionsCheckBox);
            this.groupAppearance.Controls.Add(this.selectAmbienceComboBox);
            this.groupAppearance.Controls.Add(this.label);
            this.groupAppearance.Location = new System.Drawing.Point(8, 8);
            this.groupAppearance.Name = "groupAppearance";
            this.groupAppearance.Size = new System.Drawing.Size(386, 116);
            this.groupAppearance.TabIndex = 5;
            this.groupAppearance.TabStop = false;
            this.groupAppearance.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.VisualStyleGroupBox}";
            // 
            // preferProjectAmbienceCheckBox
            // 
            this.preferProjectAmbienceCheckBox.Location = new System.Drawing.Point(12, 87);
            this.preferProjectAmbienceCheckBox.Name = "preferProjectAmbienceCheckBox";
            this.preferProjectAmbienceCheckBox.Size = new System.Drawing.Size(355, 18);
            this.preferProjectAmbienceCheckBox.TabIndex = 4;
            this.preferProjectAmbienceCheckBox.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.UseProjectAmbience}";
            // 
            // showExtensionsCheckBox
            // 
            this.showExtensionsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showExtensionsCheckBox.Location = new System.Drawing.Point(12, 18);
            this.showExtensionsCheckBox.Name = "showExtensionsCheckBox";
            this.showExtensionsCheckBox.Size = new System.Drawing.Size(355, 18);
            this.showExtensionsCheckBox.TabIndex = 1;
            this.showExtensionsCheckBox.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.ShowExtensionsCheckBox}";
            // 
            // selectAmbienceComboBox
            // 
            this.selectAmbienceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectAmbienceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectAmbienceComboBox.FormattingEnabled = true;
            this.selectAmbienceComboBox.Location = new System.Drawing.Point(44, 62);
            this.selectAmbienceComboBox.Name = "selectAmbienceComboBox";
            this.selectAmbienceComboBox.Size = new System.Drawing.Size(323, 20);
            this.selectAmbienceComboBox.TabIndex = 3;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(12, 40);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(355, 18);
            this.label.TabIndex = 2;
            this.label.Text = "${res:Dialog.Options.IDEOptions.SelectStyle.SelectAmbienceLabel}";
            this.label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // AppearancePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.useProfessionalStyleCheckBox);
            this.Controls.Add(this.showToolBarCheckBox);
            this.Controls.Add(this.showStatusBarCheckBox);
            this.Controls.Add(this.groupAppearance);
            this.Name = "AppearancePanel";
            this.Size = new System.Drawing.Size(404, 380);
            this.groupAppearance.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox useProfessionalStyleCheckBox;
        private System.Windows.Forms.CheckBox showToolBarCheckBox;
        private System.Windows.Forms.CheckBox showStatusBarCheckBox;
        private System.Windows.Forms.GroupBox groupAppearance;
        private System.Windows.Forms.CheckBox preferProjectAmbienceCheckBox;
        private System.Windows.Forms.CheckBox showExtensionsCheckBox;
        private System.Windows.Forms.ComboBox selectAmbienceComboBox;
        private System.Windows.Forms.Label label;
    }
}
