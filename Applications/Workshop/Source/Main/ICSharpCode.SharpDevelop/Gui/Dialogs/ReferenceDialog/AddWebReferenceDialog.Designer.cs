using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class AddWebReferenceDialog
	{
        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.urlComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.goButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.webBrowserTabPage = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.webServicesTabPage = new System.Windows.Forms.TabPage();
            this.webServicesView = new ICSharpCode.SharpDevelop.Gui.WebServicesView();
            this.referenceNameLabel = new System.Windows.Forms.Label();
            this.referenceNameTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.namespaceTextBox = new System.Windows.Forms.TextBox();
            this.namespaceLabel = new System.Windows.Forms.Label();
            this.toolStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.webBrowserTabPage.SuspendLayout();
            this.webServicesTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backButton,
            this.forwardButton,
            this.refreshButton,
            this.stopButton,
            this.urlComboBox,
            this.goButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(630, 25);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip";
            this.toolStrip.Leave += new System.EventHandler(this.ToolStripLeave);
            this.toolStrip.Enter += new System.EventHandler(this.ToolStripEnter);
            this.toolStrip.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ToolStripPreviewKeyDown);
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backButton.Enabled = false;
            this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 22);
            this.backButton.Text = "Back";
            this.backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(23, 22);
            this.forwardButton.Text = "forward";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Enabled = false;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.ToolTipText = "Stop";
            this.stopButton.Click += new System.EventHandler(this.StopButtonClick);
            // 
            // urlComboBox
            // 
            this.urlComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.urlComboBox.AutoSize = false;
            this.urlComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.urlComboBox.Name = "urlComboBox";
            this.urlComboBox.Size = new System.Drawing.Size(361, 20);
            this.urlComboBox.SelectedIndexChanged += new System.EventHandler(this.UrlComboBoxSelectedIndexChanged);
            this.urlComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UrlComboBoxKeyDown);
            // 
            // goButton
            // 
            this.goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(23, 22);
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.GoButtonClick);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.webBrowserTabPage);
            this.tabControl.Controls.Add(this.webServicesTabPage);
            this.tabControl.Location = new System.Drawing.Point(6, 31);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(618, 297);
            this.tabControl.TabIndex = 1;
            // 
            // webBrowserTabPage
            // 
            this.webBrowserTabPage.Controls.Add(this.webBrowser);
            this.webBrowserTabPage.Location = new System.Drawing.Point(4, 21);
            this.webBrowserTabPage.Name = "webBrowserTabPage";
            this.webBrowserTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.webBrowserTabPage.Size = new System.Drawing.Size(610, 272);
            this.webBrowserTabPage.TabIndex = 0;
            this.webBrowserTabPage.Text = "WSDL";
            this.webBrowserTabPage.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 18);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(604, 266);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TabStop = false;
            this.webBrowser.CanGoForwardChanged += new System.EventHandler(this.WebBrowserCanGoForwardChanged);
            this.webBrowser.CanGoBackChanged += new System.EventHandler(this.WebBrowserCanGoBackChanged);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
            this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
            // 
            // webServicesTabPage
            // 
            this.webServicesTabPage.Controls.Add(this.webServicesView);
            this.webServicesTabPage.Location = new System.Drawing.Point(4, 21);
            this.webServicesTabPage.Name = "webServicesTabPage";
            this.webServicesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.webServicesTabPage.Size = new System.Drawing.Size(495, 197);
            this.webServicesTabPage.TabIndex = 1;
            this.webServicesTabPage.Text = "Available Web Services";
            this.webServicesTabPage.UseVisualStyleBackColor = true;
            // 
            // webServicesView
            // 
            this.webServicesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webServicesView.Location = new System.Drawing.Point(3, 3);
            this.webServicesView.Name = "webServicesView";
            this.webServicesView.Size = new System.Drawing.Size(489, 191);
            this.webServicesView.TabIndex = 0;
            // 
            // referenceNameLabel
            // 
            this.referenceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.referenceNameLabel.Location = new System.Drawing.Point(9, 333);
            this.referenceNameLabel.Name = "referenceNameLabel";
            this.referenceNameLabel.Size = new System.Drawing.Size(112, 18);
            this.referenceNameLabel.TabIndex = 2;
            this.referenceNameLabel.Text = "&Reference Name:";
            this.referenceNameLabel.UseCompatibleTextRendering = true;
            // 
            // referenceNameTextBox
            // 
            this.referenceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.referenceNameTextBox.Location = new System.Drawing.Point(127, 334);
            this.referenceNameTextBox.Name = "referenceNameTextBox";
            this.referenceNameTextBox.Size = new System.Drawing.Size(420, 19);
            this.referenceNameTextBox.TabIndex = 4;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(553, 334);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(73, 19);
            this.addButton.TabIndex = 6;
            this.addButton.Text = "&Add";
            this.addButton.UseCompatibleTextRendering = true;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(553, 355);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(73, 19);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // namespaceTextBox
            // 
            this.namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namespaceTextBox.Location = new System.Drawing.Point(127, 355);
            this.namespaceTextBox.Name = "namespaceTextBox";
            this.namespaceTextBox.Size = new System.Drawing.Size(420, 19);
            this.namespaceTextBox.TabIndex = 5;
            // 
            // namespaceLabel
            // 
            this.namespaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.namespaceLabel.Location = new System.Drawing.Point(9, 354);
            this.namespaceLabel.Name = "namespaceLabel";
            this.namespaceLabel.Size = new System.Drawing.Size(112, 18);
            this.namespaceLabel.TabIndex = 3;
            this.namespaceLabel.Text = "&Namespace:";
            this.namespaceLabel.UseCompatibleTextRendering = true;
            // 
            // AddWebReferenceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(630, 385);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.namespaceTextBox);
            this.Controls.Add(this.namespaceLabel);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.referenceNameTextBox);
            this.Controls.Add(this.referenceNameLabel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 187);
            this.Name = "AddWebReferenceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Web Reference";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddWebReferenceDialogFormClosing);
            this.Resize += new System.EventHandler(this.AddWebReferenceDialogResize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.webBrowserTabPage.ResumeLayout(false);
            this.webServicesTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label namespaceLabel;
        private System.Windows.Forms.TextBox namespaceTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox referenceNameTextBox;
        private System.Windows.Forms.Label referenceNameLabel;
        private System.Windows.Forms.TabPage webBrowserTabPage;
        private System.Windows.Forms.TabPage webServicesTabPage;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ToolStripButton goButton;
        private System.Windows.Forms.ToolStripComboBox urlComboBox;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton backButton;
        private ICSharpCode.SharpDevelop.Gui.WebServicesView webServicesView;
    }
}
