// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision: 2703 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class StatusBarStrip : StatusStrip
	{
        private Control statusBarOwner;

		private ToolStripProgressBar statusProgressBar;
        private ToolStripStatusLabel jobNamePanel;

        private ToolStripStatusLabel txtStatusBarPanel;
        private ToolStripStatusLabel cursorStatusBarPanel;
        private ToolStripStatusLabel modeStatusBarPanel;
        private ToolStripStatusLabel springLabel;
		
		public StatusBarStrip()
		{
            statusProgressBar    = new ToolStripProgressBar();
            jobNamePanel         = new ToolStripStatusLabel();

            txtStatusBarPanel    = new ToolStripStatusLabel();
            cursorStatusBarPanel = new ToolStripStatusLabel();
            modeStatusBarPanel   = new ToolStripStatusLabel();
            springLabel          = new ToolStripStatusLabel();

			springLabel.Spring            = true;

			cursorStatusBarPanel.AutoSize = false;
			cursorStatusBarPanel.Width    = 150;
			
            modeStatusBarPanel.AutoSize   = false;
			modeStatusBarPanel.Width      = 25;
			
            statusProgressBar.Visible     = false;
			statusProgressBar.Width       = 100;
			
			Items.AddRange(new ToolStripItem[] { txtStatusBarPanel, springLabel, jobNamePanel, statusProgressBar, cursorStatusBarPanel, modeStatusBarPanel });

            //this.RenderMode = ToolStripRenderMode.ManagerRenderMode;  
		}

        public Control Owner
        {
            get
            {
                return statusBarOwner;
            }
            set
            {
                if (value != null)
                {
                    statusBarOwner = value;
                }
            }
        }
		
		public ToolStripStatusLabel  CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public ToolStripStatusLabel  ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
		
		public void ShowErrorMessage(string message)
		{
			SetMessage("Error : " + message);
		}
		
		public void ShowErrorMessage(Image image, string message)
		{
			SetMessage(image, "Error : " + message);
		}
		
		public void SetMessage(string message)
		{
			SetMessage(message, false);
		}
		
		public void SetMessage(string message, bool highlighted)
		{
			Action setMessageAction = delegate {
				if (highlighted) {
					txtStatusBarPanel.BackColor = SystemColors.Highlight;
					txtStatusBarPanel.ForeColor = Color.White;
				} else if (txtStatusBarPanel.BackColor == SystemColors.Highlight) {
					txtStatusBarPanel.BackColor = SystemColors.Control;
					txtStatusBarPanel.ForeColor = SystemColors.ControlText;
				}
				txtStatusBarPanel.Text = message;
			};

            if (this.Owner.InvokeRequired)
                this.SafeThreadAsyncCall(setMessageAction);
			else
				setMessageAction();
		}
		
		public void SetMessage(Image image, string message)
		{
			SetMessage(message);
		}
		
		// Displaying progress
		
		bool statusProgressBarIsVisible;
		string currentTaskName;
		
		public void DisplayProgress(string taskName, int workDone, int totalWork)
		{
			if (taskName == null)
				taskName = "";
			if (totalWork < 0)
				totalWork = 0;
			if (workDone < 0)
				workDone = 0;
			if (workDone > totalWork)
				workDone = totalWork;

            Action displayProgressAction = delegate
            {
                if (!statusProgressBarIsVisible)
                {
                    statusProgressBar.Visible = true;
                    statusProgressBarIsVisible = true;
                }

                if (totalWork == 0)
                {
                    statusProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    statusProgressBar.Style = ProgressBarStyle.Continuous;
                    if (statusProgressBar.Maximum != totalWork)
                    {
                        if (statusProgressBar.Value > totalWork)
                            statusProgressBar.Value = 0;
                        statusProgressBar.Maximum = totalWork;
                    }
                    statusProgressBar.Value = workDone;
                }

                if (currentTaskName != taskName)
                {
                    currentTaskName = taskName;
                    jobNamePanel.Text = StringParser.Parse(taskName);
                }
            };

            this.SafeThreadAsyncCall(displayProgressAction);
		}
		
		public void HideProgress()
		{
            Action hideProgressAction = delegate
            {
                statusProgressBarIsVisible = false;
                statusProgressBar.Visible = false;
                jobNamePanel.Text = currentTaskName = "";
            };

            this.SafeThreadAsyncCall(hideProgressAction);
		}

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (statusBarOwner == null)
            {
                Control mainWin = this.Parent;
                while (mainWin != null)
                {
                    statusBarOwner = mainWin;
                    mainWin = mainWin.Parent;
                }
            }
        }

        private void SafeThreadAsyncCall(Delegate method)
        {
            this.SafeThreadAsyncCall(method, new object[0]);
        }

        private void SafeThreadAsyncCall(Delegate method, object[] arguments)
        {
            Control ctl = this.Owner;

            if (ctl == null || ctl.IsDisposed)
            {
                return;
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            try
            {
                ctl.BeginInvoke(method, arguments);
            }
            catch (InvalidOperationException ex)
            {
                LoggingService.Warn("Error in SafeThreadAsyncCall", ex);
            }
        }

	}
}
