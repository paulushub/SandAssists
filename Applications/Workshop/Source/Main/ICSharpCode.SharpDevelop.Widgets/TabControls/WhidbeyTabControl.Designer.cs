namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
    partial class WhidbeyTabControl
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
            this.whidbeySlider = new WhidbeySlider();
            this.whidbeyContainer = new WhidbeyContainer();
            this.SuspendLayout();
            // 
            // whidbeySlider
            // 
            this.whidbeySlider.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.whidbeySlider.Dock = System.Windows.Forms.DockStyle.Left;
            this.whidbeySlider.Location = new System.Drawing.Point(12, 13);
            this.whidbeySlider.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.whidbeySlider.Name = "whidbeySlider";
            this.whidbeySlider.Padding = new System.Windows.Forms.Padding(2);
            this.whidbeySlider.SelectedIndex = -1;
            this.whidbeySlider.Size = new System.Drawing.Size(132, 551);
            this.whidbeySlider.TabHeight = 32;
            this.whidbeySlider.TabIndex = 0;
            // 
            // whidbeyContainer
            // 
            this.whidbeyContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.whidbeyContainer.Location = new System.Drawing.Point(144, 13);
            this.whidbeyContainer.Name = "whidbeyContainer";
            this.whidbeyContainer.Padding = new System.Windows.Forms.Padding(4);
            this.whidbeyContainer.SelectedIndex = -1;
            this.whidbeyContainer.Size = new System.Drawing.Size(366, 551);
            this.whidbeyContainer.TabIndex = 1;
            // 
            // WhidbeyTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.whidbeyContainer);
            this.Controls.Add(this.whidbeySlider);
            this.Name = "WhidbeyTabControl";
            this.Padding = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Size = new System.Drawing.Size(522, 577);
            this.Load += new System.EventHandler(this.OnTabLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private WhidbeySlider whidbeySlider;
        private WhidbeyContainer whidbeyContainer;
    }
}
