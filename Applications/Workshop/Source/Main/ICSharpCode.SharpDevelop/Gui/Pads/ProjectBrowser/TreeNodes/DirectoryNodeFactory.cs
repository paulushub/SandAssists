// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrEer" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3160 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Static factory methods for creating Directory nodes.
	/// </summary>
	public static class DirectoryNodeFactory
	{
		public static DirectoryNode CreateDirectoryNode(TreeNode parent, IProject project, string directory)
		{
            DirectoryNode node = null;

			if (!string.IsNullOrEmpty(project.AppDesignerFolder)
			    && directory == Path.Combine(project.Directory, project.AppDesignerFolder))
			{
                node = new PropertiesFolderNode(project, directory);
			} 
            else if (DirectoryNode.IsWebReferencesFolder(project, directory)) 
            {
				node = new WebReferenceFolderNode(directory);
			} 
            else if (parent != null && parent is WebReferenceFolderNode) 
            {
				node = new WebReferenceNode(directory);
			}
            if (node == null)
            {
                node = new DirectoryNode(directory);
            }

			return node;
		}
		
		public static DirectoryNode CreateDirectoryNode(ProjectItem item, FileNodeStatus status)
		{
			DirectoryNode node;
			if (item is WebReferencesProjectItem) {
				node = new WebReferenceFolderNode((WebReferencesProjectItem)item);
				node.FileNodeStatus = status;
			} else {
				node = new DirectoryNode(item.FileName.Trim('\\', '/'), status);
				node.ProjectItem = item;
			}
			return node;
		}
	}
}
