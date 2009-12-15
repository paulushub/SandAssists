namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class ReferencePathsPanel
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
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
            this.editor = new ICSharpCode.SharpDevelop.Gui.StringListEditor();
            this.SuspendLayout();
            // 
            // editor
            // 
            this.editor.AddButtonText = "Add Item";
            this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editor.AutoAddAfterBrowse = false;
            this.editor.BrowseForDirectory = false;
            this.editor.ListCaption = "List:";
            this.editor.Location = new System.Drawing.Point(0, 0);
            this.editor.ManualOrder = true;
            this.editor.MinimumSize = new System.Drawing.Size(338, 243);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(460, 471);
            this.editor.TabIndex = 0;
            this.editor.TitleText = "Title:";
            // 
            // ReferencePathsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editor);
            this.Name = "ReferencePathsPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private StringListEditor editor;
    }
}
