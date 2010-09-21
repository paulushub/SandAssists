using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Sda
{
    partial class ExceptionBox
    {
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.closeButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.continueButton = new System.Windows.Forms.Button();
            this.reportButton = new System.Windows.Forms.Button();
            this.copyErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.exceptionTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(227, 424);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(209, 23);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ExitSharpDevelop}";
            this.closeButton.Click += new System.EventHandler(this.OnCloseApplicationClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(199, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(448, 23);
            this.label3.TabIndex = 9;
            this.label3.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ThankYouMsg}";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(199, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(448, 95);
            this.label2.TabIndex = 8;
            this.label2.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.HelpText2}";
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(199, 8);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(448, 48);
            this.label.TabIndex = 6;
            this.label.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.HelpText1}";
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.Location = new System.Drawing.Point(442, 424);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(209, 23);
            this.continueButton.TabIndex = 6;
            this.continueButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.Continue}";
            this.continueButton.Click += new System.EventHandler(this.OnContinueApplicationClick);
            // 
            // reportButton
            // 
            this.reportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.reportButton.Location = new System.Drawing.Point(12, 424);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(209, 23);
            this.reportButton.TabIndex = 4;
            this.reportButton.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.ReportError}";
            this.reportButton.Click += new System.EventHandler(this.OnReportApplicationClick);
            // 
            // copyErrorCheckBox
            // 
            this.copyErrorCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.copyErrorCheckBox.Checked = true;
            this.copyErrorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.copyErrorCheckBox.Location = new System.Drawing.Point(202, 390);
            this.copyErrorCheckBox.Name = "copyErrorCheckBox";
            this.copyErrorCheckBox.Size = new System.Drawing.Size(440, 24);
            this.copyErrorCheckBox.TabIndex = 2;
            this.copyErrorCheckBox.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.CopyToClipboard}";
            // 
            // exceptionTextBox
            // 
            this.exceptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.exceptionTextBox.Location = new System.Drawing.Point(12, 183);
            this.exceptionTextBox.Multiline = true;
            this.exceptionTextBox.Name = "exceptionTextBox";
            this.exceptionTextBox.ReadOnly = true;
            this.exceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.exceptionTextBox.Size = new System.Drawing.Size(639, 201);
            this.exceptionTextBox.TabIndex = 1;
            this.exceptionTextBox.Text = "textBoxExceptionText";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 8);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(164, 164);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // ExceptionBox
            // 
            this.ClientSize = new System.Drawing.Size(660, 453);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.copyErrorCheckBox);
            this.Controls.Add(this.exceptionTextBox);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.Title}";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox exceptionTextBox;
        private System.Windows.Forms.CheckBox copyErrorCheckBox;
        //private System.Windows.Forms.CheckBox includeSysInfoCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Button reportButton;
        private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Button closeButton;
    }
}
