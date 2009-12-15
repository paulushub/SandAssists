using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class WizardDialog
    {
        #region Windows Form Designer generated code

        private void InitializeComponents()
        {
            this.label1 = new Label();

            this.backButton = new Button();
            this.nextButton = new Button();
            this.finishButton = new Button();
            this.cancelButton = new Button();
            this.helpButton = new Button();

            this.SuspendLayout();

            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = MaximizeBox = false;
            this.Icon = null;
            this.ClientSize = new Size(640, 440);

            int buttonSize = 92;
            int buttonYLoc = 464 - 2 * 24 - 4;
            int buttonXStart = Width - ((buttonSize + 4) * 4) - 4;

            this.label1.Size = new Size(Width - 4, 1);
            this.label1.BorderStyle = BorderStyle.Fixed3D;
            this.label1.Location = new Point(2, 404 - 2);
            this.label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            this.Controls.Add(label1);

            this.backButton.Text = ResourceService.GetString("Global.BackButtonText");
            this.backButton.Location = new Point(buttonXStart, buttonYLoc);
            this.backButton.ClientSize = new Size(buttonSize, 26);
            this.backButton.Click += new EventHandler(ShowPrevPanelEvent);
            this.backButton.FlatStyle = FlatStyle.System;
            this.backButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(backButton);

            this.nextButton.Text = ResourceService.GetString("Global.NextButtonText");
            this.nextButton.Location = new Point(buttonXStart + buttonSize + 4, buttonYLoc);
            this.nextButton.ClientSize = new Size(buttonSize, 26);
            this.nextButton.Click += new EventHandler(ShowNextPanelEvent);
            this.nextButton.FlatStyle = FlatStyle.System;
            this.nextButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Controls.Add(nextButton);

            this.finishButton.Text = ResourceService.GetString("Dialog.WizardDialog.FinishButton");
            this.finishButton.Location = new Point(buttonXStart + 2 * (buttonSize + 4), buttonYLoc);
            this.finishButton.ClientSize = new Size(buttonSize, 26);
            this.finishButton.Click += new EventHandler(FinishEvent);
            this.finishButton.FlatStyle = FlatStyle.System;
            this.finishButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(finishButton);

            this.cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
            this.cancelButton.Location = new Point(buttonXStart + 3 * (buttonSize + 4), buttonYLoc);
            this.cancelButton.ClientSize = new Size(buttonSize, 26);
            this.cancelButton.Click += new EventHandler(CancelEvent);
            this.cancelButton.FlatStyle = FlatStyle.System;
            this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(cancelButton);

            //	this.helpButton.Text = ResourceService.GetString("Global.HelpButtonText");
            //	this.helpButton.Location = new Point(buttonXStart + 4 * (buttonSize + 4), buttonYLoc);
            //	this.helpButton.ClientSize     = new Size(buttonSize, 26);
            //	this.helpButton.Click   += new EventHandler(HelpEvent);
            //	this.helpButton.FlatStyle = FlatStyle.System;
            //	this.helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //	this.Controls.Add(helpButton);

            this.statusPanel = new StatusPanel(this);
            this.statusPanel.Location = new Point(2, 2);
            this.statusPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(statusPanel);

            this.curPanel = new CurrentPanelPanel(this);
            this.curPanel.Location = new Point(200, 2);
            this.curPanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(curPanel);

            this.dialogPanel.Location = new Point(200, 27);
            // this.dialogPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dialogPanel.Size = new Size(Width - 8 - statusPanel.Bounds.Right,
                                               label1.Location.Y - dialogPanel.Location.Y);
            this.dialogPanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            this.Controls.Add(dialogPanel);
            this.ResumeLayout(true);
        }

        #endregion

        private Label label1;

        private Button backButton;
        private Button nextButton;
        private Button finishButton;
        private Button cancelButton;
        private Button helpButton;
    }
}
