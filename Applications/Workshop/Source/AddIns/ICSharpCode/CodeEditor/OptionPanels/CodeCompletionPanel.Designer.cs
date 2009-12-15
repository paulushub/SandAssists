namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    partial class CodeCompletionPanel
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.refreshInsightOnCommaCheckBox = new System.Windows.Forms.CheckBox();
            this.useDebugTooltipsOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.clearDataUseCacheButton = new System.Windows.Forms.Button();
            this.useTooltipsCheckBox = new System.Windows.Forms.CheckBox();
            this.useInsightCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.completeWhenTypingCheckBox = new System.Windows.Forms.CheckBox();
            this.useKeywordCompletionCheckBox = new System.Windows.Forms.CheckBox();
            this.dataUsageCacheLabel2 = new System.Windows.Forms.Label();
            this.dataUsageCacheItemCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dataUsageCacheLabel1 = new System.Windows.Forms.Label();
            this.useDataUsageCacheCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.codeCompletionEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataUsageCacheItemCountNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.refreshInsightOnCommaCheckBox);
            this.groupBox.Controls.Add(this.useDebugTooltipsOnlyCheckBox);
            this.groupBox.Controls.Add(this.clearDataUseCacheButton);
            this.groupBox.Controls.Add(this.useTooltipsCheckBox);
            this.groupBox.Controls.Add(this.useInsightCheckBox);
            this.groupBox.Controls.Add(this.label4);
            this.groupBox.Controls.Add(this.completeWhenTypingCheckBox);
            this.groupBox.Controls.Add(this.useKeywordCompletionCheckBox);
            this.groupBox.Controls.Add(this.dataUsageCacheLabel2);
            this.groupBox.Controls.Add(this.dataUsageCacheItemCountNumericUpDown);
            this.groupBox.Controls.Add(this.dataUsageCacheLabel1);
            this.groupBox.Controls.Add(this.useDataUsageCacheCheckBox);
            this.groupBox.Location = new System.Drawing.Point(8, 53);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(386, 352);
            this.groupBox.TabIndex = 5;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.DetailSettings}";
            // 
            // refreshInsightOnCommaCheckBox
            // 
            this.refreshInsightOnCommaCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshInsightOnCommaCheckBox.Location = new System.Drawing.Point(22, 242);
            this.refreshInsightOnCommaCheckBox.Name = "refreshInsightOnCommaCheckBox";
            this.refreshInsightOnCommaCheckBox.Size = new System.Drawing.Size(356, 20);
            this.refreshInsightOnCommaCheckBox.TabIndex = 11;
            this.refreshInsightOnCommaCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.RefreshInsightOnComma}";
            // 
            // useDebugTooltipsOnlyCheckBox
            // 
            this.useDebugTooltipsOnlyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useDebugTooltipsOnlyCheckBox.Location = new System.Drawing.Point(22, 95);
            this.useDebugTooltipsOnlyCheckBox.Name = "useDebugTooltipsOnlyCheckBox";
            this.useDebugTooltipsOnlyCheckBox.Size = new System.Drawing.Size(349, 21);
            this.useDebugTooltipsOnlyCheckBox.TabIndex = 6;
            this.useDebugTooltipsOnlyCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.UseDebugTooltipsOnly}";
            // 
            // clearDataUseCacheButton
            // 
            this.clearDataUseCacheButton.Location = new System.Drawing.Point(225, 40);
            this.clearDataUseCacheButton.Name = "clearDataUseCacheButton";
            this.clearDataUseCacheButton.Size = new System.Drawing.Size(101, 23);
            this.clearDataUseCacheButton.TabIndex = 4;
            this.clearDataUseCacheButton.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.ClearCache}";
            // 
            // useTooltipsCheckBox
            // 
            this.useTooltipsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useTooltipsCheckBox.Checked = true;
            this.useTooltipsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useTooltipsCheckBox.Location = new System.Drawing.Point(8, 72);
            this.useTooltipsCheckBox.Name = "useTooltipsCheckBox";
            this.useTooltipsCheckBox.Size = new System.Drawing.Size(368, 21);
            this.useTooltipsCheckBox.TabIndex = 5;
            this.useTooltipsCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.UseTooltips}";
            // 
            // useInsightCheckBox
            // 
            this.useInsightCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useInsightCheckBox.Checked = true;
            this.useInsightCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useInsightCheckBox.Location = new System.Drawing.Point(8, 218);
            this.useInsightCheckBox.Name = "useInsightCheckBox";
            this.useInsightCheckBox.Size = new System.Drawing.Size(368, 20);
            this.useInsightCheckBox.TabIndex = 10;
            this.useInsightCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.UseInsight}";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(8, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(370, 32);
            this.label4.TabIndex = 7;
            this.label4.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.LanguageDependend}";
            // 
            // completeWhenTypingCheckBox
            // 
            this.completeWhenTypingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.completeWhenTypingCheckBox.Location = new System.Drawing.Point(8, 174);
            this.completeWhenTypingCheckBox.Name = "completeWhenTypingCheckBox";
            this.completeWhenTypingCheckBox.Size = new System.Drawing.Size(368, 20);
            this.completeWhenTypingCheckBox.TabIndex = 8;
            this.completeWhenTypingCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.CompleteWhenTyping}";
            // 
            // useKeywordCompletionCheckBox
            // 
            this.useKeywordCompletionCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useKeywordCompletionCheckBox.Location = new System.Drawing.Point(8, 196);
            this.useKeywordCompletionCheckBox.Name = "useKeywordCompletionCheckBox";
            this.useKeywordCompletionCheckBox.Size = new System.Drawing.Size(368, 20);
            this.useKeywordCompletionCheckBox.TabIndex = 9;
            this.useKeywordCompletionCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.UseKeywordCompletion}";
            // 
            // dataUsageCacheLabel2
            // 
            this.dataUsageCacheLabel2.Location = new System.Drawing.Point(151, 42);
            this.dataUsageCacheLabel2.Name = "dataUsageCacheLabel2";
            this.dataUsageCacheLabel2.Size = new System.Drawing.Size(65, 20);
            this.dataUsageCacheLabel2.TabIndex = 3;
            this.dataUsageCacheLabel2.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.SaveItemCountAfterNumber}";
            this.dataUsageCacheLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataUsageCacheItemCountNumericUpDown
            // 
            this.dataUsageCacheItemCountNumericUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.dataUsageCacheItemCountNumericUpDown.Location = new System.Drawing.Point(80, 42);
            this.dataUsageCacheItemCountNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.dataUsageCacheItemCountNumericUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.dataUsageCacheItemCountNumericUpDown.Name = "dataUsageCacheItemCountNumericUpDown";
            this.dataUsageCacheItemCountNumericUpDown.Size = new System.Drawing.Size(68, 20);
            this.dataUsageCacheItemCountNumericUpDown.TabIndex = 2;
            this.dataUsageCacheItemCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.dataUsageCacheItemCountNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // dataUsageCacheLabel1
            // 
            this.dataUsageCacheLabel1.Location = new System.Drawing.Point(21, 42);
            this.dataUsageCacheLabel1.Name = "dataUsageCacheLabel1";
            this.dataUsageCacheLabel1.Size = new System.Drawing.Size(56, 20);
            this.dataUsageCacheLabel1.TabIndex = 1;
            this.dataUsageCacheLabel1.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.SaveItemCountBeforeNumber}";
            this.dataUsageCacheLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // useDataUsageCacheCheckBox
            // 
            this.useDataUsageCacheCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.useDataUsageCacheCheckBox.Checked = true;
            this.useDataUsageCacheCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useDataUsageCacheCheckBox.Location = new System.Drawing.Point(8, 20);
            this.useDataUsageCacheCheckBox.Name = "useDataUsageCacheCheckBox";
            this.useDataUsageCacheCheckBox.Size = new System.Drawing.Size(368, 20);
            this.useDataUsageCacheCheckBox.TabIndex = 0;
            this.useDataUsageCacheCheckBox.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.UseDataUsageCache}";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(386, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "${res:Dialog.Options.IDEOptions.CodeCompletion.MainOption}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // codeCompletionEnabledCheckBox
            // 
            this.codeCompletionEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.codeCompletionEnabledCheckBox.Location = new System.Drawing.Point(30, 31);
            this.codeCompletionEnabledCheckBox.Name = "codeCompletionEnabledCheckBox";
            this.codeCompletionEnabledCheckBox.Size = new System.Drawing.Size(357, 20);
            this.codeCompletionEnabledCheckBox.TabIndex = 4;
            this.codeCompletionEnabledCheckBox.Text = "${res:Dialog.Options.IDEOptions.TextEditor.General.CodeCompletionCheckBox}";
            // 
            // CodeCompletionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.codeCompletionEnabledCheckBox);
            this.Name = "CodeCompletionPanel";
            this.groupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataUsageCacheItemCountNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.CheckBox refreshInsightOnCommaCheckBox;
        private System.Windows.Forms.CheckBox useDebugTooltipsOnlyCheckBox;
        private System.Windows.Forms.Button clearDataUseCacheButton;
        private System.Windows.Forms.CheckBox useTooltipsCheckBox;
        private System.Windows.Forms.CheckBox useInsightCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox completeWhenTypingCheckBox;
        private System.Windows.Forms.CheckBox useKeywordCompletionCheckBox;
        private System.Windows.Forms.Label dataUsageCacheLabel2;
        private System.Windows.Forms.NumericUpDown dataUsageCacheItemCountNumericUpDown;
        private System.Windows.Forms.Label dataUsageCacheLabel1;
        private System.Windows.Forms.CheckBox useDataUsageCacheCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox codeCompletionEnabledCheckBox;
    }
}
