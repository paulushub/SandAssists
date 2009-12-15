namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class RuleSetOptionPanel
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
            this.escCharTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.igcaseBox = new System.Windows.Forms.CheckBox();
            this.refBox = new System.Windows.Forms.TextBox();
            this.label3a = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label3b = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.explLabel = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.delimBox = new System.Windows.Forms.TextBox();
            this.label3c = new System.Windows.Forms.Label();
            this.label3d = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // escCharTextBox
            // 
            this.escCharTextBox.Location = new System.Drawing.Point(154, 133);
            this.escCharTextBox.MaxLength = 1;
            this.escCharTextBox.Name = "escCharTextBox";
            this.escCharTextBox.Size = new System.Drawing.Size(109, 19);
            this.escCharTextBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 18);
            this.label1.TabIndex = 17;
            this.label1.Text = "${res:Dialog.HighlightingEditor.Span.EscChar}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // igcaseBox
            // 
            this.igcaseBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.igcaseBox.Location = new System.Drawing.Point(8, 108);
            this.igcaseBox.Name = "igcaseBox";
            this.igcaseBox.Size = new System.Drawing.Size(354, 18);
            this.igcaseBox.TabIndex = 16;
            this.igcaseBox.Text = "${res:Dialog.HighlightingEditor.RuleSet.IgnoreCase}";
            // 
            // refBox
            // 
            this.refBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.refBox.Location = new System.Drawing.Point(136, 32);
            this.refBox.Name = "refBox";
            this.refBox.Size = new System.Drawing.Size(226, 19);
            this.refBox.TabIndex = 15;
            // 
            // label3a
            // 
            this.label3a.Location = new System.Drawing.Point(8, 34);
            this.label3a.Name = "label3a";
            this.label3a.Size = new System.Drawing.Size(128, 18);
            this.label3a.TabIndex = 13;
            this.label3a.Text = "${res:Dialog.HighlightingEditor.RuleSet.Reference}";
            this.label3a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nameBox
            // 
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(136, 8);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(226, 19);
            this.nameBox.TabIndex = 10;
            // 
            // label3b
            // 
            this.label3b.Location = new System.Drawing.Point(8, 8);
            this.label3b.Name = "label3b";
            this.label3b.Size = new System.Drawing.Size(128, 18);
            this.label3b.TabIndex = 9;
            this.label3b.Text = "${res:Dialog.HighlightingEditor.RuleSet.Name}";
            this.label3b.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.explLabel);
            this.panel.Controls.Add(this.label);
            this.panel.Controls.Add(this.label2);
            this.panel.Location = new System.Drawing.Point(8, 158);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(354, 216);
            this.panel.TabIndex = 8;
            // 
            // explLabel
            // 
            this.explLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.explLabel.Location = new System.Drawing.Point(10, 20);
            this.explLabel.Name = "explLabel";
            this.explLabel.Size = new System.Drawing.Size(333, 192);
            this.explLabel.TabIndex = 2;
            this.explLabel.Text = "${res:Dialog.HighlightingEditor.RuleSet.Explanation}";
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(338, 18);
            this.label.TabIndex = 0;
            this.label.Text = "${res:Dialog.HighlightingEditor.Explanation}";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(0, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 0);
            this.label2.TabIndex = 1;
            // 
            // delimBox
            // 
            this.delimBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.delimBox.Location = new System.Drawing.Point(136, 57);
            this.delimBox.Name = "delimBox";
            this.delimBox.Size = new System.Drawing.Size(226, 19);
            this.delimBox.TabIndex = 14;
            // 
            // label3c
            // 
            this.label3c.Location = new System.Drawing.Point(8, 59);
            this.label3c.Name = "label3c";
            this.label3c.Size = new System.Drawing.Size(128, 18);
            this.label3c.TabIndex = 12;
            this.label3c.Text = "${res:Dialog.HighlightingEditor.RuleSet.Delimiters}";
            this.label3c.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3d
            // 
            this.label3d.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3d.Location = new System.Drawing.Point(24, 84);
            this.label3d.Name = "label3d";
            this.label3d.Size = new System.Drawing.Size(319, 18);
            this.label3d.TabIndex = 11;
            this.label3d.Text = "${res:Dialog.HighlightingEditor.RuleSet.SpaceAndTab}";
            // 
            // RuleSetOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.escCharTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.igcaseBox);
            this.Controls.Add(this.refBox);
            this.Controls.Add(this.label3a);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label3b);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.delimBox);
            this.Controls.Add(this.label3c);
            this.Controls.Add(this.label3d);
            this.Name = "RuleSetOptionPanel";
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox escCharTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox igcaseBox;
        private System.Windows.Forms.TextBox refBox;
        private System.Windows.Forms.Label label3a;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label3b;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label explLabel;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox delimBox;
        private System.Windows.Forms.Label label3c;
        private System.Windows.Forms.Label label3d;
    }
}
