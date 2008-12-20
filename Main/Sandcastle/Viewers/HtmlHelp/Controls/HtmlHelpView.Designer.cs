namespace Sandcastle.Viewers.HtmlHelp.Controls
{
    partial class HtmlHelpView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabContents = new System.Windows.Forms.TabPage();
            this.tabIndex = new System.Windows.Forms.TabPage();
            this.tabSearch = new System.Windows.Forms.TabPage();
            this.helpTocTree = new Sandcastle.Viewers.HtmlHelp.Controls.HelpToc();
            this.helpIndex = new Sandcastle.Viewers.HtmlHelp.Controls.HelpIndex();
            this.helpSearch = new Sandcastle.Viewers.HtmlHelp.Controls.HelpSearch();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabContents.SuspendLayout();
            this.tabIndex.SuspendLayout();
            this.tabSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(778, 565);
            this.splitContainer1.SplitterDistance = 276;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabContents);
            this.tabControl.Controls.Add(this.tabIndex);
            this.tabControl.Controls.Add(this.tabSearch);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(276, 565);
            this.tabControl.TabIndex = 0;
            // 
            // tabContents
            // 
            this.tabContents.Controls.Add(this.helpTocTree);
            this.tabContents.Location = new System.Drawing.Point(4, 22);
            this.tabContents.Name = "tabContents";
            this.tabContents.Padding = new System.Windows.Forms.Padding(3);
            this.tabContents.Size = new System.Drawing.Size(268, 539);
            this.tabContents.TabIndex = 0;
            this.tabContents.Text = "Contents";
            this.tabContents.UseVisualStyleBackColor = true;
            // 
            // tabIndex
            // 
            this.tabIndex.Controls.Add(this.helpIndex);
            this.tabIndex.Location = new System.Drawing.Point(4, 22);
            this.tabIndex.Name = "tabIndex";
            this.tabIndex.Padding = new System.Windows.Forms.Padding(3);
            this.tabIndex.Size = new System.Drawing.Size(268, 539);
            this.tabIndex.TabIndex = 1;
            this.tabIndex.Text = "Index";
            this.tabIndex.UseVisualStyleBackColor = true;
            // 
            // tabSearch
            // 
            this.tabSearch.Controls.Add(this.helpSearch);
            this.tabSearch.Location = new System.Drawing.Point(4, 22);
            this.tabSearch.Name = "tabSearch";
            this.tabSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabSearch.Size = new System.Drawing.Size(268, 539);
            this.tabSearch.TabIndex = 2;
            this.tabSearch.Text = "Search";
            this.tabSearch.UseVisualStyleBackColor = true;
            // 
            // helpTocTree
            // 
            this.helpTocTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpTocTree.Location = new System.Drawing.Point(3, 3);
            this.helpTocTree.Name = "helpTocTree";
            this.helpTocTree.Padding = new System.Windows.Forms.Padding(2);
            this.helpTocTree.Size = new System.Drawing.Size(262, 533);
            this.helpTocTree.TabIndex = 0;
            // 
            // helpIndex
            // 
            this.helpIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpIndex.Location = new System.Drawing.Point(3, 3);
            this.helpIndex.Name = "helpIndex";
            this.helpIndex.Size = new System.Drawing.Size(262, 533);
            this.helpIndex.TabIndex = 0;
            // 
            // helpSearch
            // 
            this.helpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpSearch.Location = new System.Drawing.Point(3, 3);
            this.helpSearch.Name = "helpSearch";
            this.helpSearch.Size = new System.Drawing.Size(262, 533);
            this.helpSearch.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(498, 565);
            this.webBrowser.TabIndex = 0;
            // 
            // HtmlHelpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.splitContainer1);
            this.Name = "HtmlHelpView";
            this.Size = new System.Drawing.Size(778, 565);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabContents.ResumeLayout(false);
            this.tabIndex.ResumeLayout(false);
            this.tabSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabContents;
        private System.Windows.Forms.TabPage tabIndex;
        private System.Windows.Forms.TabPage tabSearch;
        private Sandcastle.Viewers.HtmlHelp.Controls.HelpToc helpTocTree;
        private Sandcastle.Viewers.HtmlHelp.Controls.HelpIndex helpIndex;
        private Sandcastle.Viewers.HtmlHelp.Controls.HelpSearch helpSearch;
        private System.Windows.Forms.WebBrowser webBrowser;
    }
}
