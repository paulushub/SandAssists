using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
    partial class WebServicesView
    {
        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.webServicesTreeView = new System.Windows.Forms.TreeView();
            this.webServicesListView = new System.Windows.Forms.ListView();
            this.propertyColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.webServicesTreeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webServicesListView);
            this.splitContainer.Size = new System.Drawing.Size(471, 305);
            this.splitContainer.SplitterDistance = 156;
            this.splitContainer.TabIndex = 1;
            // 
            // webServicesTreeView
            // 
            this.webServicesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webServicesTreeView.Location = new System.Drawing.Point(0, 0);
            this.webServicesTreeView.Name = "webServicesTreeView";
            this.webServicesTreeView.Size = new System.Drawing.Size(156, 305);
            this.webServicesTreeView.TabIndex = 0;
            this.webServicesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.WebServicesTreeViewAfterSelect);
            // 
            // webServicesListView
            // 
            this.webServicesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                                          	this.propertyColumnHeader,
			                                          	this.valueColumnHeader});
            this.webServicesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webServicesListView.Location = new System.Drawing.Point(0, 0);
            this.webServicesListView.Name = "webServicesListView";
            this.webServicesListView.Size = new System.Drawing.Size(311, 305);
            this.webServicesListView.TabIndex = 2;
            this.webServicesListView.UseCompatibleStateImageBehavior = false;
            this.webServicesListView.View = System.Windows.Forms.View.Details;
            // 
            // propertyColumnHeader
            // 
            this.propertyColumnHeader.Text = "Property";
            this.propertyColumnHeader.Width = 120;
            // 
            // valueColumnHeader
            // 
            this.valueColumnHeader.Text = "Value";
            this.valueColumnHeader.Width = 191;
            // 
            // WebServicesView
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "WebServicesView";
            this.Size = new System.Drawing.Size(471, 305);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ColumnHeader propertyColumnHeader;
        private System.Windows.Forms.ColumnHeader valueColumnHeader;
        private System.Windows.Forms.TreeView webServicesTreeView;
        private System.Windows.Forms.ListView webServicesListView;
        private System.Windows.Forms.SplitContainer splitContainer;
    }
}
