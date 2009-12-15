namespace VBNetBinding.OptionPanels
{
    partial class ProjectImportsPanel
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
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.namespacesComboBox = new System.Windows.Forms.ComboBox();
            this.addImportButton = new System.Windows.Forms.Button();
            this.removeImportButton = new System.Windows.Forms.Button();
            this.importsListBox = new System.Windows.Forms.ListBox();
            this.generalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.label1);
            this.generalGroupBox.Controls.Add(this.namespacesComboBox);
            this.generalGroupBox.Controls.Add(this.addImportButton);
            this.generalGroupBox.Controls.Add(this.removeImportButton);
            this.generalGroupBox.Controls.Add(this.importsListBox);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 9);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(444, 465);
            this.generalGroupBox.TabIndex = 1;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "${res:Dialog.ProjectOptions.ProjectImports.Title}";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "${res:Dialog.ProjectOptions.ProjectImports.Namespace}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // namespacesComboBox
            // 
            this.namespacesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namespacesComboBox.FormattingEnabled = true;
            this.namespacesComboBox.Location = new System.Drawing.Point(116, 17);
            this.namespacesComboBox.Name = "namespacesComboBox";
            this.namespacesComboBox.Size = new System.Drawing.Size(190, 21);
            this.namespacesComboBox.Sorted = true;
            this.namespacesComboBox.TabIndex = 3;
            // 
            // addImportButton
            // 
            this.addImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addImportButton.Location = new System.Drawing.Point(312, 48);
            this.addImportButton.Name = "addImportButton";
            this.addImportButton.Size = new System.Drawing.Size(124, 25);
            this.addImportButton.TabIndex = 2;
            this.addImportButton.Text = "${res:Dialog.ProjectOptions.ProjectImports.AddImport}";
            this.addImportButton.UseVisualStyleBackColor = true;
            // 
            // removeImportButton
            // 
            this.removeImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeImportButton.Location = new System.Drawing.Point(312, 79);
            this.removeImportButton.Name = "removeImportButton";
            this.removeImportButton.Size = new System.Drawing.Size(124, 25);
            this.removeImportButton.TabIndex = 1;
            this.removeImportButton.Text = "${res:Dialog.ProjectOptions.ProjectImports.RemoveImport}";
            this.removeImportButton.UseVisualStyleBackColor = true;
            // 
            // importsListBox
            // 
            this.importsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importsListBox.FormattingEnabled = true;
            this.importsListBox.Location = new System.Drawing.Point(8, 48);
            this.importsListBox.Name = "importsListBox";
            this.importsListBox.Size = new System.Drawing.Size(298, 394);
            this.importsListBox.TabIndex = 0;
            // 
            // ProjectImportsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generalGroupBox);
            this.Name = "ProjectImportsPanel";
            this.generalGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox namespacesComboBox;
        private System.Windows.Forms.Button addImportButton;
        private System.Windows.Forms.Button removeImportButton;
        private System.Windows.Forms.ListBox importsListBox;
    }
}
