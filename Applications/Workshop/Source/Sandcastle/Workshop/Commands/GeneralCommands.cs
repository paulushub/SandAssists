using System;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
   
using Sandcastle.Workshop.Dialogs;

namespace Sandcastle.Workshop.Commands
{
    public sealed class HelpAboutWorkshop : AbstractMenuCommand
    {
        public override void Run()
        {
            using (AboutWorkshopDialog ad = new AboutWorkshopDialog())
            {
                ad.Owner = WorkbenchSingleton.MainForm;
                ad.ShowDialog(WorkbenchSingleton.MainForm);
            }
        }
    }
}
