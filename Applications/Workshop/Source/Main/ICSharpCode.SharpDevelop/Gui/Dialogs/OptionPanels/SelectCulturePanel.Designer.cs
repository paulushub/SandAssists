using System;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class SelectCulturePanel
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
            this.listView = new System.Windows.Forms.ListView();
            this.newCulture = new System.Windows.Forms.Label();
            this.culture = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Location = new System.Drawing.Point(6, 18);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(345, 300);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.ItemActivate += new System.EventHandler(this.ChangeCulture);
            // 
            // newCulture
            // 
            this.newCulture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newCulture.ForeColor = System.Drawing.SystemColors.Highlight;
            this.newCulture.Location = new System.Drawing.Point(37, 357);
            this.newCulture.Name = "newCulture";
            this.newCulture.Size = new System.Drawing.Size(318, 18);
            this.newCulture.TabIndex = 3;
            this.newCulture.Text = "Selected Culture";
            // 
            // culture
            // 
            this.culture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.culture.Location = new System.Drawing.Point(37, 335);
            this.culture.Name = "culture";
            this.culture.Size = new System.Drawing.Size(318, 18);
            this.culture.TabIndex = 1;
            this.culture.Text = "Current Culture";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.listView);
            this.groupBox.Location = new System.Drawing.Point(23, 6);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(357, 324);
            this.groupBox.TabIndex = 4;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Available Cultures";
            // 
            // SelectCulturePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.culture);
            this.Controls.Add(this.newCulture);
            this.Name = "SelectCulturePanel";
            this.Size = new System.Drawing.Size(404, 384);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Label newCulture;
        private System.Windows.Forms.Label culture;
        private System.Windows.Forms.GroupBox groupBox;
    }
}
