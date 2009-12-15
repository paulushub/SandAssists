namespace ICSharpCode.TextEditor.Searching
{
    partial class ReplacePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelFind = new System.Windows.Forms.Label();
            this.findComboBox = new System.Windows.Forms.ComboBox();
            this.labelReplace = new System.Windows.Forms.Label();
            this.replaceComboBox = new System.Windows.Forms.ComboBox();
            this.labelLookIn = new System.Windows.Forms.Label();
            this.lookInComboBox = new System.Windows.Forms.ComboBox();
            this.lookInBrowseButton = new System.Windows.Forms.Button();
            this.includeSubFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.labelFileTypes = new System.Windows.Forms.Label();
            this.fileTypesComboBox = new System.Windows.Forms.ComboBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
            this.findNextButton = new System.Windows.Forms.Button();
            this.replaceButton = new System.Windows.Forms.Button();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.pathComboBox = new System.Windows.Forms.ComboBox();
            this.labelSeparator = new System.Windows.Forms.Label();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFind
            // 
            this.labelFind.Location = new System.Drawing.Point(3, 3);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(84, 20);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "${res:Dialog.NewProject.SearchReplace.FindWhat}";
            this.labelFind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // findComboBox
            // 
            this.findComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findComboBox.FormattingEnabled = true;
            this.findComboBox.Location = new System.Drawing.Point(92, 3);
            this.findComboBox.Name = "findComboBox";
            this.findComboBox.Size = new System.Drawing.Size(338, 21);
            this.findComboBox.TabIndex = 1;
            this.findComboBox.TextChanged += new System.EventHandler(this.FindPatternChanged);
            // 
            // labelReplace
            // 
            this.labelReplace.Location = new System.Drawing.Point(3, 29);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(84, 20);
            this.labelReplace.TabIndex = 2;
            this.labelReplace.Text = "${res:Dialog.NewProject.SearchReplace.ReplaceWith}";
            this.labelReplace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // replaceComboBox
            // 
            this.replaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceComboBox.FormattingEnabled = true;
            this.replaceComboBox.Location = new System.Drawing.Point(92, 29);
            this.replaceComboBox.Name = "replaceComboBox";
            this.replaceComboBox.Size = new System.Drawing.Size(338, 21);
            this.replaceComboBox.TabIndex = 3;
            // 
            // labelLookIn
            // 
            this.labelLookIn.Location = new System.Drawing.Point(3, 56);
            this.labelLookIn.Name = "labelLookIn";
            this.labelLookIn.Size = new System.Drawing.Size(84, 20);
            this.labelLookIn.TabIndex = 4;
            this.labelLookIn.Text = "${res:Dialog.NewProject.SearchReplace.SearchIn}";
            this.labelLookIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lookInComboBox
            // 
            this.lookInComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lookInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lookInComboBox.FormattingEnabled = true;
            this.lookInComboBox.Location = new System.Drawing.Point(92, 56);
            this.lookInComboBox.Name = "lookInComboBox";
            this.lookInComboBox.Size = new System.Drawing.Size(338, 21);
            this.lookInComboBox.TabIndex = 5;
            this.lookInComboBox.SelectedIndexChanged += new System.EventHandler(this.LookInSelectedIndexChanged);
            // 
            // lookInBrowseButton
            // 
            this.lookInBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lookInBrowseButton.Location = new System.Drawing.Point(409, 81);
            this.lookInBrowseButton.Name = "lookInBrowseButton";
            this.lookInBrowseButton.Size = new System.Drawing.Size(21, 25);
            this.lookInBrowseButton.TabIndex = 6;
            this.lookInBrowseButton.Text = "...";
            this.lookInBrowseButton.UseVisualStyleBackColor = true;
            this.lookInBrowseButton.Click += new System.EventHandler(this.LookInBrowseButtonClicked);
            // 
            // includeSubFolderCheckBox
            // 
            this.includeSubFolderCheckBox.Location = new System.Drawing.Point(92, 108);
            this.includeSubFolderCheckBox.Name = "includeSubFolderCheckBox";
            this.includeSubFolderCheckBox.Size = new System.Drawing.Size(331, 20);
            this.includeSubFolderCheckBox.TabIndex = 7;
            this.includeSubFolderCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.IncludeSubFolders}";
            this.includeSubFolderCheckBox.UseVisualStyleBackColor = true;
            // 
            // labelFileTypes
            // 
            this.labelFileTypes.Location = new System.Drawing.Point(10, 51);
            this.labelFileTypes.Name = "labelFileTypes";
            this.labelFileTypes.Size = new System.Drawing.Size(400, 18);
            this.labelFileTypes.TabIndex = 8;
            this.labelFileTypes.Text = "${res:Dialog.NewProject.SearchReplace.LookAtFileTypes}";
            this.labelFileTypes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // fileTypesComboBox
            // 
            this.fileTypesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypesComboBox.FormattingEnabled = true;
            this.fileTypesComboBox.Location = new System.Drawing.Point(24, 73);
            this.fileTypesComboBox.Name = "fileTypesComboBox";
            this.fileTypesComboBox.Size = new System.Drawing.Size(395, 21);
            this.fileTypesComboBox.TabIndex = 9;
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.Location = new System.Drawing.Point(10, 16);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(337, 17);
            this.matchCaseCheckBox.TabIndex = 10;
            this.matchCaseCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.MatchCase}";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // matchWholeWordCheckBox
            // 
            this.matchWholeWordCheckBox.Location = new System.Drawing.Point(10, 35);
            this.matchWholeWordCheckBox.Name = "matchWholeWordCheckBox";
            this.matchWholeWordCheckBox.Size = new System.Drawing.Size(337, 17);
            this.matchWholeWordCheckBox.TabIndex = 11;
            this.matchWholeWordCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.MatchWholeWord}";
            this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(132, 234);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(96, 25);
            this.findNextButton.TabIndex = 12;
            this.findNextButton.Text = "${res:Dialog.NewProject.SearchReplace.FindNextButton}";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.FindNextButtonClicked);
            // 
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.Location = new System.Drawing.Point(233, 234);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(96, 25);
            this.replaceButton.TabIndex = 13;
            this.replaceButton.Text = "${res:Dialog.NewProject.SearchReplace.ReplaceButton}";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButtonClicked);
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceAllButton.Location = new System.Drawing.Point(333, 234);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(96, 25);
            this.replaceAllButton.TabIndex = 14;
            this.replaceAllButton.Text = "${res:Dialog.NewProject.SearchReplace.ReplaceAllButton}";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new System.EventHandler(this.ReplaceAllButtonClicked);
            // 
            // labelPath
            // 
            this.labelPath.Location = new System.Drawing.Point(3, 85);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(84, 20);
            this.labelPath.TabIndex = 15;
            this.labelPath.Text = "${res:Dialog.NewProject.SearchReplace.Path}";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathComboBox
            // 
            this.pathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathComboBox.FormattingEnabled = true;
            this.pathComboBox.Location = new System.Drawing.Point(92, 83);
            this.pathComboBox.Name = "pathComboBox";
            this.pathComboBox.Size = new System.Drawing.Size(312, 21);
            this.pathComboBox.TabIndex = 16;
            // 
            // labelSeparator
            // 
            this.labelSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSeparator.Location = new System.Drawing.Point(2, 229);
            this.labelSeparator.Name = "labelSeparator";
            this.labelSeparator.Size = new System.Drawing.Size(428, 2);
            this.labelSeparator.TabIndex = 17;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.matchWholeWordCheckBox);
            this.optionsGroupBox.Controls.Add(this.labelFileTypes);
            this.optionsGroupBox.Controls.Add(this.fileTypesComboBox);
            this.optionsGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(3, 126);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(426, 100);
            this.optionsGroupBox.TabIndex = 18;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Find Options";
            // 
            // ReplacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelSeparator);
            this.Controls.Add(this.pathComboBox);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.replaceAllButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.includeSubFolderCheckBox);
            this.Controls.Add(this.lookInBrowseButton);
            this.Controls.Add(this.lookInComboBox);
            this.Controls.Add(this.labelLookIn);
            this.Controls.Add(this.replaceComboBox);
            this.Controls.Add(this.labelReplace);
            this.Controls.Add(this.findComboBox);
            this.Controls.Add(this.labelFind);
            this.Controls.Add(this.optionsGroupBox);
            this.Name = "ReplacePanel";
            this.Size = new System.Drawing.Size(432, 260);
            this.optionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.ComboBox findComboBox;
        private System.Windows.Forms.Label labelReplace;
        private System.Windows.Forms.ComboBox replaceComboBox;
        private System.Windows.Forms.Label labelLookIn;
        private System.Windows.Forms.ComboBox lookInComboBox;
        private System.Windows.Forms.Button lookInBrowseButton;
        private System.Windows.Forms.CheckBox includeSubFolderCheckBox;
        private System.Windows.Forms.Label labelFileTypes;
        private System.Windows.Forms.ComboBox fileTypesComboBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.CheckBox matchWholeWordCheckBox;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.ComboBox pathComboBox;
        private System.Windows.Forms.Label labelSeparator;
        private System.Windows.Forms.GroupBox optionsGroupBox;
    }
}
