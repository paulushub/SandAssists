namespace ICSharpCode.CodeEditor.Searching
{
    partial class FileExplorerNet
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
            if (disposing && (components != null)) {
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
            this.treExplorer = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treExplorer
            // 
            this.treExplorer.BackColor = System.Drawing.SystemColors.Window;
            this.treExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treExplorer.CheckBoxes = true;
            this.treExplorer.Cursor = System.Windows.Forms.Cursors.Default;
            this.treExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treExplorer.FullRowSelect = true;
            this.treExplorer.Indent = 16;
            this.treExplorer.ItemHeight = 18;
            this.treExplorer.Location = new System.Drawing.Point(0, 0);
            this.treExplorer.Name = "treExplorer";
            this.treExplorer.Size = new System.Drawing.Size(298, 511);
            this.treExplorer.TabIndex = 0;
            // 
            // FileExplorerNet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treExplorer);
            this.Name = "FileExplorerNet";
            this.Size = new System.Drawing.Size(298, 511);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treExplorer;
    }
}
