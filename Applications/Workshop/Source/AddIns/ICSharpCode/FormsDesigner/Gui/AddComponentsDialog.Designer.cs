namespace ICSharpCode.FormsDesigner.Gui
{
    partial class AddComponentsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.componentListView = new System.Windows.Forms.ListView();
            this.componentColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.componentColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.componentColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.componentColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.splitter = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.gacTabPage = new System.Windows.Forms.TabPage();
            this.gacListView = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.customTabPage = new System.Windows.Forms.TabPage();
            this.browseButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.loadButton = new System.Windows.Forms.Button();
            this.panel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.gacTabPage.SuspendLayout();
            this.customTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(598, 363);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(518, 363);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "${res:Global.OKButtonText}";
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.componentListView);
            this.panel.Controls.Add(this.label2);
            this.panel.Controls.Add(this.splitter);
            this.panel.Controls.Add(this.panel2);
            this.panel.Location = new System.Drawing.Point(9, 8);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(664, 347);
            this.panel.TabIndex = 3;
            // 
            // componentListView
            // 
            this.componentListView.CheckBoxes = true;
            this.componentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.componentColumnHeader1,
            this.componentColumnHeader2,
            this.componentColumnHeader3,
            this.componentColumnHeader4});
            this.componentListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentListView.FullRowSelect = true;
            this.componentListView.GridLines = true;
            this.componentListView.HideSelection = false;
            this.componentListView.Location = new System.Drawing.Point(264, 18);
            this.componentListView.Name = "componentListView";
            this.componentListView.Size = new System.Drawing.Size(400, 329);
            this.componentListView.TabIndex = 3;
            this.componentListView.UseCompatibleStateImageBehavior = false;
            this.componentListView.View = System.Windows.Forms.View.Details;
            // 
            // componentColumnHeader1
            // 
            this.componentColumnHeader1.Name = "componentColumnHeader1";
            this.componentColumnHeader1.Text = "${res:Global.Name}";
            this.componentColumnHeader1.Width = 128;
            // 
            // componentColumnHeader2
            // 
            this.componentColumnHeader2.Name = "componentColumnHeader2";
            this.componentColumnHeader2.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.Components.N" +
                "amespace}";
            this.componentColumnHeader2.Width = 88;
            // 
            // componentColumnHeader3
            // 
            this.componentColumnHeader3.Name = "componentColumnHeader3";
            this.componentColumnHeader3.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.Components.A" +
                "ssembly}";
            // 
            // componentColumnHeader4
            // 
            this.componentColumnHeader4.Name = "componentColumnHeader4";
            this.componentColumnHeader4.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.Components.L" +
                "ocation}";
            this.componentColumnHeader4.Width = 113;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(264, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(400, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.ComponentsTo" +
                "AddLabel}";
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(261, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 347);
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(261, 347);
            this.panel2.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.gacTabPage);
            this.tabControl.Controls.Add(this.customTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(261, 347);
            this.tabControl.TabIndex = 1;
            // 
            // gacTabPage
            // 
            this.gacTabPage.Controls.Add(this.gacListView);
            this.gacTabPage.Location = new System.Drawing.Point(4, 21);
            this.gacTabPage.Name = "gacTabPage";
            this.gacTabPage.Size = new System.Drawing.Size(253, 322);
            this.gacTabPage.TabIndex = 0;
            this.gacTabPage.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.GACTabPage}";
            this.gacTabPage.UseVisualStyleBackColor = true;
            // 
            // gacListView
            // 
            this.gacListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.gacListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gacListView.FullRowSelect = true;
            this.gacListView.HideSelection = false;
            this.gacListView.Location = new System.Drawing.Point(0, 0);
            this.gacListView.MultiSelect = false;
            this.gacListView.Name = "gacListView";
            this.gacListView.Size = new System.Drawing.Size(253, 322);
            this.gacListView.TabIndex = 0;
            this.gacListView.UseCompatibleStateImageBehavior = false;
            this.gacListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Name = "columnHeader5";
            this.columnHeader5.Text = "${res:Global.Name}";
            this.columnHeader5.Width = 147;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Name = "columnHeader6";
            this.columnHeader6.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.GAC.Version}" +
                "";
            this.columnHeader6.Width = 80;
            // 
            // customTabPage
            // 
            this.customTabPage.Controls.Add(this.browseButton);
            this.customTabPage.Controls.Add(this.fileNameTextBox);
            this.customTabPage.Controls.Add(this.label3);
            this.customTabPage.Controls.Add(this.loadButton);
            this.customTabPage.Location = new System.Drawing.Point(4, 21);
            this.customTabPage.Name = "customTabPage";
            this.customTabPage.Size = new System.Drawing.Size(253, 322);
            this.customTabPage.TabIndex = 1;
            this.customTabPage.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.CustomTabPag" +
                "e}";
            this.customTabPage.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(213, 52);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(32, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "...";
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTextBox.Location = new System.Drawing.Point(8, 28);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(237, 19);
            this.fileNameTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(8, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponentsFileNameLabel" +
                "}";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(8, 78);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(150, 23);
            this.loadButton.TabIndex = 3;
            this.loadButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ShowComponentsButton}";
            // 
            // AddComponentsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 394);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.panel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddComponentsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.DialogName}";
            this.panel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.gacTabPage.ResumeLayout(false);
            this.customTabPage.ResumeLayout(false);
            this.customTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ListView componentListView;
        private System.Windows.Forms.ColumnHeader componentColumnHeader1;
        private System.Windows.Forms.ColumnHeader componentColumnHeader2;
        private System.Windows.Forms.ColumnHeader componentColumnHeader3;
        private System.Windows.Forms.ColumnHeader componentColumnHeader4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage gacTabPage;
        private System.Windows.Forms.ListView gacListView;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TabPage customTabPage;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadButton;
    }
}