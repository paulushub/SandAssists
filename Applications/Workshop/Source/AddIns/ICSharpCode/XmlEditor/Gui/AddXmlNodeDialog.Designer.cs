using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.XmlEditor
{
    partial class AddXmlNodeDialog
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Forms Designer generated code

        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.namesListBox = new System.Windows.Forms.ListBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.customNameTextBoxLabel = new System.Windows.Forms.Label();
            this.customNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // namesListBox
            // 
            this.namesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namesListBox.FormattingEnabled = true;
            this.namesListBox.Location = new System.Drawing.Point(8, 8);
            this.namesListBox.Name = "namesListBox";
            this.namesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.namesListBox.Size = new System.Drawing.Size(343, 199);
            this.namesListBox.Sorted = true;
            this.namesListBox.TabIndex = 1;
            this.namesListBox.SelectedIndexChanged += new System.EventHandler(this.NamesListBoxSelectedIndexChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // customNameTextBoxLabel
            // 
            this.customNameTextBoxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.customNameTextBoxLabel.Location = new System.Drawing.Point(31, 218);
            this.customNameTextBoxLabel.Name = "customNameTextBoxLabel";
            this.customNameTextBoxLabel.Size = new System.Drawing.Size(54, 20);
            this.customNameTextBoxLabel.TabIndex = 3;
            this.customNameTextBoxLabel.Text = "Custom:";
            this.customNameTextBoxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // customNameTextBox
            // 
            this.customNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.customNameTextBox.Location = new System.Drawing.Point(90, 217);
            this.customNameTextBox.Name = "customNameTextBox";
            this.customNameTextBox.Size = new System.Drawing.Size(221, 20);
            this.customNameTextBox.TabIndex = 4;
            this.customNameTextBox.TextChanged += new System.EventHandler(this.CustomNameTextBoxTextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(278, 246);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(197, 246);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // AddXmlNodeDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(359, 275);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.customNameTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.customNameTextBoxLabel);
            this.Controls.Add(this.namesListBox);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(289, 143);
            this.Name = "AddXmlNodeDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox customNameTextBox;
        private System.Windows.Forms.Label customNameTextBoxLabel;
        private System.Windows.Forms.ListBox namesListBox;
    }
}
