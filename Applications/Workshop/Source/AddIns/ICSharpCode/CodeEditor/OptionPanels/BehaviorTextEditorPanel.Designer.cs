namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    partial class BehaviorTextEditorPanel
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
            this.CreatedObject13 = new System.Windows.Forms.Label();
            this.tabSizeTextBox = new System.Windows.Forms.TextBox();
            this.CreatedObject18 = new System.Windows.Forms.Label();
            this.indentSizeTextBox = new System.Windows.Forms.TextBox();
            this.groupIndenting = new System.Windows.Forms.GroupBox();
            this.indentNone = new System.Windows.Forms.RadioButton();
            this.indentAutomatic = new System.Windows.Forms.RadioButton();
            this.indentSmart = new System.Windows.Forms.RadioButton();
            this.insertSpaces = new System.Windows.Forms.RadioButton();
            this.insertTabs = new System.Windows.Forms.RadioButton();
            this.CreatedObject2.SuspendLayout();
            this.groupIndenting.SuspendLayout();
            this.SuspendLayout();
            // 
            // CreatedObject2
            // 
            this.CreatedObject2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject2.Controls.Add(this.insertTabs);
            this.CreatedObject2.Controls.Add(this.insertSpaces);
            this.CreatedObject2.Controls.Add(this.CreatedObject13);
            this.CreatedObject2.Controls.Add(this.tabSizeTextBox);
            this.CreatedObject2.Controls.Add(this.CreatedObject18);
            this.CreatedObject2.Controls.Add(this.indentSizeTextBox);
            this.CreatedObject2.Location = new System.Drawing.Point(8, 105);
            this.CreatedObject2.Name = "CreatedObject2";
            this.CreatedObject2.Size = new System.Drawing.Size(386, 119);
            this.CreatedObject2.TabIndex = 1;
            this.CreatedObject2.TabStop = false;
            this.CreatedObject2.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.Tab}";
            // 
            // CreatedObject13
            // 
            this.CreatedObject13.Location = new System.Drawing.Point(8, 16);
            this.CreatedObject13.Name = "CreatedObject13";
            this.CreatedObject13.Size = new System.Drawing.Size(112, 20);
            this.CreatedObject13.TabIndex = 2;
            this.CreatedObject13.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabSizeLabel}";
            this.CreatedObject13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabSizeTextBox
            // 
            this.tabSizeTextBox.Location = new System.Drawing.Point(120, 16);
            this.tabSizeTextBox.Name = "tabSizeTextBox";
            this.tabSizeTextBox.Size = new System.Drawing.Size(56, 20);
            this.tabSizeTextBox.TabIndex = 0;
            // 
            // CreatedObject18
            // 
            this.CreatedObject18.Location = new System.Drawing.Point(8, 40);
            this.CreatedObject18.Name = "CreatedObject18";
            this.CreatedObject18.Size = new System.Drawing.Size(112, 20);
            this.CreatedObject18.TabIndex = 3;
            this.CreatedObject18.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentSizeLabel}";
            this.CreatedObject18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // indentSizeTextBox
            // 
            this.indentSizeTextBox.Location = new System.Drawing.Point(120, 40);
            this.indentSizeTextBox.Name = "indentSizeTextBox";
            this.indentSizeTextBox.Size = new System.Drawing.Size(56, 20);
            this.indentSizeTextBox.TabIndex = 1;
            // 
            // groupIndenting
            // 
            this.groupIndenting.Controls.Add(this.indentSmart);
            this.groupIndenting.Controls.Add(this.indentAutomatic);
            this.groupIndenting.Controls.Add(this.indentNone);
            this.groupIndenting.Location = new System.Drawing.Point(8, 8);
            this.groupIndenting.Name = "groupIndenting";
            this.groupIndenting.Size = new System.Drawing.Size(386, 89);
            this.groupIndenting.TabIndex = 0;
            this.groupIndenting.TabStop = false;
            this.groupIndenting.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.Indenting}";
            // 
            // indentNone
            // 
            this.indentNone.Location = new System.Drawing.Point(8, 16);
            this.indentNone.Name = "indentNone";
            this.indentNone.Size = new System.Drawing.Size(370, 20);
            this.indentNone.TabIndex = 0;
            this.indentNone.TabStop = true;
            this.indentNone.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}";
            this.indentNone.UseVisualStyleBackColor = true;
            // 
            // indentAutomatic
            // 
            this.indentAutomatic.Location = new System.Drawing.Point(8, 38);
            this.indentAutomatic.Name = "indentAutomatic";
            this.indentAutomatic.Size = new System.Drawing.Size(370, 20);
            this.indentAutomatic.TabIndex = 1;
            this.indentAutomatic.TabStop = true;
            this.indentAutomatic.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}";
            this.indentAutomatic.UseVisualStyleBackColor = true;
            // 
            // indentSmart
            // 
            this.indentSmart.Location = new System.Drawing.Point(8, 62);
            this.indentSmart.Name = "indentSmart";
            this.indentSmart.Size = new System.Drawing.Size(370, 20);
            this.indentSmart.TabIndex = 2;
            this.indentSmart.TabStop = true;
            this.indentSmart.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}";
            this.indentSmart.UseVisualStyleBackColor = true;
            // 
            // insertSpaces
            // 
            this.insertSpaces.Location = new System.Drawing.Point(8, 69);
            this.insertSpaces.Name = "insertSpaces";
            this.insertSpaces.Size = new System.Drawing.Size(370, 20);
            this.insertSpaces.TabIndex = 4;
            this.insertSpaces.TabStop = true;
            this.insertSpaces.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabsToSpaces}";
            this.insertSpaces.UseVisualStyleBackColor = true;
            // 
            // insertTabs
            // 
            this.insertTabs.Location = new System.Drawing.Point(8, 93);
            this.insertTabs.Name = "insertTabs";
            this.insertTabs.Size = new System.Drawing.Size(370, 20);
            this.insertTabs.TabIndex = 5;
            this.insertTabs.TabStop = true;
            this.insertTabs.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.SpacesToTabs}";
            this.insertTabs.UseVisualStyleBackColor = true;
            // 
            // BehaviorTextEditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupIndenting);
            this.Controls.Add(this.CreatedObject2);
            this.Name = "BehaviorTextEditorPanel";
            this.CreatedObject2.ResumeLayout(false);
            this.CreatedObject2.PerformLayout();
            this.groupIndenting.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CreatedObject2;
        private System.Windows.Forms.Label CreatedObject13;
        private System.Windows.Forms.TextBox tabSizeTextBox;
        private System.Windows.Forms.Label CreatedObject18;
        private System.Windows.Forms.TextBox indentSizeTextBox;
        private System.Windows.Forms.GroupBox groupIndenting;
        private System.Windows.Forms.RadioButton indentNone;
        private System.Windows.Forms.RadioButton indentSmart;
        private System.Windows.Forms.RadioButton indentAutomatic;
        private System.Windows.Forms.RadioButton insertSpaces;
        private System.Windows.Forms.RadioButton insertTabs;
    }
}
