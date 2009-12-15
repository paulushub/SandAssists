namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    partial class MarkersTextEditorPanel
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
            this.highlightAfterCaret = new System.Windows.Forms.RadioButton();
            this.highlightBeforeCaret = new System.Windows.Forms.RadioButton();
            this.showLineHighlighterCheckBox = new System.Windows.Forms.CheckBox();
            this.showLineNumberCheckBox = new System.Windows.Forms.CheckBox();
            this.showErrorsCheckBox = new System.Windows.Forms.CheckBox();
            this.showBracketHighlighterCheckBox = new System.Windows.Forms.CheckBox();
            this.showInvalidLinesCheckBox = new System.Windows.Forms.CheckBox();
            this.showHRulerCheckBox = new System.Windows.Forms.CheckBox();
            this.showEOLMarkersCheckBox = new System.Windows.Forms.CheckBox();
            this.showVRulerCheckBox = new System.Windows.Forms.CheckBox();
            this.showTabCharsCheckBox = new System.Windows.Forms.CheckBox();
            this.showSpaceCharsCheckBox = new System.Windows.Forms.CheckBox();
            this.CreatedObject23 = new System.Windows.Forms.Label();
            this.vRulerRowTextBox = new System.Windows.Forms.TextBox();
            this.CreatedObject2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CreatedObject2
            // 
            this.CreatedObject2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject2.Controls.Add(this.highlightAfterCaret);
            this.CreatedObject2.Controls.Add(this.highlightBeforeCaret);
            this.CreatedObject2.Controls.Add(this.showLineHighlighterCheckBox);
            this.CreatedObject2.Controls.Add(this.showLineNumberCheckBox);
            this.CreatedObject2.Controls.Add(this.showErrorsCheckBox);
            this.CreatedObject2.Controls.Add(this.showBracketHighlighterCheckBox);
            this.CreatedObject2.Controls.Add(this.showInvalidLinesCheckBox);
            this.CreatedObject2.Controls.Add(this.showHRulerCheckBox);
            this.CreatedObject2.Controls.Add(this.showEOLMarkersCheckBox);
            this.CreatedObject2.Controls.Add(this.showVRulerCheckBox);
            this.CreatedObject2.Controls.Add(this.showTabCharsCheckBox);
            this.CreatedObject2.Controls.Add(this.showSpaceCharsCheckBox);
            this.CreatedObject2.Controls.Add(this.CreatedObject23);
            this.CreatedObject2.Controls.Add(this.vRulerRowTextBox);
            this.CreatedObject2.Location = new System.Drawing.Point(8, 8);
            this.CreatedObject2.Name = "CreatedObject2";
            this.CreatedObject2.Size = new System.Drawing.Size(386, 312);
            this.CreatedObject2.TabIndex = 0;
            this.CreatedObject2.TabStop = false;
            this.CreatedObject2.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.MarkersGroupBox}";
            // 
            // highlightAfterCaret
            // 
            this.highlightAfterCaret.Location = new System.Drawing.Point(41, 193);
            this.highlightAfterCaret.Name = "highlightAfterCaret";
            this.highlightAfterCaret.Size = new System.Drawing.Size(306, 20);
            this.highlightAfterCaret.TabIndex = 9;
            this.highlightAfterCaret.TabStop = true;
            this.highlightAfterCaret.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.AfterCare" +
                "t}";
            this.highlightAfterCaret.UseVisualStyleBackColor = true;
            // 
            // highlightBeforeCaret
            // 
            this.highlightBeforeCaret.Location = new System.Drawing.Point(41, 171);
            this.highlightBeforeCaret.Name = "highlightBeforeCaret";
            this.highlightBeforeCaret.Size = new System.Drawing.Size(306, 20);
            this.highlightBeforeCaret.TabIndex = 8;
            this.highlightBeforeCaret.TabStop = true;
            this.highlightBeforeCaret.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.BeforeCar" +
                "et}";
            this.highlightBeforeCaret.UseVisualStyleBackColor = true;
            // 
            // showLineHighlighterCheckBox
            // 
            this.showLineHighlighterCheckBox.Location = new System.Drawing.Point(8, 127);
            this.showLineHighlighterCheckBox.Name = "showLineHighlighterCheckBox";
            this.showLineHighlighterCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showLineHighlighterCheckBox.TabIndex = 6;
            this.showLineHighlighterCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.LineMarkerLabel}";
            this.showLineHighlighterCheckBox.UseVisualStyleBackColor = true;
            // 
            // showLineNumberCheckBox
            // 
            this.showLineNumberCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showLineNumberCheckBox.Location = new System.Drawing.Point(8, 83);
            this.showLineNumberCheckBox.Name = "showLineNumberCheckBox";
            this.showLineNumberCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showLineNumberCheckBox.TabIndex = 4;
            this.showLineNumberCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.LineNumberCheckBox}";
            // 
            // showErrorsCheckBox
            // 
            this.showErrorsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showErrorsCheckBox.Location = new System.Drawing.Point(8, 105);
            this.showErrorsCheckBox.Name = "showErrorsCheckBox";
            this.showErrorsCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showErrorsCheckBox.TabIndex = 5;
            this.showErrorsCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.UnderLineErrorsCheckBox}";
            // 
            // showBracketHighlighterCheckBox
            // 
            this.showBracketHighlighterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showBracketHighlighterCheckBox.Location = new System.Drawing.Point(8, 149);
            this.showBracketHighlighterCheckBox.Name = "showBracketHighlighterCheckBox";
            this.showBracketHighlighterCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showBracketHighlighterCheckBox.TabIndex = 7;
            this.showBracketHighlighterCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.HiglightBracketCheckBox}";
            // 
            // showInvalidLinesCheckBox
            // 
            this.showInvalidLinesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showInvalidLinesCheckBox.Location = new System.Drawing.Point(8, 217);
            this.showInvalidLinesCheckBox.Name = "showInvalidLinesCheckBox";
            this.showInvalidLinesCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showInvalidLinesCheckBox.TabIndex = 10;
            this.showInvalidLinesCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.InvalidLinesCheckBox}";
            // 
            // showHRulerCheckBox
            // 
            this.showHRulerCheckBox.Location = new System.Drawing.Point(8, 16);
            this.showHRulerCheckBox.Name = "showHRulerCheckBox";
            this.showHRulerCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showHRulerCheckBox.TabIndex = 0;
            this.showHRulerCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.HorizontalRulerCheckBox}";
            // 
            // showEOLMarkersCheckBox
            // 
            this.showEOLMarkersCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showEOLMarkersCheckBox.Location = new System.Drawing.Point(8, 239);
            this.showEOLMarkersCheckBox.Name = "showEOLMarkersCheckBox";
            this.showEOLMarkersCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showEOLMarkersCheckBox.TabIndex = 11;
            this.showEOLMarkersCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.EOLMarkersCheckBox}";
            // 
            // showVRulerCheckBox
            // 
            this.showVRulerCheckBox.Location = new System.Drawing.Point(8, 38);
            this.showVRulerCheckBox.Name = "showVRulerCheckBox";
            this.showVRulerCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showVRulerCheckBox.TabIndex = 1;
            this.showVRulerCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.VerticalRulerCheckBox}";
            // 
            // showTabCharsCheckBox
            // 
            this.showTabCharsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showTabCharsCheckBox.Location = new System.Drawing.Point(8, 283);
            this.showTabCharsCheckBox.Name = "showTabCharsCheckBox";
            this.showTabCharsCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showTabCharsCheckBox.TabIndex = 13;
            this.showTabCharsCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.TabsCheckBox}";
            // 
            // showSpaceCharsCheckBox
            // 
            this.showSpaceCharsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showSpaceCharsCheckBox.Location = new System.Drawing.Point(8, 261);
            this.showSpaceCharsCheckBox.Name = "showSpaceCharsCheckBox";
            this.showSpaceCharsCheckBox.Size = new System.Drawing.Size(370, 20);
            this.showSpaceCharsCheckBox.TabIndex = 12;
            this.showSpaceCharsCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.SpacesCheckBox}";
            // 
            // CreatedObject23
            // 
            this.CreatedObject23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject23.Location = new System.Drawing.Point(41, 60);
            this.CreatedObject23.Name = "CreatedObject23";
            this.CreatedObject23.Size = new System.Drawing.Size(104, 20);
            this.CreatedObject23.TabIndex = 2;
            this.CreatedObject23.Text = "${res:Dialog.Options.IDEOptions.TextEditor.Markers.AtRowLabel}";
            this.CreatedObject23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // vRulerRowTextBox
            // 
            this.vRulerRowTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vRulerRowTextBox.Location = new System.Drawing.Point(146, 60);
            this.vRulerRowTextBox.Name = "vRulerRowTextBox";
            this.vRulerRowTextBox.Size = new System.Drawing.Size(64, 20);
            this.vRulerRowTextBox.TabIndex = 3;
            // 
            // MarkersTextEditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CreatedObject2);
            this.Name = "MarkersTextEditorPanel";
            this.CreatedObject2.ResumeLayout(false);
            this.CreatedObject2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CreatedObject2;
        private System.Windows.Forms.CheckBox showLineNumberCheckBox;
        private System.Windows.Forms.CheckBox showErrorsCheckBox;
        private System.Windows.Forms.CheckBox showBracketHighlighterCheckBox;
        private System.Windows.Forms.CheckBox showInvalidLinesCheckBox;
        private System.Windows.Forms.CheckBox showHRulerCheckBox;
        private System.Windows.Forms.CheckBox showEOLMarkersCheckBox;
        private System.Windows.Forms.CheckBox showVRulerCheckBox;
        private System.Windows.Forms.CheckBox showTabCharsCheckBox;
        private System.Windows.Forms.CheckBox showSpaceCharsCheckBox;
        private System.Windows.Forms.Label CreatedObject23;
        private System.Windows.Forms.TextBox vRulerRowTextBox;
        private System.Windows.Forms.CheckBox showLineHighlighterCheckBox;
        private System.Windows.Forms.RadioButton highlightBeforeCaret;
        private System.Windows.Forms.RadioButton highlightAfterCaret;
    }
}
