// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3813 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	/// <summary>
	/// This class represents the base class for all nodes in the class browser.
	/// </summary>
	public class DerivedTypesNode : ExtFolderNode
	{
        private IClass _classItem;
		private IProject _projectItem;
		
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowserPad.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowBaseAndDerivedTypes) != 0;
			}
		}
		
		public IProject Project {
			get {
				return _projectItem;
			}
		}
		
		public DerivedTypesNode(IProject project, IClass c)
		{
			sortOrder = 1;
			
			this._projectItem = project;
			this._classItem   = c;
			this.Text         = ResourceService.GetString("MainWindow.Windows.ClassBrowser.DerivedTypes");
			
			this.OpenedIcon   = "ProjectBrowser.Folder.Open";
			this.ClosedIcon   = "ProjectBrowser.Folder.Closed";
			
			this.Nodes.Add(new TreeNode(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode")));
		}

        protected override void Initialize()
        {
            base.Initialize();
            Nodes.Clear();

            List<IProjectContent> contentList = new List<IProjectContent>();
            if (ProjectService.OpenSolution != null)
            {
                foreach (IProject project in ProjectService.OpenSolution.Projects)
                {
                    IProjectContent projectContent = ParserService.GetProjectContent(project);
                    if (projectContent != null)
                    {
                        contentList.Add(projectContent);
                    }
                }
            }
            foreach (IClass derivedClass in RefactoringService.FindDerivedClasses(
                _classItem, contentList, true))
            {
                new ClassNode(_projectItem, derivedClass).AddTo(this);
            }

            if (Nodes.Count == 0)
            {
                this.SetIcon(this.ClosedIcon);
                this.OpenedIcon = this.ClosedIcon = null;
            }
        }
    }
}
