// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3082 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public interface ISideTabFactory
    {
        SideTab CreateSideTab(SideBarControl sideBar, string name);
    }
}
