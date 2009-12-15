namespace CSharpBinding.OptionPanels
{
    partial class BuildPanel
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
            this.defineTraceCheckBox = new System.Windows.Forms.CheckBox();
            this.defineDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.conditionalSymbolsLabel = new System.Windows.Forms.Label();
            this.targetCpuComboBox = new System.Windows.Forms.ComboBox();
            this.conditionalSymbolsTextBox = new System.Windows.Forms.TextBox();
            this.optimizeCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.targetCpuLabel = new System.Windows.Forms.Label();
            this.allowUnsafeCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.advancedSettings = new System.Windows.Forms.Button();
            this.generateSerializationAssemblyLabel = new System.Windows.Forms.Label();
            this.outputPathLabel = new System.Windows.Forms.Label();
            this.generateSerializationAssemblyComboBox = new System.Windows.Forms.ComboBox();
            this.outputPathTextBox = new System.Windows.Forms.TextBox();
            this.registerCOMInteropCheckBox = new System.Windows.Forms.CheckBox();
            this.outputPathBrowseButton = new System.Windows.Forms.Button();
            this.xmlDocumentationCheckBox = new System.Windows.Forms.CheckBox();
            this.xmlDocumentationTextBox = new System.Windows.Forms.TextBox();
            this.warningsGroupBox = new System.Windows.Forms.GroupBox();
            this.warningLevelLabel = new System.Windows.Forms.Label();
            this.warningLevelComboBox = new System.Windows.Forms.ComboBox();
            this.suppressWarningsLabel = new System.Windows.Forms.Label();
            this.suppressWarningsTextBox = new System.Windows.Forms.TextBox();
            this.treatWarningsAsErrorsGroupBox = new System.Windows.Forms.GroupBox();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.specificWarningsRadioButton = new System.Windows.Forms.RadioButton();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.specificWarningsTextBox = new System.Windows.Forms.TextBox();
            this.generalGroupBox.SuspendLayout();
            this.outputGroupBox.SuspendLayout();
            this.warningsGroupBox.SuspendLayout();
            this.treatWarningsAsErrorsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.defineTraceCheckBox);
            this.generalGroupBox.Controls.Add(this.defineDebugCheckBox);
            this.generalGroupBox.Controls.Add(this.conditionalSymbolsLabel);
            this.generalGroupBox.Controls.Add(this.targetCpuComboBox);
            this.generalGroupBox.Controls.Add(this.conditionalSymbolsTextBox);
            this.generalGroupBox.Controls.Add(this.optimizeCodeCheckBox);
            this.generalGroupBox.Controls.Add(this.targetCpuLabel);
            this.generalGroupBox.Controls.Add(this.allowUnsafeCodeCheckBox);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 9);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(444, 163);
            this.generalGroupBox.TabIndex = 4;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.General}";
            this.generalGroupBox.UseCompatibleTextRendering = true;
            // 
            // defineTraceCheckBox
            // 
            this.defineTraceCheckBox.Enabled = false;
            this.defineTraceCheckBox.Location = new System.Drawing.Point(8, 66);
            this.defineTraceCheckBox.Name = "defineTraceCheckBox";
            this.defineTraceCheckBox.Size = new System.Drawing.Size(430, 20);
            this.defineTraceCheckBox.TabIndex = 6;
            this.defineTraceCheckBox.Text = "Define TRACE constant";
            this.defineTraceCheckBox.UseVisualStyleBackColor = true;
            // 
            // defineDebugCheckBox
            // 
            this.defineDebugCheckBox.Enabled = false;
            this.defineDebugCheckBox.Location = new System.Drawing.Point(8, 42);
            this.defineDebugCheckBox.Name = "defineDebugCheckBox";
            this.defineDebugCheckBox.Size = new System.Drawing.Size(430, 20);
            this.defineDebugCheckBox.TabIndex = 5;
            this.defineDebugCheckBox.Text = "Define DEBUG constant";
            this.defineDebugCheckBox.UseVisualStyleBackColor = true;
            // 
            // conditionalSymbolsLabel
            // 
            this.conditionalSymbolsLabel.Location = new System.Drawing.Point(8, 17);
            this.conditionalSymbolsLabel.Name = "conditionalSymbolsLabel";
            this.conditionalSymbolsLabel.Size = new System.Drawing.Size(192, 20);
            this.conditionalSymbolsLabel.TabIndex = 0;
            this.conditionalSymbolsLabel.Text = "${res:Dialog.ProjectOptions.BuildOptions.ConditionalSymbols}";
            this.conditionalSymbolsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.conditionalSymbolsLabel.UseCompatibleTextRendering = true;
            // 
            // targetCpuComboBox
            // 
            this.targetCpuComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetCpuComboBox.FormattingEnabled = true;
            this.targetCpuComboBox.Location = new System.Drawing.Point(210, 90);
            this.targetCpuComboBox.Name = "targetCpuComboBox";
            this.targetCpuComboBox.Size = new System.Drawing.Size(226, 21);
            this.targetCpuComboBox.TabIndex = 4;
            // 
            // conditionalSymbolsTextBox
            // 
            this.conditionalSymbolsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionalSymbolsTextBox.Location = new System.Drawing.Point(210, 17);
            this.conditionalSymbolsTextBox.Name = "conditionalSymbolsTextBox";
            this.conditionalSymbolsTextBox.Size = new System.Drawing.Size(228, 20);
            this.conditionalSymbolsTextBox.TabIndex = 1;
            // 
            // optimizeCodeCheckBox
            // 
            this.optimizeCodeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optimizeCodeCheckBox.Location = new System.Drawing.Point(8, 115);
            this.optimizeCodeCheckBox.Name = "optimizeCodeCheckBox";
            this.optimizeCodeCheckBox.Size = new System.Drawing.Size(430, 20);
            this.optimizeCodeCheckBox.TabIndex = 2;
            this.optimizeCodeCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.OptimizeCode}";
            this.optimizeCodeCheckBox.UseCompatibleTextRendering = true;
            // 
            // targetCpuLabel
            // 
            this.targetCpuLabel.Location = new System.Drawing.Point(8, 90);
            this.targetCpuLabel.Name = "targetCpuLabel";
            this.targetCpuLabel.Size = new System.Drawing.Size(192, 20);
            this.targetCpuLabel.TabIndex = 3;
            this.targetCpuLabel.Text = "${res:Dialog.ProjectOptions.Build.TargetCPU}";
            this.targetCpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.targetCpuLabel.UseCompatibleTextRendering = true;
            // 
            // allowUnsafeCodeCheckBox
            // 
            this.allowUnsafeCodeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.allowUnsafeCodeCheckBox.Location = new System.Drawing.Point(8, 137);
            this.allowUnsafeCodeCheckBox.Name = "allowUnsafeCodeCheckBox";
            this.allowUnsafeCodeCheckBox.Size = new System.Drawing.Size(430, 20);
            this.allowUnsafeCodeCheckBox.TabIndex = 3;
            this.allowUnsafeCodeCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.AllowUnsafeCode}";
            this.allowUnsafeCodeCheckBox.UseCompatibleTextRendering = true;
            // 
            // outputGroupBox
            // 
            this.outputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputGroupBox.Controls.Add(this.advancedSettings);
            this.outputGroupBox.Controls.Add(this.generateSerializationAssemblyLabel);
            this.outputGroupBox.Controls.Add(this.outputPathLabel);
            this.outputGroupBox.Controls.Add(this.generateSerializationAssemblyComboBox);
            this.outputGroupBox.Controls.Add(this.outputPathTextBox);
            this.outputGroupBox.Controls.Add(this.registerCOMInteropCheckBox);
            this.outputGroupBox.Controls.Add(this.outputPathBrowseButton);
            this.outputGroupBox.Controls.Add(this.xmlDocumentationCheckBox);
            this.outputGroupBox.Controls.Add(this.xmlDocumentationTextBox);
            this.outputGroupBox.Location = new System.Drawing.Point(8, 351);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(444, 155);
            this.outputGroupBox.TabIndex = 5;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "${res:Dialog.ProjectOptions.Build.Output}";
            this.outputGroupBox.UseCompatibleTextRendering = true;
            // 
            // advancedSettings
            // 
            this.advancedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedSettings.Location = new System.Drawing.Point(330, 122);
            this.advancedSettings.Name = "advancedSettings";
            this.advancedSettings.Size = new System.Drawing.Size(106, 25);
            this.advancedSettings.TabIndex = 13;
            this.advancedSettings.Text = "${res:Dialog.ProjectOptions.Build.Advanced}...";
            this.advancedSettings.UseVisualStyleBackColor = true;
            this.advancedSettings.Click += new System.EventHandler(this.OnAdvancedSettings);
            // 
            // generateSerializationAssemblyLabel
            // 
            this.generateSerializationAssemblyLabel.Location = new System.Drawing.Point(8, 94);
            this.generateSerializationAssemblyLabel.Name = "generateSerializationAssemblyLabel";
            this.generateSerializationAssemblyLabel.Size = new System.Drawing.Size(196, 20);
            this.generateSerializationAssemblyLabel.TabIndex = 1;
            this.generateSerializationAssemblyLabel.Text = "${res:Dialog.ProjectOptions.Build.GenerateSerializationAssembly}";
            this.generateSerializationAssemblyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.generateSerializationAssemblyLabel.UseCompatibleTextRendering = true;
            // 
            // outputPathLabel
            // 
            this.outputPathLabel.Location = new System.Drawing.Point(8, 17);
            this.outputPathLabel.Name = "outputPathLabel";
            this.outputPathLabel.Size = new System.Drawing.Size(201, 20);
            this.outputPathLabel.TabIndex = 3;
            this.outputPathLabel.Text = "${res:Dialog.ProjectOptions.Build.OutputPath}";
            this.outputPathLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.outputPathLabel.UseCompatibleTextRendering = true;
            // 
            // generateSerializationAssemblyComboBox
            // 
            this.generateSerializationAssemblyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.generateSerializationAssemblyComboBox.FormattingEnabled = true;
            this.generateSerializationAssemblyComboBox.Location = new System.Drawing.Point(210, 94);
            this.generateSerializationAssemblyComboBox.Name = "generateSerializationAssemblyComboBox";
            this.generateSerializationAssemblyComboBox.Size = new System.Drawing.Size(190, 21);
            this.generateSerializationAssemblyComboBox.TabIndex = 2;
            // 
            // outputPathTextBox
            // 
            this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathTextBox.Location = new System.Drawing.Point(210, 17);
            this.outputPathTextBox.Name = "outputPathTextBox";
            this.outputPathTextBox.Size = new System.Drawing.Size(190, 20);
            this.outputPathTextBox.TabIndex = 4;
            // 
            // registerCOMInteropCheckBox
            // 
            this.registerCOMInteropCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.registerCOMInteropCheckBox.Location = new System.Drawing.Point(8, 69);
            this.registerCOMInteropCheckBox.Name = "registerCOMInteropCheckBox";
            this.registerCOMInteropCheckBox.Size = new System.Drawing.Size(430, 20);
            this.registerCOMInteropCheckBox.TabIndex = 0;
            this.registerCOMInteropCheckBox.Text = "${res:Dialog.ProjectOptions.Build.RegisterForCOM}";
            this.registerCOMInteropCheckBox.UseCompatibleTextRendering = true;
            // 
            // outputPathBrowseButton
            // 
            this.outputPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathBrowseButton.Location = new System.Drawing.Point(406, 16);
            this.outputPathBrowseButton.Name = "outputPathBrowseButton";
            this.outputPathBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.outputPathBrowseButton.TabIndex = 5;
            this.outputPathBrowseButton.Text = "...";
            this.outputPathBrowseButton.UseCompatibleTextRendering = true;
            this.outputPathBrowseButton.UseVisualStyleBackColor = true;
            // 
            // xmlDocumentationCheckBox
            // 
            this.xmlDocumentationCheckBox.Location = new System.Drawing.Point(8, 43);
            this.xmlDocumentationCheckBox.Name = "xmlDocumentationCheckBox";
            this.xmlDocumentationCheckBox.Size = new System.Drawing.Size(154, 20);
            this.xmlDocumentationCheckBox.TabIndex = 6;
            this.xmlDocumentationCheckBox.Text = "${res:Dialog.ProjectOptions.Build.XmlDocumentationFile}";
            this.xmlDocumentationCheckBox.UseCompatibleTextRendering = true;
            // 
            // xmlDocumentationTextBox
            // 
            this.xmlDocumentationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlDocumentationTextBox.Location = new System.Drawing.Point(210, 43);
            this.xmlDocumentationTextBox.Name = "xmlDocumentationTextBox";
            this.xmlDocumentationTextBox.Size = new System.Drawing.Size(190, 20);
            this.xmlDocumentationTextBox.TabIndex = 7;
            // 
            // warningsGroupBox
            // 
            this.warningsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.warningsGroupBox.Controls.Add(this.warningLevelLabel);
            this.warningsGroupBox.Controls.Add(this.warningLevelComboBox);
            this.warningsGroupBox.Controls.Add(this.suppressWarningsLabel);
            this.warningsGroupBox.Controls.Add(this.suppressWarningsTextBox);
            this.warningsGroupBox.Location = new System.Drawing.Point(8, 178);
            this.warningsGroupBox.Name = "warningsGroupBox";
            this.warningsGroupBox.Size = new System.Drawing.Size(444, 72);
            this.warningsGroupBox.TabIndex = 6;
            this.warningsGroupBox.TabStop = false;
            this.warningsGroupBox.Text = "${res:Dialog.ProjectOptions.Build.ErrorsAndWarnings}";
            this.warningsGroupBox.UseCompatibleTextRendering = true;
            // 
            // warningLevelLabel
            // 
            this.warningLevelLabel.Location = new System.Drawing.Point(8, 17);
            this.warningLevelLabel.Name = "warningLevelLabel";
            this.warningLevelLabel.Size = new System.Drawing.Size(201, 20);
            this.warningLevelLabel.TabIndex = 0;
            this.warningLevelLabel.Text = "${res:Dialog.ProjectOptions.Build.WarningLevel}";
            this.warningLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningLevelLabel.UseCompatibleTextRendering = true;
            // 
            // warningLevelComboBox
            // 
            this.warningLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.warningLevelComboBox.FormattingEnabled = true;
            this.warningLevelComboBox.Location = new System.Drawing.Point(210, 17);
            this.warningLevelComboBox.Name = "warningLevelComboBox";
            this.warningLevelComboBox.Size = new System.Drawing.Size(109, 21);
            this.warningLevelComboBox.TabIndex = 1;
            // 
            // suppressWarningsLabel
            // 
            this.suppressWarningsLabel.Location = new System.Drawing.Point(8, 43);
            this.suppressWarningsLabel.Name = "suppressWarningsLabel";
            this.suppressWarningsLabel.Size = new System.Drawing.Size(201, 20);
            this.suppressWarningsLabel.TabIndex = 2;
            this.suppressWarningsLabel.Text = "${res:Dialog.ProjectOptions.Build.SuppressWarnings}";
            this.suppressWarningsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.suppressWarningsLabel.UseCompatibleTextRendering = true;
            // 
            // suppressWarningsTextBox
            // 
            this.suppressWarningsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressWarningsTextBox.Location = new System.Drawing.Point(210, 43);
            this.suppressWarningsTextBox.Name = "suppressWarningsTextBox";
            this.suppressWarningsTextBox.Size = new System.Drawing.Size(227, 20);
            this.suppressWarningsTextBox.TabIndex = 3;
            // 
            // treatWarningsAsErrorsGroupBox
            // 
            this.treatWarningsAsErrorsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.noneRadioButton);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.specificWarningsRadioButton);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.allRadioButton);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.specificWarningsTextBox);
            this.treatWarningsAsErrorsGroupBox.Location = new System.Drawing.Point(8, 256);
            this.treatWarningsAsErrorsGroupBox.Name = "treatWarningsAsErrorsGroupBox";
            this.treatWarningsAsErrorsGroupBox.Size = new System.Drawing.Size(444, 89);
            this.treatWarningsAsErrorsGroupBox.TabIndex = 7;
            this.treatWarningsAsErrorsGroupBox.TabStop = false;
            this.treatWarningsAsErrorsGroupBox.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors}";
            this.treatWarningsAsErrorsGroupBox.UseCompatibleTextRendering = true;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.Location = new System.Drawing.Point(6, 17);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(203, 20);
            this.noneRadioButton.TabIndex = 0;
            this.noneRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.None}";
            this.noneRadioButton.UseCompatibleTextRendering = true;
            // 
            // specificWarningsRadioButton
            // 
            this.specificWarningsRadioButton.Location = new System.Drawing.Point(6, 41);
            this.specificWarningsRadioButton.Name = "specificWarningsRadioButton";
            this.specificWarningsRadioButton.Size = new System.Drawing.Size(203, 20);
            this.specificWarningsRadioButton.TabIndex = 1;
            this.specificWarningsRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.Specific}";
            this.specificWarningsRadioButton.UseCompatibleTextRendering = true;
            // 
            // allRadioButton
            // 
            this.allRadioButton.Location = new System.Drawing.Point(6, 63);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(203, 20);
            this.allRadioButton.TabIndex = 3;
            this.allRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.All}";
            this.allRadioButton.UseCompatibleTextRendering = true;
            // 
            // specificWarningsTextBox
            // 
            this.specificWarningsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.specificWarningsTextBox.Location = new System.Drawing.Point(210, 41);
            this.specificWarningsTextBox.Name = "specificWarningsTextBox";
            this.specificWarningsTextBox.Size = new System.Drawing.Size(226, 20);
            this.specificWarningsTextBox.TabIndex = 2;
            // 
            // BuildPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generalGroupBox);
            this.Controls.Add(this.outputGroupBox);
            this.Controls.Add(this.warningsGroupBox);
            this.Controls.Add(this.treatWarningsAsErrorsGroupBox);
            this.Name = "BuildPanel";
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.outputGroupBox.ResumeLayout(false);
            this.outputGroupBox.PerformLayout();
            this.warningsGroupBox.ResumeLayout(false);
            this.warningsGroupBox.PerformLayout();
            this.treatWarningsAsErrorsGroupBox.ResumeLayout(false);
            this.treatWarningsAsErrorsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.Label conditionalSymbolsLabel;
        private System.Windows.Forms.TextBox conditionalSymbolsTextBox;
        private System.Windows.Forms.CheckBox optimizeCodeCheckBox;
        private System.Windows.Forms.CheckBox allowUnsafeCodeCheckBox;
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.Label outputPathLabel;
        private System.Windows.Forms.TextBox outputPathTextBox;
        private System.Windows.Forms.Button outputPathBrowseButton;
        private System.Windows.Forms.CheckBox xmlDocumentationCheckBox;
        private System.Windows.Forms.TextBox xmlDocumentationTextBox;
        private System.Windows.Forms.Label generateSerializationAssemblyLabel;
        private System.Windows.Forms.CheckBox registerCOMInteropCheckBox;
        private System.Windows.Forms.ComboBox targetCpuComboBox;
        private System.Windows.Forms.Label targetCpuLabel;
        private System.Windows.Forms.ComboBox generateSerializationAssemblyComboBox;
        private System.Windows.Forms.GroupBox warningsGroupBox;
        private System.Windows.Forms.Label warningLevelLabel;
        private System.Windows.Forms.ComboBox warningLevelComboBox;
        private System.Windows.Forms.Label suppressWarningsLabel;
        private System.Windows.Forms.TextBox suppressWarningsTextBox;
        private System.Windows.Forms.GroupBox treatWarningsAsErrorsGroupBox;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton specificWarningsRadioButton;
        private System.Windows.Forms.RadioButton allRadioButton;
        private System.Windows.Forms.TextBox specificWarningsTextBox;
        private System.Windows.Forms.CheckBox defineTraceCheckBox;
        private System.Windows.Forms.CheckBox defineDebugCheckBox;
        private System.Windows.Forms.Button advancedSettings;
    }
}
