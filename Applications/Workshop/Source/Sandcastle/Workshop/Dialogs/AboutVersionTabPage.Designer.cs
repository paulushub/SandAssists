using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sandcastle.Workshop.Dialogs
{
	partial class AboutVersionTabPage
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
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.button = new System.Windows.Forms.Button();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "${res:Global.Name}";
            this.columnHeader.Width = 130;
            // 
            // button
            // 
            this.button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button.Location = new System.Drawing.Point(474, 220);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(81, 23);
            this.button.TabIndex = 1;
            this.button.Text = "${res:Dialog.About.VersionInfoTabName.CopyButton}";
            this.button.Click += new System.EventHandler(this.CopyButtonClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "${res:Dialog.About.VersionInfoTabName.VersionColumn}";
            this.columnHeader2.Width = 100;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader2,
            this.columnHeader3});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(10, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(545, 211);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "${res:Global.Path}";
            this.columnHeader3.Width = 301;
            // 
            // VersionInformationTabPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button);
            this.Controls.Add(this.listView);
            this.Name = "VersionInformationTabPage";
            this.Size = new System.Drawing.Size(568, 250);
            this.ResumeLayout(false);

        }

		#endregion

        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.ColumnHeader columnHeader;
    }
}
