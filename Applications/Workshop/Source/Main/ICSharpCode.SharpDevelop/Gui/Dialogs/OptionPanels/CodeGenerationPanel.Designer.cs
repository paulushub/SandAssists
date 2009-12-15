namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class CodeGenerationPanel
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
            this.commentGeneratingGroupBox = new System.Windows.Forms.GroupBox();
            this.generateAdditonalCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.generateDocCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.codeGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.useFullTypeNamesCheckBox = new System.Windows.Forms.CheckBox();
            this.blankLinesBetweenMemberCheckBox = new System.Windows.Forms.CheckBox();
            this.elseOnClosingCheckbox = new System.Windows.Forms.CheckBox();
            this.startBlockOnTheSameLineCheckBox = new System.Windows.Forms.CheckBox();
            this.commentGeneratingGroupBox.SuspendLayout();
            this.codeGenerationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // commentGeneratingGroupBox
            // 
            this.commentGeneratingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commentGeneratingGroupBox.Controls.Add(this.generateAdditonalCommentsCheckBox);
            this.commentGeneratingGroupBox.Controls.Add(this.generateDocCommentsCheckBox);
            this.commentGeneratingGroupBox.Location = new System.Drawing.Point(8, 123);
            this.commentGeneratingGroupBox.Name = "commentGeneratingGroupBox";
            this.commentGeneratingGroupBox.Size = new System.Drawing.Size(386, 64);
            this.commentGeneratingGroupBox.TabIndex = 3;
            this.commentGeneratingGroupBox.TabStop = false;
            this.commentGeneratingGroupBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CommentGenerationOptio" +
                "nsGroupBox}";
            // 
            // generateAdditonalCommentsCheckBox
            // 
            this.generateAdditonalCommentsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generateAdditonalCommentsCheckBox.Location = new System.Drawing.Point(8, 38);
            this.generateAdditonalCommentsCheckBox.Name = "generateAdditonalCommentsCheckBox";
            this.generateAdditonalCommentsCheckBox.Size = new System.Drawing.Size(370, 18);
            this.generateAdditonalCommentsCheckBox.TabIndex = 1;
            this.generateAdditonalCommentsCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateAdditionalComm" +
                "entsCheckBox}";
            // 
            // generateDocCommentsCheckBox
            // 
            this.generateDocCommentsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generateDocCommentsCheckBox.Location = new System.Drawing.Point(8, 16);
            this.generateDocCommentsCheckBox.Name = "generateDocCommentsCheckBox";
            this.generateDocCommentsCheckBox.Size = new System.Drawing.Size(370, 18);
            this.generateDocCommentsCheckBox.TabIndex = 0;
            this.generateDocCommentsCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateDocCommentsChe" +
                "ckBox}";
            // 
            // codeGenerationGroupBox
            // 
            this.codeGenerationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.codeGenerationGroupBox.Controls.Add(this.useFullTypeNamesCheckBox);
            this.codeGenerationGroupBox.Controls.Add(this.blankLinesBetweenMemberCheckBox);
            this.codeGenerationGroupBox.Controls.Add(this.elseOnClosingCheckbox);
            this.codeGenerationGroupBox.Controls.Add(this.startBlockOnTheSameLineCheckBox);
            this.codeGenerationGroupBox.Location = new System.Drawing.Point(8, 8);
            this.codeGenerationGroupBox.Name = "codeGenerationGroupBox";
            this.codeGenerationGroupBox.Size = new System.Drawing.Size(386, 108);
            this.codeGenerationGroupBox.TabIndex = 2;
            this.codeGenerationGroupBox.TabStop = false;
            this.codeGenerationGroupBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CodeGenerationOptionsG" +
                "roupBox}";
            // 
            // useFullTypeNamesCheckBox
            // 
            this.useFullTypeNamesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useFullTypeNamesCheckBox.Location = new System.Drawing.Point(8, 82);
            this.useFullTypeNamesCheckBox.Name = "useFullTypeNamesCheckBox";
            this.useFullTypeNamesCheckBox.Size = new System.Drawing.Size(370, 18);
            this.useFullTypeNamesCheckBox.TabIndex = 3;
            this.useFullTypeNamesCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.UseFullTypeNamesCheckB" +
                "ox}";
            // 
            // blankLinesBetweenMemberCheckBox
            // 
            this.blankLinesBetweenMemberCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.blankLinesBetweenMemberCheckBox.Location = new System.Drawing.Point(8, 60);
            this.blankLinesBetweenMemberCheckBox.Name = "blankLinesBetweenMemberCheckBox";
            this.blankLinesBetweenMemberCheckBox.Size = new System.Drawing.Size(370, 18);
            this.blankLinesBetweenMemberCheckBox.TabIndex = 2;
            this.blankLinesBetweenMemberCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.BlankLinesBetweenMembe" +
                "rsCheckBox}";
            // 
            // elseOnClosingCheckbox
            // 
            this.elseOnClosingCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.elseOnClosingCheckbox.Location = new System.Drawing.Point(8, 38);
            this.elseOnClosingCheckbox.Name = "elseOnClosingCheckbox";
            this.elseOnClosingCheckbox.Size = new System.Drawing.Size(370, 18);
            this.elseOnClosingCheckbox.TabIndex = 1;
            this.elseOnClosingCheckbox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.ElseOnClosingCheckBox}" +
                "";
            // 
            // startBlockOnTheSameLineCheckBox
            // 
            this.startBlockOnTheSameLineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startBlockOnTheSameLineCheckBox.Location = new System.Drawing.Point(8, 16);
            this.startBlockOnTheSameLineCheckBox.Name = "startBlockOnTheSameLineCheckBox";
            this.startBlockOnTheSameLineCheckBox.Size = new System.Drawing.Size(370, 18);
            this.startBlockOnTheSameLineCheckBox.TabIndex = 0;
            this.startBlockOnTheSameLineCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.StartBlockOnTheSameLin" +
                "eCheckBox}";
            // 
            // CodeGenerationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.commentGeneratingGroupBox);
            this.Controls.Add(this.codeGenerationGroupBox);
            this.Name = "CodeGenerationPanel";
            this.Size = new System.Drawing.Size(404, 380);
            this.commentGeneratingGroupBox.ResumeLayout(false);
            this.codeGenerationGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox commentGeneratingGroupBox;
        private System.Windows.Forms.CheckBox generateAdditonalCommentsCheckBox;
        private System.Windows.Forms.CheckBox generateDocCommentsCheckBox;
        private System.Windows.Forms.GroupBox codeGenerationGroupBox;
        private System.Windows.Forms.CheckBox useFullTypeNamesCheckBox;
        private System.Windows.Forms.CheckBox blankLinesBetweenMemberCheckBox;
        private System.Windows.Forms.CheckBox elseOnClosingCheckbox;
        private System.Windows.Forms.CheckBox startBlockOnTheSameLineCheckBox;
    }
}
