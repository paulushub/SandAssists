using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AddInManager
{
    partial class ManagerForm
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
            this.actionGroupBox = new System.Windows.Forms.GroupBox();
            this.dependencyTable = new System.Windows.Forms.TableLayoutPanel();
            this.dummyLabel1 = new System.Windows.Forms.Label();
            this.dummyLabel2 = new System.Windows.Forms.Label();
            this.actionDescription = new System.Windows.Forms.Label();
            this.preinstalledCheckBox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.panelBottom = new ICSharpCode.SharpDevelop.Gui.PanelWithSizeGrip();
            this.aboutSplitContainer = new System.Windows.Forms.SplitContainer();
            this.aboutTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.actionGroupBox.SuspendLayout();
            this.dependencyTable.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.aboutSplitContainer.Panel1.SuspendLayout();
            this.aboutSplitContainer.Panel2.SuspendLayout();
            this.aboutSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.AllowDrop = true;
            this.splitContainer.Panel1.AutoScroll = true;
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnSplitContainerPanel1Paint);
            this.splitContainer.Panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Panel1DragDrop);
            this.splitContainer.Panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Panel1DragEnter);
            this.splitContainer.Panel1MinSize = 100;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.actionGroupBox);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Panel2MinSize = 100;
            this.splitContainer.Size = new System.Drawing.Size(618, 333);
            this.splitContainer.SplitterDistance = 409;
            this.splitContainer.TabIndex = 2;
            // 
            // actionGroupBox
            // 
            this.actionGroupBox.Controls.Add(this.dependencyTable);
            this.actionGroupBox.Controls.Add(this.actionDescription);
            this.actionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionGroupBox.Location = new System.Drawing.Point(3, 3);
            this.actionGroupBox.Name = "actionGroupBox";
            this.actionGroupBox.Size = new System.Drawing.Size(199, 327);
            this.actionGroupBox.TabIndex = 0;
            this.actionGroupBox.TabStop = false;
            this.actionGroupBox.Text = "actionGroupBox";
            this.actionGroupBox.UseCompatibleTextRendering = true;
            // 
            // dependencyTable
            // 
            this.dependencyTable.AutoSize = true;
            this.dependencyTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dependencyTable.ColumnCount = 2;
            this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.dependencyTable.Controls.Add(this.dummyLabel1, 1, 0);
            this.dependencyTable.Controls.Add(this.dummyLabel2, 1, 1);
            this.dependencyTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dependencyTable.Location = new System.Drawing.Point(3, 33);
            this.dependencyTable.Name = "dependencyTable";
            this.dependencyTable.RowCount = 2;
            this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.dependencyTable.Size = new System.Drawing.Size(193, 291);
            this.dependencyTable.TabIndex = 1;
            // 
            // dummyLabel1
            // 
            this.dummyLabel1.AutoSize = true;
            this.dummyLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dummyLabel1.Location = new System.Drawing.Point(23, 0);
            this.dummyLabel1.Name = "dummyLabel1";
            this.dummyLabel1.Size = new System.Drawing.Size(167, 17);
            this.dummyLabel1.TabIndex = 0;
            this.dummyLabel1.Text = "dep1";
            this.dummyLabel1.UseCompatibleTextRendering = true;
            // 
            // dummyLabel2
            // 
            this.dummyLabel2.AutoSize = true;
            this.dummyLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dummyLabel2.Location = new System.Drawing.Point(23, 17);
            this.dummyLabel2.Name = "dummyLabel2";
            this.dummyLabel2.Size = new System.Drawing.Size(167, 274);
            this.dummyLabel2.TabIndex = 1;
            this.dummyLabel2.Text = "dep2";
            this.dummyLabel2.UseCompatibleTextRendering = true;
            // 
            // actionDescription
            // 
            this.actionDescription.AutoEllipsis = true;
            this.actionDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.actionDescription.Location = new System.Drawing.Point(3, 16);
            this.actionDescription.Name = "actionDescription";
            this.actionDescription.Size = new System.Drawing.Size(193, 17);
            this.actionDescription.TabIndex = 0;
            this.actionDescription.Text = "actionDescription";
            this.actionDescription.UseCompatibleTextRendering = true;
            // 
            // preinstalledCheckBox
            // 
            this.preinstalledCheckBox.AutoSize = true;
            this.preinstalledCheckBox.Location = new System.Drawing.Point(4, 10);
            this.preinstalledCheckBox.Name = "preinstalledCheckBox";
            this.preinstalledCheckBox.Size = new System.Drawing.Size(145, 17);
            this.preinstalledCheckBox.TabIndex = 3;
            this.preinstalledCheckBox.Text = "Show preinstalled AddIns";
            this.preinstalledCheckBox.UseVisualStyleBackColor = true;
            this.preinstalledCheckBox.CheckedChanged += new System.EventHandler(this.OnShowPreinstalledAddIns);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(539, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.closeButton);
            this.panelBottom.Controls.Add(this.preinstalledCheckBox);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 514);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(618, 35);
            this.panelBottom.TabIndex = 5;
            // 
            // aboutSplitContainer
            // 
            this.aboutSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.aboutSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.aboutSplitContainer.Name = "aboutSplitContainer";
            this.aboutSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // aboutSplitContainer.Panel1
            // 
            this.aboutSplitContainer.Panel1.Controls.Add(this.splitContainer);
            // 
            // aboutSplitContainer.Panel2
            // 
            this.aboutSplitContainer.Panel2.Controls.Add(this.aboutTablePanel);
            this.aboutSplitContainer.Size = new System.Drawing.Size(618, 514);
            this.aboutSplitContainer.SplitterDistance = 333;
            this.aboutSplitContainer.SplitterWidth = 8;
            this.aboutSplitContainer.TabIndex = 6;
            // 
            // aboutTablePanel
            // 
            this.aboutTablePanel.AutoScroll = true;
            this.aboutTablePanel.BackColor = System.Drawing.SystemColors.Control;
            this.aboutTablePanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.aboutTablePanel.ColumnCount = 2;
            this.aboutTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.aboutTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aboutTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutTablePanel.Location = new System.Drawing.Point(0, 0);
            this.aboutTablePanel.Name = "aboutTablePanel";
            this.aboutTablePanel.Padding = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.aboutTablePanel.RowCount = 2;
            this.aboutTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.aboutTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.aboutTablePanel.Size = new System.Drawing.Size(618, 173);
            this.aboutTablePanel.TabIndex = 0;
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 549);
            this.Controls.Add(this.aboutSplitContainer);
            this.Controls.Add(this.panelBottom);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(624, 460);
            this.Name = "ManagerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "AddIn Manager";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.actionGroupBox.ResumeLayout(false);
            this.actionGroupBox.PerformLayout();
            this.dependencyTable.ResumeLayout(false);
            this.dependencyTable.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.aboutSplitContainer.Panel1.ResumeLayout(false);
            this.aboutSplitContainer.Panel2.ResumeLayout(false);
            this.aboutSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        private System.Windows.Forms.Label dummyLabel2;
        private System.Windows.Forms.Label dummyLabel1;
        private System.Windows.Forms.TableLayoutPanel dependencyTable;
        private System.Windows.Forms.Label actionDescription;
        private System.Windows.Forms.GroupBox actionGroupBox;
        private System.Windows.Forms.SplitContainer splitContainer;
        private CheckBox preinstalledCheckBox;
        private Button closeButton;
        private PanelWithSizeGrip panelBottom;
        private SplitContainer aboutSplitContainer;
        private TableLayoutPanel aboutTablePanel;
    }
}
