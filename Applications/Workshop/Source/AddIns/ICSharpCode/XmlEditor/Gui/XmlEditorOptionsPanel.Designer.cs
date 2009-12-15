namespace ICSharpCode.XmlEditor
{
    partial class XmlEditorOptionsPanel
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
            this.xmlFileExtensionGroupBox = new System.Windows.Forms.GroupBox();
            this.namespacePrefixTextBox = new System.Windows.Forms.TextBox();
            this.prefixLabel = new System.Windows.Forms.Label();
            this.schemaTextBox = new System.Windows.Forms.TextBox();
            this.fileExtensionComboBox = new System.Windows.Forms.ComboBox();
            this.schemaLabel = new System.Windows.Forms.Label();
            this.fileExtensionLabel = new System.Windows.Forms.Label();
            this.changeSchemaButton = new System.Windows.Forms.Button();
            this.schemasGroupBox = new System.Windows.Forms.GroupBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.schemaListBox = new System.Windows.Forms.ListBox();
            this.foldingGroupBox = new System.Windows.Forms.GroupBox();
            this.showAttributesWhenFoldedCheckBox = new System.Windows.Forms.CheckBox();
            this.xmlCompletionGroupBox = new System.Windows.Forms.GroupBox();
            this.showSchemaAnnotationCheckBox = new System.Windows.Forms.CheckBox();
            this.xmlFileExtensionGroupBox.SuspendLayout();
            this.schemasGroupBox.SuspendLayout();
            this.foldingGroupBox.SuspendLayout();
            this.xmlCompletionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // xmlFileExtensionGroupBox
            // 
            this.xmlFileExtensionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlFileExtensionGroupBox.Controls.Add(this.namespacePrefixTextBox);
            this.xmlFileExtensionGroupBox.Controls.Add(this.prefixLabel);
            this.xmlFileExtensionGroupBox.Controls.Add(this.schemaTextBox);
            this.xmlFileExtensionGroupBox.Controls.Add(this.fileExtensionComboBox);
            this.xmlFileExtensionGroupBox.Controls.Add(this.schemaLabel);
            this.xmlFileExtensionGroupBox.Controls.Add(this.fileExtensionLabel);
            this.xmlFileExtensionGroupBox.Controls.Add(this.changeSchemaButton);
            this.xmlFileExtensionGroupBox.Location = new System.Drawing.Point(8, 307);
            this.xmlFileExtensionGroupBox.Name = "xmlFileExtensionGroupBox";
            this.xmlFileExtensionGroupBox.Size = new System.Drawing.Size(386, 98);
            this.xmlFileExtensionGroupBox.TabIndex = 6;
            this.xmlFileExtensionGroupBox.TabStop = false;
            this.xmlFileExtensionGroupBox.Text = "${res:ICSharpCode.XmlEditor.XmlSchemaPanel.FileExtensionsGroupBoxText}";
            // 
            // namespacePrefixTextBox
            // 
            this.namespacePrefixTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namespacePrefixTextBox.Location = new System.Drawing.Point(80, 69);
            this.namespacePrefixTextBox.Name = "namespacePrefixTextBox";
            this.namespacePrefixTextBox.Size = new System.Drawing.Size(298, 20);
            this.namespacePrefixTextBox.TabIndex = 10;
            // 
            // prefixLabel
            // 
            this.prefixLabel.Location = new System.Drawing.Point(8, 69);
            this.prefixLabel.Name = "prefixLabel";
            this.prefixLabel.Size = new System.Drawing.Size(72, 20);
            this.prefixLabel.TabIndex = 9;
            this.prefixLabel.Text = "${res:ICSharpCode.XmlEditor.XmlSchemaPanel.NamespacePrefixLabelText}";
            this.prefixLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // schemaTextBox
            // 
            this.schemaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.schemaTextBox.Location = new System.Drawing.Point(80, 43);
            this.schemaTextBox.Name = "schemaTextBox";
            this.schemaTextBox.ReadOnly = true;
            this.schemaTextBox.Size = new System.Drawing.Size(266, 20);
            this.schemaTextBox.TabIndex = 8;
            // 
            // fileExtensionComboBox
            // 
            this.fileExtensionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileExtensionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileExtensionComboBox.Location = new System.Drawing.Point(80, 16);
            this.fileExtensionComboBox.Name = "fileExtensionComboBox";
            this.fileExtensionComboBox.Size = new System.Drawing.Size(298, 21);
            this.fileExtensionComboBox.TabIndex = 7;
            // 
            // schemaLabel
            // 
            this.schemaLabel.Location = new System.Drawing.Point(8, 43);
            this.schemaLabel.Name = "schemaLabel";
            this.schemaLabel.Size = new System.Drawing.Size(72, 20);
            this.schemaLabel.TabIndex = 1;
            this.schemaLabel.Text = "${res:ICSharpCode.XmlEditor.XmlSchemaPanel.SchemaLabelText}";
            this.schemaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileExtensionLabel
            // 
            this.fileExtensionLabel.Location = new System.Drawing.Point(8, 16);
            this.fileExtensionLabel.Name = "fileExtensionLabel";
            this.fileExtensionLabel.Size = new System.Drawing.Size(72, 20);
            this.fileExtensionLabel.TabIndex = 0;
            this.fileExtensionLabel.Text = "${res:ICSharpCode.XmlEditor.XmlSchemaPanel.FileExtensionLabelText}";
            this.fileExtensionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // changeSchemaButton
            // 
            this.changeSchemaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changeSchemaButton.Location = new System.Drawing.Point(354, 43);
            this.changeSchemaButton.Name = "changeSchemaButton";
            this.changeSchemaButton.Size = new System.Drawing.Size(24, 21);
            this.changeSchemaButton.TabIndex = 6;
            this.changeSchemaButton.Text = "...";
            // 
            // schemasGroupBox
            // 
            this.schemasGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.schemasGroupBox.Controls.Add(this.addButton);
            this.schemasGroupBox.Controls.Add(this.removeButton);
            this.schemasGroupBox.Controls.Add(this.schemaListBox);
            this.schemasGroupBox.Location = new System.Drawing.Point(8, 100);
            this.schemasGroupBox.Name = "schemasGroupBox";
            this.schemasGroupBox.Size = new System.Drawing.Size(386, 200);
            this.schemasGroupBox.TabIndex = 5;
            this.schemasGroupBox.TabStop = false;
            this.schemasGroupBox.Text = "${res:ICSharpCode.XmlEditor.XmlSchemaPanel.SchemasGroupBoxText}";
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(210, 169);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(80, 24);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "${res:Global.AddButtonText}...";
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(298, 169);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(80, 24);
            this.removeButton.TabIndex = 5;
            this.removeButton.Text = "${res:Global.RemoveButtonText}";
            // 
            // schemaListBox
            // 
            this.schemaListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.schemaListBox.Location = new System.Drawing.Point(8, 16);
            this.schemaListBox.Name = "schemaListBox";
            this.schemaListBox.Size = new System.Drawing.Size(370, 147);
            this.schemaListBox.TabIndex = 3;
            // 
            // foldingGroupBox
            // 
            this.foldingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.foldingGroupBox.Controls.Add(this.showAttributesWhenFoldedCheckBox);
            this.foldingGroupBox.Location = new System.Drawing.Point(8, 8);
            this.foldingGroupBox.Name = "foldingGroupBox";
            this.foldingGroupBox.Size = new System.Drawing.Size(386, 40);
            this.foldingGroupBox.TabIndex = 7;
            this.foldingGroupBox.TabStop = false;
            this.foldingGroupBox.Text = "${res:ICSharpCode.XmlEditor.XmlEditorOptionsPanel.FoldingGroupLabel}";
            // 
            // showAttributesWhenFoldedCheckBox
            // 
            this.showAttributesWhenFoldedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showAttributesWhenFoldedCheckBox.Location = new System.Drawing.Point(8, 16);
            this.showAttributesWhenFoldedCheckBox.Name = "showAttributesWhenFoldedCheckBox";
            this.showAttributesWhenFoldedCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showAttributesWhenFoldedCheckBox.TabIndex = 0;
            this.showAttributesWhenFoldedCheckBox.Text = "${res:ICSharpCode.XmlEditor.XmlEditorOptionsPanel.ShowAttributesWhenFoldedLabel}";
            // 
            // xmlCompletionGroupBox
            // 
            this.xmlCompletionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlCompletionGroupBox.Controls.Add(this.showSchemaAnnotationCheckBox);
            this.xmlCompletionGroupBox.Location = new System.Drawing.Point(8, 54);
            this.xmlCompletionGroupBox.Name = "xmlCompletionGroupBox";
            this.xmlCompletionGroupBox.Size = new System.Drawing.Size(386, 40);
            this.xmlCompletionGroupBox.TabIndex = 8;
            this.xmlCompletionGroupBox.TabStop = false;
            this.xmlCompletionGroupBox.Text = "${res:ICSharpCode.XmlEditor.XmlEditorOptionsPanel.XmlCompletionGroupLabel}";
            // 
            // showSchemaAnnotationCheckBox
            // 
            this.showSchemaAnnotationCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showSchemaAnnotationCheckBox.Location = new System.Drawing.Point(8, 16);
            this.showSchemaAnnotationCheckBox.Name = "showSchemaAnnotationCheckBox";
            this.showSchemaAnnotationCheckBox.Size = new System.Drawing.Size(370, 16);
            this.showSchemaAnnotationCheckBox.TabIndex = 0;
            this.showSchemaAnnotationCheckBox.Text = "${res:ICSharpCode.XmlEditor.XmlEditorOptionsPanel.ShowSchemaAnnotationLabel}";
            // 
            // XmlEditorOptionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.foldingGroupBox);
            this.Controls.Add(this.xmlCompletionGroupBox);
            this.Controls.Add(this.xmlFileExtensionGroupBox);
            this.Controls.Add(this.schemasGroupBox);
            this.Name = "XmlEditorOptionsPanel";
            this.xmlFileExtensionGroupBox.ResumeLayout(false);
            this.xmlFileExtensionGroupBox.PerformLayout();
            this.schemasGroupBox.ResumeLayout(false);
            this.foldingGroupBox.ResumeLayout(false);
            this.xmlCompletionGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox xmlFileExtensionGroupBox;
        private System.Windows.Forms.TextBox namespacePrefixTextBox;
        private System.Windows.Forms.Label prefixLabel;
        private System.Windows.Forms.TextBox schemaTextBox;
        private System.Windows.Forms.ComboBox fileExtensionComboBox;
        private System.Windows.Forms.Label schemaLabel;
        private System.Windows.Forms.Label fileExtensionLabel;
        private System.Windows.Forms.Button changeSchemaButton;
        private System.Windows.Forms.GroupBox schemasGroupBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ListBox schemaListBox;
        private System.Windows.Forms.GroupBox foldingGroupBox;
        private System.Windows.Forms.CheckBox showAttributesWhenFoldedCheckBox;
        private System.Windows.Forms.GroupBox xmlCompletionGroupBox;
        private System.Windows.Forms.CheckBox showSchemaAnnotationCheckBox;
    }
}
