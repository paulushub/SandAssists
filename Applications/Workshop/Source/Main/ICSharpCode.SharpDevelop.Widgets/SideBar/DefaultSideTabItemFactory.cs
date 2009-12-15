// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3082 $</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public class DefaultSideTabItemFactory : ISideTabItemFactory
    {
        public SideTabItem CreateSideTabItem(string name)
        {
            return new SideTabItem(name);
        }

        public SideTabItem CreateSideTabItem(string name, object tag)
        {
            return new SideTabItem(name, tag);
        }
        public SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap)
        {
            return new SideTabItem(name, tag, bitmap);
        }

    }
}
