namespace ICSharpCode.TextEditor.Searching
{
    partial class FindPanel
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
            this.bookmarkAllButton = new System.Windows.Forms.Button();
            this.findNextButton = new System.Windows.Forms.Button();
            this.findAllButton = new System.Windows.Forms.Button();
            this.matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.fileTypesComboBox = new System.Windows.Forms.ComboBox();
            this.labelFileTypes = new System.Windows.Forms.Label();
            this.includeSubFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.lookInSelectButton = new System.Windows.Forms.Button();
            this.lookInComboBox = new System.Windows.Forms.ComboBox();
            this.labelLookIn = new System.Windows.Forms.Label();
            this.findComboBox = new System.Windows.Forms.ComboBox();
            this.labelFind = new System.Windows.Forms.Label();
            this.pathComboBox = new System.Windows.Forms.ComboBox();
            this.labelPath = new System.Windows.Forms.Label();
            this.labelSeparator = new System.Windows.Forms.Label();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.findRegexButton = new System.Windows.Forms.Button();
            this.pathBrowseButton = new System.Windows.Forms.Button();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // bookmarkAllButton
            // 
            this.bookmarkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookmarkAllButton.Location = new System.Drawing.Point(333, 234);
            this.bookmarkAllButton.Name = "bookmarkAllButton";
            this.bookmarkAllButton.Size = new System.Drawing.Size(96, 25);
            this.bookmarkAllButton.TabIndex = 10;
            this.bookmarkAllButton.Text = "${res:Dialog.NewProject.SearchReplace.MarkAllButton}";
            this.bookmarkAllButton.UseVisualStyleBackColor = true;
            this.bookmarkAllButton.Click += new System.EventHandler(this.BookmarkAllButtonClicked);
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(232, 234);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(96, 25);
            this.findNextButton.TabIndex = 9;
            this.findNextButton.Text = "${res:Dialog.NewProject.SearchReplace.FindButton}";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.FindNextButtonClicked);
            // 
            // findAllButton
            // 
            this.findAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findAllButton.Location = new System.Drawing.Point(131, 234);
            this.findAllButton.Name = "findAllButton";
            this.findAllButton.Size = new System.Drawing.Size(96, 25);
            this.findAllButton.TabIndex = 8;
            this.findAllButton.Text = "${res:Dialog.NewProject.SearchReplace.FindAll}";
            this.findAllButton.UseVisualStyleBackColor = true;
            this.findAllButton.Click += new System.EventHandler(this.FindAllButtonClicked);
            // 
            // matchWholeWordCheckBox
            // 
            this.matchWholeWordCheckBox.Location = new System.Drawing.Point(10, 40);
            this.matchWholeWordCheckBox.Name = "matchWholeWordCheckBox";
            this.matchWholeWordCheckBox.Size = new System.Drawing.Size(335, 20);
            this.matchWholeWordCheckBox.TabIndex = 1;
            this.matchWholeWordCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.MatchWholeWord}";
            this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.Location = new System.Drawing.Point(10, 16);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(335, 20);
            this.matchCaseCheckBox.TabIndex = 0;
            this.matchCaseCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.MatchCase}";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileTypesComboBox
            // 
            this.fileTypesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypesComboBox.FormattingEnabled = true;
            this.fileTypesComboBox.Location = new System.Drawing.Point(24, 87);
            this.fileTypesComboBox.Name = "fileTypesComboBox";
            this.fileTypesComboBox.Size = new System.Drawing.Size(395, 21);
            this.fileTypesComboBox.TabIndex = 2;
            this.fileTypesComboBox.SelectedIndexChanged += new System.EventHandler(this.OnFileTypesIndexChanged);
            this.fileTypesComboBox.Leave += new System.EventHandler(this.OnFileTypesLeave);
            // 
            // labelFileTypes
            // 
            this.labelFileTypes.Location = new System.Drawing.Point(10, 63);
            this.labelFileTypes.Name = "labelFileTypes";
            this.labelFileTypes.Size = new System.Drawing.Size(405, 20);
            this.labelFileTypes.TabIndex = 3;
            this.labelFileTypes.Text = "${res:Dialog.NewProject.SearchReplace.LookAtFileTypes}";
            this.labelFileTypes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // includeSubFolderCheckBox
            // 
            this.includeSubFolderCheckBox.Location = new System.Drawing.Point(92, 83);
            this.includeSubFolderCheckBox.Name = "includeSubFolderCheckBox";
            this.includeSubFolderCheckBox.Size = new System.Drawing.Size(337, 20);
            this.includeSubFolderCheckBox.TabIndex = 6;
            this.includeSubFolderCheckBox.Text = "${res:Dialog.NewProject.SearchReplace.IncludeSubFolders}";
            this.includeSubFolderCheckBox.UseVisualStyleBackColor = true;
            // 
            // lookInSelectButton
            // 
            this.lookInSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lookInSelectButton.Enabled = false;
            this.lookInSelectButton.Location = new System.Drawing.Point(404, 29);
            this.lookInSelectButton.Name = "lookInSelectButton";
            this.lookInSelectButton.Size = new System.Drawing.Size(25, 25);
            this.lookInSelectButton.TabIndex = 3;
            this.lookInSelectButton.Text = "...";
            this.lookInSelectButton.UseVisualStyleBackColor = true;
            this.lookInSelectButton.Click += new System.EventHandler(this.OnLookInButtonClicked);
            // 
            // lookInComboBox
            // 
            this.lookInComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lookInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lookInComboBox.FormattingEnabled = true;
            this.lookInComboBox.Location = new System.Drawing.Point(92, 31);
            this.lookInComboBox.Name = "lookInComboBox";
            this.lookInComboBox.Size = new System.Drawing.Size(308, 21);
            this.lookInComboBox.TabIndex = 2;
            this.lookInComboBox.SelectedIndexChanged += new System.EventHandler(this.LookInSelectedIndexChanged);
            // 
            // labelLookIn
            // 
            this.labelLookIn.Location = new System.Drawing.Point(3, 31);
            this.labelLookIn.Name = "labelLookIn";
            this.labelLookIn.Size = new System.Drawing.Size(75, 20);
            this.labelLookIn.TabIndex = 12;
            this.labelLookIn.Text = "${res:Dialog.NewProject.SearchReplace.SearchIn}";
            this.labelLookIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // findComboBox
            // 
            this.findComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findComboBox.FormattingEnabled = true;
            this.findComboBox.Location = new System.Drawing.Point(92, 3);
            this.findComboBox.Name = "findComboBox";
            this.findComboBox.Size = new System.Drawing.Size(308, 21);
            this.findComboBox.TabIndex = 0;
            this.findComboBox.TextChanged += new System.EventHandler(this.FindPatternChanged);
            // 
            // labelFind
            // 
            this.labelFind.Location = new System.Drawing.Point(3, 3);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(84, 20);
            this.labelFind.TabIndex = 11;
            this.labelFind.Text = "${res:Dialog.NewProject.SearchReplace.FindWhat}";
            this.labelFind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathComboBox
            // 
            this.pathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathComboBox.FormattingEnabled = true;
            this.pathComboBox.Location = new System.Drawing.Point(92, 58);
            this.pathComboBox.Name = "pathComboBox";
            this.pathComboBox.Size = new System.Drawing.Size(308, 21);
            this.pathComboBox.TabIndex = 4;
            this.pathComboBox.SelectedIndexChanged += new System.EventHandler(this.OnPathIndexChanged);
            this.pathComboBox.TextChanged += new System.EventHandler(this.OnPathTextChanged);
            // 
            // labelPath
            // 
            this.labelPath.Location = new System.Drawing.Point(3, 58);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(84, 20);
            this.labelPath.TabIndex = 13;
            this.labelPath.Text = "${res:Global.Path}";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSeparator
            // 
            this.labelSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSeparator.Location = new System.Drawing.Point(2, 229);
            this.labelSeparator.Name = "labelSeparator";
            this.labelSeparator.Size = new System.Drawing.Size(428, 2);
            this.labelSeparator.TabIndex = 14;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.labelFileTypes);
            this.optionsGroupBox.Controls.Add(this.fileTypesComboBox);
            this.optionsGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionsGroupBox.Controls.Add(this.matchWholeWordCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(3, 103);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(426, 122);
            this.optionsGroupBox.TabIndex = 7;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Find Options";
            // 
            // findRegexButton
            // 
            this.findRegexButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findRegexButton.Enabled = false;
            this.findRegexButton.Location = new System.Drawing.Point(404, 1);
            this.findRegexButton.Name = "findRegexButton";
            this.findRegexButton.Size = new System.Drawing.Size(25, 25);
            this.findRegexButton.TabIndex = 1;
            this.findRegexButton.Text = ">";
            this.findRegexButton.UseVisualStyleBackColor = true;
            this.findRegexButton.Click += new System.EventHandler(this.OnFindRegexButttonClicked);
            // 
            // pathBrowseButton
            // 
            this.pathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pathBrowseButton.Enabled = false;
            this.pathBrowseButton.Location = new System.Drawing.Point(404, 55);
            this.pathBrowseButton.Name = "pathBrowseButton";
            this.pathBrowseButton.Size = new System.Drawing.Size(25, 25);
            this.pathBrowseButton.TabIndex = 5;
            this.pathBrowseButton.Text = "...";
            this.pathBrowseButton.UseVisualStyleBackColor = true;
            this.pathBrowseButton.Click += new System.EventHandler(this.OnPathBrowseButtonClicked);
            // 
            // FindPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pathBrowseButton);
            this.Controls.Add(this.findRegexButton);
            this.Controls.Add(this.labelSeparator);
            this.Controls.Add(this.pathComboBox);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.bookmarkAllButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.findAllButton);
            this.Controls.Add(this.includeSubFolderCheckBox);
            this.Controls.Add(this.lookInSelectButton);
            this.Controls.Add(this.lookInComboBox);
            this.Controls.Add(this.labelLookIn);
            this.Controls.Add(this.findComboBox);
            this.Controls.Add(this.labelFind);
            this.Controls.Add(this.optionsGroupBox);
            this.Name = "FindPanel";
            this.Size = new System.Drawing.Size(432, 260);
            this.optionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bookmarkAllButton;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button findAllButton;
        private System.Windows.Forms.CheckBox matchWholeWordCheckBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.ComboBox fileTypesComboBox;
        private System.Windows.Forms.Label labelFileTypes;
        private System.Windows.Forms.CheckBox includeSubFolderCheckBox;
        private System.Windows.Forms.Button lookInSelectButton;
        private System.Windows.Forms.ComboBox lookInComboBox;
        private System.Windows.Forms.Label labelLookIn;
        private System.Windows.Forms.ComboBox findComboBox;
        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.ComboBox pathComboBox;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelSeparator;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Button findRegexButton;
        private System.Windows.Forms.Button pathBrowseButton;
    }
}
