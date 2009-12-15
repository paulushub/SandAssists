using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
    public sealed class DomProgressMonitor : Dom.IDomProgressMonitor
    {
        IProgressMonitor monitor;

        private DomProgressMonitor(IProgressMonitor monitor)
        {
            if (monitor == null)
                throw new ArgumentNullException("monitor");
            this.monitor = monitor;
        }

        public static Dom.IDomProgressMonitor Wrap(IProgressMonitor monitor)
        {
            if (monitor == null)
                return null;
            else
                return new DomProgressMonitor(monitor);
        }

        public bool ShowingDialog
        {
            get { return monitor.ShowingDialog; }
            set { monitor.ShowingDialog = value; }
        }
    }
}
