namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class ProjectAndSolutionPanel
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.selectProjectLocationButton = new System.Windows.Forms.Button();
            this.projectLocationTextBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.loadPrevProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.CreatedObject27 = new System.Windows.Forms.GroupBox();
            this.onExecuteComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.parallelBuildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.showErrorListCheckBox = new System.Windows.Forms.CheckBox();
            this.CreatedObject2.SuspendLayout();
            this.CreatedObject27.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parallelBuildNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // CreatedObject2
            // 
            this.CreatedObject2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject2.Controls.Add(this.selectProjectLocationButton);
            this.CreatedObject2.Controls.Add(this.projectLocationTextBox);
            this.CreatedObject2.Controls.Add(this.label);
            this.CreatedObject2.Controls.Add(this.loadPrevProjectCheckBox);
            this.CreatedObject2.Location = new System.Drawing.Point(8, 7);
            this.CreatedObject2.Name = "CreatedObject2";
            this.CreatedObject2.Size = new System.Drawing.Size(386, 107);
            this.CreatedObject2.TabIndex = 2;
            this.CreatedObject2.TabStop = false;
            this.CreatedObject2.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.SettingsGroupBox}";
            // 
            // selectProjectLocationButton
            // 
            this.selectProjectLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectProjectLocationButton.Location = new System.Drawing.Point(303, 56);
            this.selectProjectLocationButton.Name = "selectProjectLocationButton";
            this.selectProjectLocationButton.Size = new System.Drawing.Size(75, 21);
            this.selectProjectLocationButton.TabIndex = 6;
            this.selectProjectLocationButton.Text = "${res:Global.BrowseButtonText}";
            // 
            // projectLocationTextBox
            // 
            this.projectLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectLocationTextBox.Location = new System.Drawing.Point(8, 34);
            this.projectLocationTextBox.Name = "projectLocationTextBox";
            this.projectLocationTextBox.Size = new System.Drawing.Size(370, 19);
            this.projectLocationTextBox.TabIndex = 1;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(8, 15);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(370, 17);
            this.label.TabIndex = 0;
            this.label.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.ProjectLocationLabel}";
            this.label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // loadPrevProjectCheckBox
            // 
            this.loadPrevProjectCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loadPrevProjectCheckBox.Location = new System.Drawing.Point(8, 83);
            this.loadPrevProjectCheckBox.Name = "loadPrevProjectCheckBox";
            this.loadPrevProjectCheckBox.Size = new System.Drawing.Size(370, 17);
            this.loadPrevProjectCheckBox.TabIndex = 8;
            this.loadPrevProjectCheckBox.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.LoadPrevProjectCheckBox" +
                "}";
            // 
            // CreatedObject27
            // 
            this.CreatedObject27.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatedObject27.Controls.Add(this.onExecuteComboBox);
            this.CreatedObject27.Controls.Add(this.label2);
            this.CreatedObject27.Controls.Add(this.parallelBuildNumericUpDown);
            this.CreatedObject27.Controls.Add(this.label1);
            this.CreatedObject27.Controls.Add(this.showErrorListCheckBox);
            this.CreatedObject27.Location = new System.Drawing.Point(8, 121);
            this.CreatedObject27.Name = "CreatedObject27";
            this.CreatedObject27.Size = new System.Drawing.Size(386, 122);
            this.CreatedObject27.TabIndex = 3;
            this.CreatedObject27.TabStop = false;
            this.CreatedObject27.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.BuildAndRunGroupBox}";
            // 
            // onExecuteComboBox
            // 
            this.onExecuteComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.onExecuteComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.onExecuteComboBox.FormattingEnabled = true;
            this.onExecuteComboBox.Location = new System.Drawing.Point(45, 93);
            this.onExecuteComboBox.Name = "onExecuteComboBox";
            this.onExecuteComboBox.Size = new System.Drawing.Size(319, 20);
            this.onExecuteComboBox.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(370, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // parallelBuildNumericUpDown
            // 
            this.parallelBuildNumericUpDown.Location = new System.Drawing.Point(45, 53);
            this.parallelBuildNumericUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.parallelBuildNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.parallelBuildNumericUpDown.Name = "parallelBuildNumericUpDown";
            this.parallelBuildNumericUpDown.Size = new System.Drawing.Size(71, 19);
            this.parallelBuildNumericUpDown.TabIndex = 13;
            this.parallelBuildNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(370, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.ParallelBuild}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // showErrorListCheckBox
            // 
            this.showErrorListCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.showErrorListCheckBox.Location = new System.Drawing.Point(8, 15);
            this.showErrorListCheckBox.Name = "showErrorListCheckBox";
            this.showErrorListCheckBox.Size = new System.Drawing.Size(370, 17);
            this.showErrorListCheckBox.TabIndex = 10;
            this.showErrorListCheckBox.Text = "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.ShowErrorListPadCheckBo" +
                "x}";
            // 
            // ProjectAndSolutionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CreatedObject2);
            this.Controls.Add(this.CreatedObject27);
            this.Name = "ProjectAndSolutionPanel";
            this.Size = new System.Drawing.Size(404, 351);
            this.CreatedObject2.ResumeLayout(false);
            this.CreatedObject2.PerformLayout();
            this.CreatedObject27.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parallelBuildNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CreatedObject2;
        private System.Windows.Forms.Button selectProjectLocationButton;
        private System.Windows.Forms.TextBox projectLocationTextBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.CheckBox loadPrevProjectCheckBox;
        private System.Windows.Forms.GroupBox CreatedObject27;
        private System.Windows.Forms.ComboBox onExecuteComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown parallelBuildNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox showErrorListCheckBox;
    }
}
