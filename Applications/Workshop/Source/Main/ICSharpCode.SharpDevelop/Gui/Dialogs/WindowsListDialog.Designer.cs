namespace ICSharpCode.SharpDevelop.Gui
{
    partial class WindowsListDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.windowList = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPath = new System.Windows.Forms.ColumnHeader();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCloseWindows = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // windowList
            // 
            this.windowList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.windowList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnPath});
            this.windowList.FullRowSelect = true;
            this.windowList.HideSelection = false;
            this.windowList.Location = new System.Drawing.Point(8, 8);
            this.windowList.Margin = new System.Windows.Forms.Padding(6);
            this.windowList.Name = "windowList";
            this.windowList.Size = new System.Drawing.Size(555, 263);
            this.windowList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.windowList.TabIndex = 0;
            this.windowList.UseCompatibleStateImageBehavior = false;
            this.windowList.View = System.Windows.Forms.View.Details;
            this.windowList.SelectedIndexChanged += new System.EventHandler(this.OnWindowsListChanged);
            this.windowList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnWindowsListColumnClicked);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            // 
            // columnPath
            // 
            this.columnPath.Text = "Path";
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnActivate.Location = new System.Drawing.Point(8, 280);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(120, 23);
            this.btnActivate.TabIndex = 1;
            this.btnActivate.Text = "&Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.OnActivateWindow);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(134, 280);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.OnSaveWindow);
            // 
            // btnCloseWindows
            // 
            this.btnCloseWindows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCloseWindows.Location = new System.Drawing.Point(260, 280);
            this.btnCloseWindows.Name = "btnCloseWindows";
            this.btnCloseWindows.Size = new System.Drawing.Size(120, 23);
            this.btnCloseWindows.TabIndex = 3;
            this.btnCloseWindows.Text = "&Close Window(s)";
            this.btnCloseWindows.UseVisualStyleBackColor = true;
            this.btnCloseWindows.Click += new System.EventHandler(this.OnCloseWindow);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(443, 280);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(120, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnActivateWindow);
            // 
            // WindowsListDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 306);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCloseWindows);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.windowList);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(460, 230);
            this.Name = "WindowsListDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Windows";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView windowList;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCloseWindows;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPath;
        private System.Windows.Forms.Button btnOK;
    }
}