// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3082 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public class DefaultSideTabFactory : ISideTabFactory
    {
        public SideTab CreateSideTab(SideBarControl sideBar, string name)
        {
            return new SideTab(sideBar, name);
        }
    }
}
