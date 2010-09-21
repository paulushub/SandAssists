// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3516 $</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;

namespace ICSharpCode.SharpDevelop.Commands
{
    public sealed class NavigationHistoryMenuBuilder : ISubmenuBuilder
	{
		// TODO: refactor BuildSubmenu to add a choice between flat and per-file, eventually per class/method sorting of the list
		
		ToolStripItem[] BuildMenuFlat(ICollection<INavigationPoint> points, int additionalItems)
		{
			ToolStripItem[] items = new ToolStripItem[points.Count+additionalItems];
			MenuCommand cmd = null;
			INavigationPoint p = null;
			List<INavigationPoint> list = new List<INavigationPoint>(points);
			
			int n = points.Count-1; // the last point
			int i = 0;
			while (i<points.Count) {
				p = list[n-i];
				cmd = new MenuCommand(p.Description, new EventHandler(NavigateTo));
				cmd.Tag = p;
//					if (p == NavigationService.CurrentPosition) {
//						cmd.Text = "*** "+cmd.Text;
//					}
				items[i++] = cmd;
			}
			return items;
		}

		ToolStripItem[] BuildMenuByFile(ICollection<INavigationPoint> points, int additionalItems)
		{
			Dictionary<string, List<INavigationPoint>> files =
				new Dictionary<string, List<INavigationPoint>>();
			List<string> fileNames = new List<string>();
			
			foreach (INavigationPoint p in points) {
				if (p.FileName==null) {
					throw new ApplicationException("should not get here!");
				}
				if (!fileNames.Contains(p.FileName)) {
					fileNames.Add(p.FileName);
					files.Add(p.FileName, new List<INavigationPoint>());
				}
				if (!files[p.FileName].Contains(p)) {
					files[p.FileName].Add(p);
				}
			}
			
			fileNames.Sort();
			
			ToolStripItem[] items =
				new ToolStripItem[fileNames.Count + additionalItems];
			ToolStripMenuItem containerItem = null;
			MenuCommand cmd = null;
			int i = 0;
			
			foreach (string fname in fileNames) {
				
				// create a menu bucket
				containerItem = new ToolStripMenuItem();
				containerItem.Text = System.IO.Path.GetFileName(fname);
				containerItem.ToolTipText = fname;
				
				// sort and populate the bucket's contents
//				files[fname].Sort();
				foreach(INavigationPoint p in files[fname]) {
					cmd = new MenuCommand(p.Description, new EventHandler(NavigateTo));
					cmd.Tag = p;
					containerItem.DropDownItems.Add(cmd);
				}
				
				// if there's only one nested item, add it
				// to the result directly, ignoring the bucket
//				if (containerItem.DropDownItems.Count==1) {
//					items[i] = containerItem.DropDownItems[0];
//					items[i].Text = ((INavigationPoint)items[i].Tag).FullDescription;
//					i++;
//				} else {
//					// add the bucket to the result
//					items[i++] = containerItem;
//				}
				// add the bucket to the result
				items[i++] = containerItem;
			}
			
			return items;
		}

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd = null;
			if (NavigationService.CanNavigateBack || NavigationService.CanNavigateForwards) {
				ICollection<INavigationPoint> points = NavigationService.Points;

				//ToolStripItem[] items = BuildMenuFlat(points, numberOfAdditionalItems);
				ToolStripItem[] items = BuildMenuByFile(points, numberOfAdditionalItems);
				
				int i = items.Length - numberOfAdditionalItems;
				
				// additional item 1
				items[i++] = new ToolStripSeparator();
				
				// additional item 2
				cmd = new MenuCommand("${res:XML.MainMenu.Navigation.ClearHistory}", 
                    new EventHandler(ClearHistory));
                cmd.Image = IconService.GetBitmap("Icons.16x16.Clear");
				items[i++] = cmd;
				
				return items;
			}
			
			// default is to disable the dropdown feature...
			return null;
		}

		int numberOfAdditionalItems = 2;
		
