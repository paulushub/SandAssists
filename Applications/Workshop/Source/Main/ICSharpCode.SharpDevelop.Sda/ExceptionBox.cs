// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3862 $</version>
// </file>

// project created on 2/6/2003 at 11:10 AM
using System;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

using ICSharpCode.SharpDevelop.Sda.Properties;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// Form used to display display unhandled errors in SharpDevelop.
	/// </summary>
    public partial class ExceptionBox : Form
	{
		Exception exceptionThrown;
		string message;
		
		internal static void RegisterExceptionBoxForUnhandledExceptions()
		{
			Application.ThreadException += ShowErrorBox;
			AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;
			MessageService.CustomErrorReporter = ShowErrorBox;
		}
		
		static void ShowErrorBox(object sender, ThreadExceptionEventArgs e)
		{
			LoggingService.Error("ThreadException caught", e.Exception);
			ShowErrorBox(e.Exception, null);
		}
		
		[SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
		static void ShowErrorBox(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			LoggingService.Fatal("UnhandledException caught", ex);
			if (e.IsTerminating)
				LoggingService.Fatal("Runtime is terminating because of unhandled exception.");
			ShowErrorBox(ex, "Unhandled exception", e.IsTerminating);
		}
		
		static void ShowErrorBox(Exception exception, string message)
		{
			ShowErrorBox(exception, message, false);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
		{
			try {
				using (ExceptionBox box = new ExceptionBox(exception, message, mustTerminate)) {
					if (ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.InvokeRequired)
						box.ShowDialog();
					else
						box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				}
			} catch (Exception ex) {
				LoggingService.Warn("Error showing ExceptionBox", ex);
				MessageBox.Show(exception.ToString(), message, MessageBoxButtons.OK,
				                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			}
		}
		
		public ExceptionBox()
		{
			InitializeComponent();
        }
		
		/// <summary>
		/// Creates a new ExceptionBox instance.
		/// </summary>
		/// <param name="exception">The exception to display</param>
		/// <param name="message">An additional message to display</param>
		/// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the
		/// continue button is not available.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public ExceptionBox(Exception exception, string message, 
            bool mustTerminate)
		{
			this.exceptionThrown = exception;
			this.message = message;
			InitializeComponent();
            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");

			if (mustTerminate) {
				closeButton.Visible  = false;
				continueButton.Text  = closeButton.Text;
				continueButton.Left -= closeButton.Width - continueButton.Width;
				continueButton.Width = closeButton.Width;
			}
			
			try {
				Translate(this);
			} catch {}
			
			exceptionTextBox.Text = getClipboardString();

            try
            {
                Bitmap errorImage = Properties.Resources.Stop;
                if (errorImage != null)
                {
                    errorImage.MakeTransparent();
                    this.pictureBox.Image = errorImage;
                }
                //this.pictureBox.Image = WinFormsResourceService.GetBitmap("ErrorReport");
            }
            catch 
            { 
            }
		}
		
		void Translate(Control ctl)
		{
			ctl.Text = StringParser.Parse(ctl.Text);
			foreach (Control child in ctl.Controls) {
				Translate(child);
			}
		}
		
		string getClipboardString()
		{
            StringBuilder sb = new StringBuilder();

            sb.Append(SharpDevelopService.GetVersionInformationString());

            sb.AppendLine();

            if (message != null)
            {
                sb.AppendLine(message);
            }
            sb.AppendLine("Exception thrown:");
            sb.AppendLine(exceptionThrown.ToString());
            sb.AppendLine();
            sb.AppendLine("---- Recent log messages:");
            try
            {
                LogMessageRecorder.AppendRecentLogMessages(sb, log4net.LogManager.GetLogger(typeof(log4netLoggingService)));
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to append recent log messages.");
                sb.AppendLine(ex.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("---- Post-error application state information:");
            try
            {
                ApplicationStateInfoService.AppendFormatted(sb);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to append application state information.");
                sb.AppendLine(ex.ToString());
            }
            return sb.ToString();
        }
		
		void CopyInfoToClipboard()
		{
			if (copyErrorCheckBox.Checked) {
                string exceptionText = exceptionTextBox.Text;
				if (Application.OleRequired() == ApartmentState.STA) {
					ClipboardWrapper.SetText(exceptionText);
				} else {
					Thread th = new Thread((ThreadStart)delegate {
					                       	ClipboardWrapper.SetText(exceptionText);
					                       });
					th.Name = "CopyInfoToClipboard";
					th.SetApartmentState(ApartmentState.STA);
					th.Start();
				}
			}
		}
		
		void buttonClick(object sender, System.EventArgs e)
		{
			CopyInfoToClipboard();
			
			StartUrl("http://www.codeplex.com/SandAssist/Thread/List.aspx" + RevisionClass.FullVersion);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static void StartUrl(string url)
		{
			try {
				Process.Start(url);
			} catch (Exception e) {
				LoggingService.Warn("Cannot start " + url, e);
			}
		}
		
		void continueButtonClick(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Ignore;
			Close();
		}
		
		void CloseButtonClick(object sender, EventArgs e)
		{
			if (MessageBox.Show(StringParser.Parse("${res:ICSharpCode.SharpDevelop.ExceptionBox.QuitWarning}"), MessageService.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly)
			    == DialogResult.Yes)
			{
				Application.Exit();
			}
		}
	}
}
