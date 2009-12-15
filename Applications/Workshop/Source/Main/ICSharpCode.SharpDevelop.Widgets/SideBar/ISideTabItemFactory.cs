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
    public interface ISideTabItemFactory
    {
        SideTabItem CreateSideTabItem(string name);
        SideTabItem CreateSideTabItem(string name, object tag);
        SideTabItem CreateSideTabItem(string name, object tag, Bitmap bitmap);
    }
}
