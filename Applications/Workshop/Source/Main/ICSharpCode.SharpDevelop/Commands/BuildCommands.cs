// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4024 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public abstract class AbstractBuildMenuCommand : AbstractMenuCommand
	{
		public virtual bool CanRunBuild {
			get {
				return ProjectService.OpenSolution!=null;
			}
		}
		public virtual void BeforeBuild()
		{
			TaskService.BuildMessageViewCategory.ClearText();
			TaskService.InUpdate = true;
			TaskService.ClearExceptCommentTasks();
			TaskService.InUpdate = false;
			ICSharpCode.SharpDevelop.Commands.SaveAllFiles.SaveAll();
		}
		
		public virtual void AfterBuild() {}
		
		public override void Run()
		{
			if (CanRunBuild) {
				if (DebuggerService.IsDebuggerLoaded && DebuggerService.CurrentDebugger.IsDebugging) {
					if (MessageService.AskQuestion("${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}",
					                               "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}"))
					{
						DebuggerService.CurrentDebugger.Stop();
					} else {
						return;
					}
				}
				BeforeBuild();
				StartBuild();
			} else {
				AddNoSingleFileCompilationError();
			}
		}
		
		BuildResults lastBuildResults;
		
		public BuildResults LastBuildResults {
			get { return lastBuildResults; }
			protected set { lastBuildResults = value; }
		}
		
		protected void CallbackMethod(BuildResults results)
		{
			lastBuildResults = results;
			ShowResults(results);
			AfterBuild();
			OnBuildComplete(EventArgs.Empty);
		}
		
		public abstract void StartBuild();
		
		public event EventHandler BuildComplete;
		
		protected virtual void OnBuildComplete(EventArgs e)
		{
			if (BuildComplete != null) {
				BuildComplete(this, e);
			}
		}
		
		public static void ShowResults(BuildResults results)
		{
			if (results != null) {
				TaskService.InUpdate = true;
				foreach (BuildError error in results.Errors) {
					TaskService.Add(new Task(error));
				}
				TaskService.InUpdate = false;
				if (results.Errors.Count > 0 && ErrorListPad.ShowAfterBuild) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
				}
			}
		}
		
		/// <summary>
		/// Notifies the user that #develp's internal MSBuildEngine
		/// implementation only supports compiling solutions and projects;
		/// it does not allow compiling individual files.
		/// </summary>
		/// <remarks>Adds a message to the <see cref="TaskService"/> and
		/// shows the <see cref="ErrorListPad"/>.</remarks>
		public static void AddNoSingleFileCompilationError()
		{
			TaskService.Add(new Task(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
	}
	
	public class Build : AbstractBuildMenuCommand
	{
		public override void BeforeBuild()
		{
			base.BeforeBuild();
			ProjectService.RaiseEventStartBuild();
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild(new BuildEventArgs(LastBuildResults));
			base.AfterBuild();
		}
	}

    public sealed class BuildBeforeExecute : Build
	{
		public override void Run()
		{
			if (BuildModifiedProjectsOnlyService.Setting == BuildOnExecuteSetting.DoNotBuild) {
				LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
				OnBuildComplete(EventArgs.Empty);
			} else {
				base.Run();
			}
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(BuildModifiedProjectsOnlyService.WrapBuildable(ProjectService.OpenSolution),
			                       new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
	}

    public sealed class BuildProjectBeforeExecute : BuildProject
    {
        public BuildProjectBeforeExecute(IProject project)
            : base(project)
        {
        }

        public override void Run()
        {
            if (BuildModifiedProjectsOnlyService.Setting == BuildOnExecuteSetting.DoNotBuild)
            {
                LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
                OnBuildComplete(EventArgs.Empty);
            }
            else
            {
                base.Run();
            }
        }

        public override void StartBuild()
        {
            BuildEngine.BuildInGui(BuildModifiedProjectsOnlyService.WrapBuildable(this.ProjectToBuild),
                                   new BuildOptions(BuildTarget.Build, CallbackMethod));
        }
    }

    public sealed class Rebuild : Build
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Rebuild, CallbackMethod));
		}
	}

    public sealed class Clean : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Clean, CallbackMethod));
		}
	}
	
	public abstract class AbstractProjectBuildMenuCommand : AbstractBuildMenuCommand
	{
		protected IProject targetProject;
		protected IProject ProjectToBuild {
			get {
				return targetProject ?? ProjectService.CurrentProject;
			}
		}
		
		public override bool CanRunBuild {
			get {
				return base.CanRunBuild && this.ProjectToBuild != null;
			}
		}
	}

	public class BuildProject : AbstractProjectBuildMenuCommand
	{
		public BuildProject()
		{
		}
		public BuildProject(IProject targetProject)
		{
			this.targetProject = targetProject;
		}
		
		public override void BeforeBuild()
		{
			base.BeforeBuild();
			ProjectService.RaiseEventStartBuild();
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild(new BuildEventArgs(LastBuildResults));
			base.AfterBuild();
		}
	}

    public sealed class RebuildProject : BuildProject
	{
		public RebuildProject() {}
		public RebuildProject(IProject targetProject) : base(targetProject) {}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Rebuild, CallbackMethod));
		}
	}

    public sealed class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Clean, CallbackMethod));
		}
	}

    public sealed class AbortBuild : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			return new[] { new MenuItem() };
		}
		
		sealed class MenuItem : ToolStripMenuItem
		{
			public MenuItem()
			{
				WorkbenchSingleton.Workbench.ProcessCommandKey += OnProcessCommandKey;
				ResourceService.LanguageChanged += OnLanguageChanged;
				OnLanguageChanged(null, null);
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing) {
					WorkbenchSingleton.Workbench.ProcessCommandKey -= OnProcessCommandKey;
					ResourceService.LanguageChanged -= OnLanguageChanged;
				}
			}
			
			public override bool Enabled {
				get { return BuildEngine.IsGuiBuildRunning; }
			}
			
			void OnLanguageChanged(object sender, EventArgs e)
			{
                this.Image = IconService.GetBitmap("Icons.16x16.BuildCancel");
				this.Text = StringParser.Parse("${res:XML.MainMenu.BuildMenu.AbortBuild}");
				this.ShortcutKeyDisplayString = StringParser.Parse("${res:XML.MainMenu.BuildMenu.BreakKey}");
			}
			
			void OnProcessCommandKey(object sender, KeyEventArgs e)
			{
				// ToolStripMenuItem does not support Pause/Break as shortcut key, so we handle it manually
				if (e.KeyData == Keys.Pause) {
					if (Enabled) {
						LoggingService.Debug("BREAK was pressed, aborting build.");
						PerformClick();
						e.Handled = true;
					}
				}
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				BuildEngine.CancelGuiBuild();
			}
		}
	}

    public sealed class SetConfigurationMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new ToolStripItem[0];
			IList<string> configurationNames = ProjectService.OpenSolution.GetConfigurationNames();
			string activeConfiguration = ProjectService.OpenSolution.Preferences.ActiveConfiguration;
			ToolStripMenuItem[] items = new ToolStripMenuItem[configurationNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new ToolStripMenuItem(configurationNames[i]);
				items[i].Click += SetConfigurationItemClick;
				items[i].Checked = activeConfiguration == configurationNames[i];
			}
			return items;
		}
		
		void SetConfigurationItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActiveConfiguration = item.Text;
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}

    public sealed class SetPlatformMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new ToolStripItem[0];
			IList<string> platformNames = ProjectService.OpenSolution.GetPlatformNames();
			string activePlatform = ProjectService.OpenSolution.Preferences.ActivePlatform;
			ToolStripMenuItem[] items = new ToolStripMenuItem[platformNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new ToolStripMenuItem(platformNames[i]);
				items[i].Click += SetPlatformItemClick;
				items[i].Checked = activePlatform == platformNames[i];
			}
			return items;
		}
		
		void SetPlatformItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActivePlatform = item.Text;
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}

    public sealed class EditConfigurationsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor()) {
				sce.ShowDialog();
				ProjectService.SaveSolution();
				ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
		}
	}

    public sealed class BuildBreakCommand : AbstractButtonCommand
    {
        public BuildBreakCommand()
            : base(false)
        {   
        }

        public override bool IsEnabled
        {
            get
            {
                return BuildEngine.IsGuiBuildRunning;
            }

            set
            {
                base.IsEnabled = BuildEngine.IsGuiBuildRunning;
            }
        }

        public override void Run()
        {
            BuildEngine.CancelGuiBuild();
        }
    }

    public abstract class BuildComboBoxCommand : AbstractComboBoxCommand
    {
        protected BuildComboBoxCommand()
        {   
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem == null)
            {
                return;
            }

            toolbarItem.DropDown += new EventHandler(OnComboDropDown);
        }

        private void OnComboDropDown(object sender, EventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            try
            {
                ComboBox senderComboBox = toolbarItem.ComboBox;
                int width = senderComboBox.Width;
                Graphics g = senderComboBox.CreateGraphics();
                Font font = senderComboBox.Font;

                //checks if a scrollbar will be displayed.
                //If yes, then get its width to adjust the size of the drop down list.
                int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ? 
                SystemInformation.VerticalScrollBarWidth : 0;

                //Loop through list items and check size of each items.
                //set the width of the drop down list to the width of the largest item.

                int newWidth;
                foreach (string s in toolbarItem.Items)
                {
                    if (s != null)
                    {
                        string text = s.Trim();
                        newWidth = (int)g.MeasureString(text, font).Width + vertScrollBarWidth;
                        if (width < newWidth)
                        {
                            width = newWidth;
                        }
                    }
                }
                if (width > senderComboBox.DropDownWidth)
                {
                    senderComboBox.DropDownWidth = width + 12;
                }

                g.Dispose();
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }
    }

    public sealed class BuildConfigurationCommand : BuildComboBoxCommand
    {
        private int oldIndex;
        private int editIndex;

        public BuildConfigurationCommand()
        {
            ProjectService.SolutionLoaded += 
                new EventHandler<SolutionEventArgs>(OnSolutionLoaded);
            ProjectService.SolutionClosed += new EventHandler(OnSolutionClosed);
            ProjectService.SolutionConfigurationChanged += new SolutionConfigurationEventHandler(OnSolutionConfigurationChanged);
        }

        public override void Run()
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            if (toolbarItem == null)
            {
                return;
            }
            ComboBox comboBox = toolbarItem.ComboBox;
            if (comboBox == null)
            {
                return;
            }

            int selIndex = comboBox.SelectedIndex;
            if (selIndex == editIndex)
            {
                comboBox.SelectedIndex = oldIndex;

                Control parent = comboBox.Parent;
                if (parent != null)
                {
                    parent.Focus();
                }      

                using (SolutionConfigurationEditor dlg =
                    new SolutionConfigurationEditor())
                {
                    dlg.ShowDialog(); // TODO--It is 'standardly' implementated.
                    ProjectService.SaveSolution();
                    ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
                    ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
                }
            }
            else
            {
                oldIndex = selIndex;
            }
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
            toolbarItem.Enabled = (ProjectService.OpenSolution != null);
        }

        private void OnSolutionClosed(object sender, EventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            if (toolbarItem != null)
            {
                toolbarItem.Enabled = false;
            }
        }

        private void OnSolutionLoaded(object sender, SolutionEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem !=  null)
            {
                LoadItems(toolbarItem);
                toolbarItem.Enabled = true;
            }
        }

        private void OnSolutionConfigurationChanged(object sender, 
            SolutionConfigurationEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem != null)
            {
                LoadItems(toolbarItem);
            }
        }

        private void OnProjectConfigurationChanged(object sender, 
            ProjectConfigurationEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem != null)
            {
                LoadItems(toolbarItem);
            }
        }

        private void LoadItems(ToolBarComboBox toolbarItem)
        {
            oldIndex = -1;
            if (toolbarItem == null)
            {
                return;
            }
            ComboBox comboBox = toolbarItem.ComboBox;
            if (comboBox == null)
            {
                return;
            }
            comboBox.Items.Clear();

            Solution openedSolution = ProjectService.OpenSolution;
            if (openedSolution == null)
            {
                return;
            }
            IList<string> configNames = openedSolution.GetConfigurationNames();
            string activeConfig = openedSolution.Preferences.ActiveConfiguration;
            int selIndex  = 0;
            int itemCount = (configNames == null) ? 0 : configNames.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string configName = configNames[i];
                if (String.Equals(configName, activeConfig))
                {
                    selIndex = i;
                }
                comboBox.Items.Add(configName);
            }
            if (itemCount > 0)
            {
                string editItemText = StringParser.Parse(
                    "${res:Dialog.Options.CombineOptions.Configurations.ConfigurationManagerButton}...");
                editIndex = comboBox.Items.Add(editItemText);
                comboBox.SelectedIndex = selIndex;
                oldIndex = selIndex;
            }
        }
    }

    public sealed class BuildPlatformCommand : BuildComboBoxCommand
    {
        private int oldIndex;
        private int editIndex;

        public BuildPlatformCommand()
        {
            ProjectService.SolutionLoaded +=
               new EventHandler<SolutionEventArgs>(OnSolutionLoaded);
            ProjectService.SolutionClosed += new EventHandler(OnSolutionClosed);
            ProjectService.SolutionConfigurationChanged += new SolutionConfigurationEventHandler(OnSolutionConfigurationChanged);
        }

        public override void Run()
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            if (toolbarItem == null)
            {
                return;
            }
            ComboBox comboBox = toolbarItem.ComboBox;
            if (comboBox == null)
            {
                return;
            }

            int selIndex = comboBox.SelectedIndex;
            if (selIndex == editIndex)
            {
                comboBox.SelectedIndex = oldIndex;

                Control parent = comboBox.Parent;
                if (parent != null)
                {
                    parent.Focus();
                }

                using (SolutionConfigurationEditor dlg =
                    new SolutionConfigurationEditor())
                {
                    dlg.ShowDialog(); // TODO--It is 'standardly' implementated.
                    ProjectService.SaveSolution();
                    ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
                    ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
                }
            }
            else
            {
                oldIndex = selIndex;
            }
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            toolbarItem.Enabled = (ProjectService.OpenSolution != null);
        }


        private void OnSolutionClosed(object sender, EventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            if (toolbarItem != null)
            {
                toolbarItem.Enabled = false;
            }
        }

        private void OnSolutionLoaded(object sender, SolutionEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem != null)
            {
                LoadItems(toolbarItem);
                toolbarItem.Enabled = true;
            }
        }

        private void OnSolutionConfigurationChanged(object sender,
            SolutionConfigurationEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem != null)
            {
                LoadItems(toolbarItem);
            }
        }

        private void OnProjectConfigurationChanged(object sender,
            ProjectConfigurationEventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            if (toolbarItem != null)
            {
                LoadItems(toolbarItem);
            }
        }

        private void LoadItems(ToolBarComboBox toolbarItem)
        {
            oldIndex = -1;
            if (toolbarItem == null)
            {
                return;
            }
            ComboBox comboBox = toolbarItem.ComboBox;
            if (comboBox == null)
            {
                return;
            }
            comboBox.Items.Clear();

            Solution openedSolution = ProjectService.OpenSolution;
            if (openedSolution == null)
            {
                return;
            }
            IList<string> platformNames = openedSolution.GetPlatformNames();
            string activePlatform = openedSolution.Preferences.ActivePlatform;
            int selIndex  = 0;
            int itemCount = (platformNames == null) ? 0 : platformNames.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string platformName = platformNames[i];
                if (String.Equals(platformName, activePlatform))
                {
                    selIndex = i;
                }
                comboBox.Items.Add(platformName);
            }
            if (itemCount > 0)
            {
                string editItemText = StringParser.Parse(
                    "${res:Dialog.Options.CombineOptions.Configurations.ConfigurationManagerButton}...");
                editIndex = comboBox.Items.Add(editItemText);
                comboBox.SelectedIndex = selIndex;
                oldIndex = selIndex;
            }
        }
    }
}
