// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4450 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractPadContent : IPadContent
	{
        private bool _isDisposed;

        protected AbstractPadContent()
        {   
        }

        ~AbstractPadContent()
        {
            this.Dispose(false);
        }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

		public abstract Control Control 
        {
			get;
		}

        public bool IsVisible
        {
            get
            {
                Control ctl = this.Control;
                if (ctl == null || ctl.IsDisposed)
                {
                    return false;
                }

                return ctl.Visible && ctl.Width > 0 && ctl.Height > 0;
            }
        }
		
		public virtual void RedrawContent()
		{
		}

        public void BringToFront()
        {
            PadDescriptor d = this.PadDescriptor;
            if (d != null)
                d.BringPadToFront();
        }

        protected virtual PadDescriptor PadDescriptor
        {
            get
            {
                if (WorkbenchSingleton.Workbench == null || 
                    WorkbenchSingleton.Workbench.WorkbenchLayout == null)
                    return null;

                return WorkbenchSingleton.Workbench.GetPad(GetType());
            }
        }

        protected virtual ToolStripItem InsertHelpItem(ToolStrip toolStrip,
            bool includeText)
        {
            ToolStripItem helpItem = ToolbarService.InsertHelpItem(toolStrip, 
                includeText, new EventHandler(OnDisplayHelp));

            return helpItem;
        }

        protected virtual void OnDisplayHelp(object sender, EventArgs args)
        {
            MessageBox.Show("There is no help provided yet!");
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        #endregion
    }
}
