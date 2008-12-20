namespace Sandcastle.HelpRegister
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.lblMessage = new System.Windows.Forms.Label();
            this.grpVS = new System.Windows.Forms.GroupBox();
            this.btnRetry = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.loadingCircle = new Sandcastle.HelpRegister.LoadingCircle();
            this.grpVS.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OnDoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(121, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(348, 76);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "Please wait while the help file is registered.  This operation may require a coup" +
                "le of minutes to complete.";
            // 
            // grpVS
            // 
            this.grpVS.Controls.Add(this.btnRetry);
            this.grpVS.Controls.Add(this.listBox);
            this.grpVS.Controls.Add(this.lblDescription);
            this.grpVS.Enabled = false;
            this.grpVS.Location = new System.Drawing.Point(13, 94);
            this.grpVS.Name = "grpVS";
            this.grpVS.Size = new System.Drawing.Size(457, 174);
            this.grpVS.TabIndex = 7;
            this.grpVS.TabStop = false;
            this.grpVS.Text = "Document Explorer";
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(191, 143);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(75, 25);
            this.btnRetry.TabIndex = 2;
            this.btnRetry.Text = "Retry";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.OnRetry);
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.Location = new System.Drawing.Point(15, 56);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(428, 82);
            this.listBox.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(7, 20);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(436, 31);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Please close the following Document Explorer instances and click the Retry button" +
                " to continue.";
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.Color = System.Drawing.Color.LightSkyBlue;
            this.loadingCircle.InnerCircleRadius = 6;
            this.loadingCircle.Location = new System.Drawing.Point(12, 5);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 9;
            this.loadingCircle.OuterCircleRadius = 7;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.Size = new System.Drawing.Size(98, 80);
            this.loadingCircle.SpokeThickness = 4;
            this.loadingCircle.StylePreset = Sandcastle.HelpRegister.LoadingCircle.StylePresets.Firefox;
            this.loadingCircle.TabIndex = 6;
            this.loadingCircle.Text = "loadingCircle1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(482, 274);
            this.ControlBox = false;
            this.Controls.Add(this.grpVS);
            this.Controls.Add(this.loadingCircle);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Help 2.0 Registration";
            this.Load += new System.EventHandler(this.OnLoad);
            this.Shown += new System.EventHandler(this.OnShown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.grpVS.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label lblMessage;
        private LoadingCircle loadingCircle;
        private System.Windows.Forms.GroupBox grpVS;
        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label lblDescription;
    }
}