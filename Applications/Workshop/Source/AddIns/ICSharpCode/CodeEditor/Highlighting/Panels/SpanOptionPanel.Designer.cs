namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class SpanOptionPanel
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
            this.endSingleWordCheckBox = new System.Windows.Forms.CheckBox();
            this.beginSingleWordCheckBox = new System.Windows.Forms.CheckBox();
            this.escCharTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chgCont = new System.Windows.Forms.Button();
            this.samCont = new System.Windows.Forms.Label();
            this.chgEnd = new System.Windows.Forms.Button();
            this.useEnd = new System.Windows.Forms.CheckBox();
            this.samEnd = new System.Windows.Forms.Label();
            this.chgBegin = new System.Windows.Forms.Button();
            this.useBegin = new System.Windows.Forms.CheckBox();
            this.samBegin = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ruleBox = new System.Windows.Forms.ComboBox();
            this.label3wjej = new System.Windows.Forms.Label();
            this.stopEolBox = new System.Windows.Forms.CheckBox();
            this.beginBox = new System.Windows.Forms.TextBox();
            this.label3qgae = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label3wr = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.explLabel = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.endBox = new System.Windows.Forms.TextBox();
            this.label3b = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // endSingleWordCheckBox
            // 
            this.endSingleWordCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.endSingleWordCheckBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.endSingleWordCheckBox.Location = new System.Drawing.Point(8, 85);
            this.endSingleWordCheckBox.Name = "endSingleWordCheckBox";
            this.endSingleWordCheckBox.Size = new System.Drawing.Size(144, 18);
            this.endSingleWordCheckBox.TabIndex = 47;
            this.endSingleWordCheckBox.Text = "${res:Dialog.HighlightingEditor.Span.EndSingleWord}";
            this.endSingleWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // beginSingleWordCheckBox
            // 
            this.beginSingleWordCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.beginSingleWordCheckBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.beginSingleWordCheckBox.Location = new System.Drawing.Point(8, 45);
            this.beginSingleWordCheckBox.Name = "beginSingleWordCheckBox";
            this.beginSingleWordCheckBox.Size = new System.Drawing.Size(144, 18);
            this.beginSingleWordCheckBox.TabIndex = 46;
            this.beginSingleWordCheckBox.Text = "${res:Dialog.HighlightingEditor.Span.BeginSingleWord}";
            this.beginSingleWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // escCharTextBox
            // 
            this.escCharTextBox.Location = new System.Drawing.Point(137, 246);
            this.escCharTextBox.MaxLength = 1;
            this.escCharTextBox.Name = "escCharTextBox";
            this.escCharTextBox.Size = new System.Drawing.Size(100, 19);
            this.escCharTextBox.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 18);
            this.label1.TabIndex = 44;
            this.label1.Text = "${res:Dialog.HighlightingEditor.Span.EscChar}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chgCont
            // 
            this.chgCont.Location = new System.Drawing.Point(245, 201);
            this.chgCont.Name = "chgCont";
            this.chgCont.Size = new System.Drawing.Size(88, 23);
            this.chgCont.TabIndex = 43;
            this.chgCont.Text = "${res:Dialog.HighlightingEditor.Change}";
            // 
            // samCont
            // 
            this.samCont.Location = new System.Drawing.Point(245, 149);
            this.samCont.Name = "samCont";
            this.samCont.Size = new System.Drawing.Size(88, 30);
            this.samCont.TabIndex = 42;
            this.samCont.Text = "${res:Dialog.HighlightingEditor.Span.ContCol}";
            this.samCont.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chgEnd
            // 
            this.chgEnd.Location = new System.Drawing.Point(141, 201);
            this.chgEnd.Name = "chgEnd";
            this.chgEnd.Size = new System.Drawing.Size(88, 23);
            this.chgEnd.TabIndex = 41;
            this.chgEnd.Text = "${res:Dialog.HighlightingEditor.Change}";
            // 
            // useEnd
            // 
            this.useEnd.Location = new System.Drawing.Point(143, 182);
            this.useEnd.Name = "useEnd";
            this.useEnd.Size = new System.Drawing.Size(88, 18);
            this.useEnd.TabIndex = 40;
            this.useEnd.Text = "${res:Dialog.HighlightingEditor.Span.Use}";
            // 
            // samEnd
            // 
            this.samEnd.Location = new System.Drawing.Point(141, 149);
            this.samEnd.Name = "samEnd";
            this.samEnd.Size = new System.Drawing.Size(88, 30);
            this.samEnd.TabIndex = 39;
            this.samEnd.Text = "${res:Dialog.HighlightingEditor.Span.EndCol}";
            this.samEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chgBegin
            // 
            this.chgBegin.Location = new System.Drawing.Point(37, 201);
            this.chgBegin.Name = "chgBegin";
            this.chgBegin.Size = new System.Drawing.Size(88, 23);
            this.chgBegin.TabIndex = 38;
            this.chgBegin.Text = "${res:Dialog.HighlightingEditor.Change}";
            // 
            // useBegin
            // 
            this.useBegin.Location = new System.Drawing.Point(39, 182);
            this.useBegin.Name = "useBegin";
            this.useBegin.Size = new System.Drawing.Size(88, 18);
            this.useBegin.TabIndex = 37;
            this.useBegin.Text = "${res:Dialog.HighlightingEditor.Span.Use}";
            // 
            // samBegin
            // 
            this.samBegin.Location = new System.Drawing.Point(37, 149);
            this.samBegin.Name = "samBegin";
            this.samBegin.Size = new System.Drawing.Size(88, 30);
            this.samBegin.TabIndex = 36;
            this.samBegin.Text = "${res:Dialog.HighlightingEditor.Span.BeginCol}";
            this.samBegin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(354, 18);
            this.label7.TabIndex = 35;
            this.label7.Text = "${res:Dialog.HighlightingEditor.Span.Colors}";
            this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ruleBox
            // 
            this.ruleBox.Location = new System.Drawing.Point(137, 103);
            this.ruleBox.Name = "ruleBox";
            this.ruleBox.Size = new System.Drawing.Size(225, 20);
            this.ruleBox.TabIndex = 34;
            this.ruleBox.Text = "comboBox";
            // 
            // label3wjej
            // 
            this.label3wjej.Location = new System.Drawing.Point(8, 104);
            this.label3wjej.Name = "label3wjej";
            this.label3wjej.Size = new System.Drawing.Size(128, 18);
            this.label3wjej.TabIndex = 33;
            this.label3wjej.Text = "${res:Dialog.HighlightingEditor.Span.Ruleset}";
            this.label3wjej.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stopEolBox
            // 
            this.stopEolBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stopEolBox.Location = new System.Drawing.Point(8, 226);
            this.stopEolBox.Name = "stopEolBox";
            this.stopEolBox.Size = new System.Drawing.Size(354, 18);
            this.stopEolBox.TabIndex = 32;
            this.stopEolBox.Text = "${res:Dialog.HighlightingEditor.Span.StopEol}";
            // 
            // beginBox
            // 
            this.beginBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.beginBox.Location = new System.Drawing.Point(137, 25);
            this.beginBox.Name = "beginBox";
            this.beginBox.Size = new System.Drawing.Size(225, 19);
            this.beginBox.TabIndex = 30;
            // 
            // label3qgae
            // 
            this.label3qgae.Location = new System.Drawing.Point(8, 25);
            this.label3qgae.Name = "label3qgae";
            this.label3qgae.Size = new System.Drawing.Size(128, 18);
            this.label3qgae.TabIndex = 28;
            this.label3qgae.Text = "${res:Dialog.HighlightingEditor.Span.Begin}";
            this.label3qgae.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nameBox
            // 
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(137, 3);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(225, 19);
            this.nameBox.TabIndex = 27;
            // 
            // label3wr
            // 
            this.label3wr.Location = new System.Drawing.Point(8, 3);
            this.label3wr.Name = "label3wr";
            this.label3wr.Size = new System.Drawing.Size(128, 18);
            this.label3wr.TabIndex = 26;
            this.label3wr.Text = "${res:Dialog.HighlightingEditor.Span.Name}";
            this.label3wr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.explLabel);
            this.panel.Controls.Add(this.label);
            this.panel.Controls.Add(this.label2);
            this.panel.Location = new System.Drawing.Point(8, 267);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(354, 110);
            this.panel.TabIndex = 25;
            // 
            // explLabel
            // 
            this.explLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.explLabel.Location = new System.Drawing.Point(8, 18);
            this.explLabel.Name = "explLabel";
            this.explLabel.Size = new System.Drawing.Size(335, 88);
            this.explLabel.TabIndex = 2;
            this.explLabel.Text = "${res:Dialog.HighlightingEditor.Span.Explanation}";
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(344, 18);
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
            this.label2.Size = new System.Drawing.Size(344, 0);
            this.label2.TabIndex = 1;
            // 
            // endBox
            // 
            this.endBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.endBox.Location = new System.Drawing.Point(137, 64);
            this.endBox.Name = "endBox";
            this.endBox.Size = new System.Drawing.Size(225, 19);
            this.endBox.TabIndex = 31;
            // 
            // label3b
            // 
            this.label3b.Location = new System.Drawing.Point(8, 65);
            this.label3b.Name = "label3b";
            this.label3b.Size = new System.Drawing.Size(128, 18);
            this.label3b.TabIndex = 29;
            this.label3b.Text = "${res:Dialog.HighlightingEditor.Span.End}";
            // 
            // SpanOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.endSingleWordCheckBox);
            this.Controls.Add(this.beginSingleWordCheckBox);
            this.Controls.Add(this.escCharTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chgCont);
            this.Controls.Add(this.samCont);
            this.Controls.Add(this.chgEnd);
            this.Controls.Add(this.useEnd);
            this.Controls.Add(this.samEnd);
            this.Controls.Add(this.chgBegin);
            this.Controls.Add(this.useBegin);
            this.Controls.Add(this.samBegin);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ruleBox);
            this.Controls.Add(this.label3wjej);
            this.Controls.Add(this.stopEolBox);
            this.Controls.Add(this.beginBox);
            this.Controls.Add(this.label3qgae);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label3wr);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.endBox);
            this.Controls.Add(this.label3b);
            this.Name = "SpanOptionPanel";
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox endSingleWordCheckBox;
        private System.Windows.Forms.CheckBox beginSingleWordCheckBox;
        private System.Windows.Forms.TextBox escCharTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button chgCont;
        private System.Windows.Forms.Label samCont;
        private System.Windows.Forms.Button chgEnd;
        private System.Windows.Forms.CheckBox useEnd;
        private System.Windows.Forms.Label samEnd;
        private System.Windows.Forms.Button chgBegin;
        private System.Windows.Forms.CheckBox useBegin;
        private System.Windows.Forms.Label samBegin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox ruleBox;
        private System.Windows.Forms.Label label3wjej;
        private System.Windows.Forms.CheckBox stopEolBox;
        private System.Windows.Forms.TextBox beginBox;
        private System.Windows.Forms.Label label3qgae;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label3wr;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label explLabel;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox endBox;
        private System.Windows.Forms.Label label3b;
    }
}
