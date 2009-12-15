using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sandcastle.Workshop.Dialogs
{
	partial class AboutGeneralTabPage
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
			if (disposing && (components != null))
			{
				components.Dispose();
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
            this.buildLabel = new System.Windows.Forms.Label();
            this.buildTextBox = new System.Windows.Forms.TextBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.sponsorLabel = new System.Windows.Forms.Label();
            this.versionInfoTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buildLabel
            // 
            this.buildLabel.Location = new System.Drawing.Point(288, 10);
            this.buildLabel.Name = "buildLabel";
            this.buildLabel.Size = new System.Drawing.Size(81, 17);
            this.buildLabel.TabIndex = 2;
            this.buildLabel.Text = "${res:Dialog.About.label2Text}";
            this.buildLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buildTextBox
            // 
            this.buildTextBox.Location = new System.Drawing.Point(374, 10);
            this.buildTextBox.Name = "buildTextBox";
            this.buildTextBox.ReadOnly = true;
            this.buildTextBox.Size = new System.Drawing.Size(81, 20);
            this.buildTextBox.TabIndex = 3;
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(107, 10);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(85, 17);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "${res:Dialog.About.label1Text}";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // versionTextBox
            // 
            this.versionTextBox.Location = new System.Drawing.Point(195, 10);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.ReadOnly = true;
            this.versionTextBox.Size = new System.Drawing.Size(81, 20);
            this.versionTextBox.TabIndex = 4;
            // 
            // sponsorLabel
            // 
            this.sponsorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sponsorLabel.Location = new System.Drawing.Point(16, 240);
            this.sponsorLabel.Name = "sponsorLabel";
            this.sponsorLabel.Size = new System.Drawing.Size(538, 22);
            this.sponsorLabel.TabIndex = 8;
            this.sponsorLabel.Text = "License";
            this.sponsorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // versionInfoTextBox
            // 
            this.versionInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionInfoTextBox.Location = new System.Drawing.Point(16, 36);
            this.versionInfoTextBox.Multiline = true;
            this.versionInfoTextBox.Name = "versionInfoTextBox";
            this.versionInfoTextBox.ReadOnly = true;
            this.versionInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.versionInfoTextBox.Size = new System.Drawing.Size(538, 197);
            this.versionInfoTextBox.TabIndex = 9;
            this.versionInfoTextBox.WordWrap = false;
            // 
            // AboutGeneralTabPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.versionTextBox);
            this.Controls.Add(this.buildLabel);
            this.Controls.Add(this.buildTextBox);
            this.Controls.Add(this.sponsorLabel);
            this.Controls.Add(this.versionInfoTextBox);
            this.Name = "AboutGeneralTabPage";
            this.Size = new System.Drawing.Size(568, 271);
            this.Load += new System.EventHandler(this.OnLoadControl);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion

        private System.Windows.Forms.Label buildLabel;
        private System.Windows.Forms.TextBox buildTextBox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TextBox versionTextBox;
        private System.Windows.Forms.Label sponsorLabel;
        private System.Windows.Forms.TextBox versionInfoTextBox;
	}
}
