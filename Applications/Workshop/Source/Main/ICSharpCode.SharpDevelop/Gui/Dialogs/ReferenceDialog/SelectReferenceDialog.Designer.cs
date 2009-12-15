using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class SelectReferenceDialog
	{
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #region Windows Forms Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.referenceTabControl = new System.Windows.Forms.TabControl();
            this.gacTabPage = new System.Windows.Forms.TabPage();
            this.projectTabPage = new System.Windows.Forms.TabPage();
            this.browserTabPage = new System.Windows.Forms.TabPage();
            this.comTabPage = new System.Windows.Forms.TabPage();
            this.referencesListView = new System.Windows.Forms.ListView();
            this.referenceHeader = new System.Windows.Forms.ColumnHeader();
            this.typeHeader = new System.Windows.Forms.ColumnHeader();
            this.locationHeader = new System.Windows.Forms.ColumnHeader();
            this.selectButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.referencesLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.referenceTabControl.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // referenceTabControl
            // 
            this.referenceTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.referenceTabControl.Controls.Add(this.gacTabPage);
            this.referenceTabControl.Controls.Add(this.projectTabPage);
            this.referenceTabControl.Controls.Add(this.browserTabPage);
            this.referenceTabControl.Controls.Add(this.comTabPage);
            this.referenceTabControl.Location = new System.Drawing.Point(8, 8);
            this.referenceTabControl.Name = "referenceTabControl";
            this.referenceTabControl.SelectedIndex = 0;
            this.referenceTabControl.Size = new System.Drawing.Size(607, 290);
            this.referenceTabControl.TabIndex = 0;
            // 
            // gacTabPage
            // 
            this.gacTabPage.Location = new System.Drawing.Point(4, 21);
            this.gacTabPage.Name = "gacTabPage";
            this.gacTabPage.Size = new System.Drawing.Size(599, 265);
            this.gacTabPage.TabIndex = 0;
            this.gacTabPage.Text = "${res:Dialog.SelectReferenceDialog.GacTabPage}";
            this.gacTabPage.UseVisualStyleBackColor = true;
            // 
            // projectTabPage
            // 
            this.projectTabPage.Location = new System.Drawing.Point(4, 21);
            this.projectTabPage.Name = "projectTabPage";
            this.projectTabPage.Size = new System.Drawing.Size(599, 245);
            this.projectTabPage.TabIndex = 1;
            this.projectTabPage.Text = "${res:Dialog.SelectReferenceDialog.ProjectTabPage}";
            this.projectTabPage.UseVisualStyleBackColor = true;
            // 
            // browserTabPage
            // 
            this.browserTabPage.Location = new System.Drawing.Point(4, 21);
            this.browserTabPage.Name = "browserTabPage";
            this.browserTabPage.Size = new System.Drawing.Size(599, 245);
            this.browserTabPage.TabIndex = 2;
            this.browserTabPage.Text = "${res:Dialog.SelectReferenceDialog.BrowserTabPage}";
            this.browserTabPage.UseVisualStyleBackColor = true;
            // 
            // comTabPage
            // 
            this.comTabPage.Location = new System.Drawing.Point(4, 21);
            this.comTabPage.Name = "comTabPage";
            this.comTabPage.Size = new System.Drawing.Size(599, 245);
            this.comTabPage.TabIndex = 2;
            this.comTabPage.Text = "COM";
            this.comTabPage.UseVisualStyleBackColor = true;
            // 
            // referencesListView
            // 
            this.referencesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.referenceHeader,
            this.typeHeader,
            this.locationHeader});
            this.referencesListView.FullRowSelect = true;
            this.referencesListView.Location = new System.Drawing.Point(10, 18);
            this.referencesListView.Name = "referencesListView";
            this.referencesListView.Size = new System.Drawing.Size(507, 97);
            this.referencesListView.TabIndex = 3;
            this.referencesListView.UseCompatibleStateImageBehavior = false;
            this.referencesListView.View = System.Windows.Forms.View.Details;
            // 
            // referenceHeader
            // 
            this.referenceHeader.Text = "${res:Dialog.SelectReferenceDialog.ReferenceHeader}";
            this.referenceHeader.Width = 183;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "${res:Dialog.SelectReferenceDialog.TypeHeader}";
            this.typeHeader.Width = 57;
            // 
            // locationHeader
            // 
            this.locationHeader.Text = "${res:Dialog.SelectReferenceDialog.LocationHeader}";
            this.locationHeader.Width = 228;
            // 
            // selectButton
            // 
            this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectButton.Location = new System.Drawing.Point(523, 18);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 23);
            this.selectButton.TabIndex = 1;
            this.selectButton.Text = "${res:Dialog.SelectReferenceDialog.SelectButton}";
            this.selectButton.Click += new System.EventHandler(this.SelectReference);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(523, 47);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "${res:Global.RemoveButtonText}";
            this.removeButton.Click += new System.EventHandler(this.RemoveReference);
            // 
            // referencesLabel
            // 
            this.referencesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.referencesLabel.Location = new System.Drawing.Point(98, 431);
            this.referencesLabel.Name = "referencesLabel";
            this.referencesLabel.Size = new System.Drawing.Size(286, 20);
            this.referencesLabel.TabIndex = 2;
            this.referencesLabel.Text = "${res:Dialog.SelectReferenceDialog.ReferencesLabel}";
            this.referencesLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.referencesLabel.Visible = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(459, 432);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "${res:Global.OKButtonText}";
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(540, 432);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "${res:Global.CancelButtonText}";
            // 
            // helpButton
            // 
            this.helpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpButton.Location = new System.Drawing.Point(8, 432);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 7;
            this.helpButton.Text = "${res:Global.HelpButtonText}";
            this.helpButton.Visible = false;
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.referencesListView);
            this.groupBox.Controls.Add(this.selectButton);
            this.groupBox.Controls.Add(this.removeButton);
            this.groupBox.Location = new System.Drawing.Point(8, 304);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(607, 122);
            this.groupBox.TabIndex = 8;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "${res:Dialog.SelectReferenceDialog.ReferencesLabel}";
            // 
            // SelectReferenceDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(624, 461);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.referencesLabel);
            this.Controls.Add(this.referenceTabControl);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 350);
            this.Name = "SelectReferenceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.SelectReferenceDialog.DialogName}";
            this.referenceTabControl.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView referencesListView;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.TabPage gacTabPage;
        private System.Windows.Forms.TabPage projectTabPage;
        private System.Windows.Forms.TabPage browserTabPage;
        private System.Windows.Forms.TabPage comTabPage;
        private System.Windows.Forms.Label referencesLabel;
        private System.Windows.Forms.ColumnHeader referenceHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.ColumnHeader locationHeader;
        private System.Windows.Forms.TabControl referenceTabControl;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.GroupBox groupBox;
    }
}
