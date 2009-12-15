// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4222 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{	
	public class ClassBrowserPad : AbstractPadContent
	{
		static ClassBrowserPad instance;
		
		public static ClassBrowserPad Instance {
			get {
				return instance;
			}
		}
		ClassBrowserFilter filter               = ClassBrowserFilter.All;
		Panel              contentPanel         = new Panel();
		ExtTreeView        classBrowserTreeView = new ExtTreeView();
		
		public ClassBrowserFilter Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					if (node is ExtTreeNode) {
						((ExtTreeNode)node).UpdateVisibility();
					}
				}
			}
		}
		
		public override Control Control {
			get {
				return contentPanel;
			}
		}
		ToolStrip toolStrip;
		ToolStrip searchStrip;
		
		void UpdateToolbars()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			ToolbarService.UpdateToolbar(searchStrip);
		}
		
		public ClassBrowserPad()
		{
			instance = this;
			classBrowserTreeView.Dock         = DockStyle.Fill;
			classBrowserTreeView.ImageList    = ClassBrowserIconService.ImageList;
			classBrowserTreeView.AfterSelect += new TreeViewEventHandler(ClassBrowserTreeViewAfterSelect);
			
			contentPanel.Controls.Add(classBrowserTreeView);
			
			searchStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Searchbar");
			searchStrip.Stretch   = true;
			searchStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;

            ToolbarService.IndentItems(searchStrip);
            contentPanel.Controls.Add(searchStrip);

            searchStrip.SizeChanged += new EventHandler(OnSearchToolBarSizeChanged);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;

            // Indent the toolbar items to beautify it...
            ToolbarService.IndentItems(toolStrip);
            // Add support for the help...
            this.InsertHelpItem(toolStrip, false);

            contentPanel.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += ProjectServiceSolutionChanged;
			ProjectService.ProjectItemAdded += ProjectServiceSolutionChanged;
			ProjectService.ProjectItemRemoved += ProjectServiceSolutionChanged;
			ProjectService.ProjectAdded += ProjectServiceSolutionChanged; // rebuild view when project is added to solution
			ProjectService.SolutionFolderRemoved += ProjectServiceSolutionChanged; // rebuild view when project is removed from solution
			ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			
			ParserService.ParseInformationUpdated += new ParseInformationEventHandler(ParserServiceParseInformationUpdated);
			
			AmbienceService.AmbienceChanged += new EventHandler(AmbienceServiceAmbienceChanged);
			if (ProjectService.OpenSolution != null) {
				ProjectServiceSolutionChanged(null, null);
			}
			UpdateToolbars();
		}

		List<ParseInformationEventArgs> pending = new List<ParseInformationEventArgs>();
		
		// running on main thread, invoked by the parser thread when a compilation unit changed
		void UpdateThread()
		{
			lock (pending) {
				foreach (ParseInformationEventArgs e in pending) {
					foreach (TreeNode node in classBrowserTreeView.Nodes) {
						AbstractProjectNode prjNode = node as AbstractProjectNode;
						if (prjNode != null && e.ProjectContent.Project == prjNode.Project) {
							prjNode.UpdateParseInformation(e.OldCompilationUnit, e.NewCompilationUnit);
						}
					}
				}
				pending.Clear();
			}
		}
		
		public void ParserServiceParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			lock (pending) {
				pending.Add(e);
			}
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateThread);
		}
		
		#region Navigation
		Stack<TreeNode> previousNodes = new Stack<TreeNode>();
		Stack<TreeNode> nextNodes     = new Stack<TreeNode>();
		bool navigateBack    = false;
		bool navigateForward = false;
		
		public bool CanNavigateBackward {
			get {
				if (previousNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					return false;
				}
				return previousNodes.Count > 0;
			}
		}
		
		public bool CanNavigateForward {
			get {
				if (nextNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					return false;
				}
				return nextNodes.Count > 0;
			}
		}
		
		public void NavigateBackward()
		{
			if (previousNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					nextNodes.Push(previousNodes.Pop());
				}
				if (previousNodes.Count > 0) {
					navigateBack = true;
					this.classBrowserTreeView.SelectedNode = previousNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		public void NavigateForward()
		{
			if (nextNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					previousNodes.Push(nextNodes.Pop());
				}
				if (nextNodes.Count > 0) {
					navigateForward = true;
					this.classBrowserTreeView.SelectedNode = nextNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		void ClassBrowserTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (navigateBack) {
				nextNodes.Push(e.Node);
				navigateBack = false;
			} else {
				if (!navigateForward) {
					nextNodes.Clear();
				}
				previousNodes.Push(e.Node);
				navigateForward = false;
			}
			UpdateToolbars();
		}
		#endregion

        private bool inSearchMode = false;
        private List<TreeNode> oldNodes = new List<TreeNode>();
        private string searchTerm = "";
		
		public bool IsInSearchMode {
			get {
				return inSearchMode;
			}
		}
		public string SearchTerm {
			get {
				return searchTerm;
			}
			set {
				searchTerm = value.ToUpper().Trim();
			}
		}
		
		public void StartSearch()
		{
			if (searchTerm.Length == 0) {
				CancelSearch();
				return;
			}
			if (!inSearchMode) {
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					oldNodes.Add(node);
				}
				inSearchMode = true;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
			classBrowserTreeView.BeginUpdate();
			classBrowserTreeView.Nodes.Clear();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						foreach (IClass c in projectContent.Classes) {
							if (c.Name.ToUpper().StartsWith(searchTerm)) {
								ClassNodeBuilders.AddClassNode(classBrowserTreeView, project, c);
							}
						}
					}
				}
			}
			if (classBrowserTreeView.Nodes.Count == 0) {
				ExtTreeNode notFoundMsg = new ExtTreeNode();
				notFoundMsg.Text = ResourceService.GetString("MainWindow.Windows.ClassBrowser.NoResultsFound");
				notFoundMsg.AddTo(classBrowserTreeView);
			}
			classBrowserTreeView.Sort();
			classBrowserTreeView.EndUpdate();
		}
		
		public void CancelSearch()
		{
			if (inSearchMode) {
				classBrowserTreeView.Nodes.Clear();
				foreach (TreeNode node in oldNodes) {
					classBrowserTreeView.Nodes.Add(node);
				}
				oldNodes.Clear();
				inSearchMode = false;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
		}

        private void ProjectServiceSolutionChanged(object sender, EventArgs e)
		{
            classBrowserTreeView.BeginUpdate();

			classBrowserTreeView.Nodes.Clear();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					if (project is MissingProject || project is UnknownProject) {
						continue;
					}
					ProjectNodeBuilders.AddProjectNode(classBrowserTreeView, project);
				}
                classBrowserTreeView.Sort();
			}

            classBrowserTreeView.EndUpdate();
		}

        private void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			classBrowserTreeView.Nodes.Clear();
			previousNodes.Clear();
			nextNodes.Clear();
			UpdateToolbars();
		}

        private void AmbienceServiceAmbienceChanged(object sender, EventArgs e)
		{
		}

        private void OnSearchToolBarSizeChanged(object sender, EventArgs e)
        {
            ToolStripComboBox stripComboBox = searchStrip.Items[0] as ToolStripComboBox;
            if (stripComboBox == null)
            {
                return;
            }

            int comboWidth = 0;
            for (int i = 0; i < 3; i++)
            {
                comboWidth += searchStrip.Items[i].Width;
            }
            int curWidth = stripComboBox.Width;
            comboWidth -= curWidth;

            int stretchedWidth = searchStrip.Width - comboWidth - 16;
            if (curWidth > stretchedWidth)
            {
                stripComboBox.ComboBox.Width = stretchedWidth;
                searchStrip.PerformLayout();
            }
            else
            {
                stripComboBox.ComboBox.Width = stretchedWidth;
            }
        }
		
	}
}