		public void NavigateTo(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			NavigationService.Go((INavigationPoint)item.Tag);
		}
		
		public void ClearHistory(object sender, EventArgs e)
		{
			NavigationService.ClearHistory();
		}
	}

    public sealed class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;

            IList<RecentOpenItem> recentItems = recentOpen.RecentFiles;
            int itemCount = Math.Min(recentItems.Count, recentOpen.DisplayableFiles);

            if (itemCount > 0) 
            {
                List<MenuCommand> items = new List<MenuCommand>(itemCount);

                int letterA = Convert.ToInt32('A');
                string accelaratorKeyPrefix = null;

                int iExists = 0;
                int wantedLength = recentOpen.MaximumLength;

                for (int i = 0; i < itemCount; ++i) 
                {
                    //string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10) + " " : "";

                    RecentOpenItem recentItem = recentItems[i];
                    string recentPath = recentItem.FullPath;
                    if (!File.Exists(recentPath))
                    {
                        continue;
                    }
                    iExists++;

                    if (iExists < 10)
                    {
                        accelaratorKeyPrefix = "&" + iExists + "    ";
                    }
                    else
                    {
                        accelaratorKeyPrefix = "&" + Convert.ToChar(letterA) + "    ";
                        letterA++;
                    }

                    MenuCommand menuItem = new MenuCommand(accelaratorKeyPrefix +
                        RecentOpen.CompactPath(recentPath, wantedLength), 
                        new EventHandler(LoadRecentFile));
                    menuItem.Tag = recentPath;
                    menuItem.Description = StringParser.Parse(ResourceService.GetString(
                        "Dialog.Componnents.RichMenuItem.LoadFileDescription"),
                        new string[,] { { "FILE", recentPath } });

                    bool isOpened = (FileService.GetOpenFile(recentPath) != null);
                    menuItem.Checked = isOpened;
                    //menuItem.Enabled = !isOpened;
                    menuItem.ToolTipText = recentPath;

                    items.Add(menuItem);
				}

				return items.ToArray();
			}
			
			MenuCommand defaultMenu = new MenuCommand(
                "${res:Dialog.Componnents.RichMenuItem.NoRecentFilesString}");
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			FileService.OpenFile(item.Tag.ToString());
		}
	}

    public sealed class RecentProjectsMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;

            bool isSolutionOpened = (ProjectService.OpenSolution != null);
            string openedSolutionPath = String.Empty;
            if (isSolutionOpened)
            {
                openedSolutionPath = ProjectService.OpenSolution.FileName;
            }

            IList<RecentOpenItem> recentItems = 
                recentOpen.RecentDisplayableProjects;

            int itemCount = (recentItems == null) ? 0 : recentItems.Count;

            itemCount = Math.Min(itemCount, recentOpen.DisplayableProjects);
            if (itemCount > 0)
            {
                int letterA = Convert.ToInt32('A');
                string accelaratorKeyPrefix = null;
                int wantedLength = recentOpen.MaximumLength;

                MenuCommand[] items = new MenuCommand[itemCount];
                for (int i = 0; i < itemCount; ++i)
                {
                    RecentOpenItem recentItem = recentItems[i];

                    if (i < 9)
                    {
                        accelaratorKeyPrefix = "&" + (i + 1) + "    ";
                    }
                    else
                    {
                        accelaratorKeyPrefix = "&" + Convert.ToChar(letterA) + "    ";
                        letterA++;
                    }

                    string recentProject = recentItem.FullPath;
                    MenuCommand menuItem = new MenuCommand(accelaratorKeyPrefix +
                        RecentOpen.CompactPath(recentProject, wantedLength), 
                        new EventHandler(LoadRecentProject));
                    menuItem.Tag = recentProject;
                    menuItem.Description = StringParser.Parse(ResourceService.GetString(
                        "Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
                        new string[,] { { "PROJECT", recentProject } });

                    if (isSolutionOpened && String.Equals(openedSolutionPath,
                        recentProject, StringComparison.OrdinalIgnoreCase))
                    {
                        menuItem.Checked = true;
                        menuItem.Enabled = false;
                    }

                    menuItem.ToolTipText = recentProject;
                    if (recentItem.Pinned)
                    {
                        menuItem.Image = IconService.GetBitmap("Icons.16x16.PushPin");
                    }

                    items[i] = menuItem;
				}
				return items;
			}
			
			MenuCommand defaultMenu = new MenuCommand(
                "${res:Dialog.Componnents.RichMenuItem.NoRecentProjectsString}");
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}

		private void LoadRecentProject(object sender, EventArgs e)
		{
            WorkbenchSingleton.MainForm.Focus();

			MenuCommand item = (MenuCommand)sender;
			
			string fileName = item.Tag.ToString();
			
			FileUtility.ObservedLoad(new NamedFileOperationDelegate(
                ProjectService.LoadSolution), fileName);
		}
	}

    public sealed class ToolMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
            IList<ExternalTool> tools = ToolLoader.Tool;
            int itemCount = tools.Count;
            MenuCommand[] items = new MenuCommand[itemCount];
            for (int i = 0; i < itemCount; ++i)
            {
                ExternalTool tool = tools[i];
                MenuCommand item = new MenuCommand(tool.ToString(), new EventHandler(ToolEvt));
				item.Description = "Start tool " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
                Icon toolIcon = tool.SmallIcon;
                if (toolIcon != null)
                {
                    item.Image = toolIcon.ToBitmap();
                    toolIcon.Dispose();
                }
				items[i] = item;
			}

			return items;
		}

		/// <summary>
		/// This handler gets called when a tool in the Tool menu is clicked on.
		/// </summary>
		/// <param name="sender">The MenuCommand that sent the event.</param>
		/// <param name="e">Event arguments.</param>
		void ToolEvt(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			// TODO: ToolLoader.Tool should get a string indexor. Overloading List or making it a Dictionary<string,ExternalTool> would work.
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text != ToolLoader.Tool[i].ToString()) { continue; }
				ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
				
				// Set these to somewhat useful values in case StingParser.Parse() passes when being called on one of them.
				string command = tool.Command;
				string args = tool.Arguments;

				// This needs it's own try/catch because if parsing these messages fail, the catch block after
				// the second try would also throw because MessageService.ShowError() calls StringParser.Parse()
				try {
					command = StringParser.Parse(tool.Command);
					args    = StringParser.Parse(tool.Arguments);
				} catch (Exception ex) {
					MessageService.ShowError("${res:XML.MainMenu.ToolMenu.ExternalTools.ExecutionFailed} '" + ex.Message);
					return;
				}
				
				if (tool.PromptForArguments) {
					args = MessageService.ShowInputBox(tool.MenuCommand, "${res:XML.MainMenu.ToolMenu.ExternalTools.EnterArguments}", args);
					if (args == null)
						return;
				}
				
				try {
					if (tool.UseOutputPad) {
						ProcessRunner processRunner = new ProcessRunner();
                        processRunner.LogStandardOutputAndError = false;
						processRunner.ProcessExited += ProcessExitEvent;
						processRunner.OutputLineReceived += process_OutputLineReceived;
						processRunner.ErrorLineReceived += process_OutputLineReceived;
						processRunner.WorkingDirectory = StringParser.Parse(tool.InitialDirectory);
						if (args == null || args.Length == 0 || args.Trim('"', ' ').Length == 0) {
							processRunner.Start(command);
						} else {
							processRunner.Start(command, args);
						}
					} else {
						ProcessStartInfo startinfo;
						if (args == null || args.Length == 0 || args.Trim('"', ' ').Length == 0) {
							startinfo = new ProcessStartInfo(command);
						} else {
							startinfo = new ProcessStartInfo(command, args);
						}
						startinfo.WorkingDirectory = StringParser.Parse(tool.InitialDirectory);
						Process process = new Process();
						process.StartInfo = startinfo;
						process.Start();
					}
				} catch (Exception ex) {
					MessageService.ShowError("${res:XML.MainMenu.ToolMenu.ExternalTools.ExecutionFailed} '" + command + " " + args + "'\n" + ex.Message);
				}
				return;
			}
		}

		void ProcessExitEvent(object sender, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					ProcessRunner p = (ProcessRunner)sender;
                    TaskService.BuildMessageViewCategory.AppendLine(StringParser.Parse("${res:XML.MainMenu.ToolMenu.ExternalTools.ExitedWithCode} " + p.ExitCode));
                    p.Dispose();
				});
		}
		
		void process_OutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			TaskService.BuildMessageViewCategory.AppendLine(e.Line);
		}
	}

    public sealed class OpenContentsMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : MenuCheckBox
		{
			IWorkbenchWindow window;
			
			public MyMenuItem(IWorkbenchWindow window) 
                : base(StringParser.Parse(window.Title))
			{
				this.window = window;
                this.Image  = window.Icon.ToBitmap();

                this.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);

                this.DisplayStyle = ToolStripItemDisplayStyle.Text;

				this.Checked = true;

				window.SelectWindow();
			}
		}

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			int windowCount = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count;
			if (windowCount == 0) {
				return new ToolStripItem[] {};
			}
			ToolStripItem[] items = new ToolStripItem[windowCount + 2];
			items[0] = new MenuSeparator(null, null);
			for (int i = 0; i < windowCount; ++i) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[i];
				MenuCheckBox item = new MyMenuItem(window);
				item.Tag = window;
				item.Checked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == window;
				item.Description = "Activate this window";
				items[i + 1] = item;

                if (item.Checked)
                {
                    item.DisplayStyle = ToolStripItemDisplayStyle.Text;
                }
			}

            string windowsText = ResourceService.GetString("XML.MainMenu.WindowMenu.Windows") + "...";
            MenuCommand wndsItem = new MenuCommand(windowsText, new MoreWindows(), false);
            wndsItem.Description = "Access more windows";
            wndsItem.Image = IconService.GetBitmap("Icons.16x16.Windows");
            items[windowCount + 1] = wndsItem;

			return items;
		}
	}

    public sealed class ToolsViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Tools";
			}
		}
	}

    public sealed class MainViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Main";
			}
		}
	}

    public sealed class DebugViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Debugger";
			}
		}
	}
	
	public abstract class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : MenuCommand
		{
			PadDescriptor padDescriptor;
			
			public MyMenuItem(PadDescriptor padDescriptor) : base(null, null)
			{
				this.padDescriptor = padDescriptor;
				Text = StringParser.Parse(padDescriptor.Title);
				
				if (!string.IsNullOrEmpty(padDescriptor.Icon)) {
					base.Image = IconService.GetBitmap(padDescriptor.Icon);
				}
				
				if (padDescriptor.Shortcut != null) {
					ShortcutKeys = MenuCommand.ParseShortcut(padDescriptor.Shortcut);
				}
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				padDescriptor.BringPadToFront();
			}
			
		}

		protected abstract string Category {
			get;
		}
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> items = new List<ToolStripItem>();
			foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (padContent.Category == Category) {
					items.Add(new MyMenuItem(padContent));
				}
			}
			return items.ToArray();
		}
	}
	
    /// <summary>
    /// This builds the toolbars submenu with items corresponding to each main
    /// toolbar in the application.
    /// </summary>
    public sealed class ToolbarsMenuBuilder : ISubmenuBuilder
	{
        /// <summary>
        /// We cache the built items for speed, since the framework will continue to
        /// request the items on each drop-down event.
        /// </summary>
        private ToolStripMenuItem[] _menuItems;

        public ToolbarsMenuBuilder()
        {   
        }

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
            if (_menuItems != null && _menuItems.Length != 0)
            {
                int itemCount = _menuItems.Length;
                for (int i = 0; i < itemCount; i++)
                {
                    ToolStripMenuItem menuItem = _menuItems[i];
                    if (menuItem == null || menuItem.Tag == null)
                    {
                        continue;
                    }
                    ToolStrip toolStrip = menuItem.Tag as ToolStrip;
                    if (toolStrip != null)
                    {
                        menuItem.Checked = toolStrip.Visible;
                    }
                }

                return _menuItems;
            }

            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

            IList<ToolStrip> toolbars = ToolbarService.ToolStrips;
            if (toolbars != null && toolbars.Count > 0)
            {
                int itemCount = toolbars.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ToolStrip toolStrip = toolbars[i];

                    string toolName = toolStrip.Name;
                    string toolText = toolStrip.Text;
                    if (!String.IsNullOrEmpty(toolName) &&
                        !String.IsNullOrEmpty(toolText))
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem();

                        menuItem.Click += new EventHandler(OnToggleToolStrip);

                        menuItem.Name    = toolName;
                        menuItem.Text    = toolText;
                        menuItem.Checked = toolStrip.Visible;
                        menuItem.Tag     = toolStrip;

                        menuItems.Add(menuItem);
                    }
                }

                if (menuItems != null && menuItems.Count != 0)
                {
                    menuItems.Sort(new MenuItemComparer());

                    _menuItems = menuItems.ToArray();

                    return _menuItems;
                }
            }

            return menuItems.ToArray();
		}

        private void OnToggleToolStrip(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null || menuItem.Tag == null)
            {
                return;
            }

            ToolStrip toolStrip = menuItem.Tag as ToolStrip;
            if (toolStrip == null)
            {
                return;
            }
            bool makeVisible = !toolStrip.Visible;
            ToolbarService.SetVisibleToolStrip(toolStrip, makeVisible, true);

            menuItem.Checked = toolStrip.Visible;
        }
    }
	
