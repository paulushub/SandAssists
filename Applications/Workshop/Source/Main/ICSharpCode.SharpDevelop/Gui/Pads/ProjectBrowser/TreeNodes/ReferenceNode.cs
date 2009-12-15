// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2640 $</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : BaseNode
	{
		private ReferenceProjectItem _referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem 
        {
			get 
            {
				return _referenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			_referenceProjectItem = referenceProjectItem;

            this.Tag = referenceProjectItem; 
            this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
            this.SetIcon("Icons.16x16.Reference");
            this.Text = referenceProjectItem.ShortName;
		}
		
		#region Cut & Paste

		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			IProject project = Project;
			ProjectService.RemoveProjectItem(_referenceProjectItem.Project, _referenceProjectItem);
			Debug.Assert(Parent != null);
			Debug.Assert(Parent is ReferenceFolderNode);
			((ReferenceFolderNode)Parent).ShowReferences();
			project.Save();
		}

		#endregion
		
		public override object AcceptVisitor(
            ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
