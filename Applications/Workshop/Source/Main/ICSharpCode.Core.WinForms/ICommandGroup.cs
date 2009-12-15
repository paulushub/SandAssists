using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.Core.WinForms
{
    public interface ICommandGroup
    {
        string Operation
        {
            get;
        }

        IList<ToolStripItem> Items
        {
            get;
        }

        void Update(ToolStripItem exceptItem);
    }
}
