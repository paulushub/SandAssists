// Based on codes by Daniel Moth
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace ICSharpCode.Core.WinForms
{
    public class TreeViewVista : TreeView
    {
        public TreeViewVista()
        {   
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (Environment.OSVersion.Version.Major >= 6)
                {   
                    // lose the horizontal scrollbar
                    cp.Style |= InteropMethods.TVS_NOHSCROLL; 
                }

                return cp;
            }
        }

        protected override void OnHandleCreated(System.EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            // get style
            int dw = InteropMethods.SendMessage(this.Handle,
              InteropMethods.TVM_GETEXTENDEDSTYLE, 0, 0);

            // Update style
            dw |= InteropMethods.TVS_EX_AUTOHSCROLL;       // auto-scroll horizontally
            //dw |= InteropMethods.TVS_EX_FADEINOUTEXPANDOS; // auto hide the +/- signs

            // set style
            InteropMethods.SendMessage(this.Handle,
              InteropMethods.TVM_SETEXTENDEDSTYLE, 0, dw);

            // little black/empty arrows and blue highlight on tree nodes
            InteropMethods.SetWindowTheme(this.Handle, "explorer", null);
        }
    }
}
