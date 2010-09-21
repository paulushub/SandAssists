using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.StartPage
{
    /// <summary>
    /// Description of the view content
    /// </summary>
    public sealed class StartPageViewContent : AbstractViewContent
    {
        private StartPageViewControl _startpageControl;

        /// <summary>
        /// Creates a new StartPageViewContent object
        /// </summary>
        public StartPageViewContent()
        {               
            this.TitleName = StringParser.Parse("${res:StartPage.StartPageContentName}");

            _startpageControl = new StartPageViewControl();
            _startpageControl.Dock = DockStyle.Fill;
            _startpageControl.BackColor = System.Drawing.Color.AliceBlue;
        }

        /// <summary>
        /// The <see cref="System.Windows.Forms.Control"/> representing the view
        /// </summary>
        public override Control Control
        {
            get
            {
                return _startpageControl;
            }
        }

        public override void ViewLoad()
        {
            if (_startpageControl != null)
            {
                _startpageControl.ViewLoad();
            }
        }

        public override void ViewUnload()
        {
            if (_startpageControl != null)
            {
                _startpageControl.ViewUnload();
            }
        }

        /// <summary>
        /// Refreshes the view
        /// </summary>
        public override void RedrawContent()
        {
            // TODO: Refresh the whole view control here, renew all resource strings
            //       Note that you do not need to recreate the control.
        }

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();
            SetIcon();
        }

        private void SetIcon()
        {
            if (this.WorkbenchWindow != null)
            {
                System.Drawing.Icon icon = WinFormsResourceService.GetIcon(
                    "Icons.16x16.StartPage");
                if (icon != null)
                {
                    this.WorkbenchWindow.Icon = icon;
                }
            }
        }

        /// <summary>
        /// Cleans up all used resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // TODO: Clean up resources in this method
            if (disposing)
            {
                if (_startpageControl != null)
                {
                    _startpageControl.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
