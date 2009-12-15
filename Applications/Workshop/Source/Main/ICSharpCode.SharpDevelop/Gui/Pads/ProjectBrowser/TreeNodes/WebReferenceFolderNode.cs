// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 955 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceFolderNode : DirectoryNode
	{		
		public WebReferenceFolderNode(WebReferencesProjectItem projectItem) 
            : this(projectItem.Directory)
		{
			ProjectItem = projectItem;
		}
		
		public WebReferenceFolderNode(string directory) : base(directory)
		{
			sortOrder = 0;
			this.SpecialFolder            = SpecialFolder.WebReferenceFolder;
            this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WebReferenceFolderNode";
		}
	}
}
