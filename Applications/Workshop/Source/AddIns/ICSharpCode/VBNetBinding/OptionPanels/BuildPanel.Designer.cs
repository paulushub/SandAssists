namespace VBNetBinding.OptionPanels
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
            this.groupVBOptions = new System.Windows.Forms.GroupBox();
            this.optionExplicitComboBox = new System.Windows.Forms.ComboBox();
            this.optionStrictComboBox = new System.Windows.Forms.ComboBox();
            this.optionCompareComboBox = new System.Windows.Forms.ComboBox();
            this.defineTraceCheckBox = new System.Windows.Forms.CheckBox();
            this.defineDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.targetCpuComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.conditionalSymbolsTextBox = new System.Windows.Forms.TextBox();
            this.optimizeCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.advancedSettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.generateSerializationAssemblyComboBox = new System.Windows.Forms.ComboBox();
            this.advancedOutputGroupBox = new System.Windows.Forms.GroupBox();
            this.xmlDocumentationCheckBox = new System.Windows.Forms.CheckBox();
            this.xmlDocumentationTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.registerCOMInteropCheckBox = new System.Windows.Forms.CheckBox();
            this.outputPathTextBox = new System.Windows.Forms.TextBox();
            this.outputPathBrowseButton = new System.Windows.Forms.Button();
            this.treatWarningsAsErrorsGroupBox = new System.Windows.Forms.GroupBox();
            this.specificWarningsTextBox = new System.Windows.Forms.TextBox();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.specificWarningsRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.warningsGroupBox = new System.Windows.Forms.GroupBox();
            this.suppressWarningsTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.warningLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.optionInferComboBox = new System.Windows.Forms.ComboBox();
            this.generalGroupBox.SuspendLayout();
            this.groupVBOptions.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.treatWarningsAsErrorsGroupBox.SuspendLayout();
            this.warningsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.groupVBOptions);
            this.generalGroupBox.Controls.Add(this.defineTraceCheckBox);
            this.generalGroupBox.Controls.Add(this.defineDebugCheckBox);
            this.generalGroupBox.Controls.Add(this.targetCpuComboBox);
            this.generalGroupBox.Controls.Add(this.label9);
            this.generalGroupBox.Controls.Add(this.label1);
            this.generalGroupBox.Controls.Add(this.conditionalSymbolsTextBox);
            this.generalGroupBox.Controls.Add(this.optimizeCodeCheckBox);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 9);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(444, 189);
            this.generalGroupBox.TabIndex = 4;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.General}";
            // 
            // groupVBOptions
            // 
            this.groupVBOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupVBOptions.Controls.Add(this.optionInferComboBox);
            this.groupVBOptions.Controls.Add(this.optionExplicitComboBox);
            this.groupVBOptions.Controls.Add(this.optionStrictComboBox);
            this.groupVBOptions.Controls.Add(this.optionCompareComboBox);
            this.groupVBOptions.Location = new System.Drawing.Point(8, 133);
            this.groupVBOptions.Name = "groupVBOptions";
            this.groupVBOptions.Size = new System.Drawing.Size(430, 46);
            this.groupVBOptions.TabIndex = 10;
            this.groupVBOptions.TabStop = false;
            this.groupVBOptions.Text = "${res:Dialog.ProjectOptions.BuildOptions.VBOptionsDefaultValues}";
            // 
            // optionExplicitComboBox
            // 
            this.optionExplicitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionExplicitComboBox.FormattingEnabled = true;
            this.optionExplicitComboBox.Location = new System.Drawing.Point(5, 17);
            this.optionExplicitComboBox.Name = "optionExplicitComboBox";
            this.optionExplicitComboBox.Size = new System.Drawing.Size(102, 21);
            this.optionExplicitComboBox.TabIndex = 5;
            // 
            // optionStrictComboBox
            // 
            this.optionStrictComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionStrictComboBox.FormattingEnabled = true;
            this.optionStrictComboBox.Location = new System.Drawing.Point(111, 17);
            this.optionStrictComboBox.Name = "optionStrictComboBox";
            this.optionStrictComboBox.Size = new System.Drawing.Size(102, 21);
            this.optionStrictComboBox.TabIndex = 6;
            // 
            // optionCompareComboBox
            // 
            this.optionCompareComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionCompareComboBox.FormattingEnabled = true;
            this.optionCompareComboBox.Location = new System.Drawing.Point(217, 17);
            this.optionCompareComboBox.Name = "optionCompareComboBox";
            this.optionCompareComboBox.Size = new System.Drawing.Size(102, 21);
            this.optionCompareComboBox.TabIndex = 7;
            // 
            // defineTraceCheckBox
            // 
            this.defineTraceCheckBox.Enabled = false;
            this.defineTraceCheckBox.Location = new System.Drawing.Point(8, 62);
            this.defineTraceCheckBox.Name = "defineTraceCheckBox";
            this.defineTraceCheckBox.Size = new System.Drawing.Size(430, 20);
            this.defineTraceCheckBox.TabIndex = 9;
            this.defineTraceCheckBox.Text = "Define TRACE constant";
            this.defineTraceCheckBox.UseVisualStyleBackColor = true;
            // 
            // defineDebugCheckBox
            // 
            this.defineDebugCheckBox.Enabled = false;
            this.defineDebugCheckBox.Location = new System.Drawing.Point(8, 41);
            this.defineDebugCheckBox.Name = "defineDebugCheckBox";
            this.defineDebugCheckBox.Size = new System.Drawing.Size(430, 20);
            this.defineDebugCheckBox.TabIndex = 8;
            this.defineDebugCheckBox.Text = "Define DEBUG constant";
            this.defineDebugCheckBox.UseVisualStyleBackColor = true;
            // 
            // targetCpuComboBox
            // 
            this.targetCpuComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetCpuComboBox.FormattingEnabled = true;
            this.targetCpuComboBox.Location = new System.Drawing.Point(195, 83);
            this.targetCpuComboBox.Name = "targetCpuComboBox";
            this.targetCpuComboBox.Size = new System.Drawing.Size(241, 21);
            this.targetCpuComboBox.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(181, 20);
            this.label9.TabIndex = 3;
            this.label9.Text = "${res:Dialog.ProjectOptions.Build.TargetCPU}";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "${res:Dialog.ProjectOptions.BuildOptions.ConditionalSymbols}";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // conditionalSymbolsTextBox
            // 
            this.conditionalSymbolsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionalSymbolsTextBox.Location = new System.Drawing.Point(195, 17);
            this.conditionalSymbolsTextBox.Name = "conditionalSymbolsTextBox";
            this.conditionalSymbolsTextBox.Size = new System.Drawing.Size(243, 20);
            this.conditionalSymbolsTextBox.TabIndex = 1;
            // 
            // optimizeCodeCheckBox
            // 
            this.optimizeCodeCheckBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.optimizeCodeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optimizeCodeCheckBox.Location = new System.Drawing.Point(8, 108);
            this.optimizeCodeCheckBox.Name = "optimizeCodeCheckBox";
            this.optimizeCodeCheckBox.Size = new System.Drawing.Size(425, 20);
            this.optimizeCodeCheckBox.TabIndex = 2;
            this.optimizeCodeCheckBox.Text = "${res:Dialog.ProjectOptions.BuildOptions.OptimizeCode}";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.advancedSettings);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.generateSerializationAssemblyComboBox);
            this.groupBox3.Controls.Add(this.advancedOutputGroupBox);
            this.groupBox3.Controls.Add(this.xmlDocumentationCheckBox);
            this.groupBox3.Controls.Add(this.xmlDocumentationTextBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.registerCOMInteropCheckBox);
            this.groupBox3.Controls.Add(this.outputPathTextBox);
            this.groupBox3.Controls.Add(this.outputPathBrowseButton);
            this.groupBox3.Location = new System.Drawing.Point(8, 395);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 158);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "${res:Dialog.ProjectOptions.Build.Output}";
            // 
            // advancedSettings
            // 
            this.advancedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedSettings.Location = new System.Drawing.Point(332, 128);
            this.advancedSettings.Name = "advancedSettings";
            this.advancedSettings.Size = new System.Drawing.Size(106, 25);
            this.advancedSettings.TabIndex = 14;
            this.advancedSettings.Text = "${res:Dialog.ProjectOptions.Build.Advanced}...";
            this.advancedSettings.UseVisualStyleBackColor = true;
            this.advancedSettings.Click += new System.EventHandler(this.OnAdvancedSettings);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(11, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "${res:Dialog.ProjectOptions.Build.GenerateSerializationAssembly}";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // generateSerializationAssemblyComboBox
            // 
            this.generateSerializationAssemblyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.generateSerializationAssemblyComboBox.FormattingEnabled = true;
            this.generateSerializationAssemblyComboBox.Location = new System.Drawing.Point(195, 101);
            this.generateSerializationAssemblyComboBox.Name = "generateSerializationAssemblyComboBox";
            this.generateSerializationAssemblyComboBox.Size = new System.Drawing.Size(207, 21);
            this.generateSerializationAssemblyComboBox.TabIndex = 2;
            // 
            // advancedOutputGroupBox
            // 
            this.advancedOutputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedOutputGroupBox.Location = new System.Drawing.Point(10, 120);
            this.advancedOutputGroupBox.Name = "advancedOutputGroupBox";
            this.advancedOutputGroupBox.Size = new System.Drawing.Size(428, 0);
            this.advancedOutputGroupBox.TabIndex = 9;
            this.advancedOutputGroupBox.TabStop = false;
            this.advancedOutputGroupBox.Text = "${res:Dialog.ProjectOptions.Build.Advanced}";
            // 
            // xmlDocumentationCheckBox
            // 
            this.xmlDocumentationCheckBox.Location = new System.Drawing.Point(13, 49);
            this.xmlDocumentationCheckBox.Name = "xmlDocumentationCheckBox";
            this.xmlDocumentationCheckBox.Size = new System.Drawing.Size(176, 20);
            this.xmlDocumentationCheckBox.TabIndex = 3;
            this.xmlDocumentationCheckBox.Text = "${res:Dialog.ProjectOptions.Build.XmlDocumentationFile}";
            // 
            // xmlDocumentationTextBox
            // 
            this.xmlDocumentationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlDocumentationTextBox.Location = new System.Drawing.Point(195, 48);
            this.xmlDocumentationTextBox.Name = "xmlDocumentationTextBox";
            this.xmlDocumentationTextBox.Size = new System.Drawing.Size(207, 20);
            this.xmlDocumentationTextBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(181, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "${res:Dialog.ProjectOptions.Build.OutputPath}";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // registerCOMInteropCheckBox
            // 
            this.registerCOMInteropCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.registerCOMInteropCheckBox.Location = new System.Drawing.Point(12, 76);
            this.registerCOMInteropCheckBox.Name = "registerCOMInteropCheckBox";
            this.registerCOMInteropCheckBox.Size = new System.Drawing.Size(426, 20);
            this.registerCOMInteropCheckBox.TabIndex = 0;
            this.registerCOMInteropCheckBox.Text = "${res:Dialog.ProjectOptions.Build.RegisterForCOM}";
            // 
            // outputPathTextBox
            // 
            this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathTextBox.Location = new System.Drawing.Point(195, 20);
            this.outputPathTextBox.Name = "outputPathTextBox";
            this.outputPathTextBox.Size = new System.Drawing.Size(207, 20);
            this.outputPathTextBox.TabIndex = 1;
            // 
            // outputPathBrowseButton
            // 
            this.outputPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathBrowseButton.Location = new System.Drawing.Point(406, 17);
            this.outputPathBrowseButton.Name = "outputPathBrowseButton";
            this.outputPathBrowseButton.Size = new System.Drawing.Size(30, 25);
            this.outputPathBrowseButton.TabIndex = 2;
            this.outputPathBrowseButton.Text = "...";
            this.outputPathBrowseButton.UseVisualStyleBackColor = true;
            // 
            // treatWarningsAsErrorsGroupBox
            // 
            this.treatWarningsAsErrorsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.specificWarningsTextBox);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.allRadioButton);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.specificWarningsRadioButton);
            this.treatWarningsAsErrorsGroupBox.Controls.Add(this.noneRadioButton);
            this.treatWarningsAsErrorsGroupBox.Location = new System.Drawing.Point(8, 289);
            this.treatWarningsAsErrorsGroupBox.Name = "treatWarningsAsErrorsGroupBox";
            this.treatWarningsAsErrorsGroupBox.Size = new System.Drawing.Size(444, 98);
            this.treatWarningsAsErrorsGroupBox.TabIndex = 7;
            this.treatWarningsAsErrorsGroupBox.TabStop = false;
            this.treatWarningsAsErrorsGroupBox.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors}";
            // 
            // specificWarningsTextBox
            // 
            this.specificWarningsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.specificWarningsTextBox.Location = new System.Drawing.Point(195, 48);
            this.specificWarningsTextBox.Name = "specificWarningsTextBox";
            this.specificWarningsTextBox.Size = new System.Drawing.Size(241, 20);
            this.specificWarningsTextBox.TabIndex = 2;
            // 
            // allRadioButton
            // 
            this.allRadioButton.Location = new System.Drawing.Point(8, 74);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(181, 20);
            this.allRadioButton.TabIndex = 3;
            this.allRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.All}";
            // 
            // specificWarningsRadioButton
            // 
            this.specificWarningsRadioButton.Location = new System.Drawing.Point(8, 48);
            this.specificWarningsRadioButton.Name = "specificWarningsRadioButton";
            this.specificWarningsRadioButton.Size = new System.Drawing.Size(181, 20);
            this.specificWarningsRadioButton.TabIndex = 1;
            this.specificWarningsRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.Specific}";
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.Location = new System.Drawing.Point(8, 22);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(181, 20);
            this.noneRadioButton.TabIndex = 0;
            this.noneRadioButton.Text = "${res:Dialog.ProjectOptions.Build.TreatWarningsAsErrors.None}";
            // 
            // warningsGroupBox
            // 
            this.warningsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.warningsGroupBox.Controls.Add(this.suppressWarningsTextBox);
            this.warningsGroupBox.Controls.Add(this.label4);
            this.warningsGroupBox.Controls.Add(this.warningLevelComboBox);
            this.warningsGroupBox.Controls.Add(this.label3);
            this.warningsGroupBox.Location = new System.Drawing.Point(8, 204);
            this.warningsGroupBox.Name = "warningsGroupBox";
            this.warningsGroupBox.Size = new System.Drawing.Size(444, 77);
            this.warningsGroupBox.TabIndex = 6;
            this.warningsGroupBox.TabStop = false;
            this.warningsGroupBox.Text = "${res:Dialog.ProjectOptions.Build.ErrorsAndWarnings}";
            // 
            // suppressWarningsTextBox
            // 
            this.suppressWarningsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressWarningsTextBox.Location = new System.Drawing.Point(195, 48);
            this.suppressWarningsTextBox.Name = "suppressWarningsTextBox";
            this.suppressWarningsTextBox.Size = new System.Drawing.Size(241, 20);
            this.suppressWarningsTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "${res:Dialog.ProjectOptions.Build.SuppressWarnings}";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // warningLevelComboBox
            // 
            this.warningLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.warningLevelComboBox.FormattingEnabled = true;
            this.warningLevelComboBox.Location = new System.Drawing.Point(195, 22);
            this.warningLevelComboBox.Name = "warningLevelComboBox";
            this.warningLevelComboBox.Size = new System.Drawing.Size(134, 21);
            this.warningLevelComboBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "${res:Dialog.ProjectOptions.Build.WarningLevel}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // optionInferComboBox
            // 
            this.optionInferComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionInferComboBox.FormattingEnabled = true;
            this.optionInferComboBox.Location = new System.Drawing.Point(323, 17);
            this.optionInferComboBox.Name = "optionInferComboBox";
            this.optionInferComboBox.Size = new System.Drawing.Size(102, 21);
            this.optionInferComboBox.TabIndex = 8;
            // 
            // BuildPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generalGroupBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.treatWarningsAsErrorsGroupBox);
            this.Controls.Add(this.warningsGroupBox);
            this.Name = "BuildPanel";
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.groupVBOptions.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.treatWarningsAsErrorsGroupBox.ResumeLayout(false);
            this.treatWarningsAsErrorsGroupBox.PerformLayout();
            this.warningsGroupBox.ResumeLayout(false);
            this.warningsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.ComboBox optionCompareComboBox;
        private System.Windows.Forms.ComboBox optionStrictComboBox;
        private System.Windows.Forms.ComboBox optionExplicitComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox conditionalSymbolsTextBox;
        private System.Windows.Forms.CheckBox optimizeCodeCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox generateSerializationAssemblyComboBox;
        private System.Windows.Forms.CheckBox registerCOMInteropCheckBox;
        private System.Windows.Forms.CheckBox xmlDocumentationCheckBox;
        private System.Windows.Forms.TextBox xmlDocumentationTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox outputPathTextBox;
        private System.Windows.Forms.Button outputPathBrowseButton;
        private System.Windows.Forms.GroupBox treatWarningsAsErrorsGroupBox;
        private System.Windows.Forms.TextBox specificWarningsTextBox;
        private System.Windows.Forms.RadioButton allRadioButton;
        private System.Windows.Forms.RadioButton specificWarningsRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.GroupBox warningsGroupBox;
        private System.Windows.Forms.TextBox suppressWarningsTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox warningLevelComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox targetCpuComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox advancedOutputGroupBox;
        private System.Windows.Forms.CheckBox defineTraceCheckBox;
        private System.Windows.Forms.CheckBox defineDebugCheckBox;
        private System.Windows.Forms.GroupBox groupVBOptions;
        private System.Windows.Forms.Button advancedSettings;
        private System.Windows.Forms.ComboBox optionInferComboBox;
    }
}
