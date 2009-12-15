using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class Workbench
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DefaultWorkbench
            // 
            this.ClientSize = new System.Drawing.Size(765, 642);
            this.Name = "DefaultWorkbench";
            this.Text = "SandcastleWorkshop";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
