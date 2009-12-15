using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.Core.WinForms
{
    public class MenuItemComparer : IComparer<ToolStripMenuItem>
    {
        public MenuItemComparer()
        {
        }

        #region IComparer<MenuItemComparer> Members

        public int Compare(ToolStripMenuItem x,
            ToolStripMenuItem y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.Text.CompareTo(y.Text);
        }

        #endregion
    }
}