//	public class IncludeFilesBuilder : ISubmenuBuilder
//	{
//		public ProjectBrowserView browser;
//
//		MyMenuItem includeInCompileItem;
//		MyMenuItem includeInDeployItem;
//
//		class MyMenuItem : MenuCheckBox
//		{
//			IncludeFilesBuilder builder;
//
//			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(null, null, name)
//			{
//				base.Click += handler;
//				this.builder = builder;
//			}
//
//			public override void UpdateStatus()
//			{
//				base.UpdateStatus();
//				if (builder == null) {
//					return;
//				}
//				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
//
//				if (node == null) {
//					return;
//				}
//
//				ProjectFile finfo = node.UserData as ProjectFile;
//				if (finfo == null) {
//					builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = false;
//				} else {
//					if (!builder.includeInCompileItem.Enabled) {
//						builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = true;
//					}
//					builder.includeInCompileItem.Checked = finfo.BuildAction == BuildAction.Compile;
//					builder.includeInDeployItem.Checked  = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
//				}
//			}
//		}
//
//		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
//		{
//			browser = (ProjectBrowserView)owner;
//			includeInCompileItem = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InCompile}", new EventHandler(ChangeCompileInclude));
//			includeInDeployItem  = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InDeploy}",  new EventHandler(ChangeDeployInclude));
//
//			return new ToolStripItem[] {
//				includeInCompileItem,
//				includeInDeployItem
//			};
//
//		}
//		void ChangeCompileInclude(object sender, EventArgs e)
//		{
//			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
//
//			if (node == null) {
//				return;
//			}
//
//			ProjectFile finfo = node.UserData as ProjectFile;
//			if (finfo != null) {
//				if (finfo.BuildAction == BuildAction.Compile) {
//					finfo.BuildAction = BuildAction.Nothing;
//				} else {
//					finfo.BuildAction = BuildAction.Compile;
//				}
//			}
//
//			ProjectService.SaveCombine();
//		}
//
//		void ChangeDeployInclude(object sender, EventArgs e)
//		{
//			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
//
//			if (node == null) {
//				return;
//			}
//
//			ProjectFile finfo = node.UserData as ProjectFile;
//			if (finfo != null) {
//				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
//					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
//				} else {
//					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
//				}
//			}
//
//			ProjectService.SaveCombine();
//		}
//	}
}
