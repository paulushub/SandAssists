using System;
using System.Drawing;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class SharpDevelopSideTabItemFactory : ISideTabItemFactory
    {
        public SideTabItem CreateSideTabItem(string name)
        {
            return new SharpDevelopSideTabItem(name);
        }

        public SideTabItem CreateSideTabItem(string name, object tag)
        {
            return new SharpDevelopSideTabItem(name, tag);
        }

        public SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap)
        {
            return new SharpDevelopSideTabItem(name, tag, bitmap);
        }
    }
}
