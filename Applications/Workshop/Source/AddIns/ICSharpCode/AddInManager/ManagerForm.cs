// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3806 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;
using AddInManagerEx = ICSharpCode.Core.AddInManager;

namespace ICSharpCode.AddInManager
{
    public partial class ManagerForm : Form
    {
        #region Private Fields

        private bool _allowMultipleSelection;
        private Font boldFont;
        private string _wndTitle;
        private ToolStrip toolStrip;
        private IEnumerable<string> installFileNames;

        private AddInActionHelper _uninstallCommand;
        private AddInActionHelper _actionsCommand;

        #endregion

        #region Constructors and Destructor

        public ManagerForm()
		{
            _allowMultipleSelection = false;

			InitializeComponent();

            _uninstallCommand = new AddInActionHelper("Uninstall");
            _uninstallCommand.Click += new EventHandler(UninstallButtonClick);

            _actionsCommand = new AddInActionHelper("RunAction");
            _actionsCommand.Click += new EventHandler(RunActionButtonClick);

            this.ShowIcon = true;
            this.Icon     = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");
			
            this.Owner = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm;

            //actionDescription.Visible = false;
            try
            {
                actionDescription.Font = new Font("Verdana", 8, FontStyle.Bold);
            }
            catch
            {
                actionDescription.Font = new Font(actionDescription.Font, FontStyle.Bold);
            }
            actionDescription.ForeColor = SystemColors.Highlight;

            actionGroupBox.Text = ResourceService.GetString("AddInManager.Requirements");
            _uninstallCommand.Text = ResourceService.GetString("AddInManager.ActionUninstall");

            //installAddIn.Text = ResourceService.GetString("AddInManager.InstallButton");
            preinstalledCheckBox.Text = ResourceService.GetString("AddInManager.ShowPreinstalledAddIns");
			
            this.Text = ResourceService.GetString("AddInManager.Title");
            RightToLeftConverter.ConvertRecursive(this);

            toolStrip = ToolbarService.CreateToolStrip(this, "/AddIns/AddInManager/Toolbar");
            toolStrip.Stretch = true;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            // Indent the toolbar items to beautify it...
            ToolbarService.IndentItems(toolStrip);

            this.Controls.AddRange(new Control[] { toolStrip });
            toolStrip.Renderer = ToolbarService.StretchedRenderer;

			CreateAddInList();
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Initialization

        void CreateAddInList()
		{
			Stack<AddInControl> stack = new Stack<AddInControl>();
			int index = 0;
			AddInControl addInControl;
			
			List<AddIn> addInList = new List<AddIn>(AddInTree.AddIns);
			addInList.Sort(delegate(AddIn a, AddIn b) {
			               	return a.Name.CompareTo(b.Name);
			               });
            bool hasPreinstalledAddIns = false;
            foreach (AddIn addIn in addInList)
            {
                if (String.Equals(addIn.Properties["addInManagerHidden"], "true", 
                    StringComparison.OrdinalIgnoreCase) && 
                    IsInstalledInApplicationRoot(addIn))
                {
                    hasPreinstalledAddIns = true;
                    continue;
                }
                addInControl          = new AddInControl(addIn, this);
				addInControl.Dock     = DockStyle.Top;
				addInControl.TabIndex = index++;
				stack.Push(addInControl);
				addInControl.Enter += OnControlEnter;
                //addInControl.Click += OnControlClick;
                //addInControl.DoubleClick += OnControlClick;
                addInControl.MouseDoubleClick += new MouseEventHandler(OnControlMouseClick);
                addInControl.MouseClick += new MouseEventHandler(OnControlMouseClick);
			}
			while (stack.Count > 0) {
				splitContainer.Panel1.Controls.Add(stack.Pop());
			}
			OnShowPreinstalledAddIns(null, null);
            if (!hasPreinstalledAddIns)
            {
                preinstalledCheckBox.Enabled = false;
            }
            splitContainer.Panel2Collapsed = true;
		}
		
		void RefreshAddInList()
		{
			List<AddIn> oldSelected = selected;
			foreach (Control ctl in splitContainer.Panel1.Controls) {
				ctl.Dispose();
			}
			splitContainer.Panel1.Controls.Clear();
			CreateAddInList();
			if (oldSelected != null) {
				foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
					if (oldSelected.Contains(ctl.AddIn))
						ctl.Selected = true;
				}
			}
			UpdateActionBox();
        }

        #endregion

        #region AddInList-Management

        int visibleAddInCount = 0;
		
		private void OnShowPreinstalledAddIns(object sender, EventArgs e)
		{
			visibleAddInCount = 0;
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = false;
				bool visible;
                if (preinstalledCheckBox.Checked)
                {
					visible = true;
				} else {
					if (ctl == oldFocus)
						oldFocus = null;
                    if (IsInstalledInApplicationRoot(ctl.AddIn))
                    {
                        visible = !String.Equals(
                            ctl.AddIn.Properties["addInManagerHidden"], 
                            "preinstalled", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        visible = true;
                    }
                }
				if (visible)
					visibleAddInCount += 1;
				ctl.Visible = visible;
			}
			UpdateActionBox();
		}

        static bool IsInstalledInApplicationRoot(AddIn addin)
        {
            return FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, 
                addin.FileName);
        }

        void OnControlMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {   
			    // clicking again on already focused item:
			    // remove selection of other items / or with Ctrl: toggle selection
			    if (((Control)sender).Focused)
				    OnControlEnter(sender, e);
            }
        }
		
		AddInControl oldFocus;
		bool ignoreFocusChange;
		
		void OnControlEnter(object sender, EventArgs e)
		{
			if (ignoreFocusChange)
				return;
            if (_allowMultipleSelection)
            {
                bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control;
                if ((ModifierKeys & Keys.Shift) == Keys.Shift && sender != oldFocus)
                {
                    bool sel = false;
                    foreach (AddInControl ctl in splitContainer.Panel1.Controls)
                    {
                        if (!ctl.Visible) continue;
                        if (ctl == sender || ctl == oldFocus)
                        {
                            sel = !sel;
                            ctl.Selected = true;
                        }
                        else
                        {
                            if (sel || !ctrl)
                            {
                                ctl.Selected = sel;
                            }
                        }
                    }
                }
                else if (ctrl)
                {
                    foreach (AddInControl ctl in splitContainer.Panel1.Controls)
                    {
                        if (ctl == sender)
                            ctl.Selected = !ctl.Selected;
                    }
                    oldFocus = (AddInControl)sender;
                }
                else
                {
                    foreach (AddInControl ctl in splitContainer.Panel1.Controls)
                    {
                        ctl.Selected = ctl == sender;
                    }
                    oldFocus = (AddInControl)sender;
                }
            }
            else
            {
                foreach (AddInControl ctl in splitContainer.Panel1.Controls)
                {
                    ctl.Selected = ctl == sender;
                }
                oldFocus = (AddInControl)sender;
            }

			UpdateActionBox();
		}

		#endregion
		
		#region UpdateActionBox

		List<AddIn> selected;

        public AddIn SelectedAddIn
        {
            get
            {
                if (selected != null && selected.Count == 1)
                {
                    return selected[0];
                }

                return null;
            }
        }

		AddInAction selectedAction;
		
		static bool IsErrorAction(AddInAction action)
		{
			return action == AddInAction.DependencyError
				|| action == AddInAction.InstalledTwice
				|| action == AddInAction.CustomError;
		}
		
		void UpdateActionBox()
		{
			ignoreFocusChange = true;
			selected = new List<AddIn>();
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				if (ctl.Selected)
					selected.Add(ctl.AddIn);
			}
			splitContainer.Panel2Collapsed = selected.Count == 0;
			if (selected.Count > 0) 
            {
				dependencyTable.Visible = false;
				_actionsCommand.Visible = true;
				_uninstallCommand.Visible = true;
				
				bool allHaveIdentity = true;
				bool allEnabled      = true;
				bool allDisabled     = true;
				bool allInstalling   = true;
				bool allUninstalling = true;
				bool allUpdating     = true;
				bool allUninstallable = true;
				bool hasErrors = false;
				foreach (AddIn addIn in selected) {
					if (addIn.Manifest.PrimaryIdentity == null) {
						allHaveIdentity = false;
						break;
					}
					allEnabled      &= addIn.Action == AddInAction.Enable;
					if (IsErrorAction(addIn.Action))
						hasErrors = true;
					else
						allDisabled     &= addIn.Action == AddInAction.Disable;
					allUpdating     &= addIn.Action == AddInAction.Update;
					allInstalling   &= addIn.Action == AddInAction.Install;
					allUninstalling &= addIn.Action == AddInAction.Uninstall;
					if (allUninstallable) {
                        if (IsInstalledInApplicationRoot(addIn))
                        {
                            allUninstallable = false;
                        }
                    }
				}
				if (allEnabled == true || allHaveIdentity == false) {
                    _actionsCommand.Undoable = false;

					selectedAction = AddInAction.Disable;
					//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionDisable");
					//actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionDisable");
					//_actionsCommand.Text = ResourceService.GetString("AddInManager.ActionDisable");
					if (allHaveIdentity)
						_actionsCommand.Enabled = ShowDependencies(selected, ShowDependencyMode.Disable);
					else
						_actionsCommand.Enabled = false;
					_uninstallCommand.Enabled = allUninstallable && _actionsCommand.Enabled;
				} else if (allDisabled) {
                    _actionsCommand.Undoable = false;

                    selectedAction = AddInAction.Enable;
					//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionEnable");
					//actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionEnable");
					//_actionsCommand.Text = ResourceService.GetString("AddInManager.ActionEnable");
					_actionsCommand.Enabled = ShowDependencies(selected, ShowDependencyMode.Enable);
					if (hasErrors)
						_actionsCommand.Enabled = false;
					_uninstallCommand.Enabled = allUninstallable;
				} else if (allInstalling) {
                    _actionsCommand.Undoable = true;
                    _actionsCommand.UndoText = ResourceService.GetString("AddInManager.DescriptionCancelInstall");
                    _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelInstallation");

                    selectedAction = AddInAction.Uninstall;
					//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelInstallation");
					//actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelInstall");
					
                    //_actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelInstallation");
					_actionsCommand.Enabled = ShowDependencies(selected, ShowDependencyMode.Disable);
					_uninstallCommand.Visible = false;
				} else if (allUninstalling) {
                    _actionsCommand.Undoable = true;
                    _actionsCommand.UndoText = ResourceService.GetString("AddInManager.DescriptionCancelDeinstallation");
                    _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelDeinstallation");

                    selectedAction = AddInAction.Enable;
					//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelDeinstallation");
					//actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelDeinstallation");
					//_actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelDeinstallation");
					_actionsCommand.Enabled = ShowDependencies(selected, ShowDependencyMode.Enable);
					_uninstallCommand.Visible = false;
				} else if (allUpdating) {
                    _actionsCommand.Undoable = true;
                    _actionsCommand.Text = ResourceService.GetString("AddInManager.DescriptionCancelUpdate");
                    _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelUpdate");

                    selectedAction = AddInAction.InstalledTwice;
					//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelUpdate");
					//actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelUpdate");
					//_actionsCommand.Text = ResourceService.GetString("AddInManager.ActionCancelUpdate");
					_actionsCommand.Enabled = ShowDependencies(selected, ShowDependencyMode.CancelUpdate);
					_uninstallCommand.Visible = false;
				} else {
                    _actionsCommand.Undoable = false;

                    //actionGroupBox.Text = "";
                    actionDescription.Visible = true;
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionInconsistentSelection");
					_actionsCommand.Visible = false;
					_uninstallCommand.Visible = false;
				}
			}
			ignoreFocusChange = false;

            if (selected != null && selected.Count == 1)
            {
                this.ShowAboutAddIn(selected[0]);
            }
            ToolbarService.UpdateToolbar(toolStrip);
		}
		
		enum ShowDependencyMode {
			Disable,
			Enable,
			CancelUpdate
		}
		
		bool ShowDependencies(IList<AddIn> addIns, ShowDependencyMode mode)
		{
			List<AddInReference> dependencies = new List<AddInReference>(); // only used with enable=true
			List<KeyValuePair<AddIn, AddInReference>> dependenciesToSel = new List<KeyValuePair<AddIn, AddInReference>>();
			Dictionary<string, Version> addInDict = new Dictionary<string, Version>();
			Dictionary<string, Version> modifiedAddIns = new Dictionary<string, Version>();
			
			// add available addins
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Action != AddInAction.Enable && addIn.Action != AddInAction.Install)
					continue;
				if (addIns.Contains(addIn))
					continue;
				foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
					addInDict[pair.Key] = pair.Value;
				}
			}
			
			// create list of modified addin names
			foreach (AddIn addIn in addIns) {
				foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
					modifiedAddIns[pair.Key] = pair.Value;
				}
			}
			
			// add new addins
			if (mode != ShowDependencyMode.Disable) {
				foreach (AddIn addIn in addIns) {
					if (mode == ShowDependencyMode.CancelUpdate && !addIn.Enabled) {
						continue;
					}
					foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
						addInDict[pair.Key] = pair.Value;
					}
					foreach (AddInReference dep in addIn.Manifest.Dependencies) {
						if (!dependencies.Contains(dep))
							dependencies.Add(dep);
					}
				}
			}
			
			// add dependencies to the to-be-changed addins
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Action != AddInAction.Enable && addIn.Action != AddInAction.Install)
					continue;
				if (addIns.Contains(addIn))
					continue;
				foreach (AddInReference dep in addIn.Manifest.Dependencies) {
					if (modifiedAddIns.ContainsKey(dep.Name)) {
						dependenciesToSel.Add(new KeyValuePair<AddIn, AddInReference>(addIn, dep));
					}
				}
			}
			
			foreach (Control ctl in dependencyTable.Controls) {
				ctl.Dispose();
			}
			dependencyTable.Controls.Clear();
			bool allDepenciesOK = true;
			if (dependencies.Count > 0 || dependenciesToSel.Count > 0) {
				if (dependencies.Count == 0) {
					dependencyTable.RowCount = 1 + dependenciesToSel.Count;
				} else if (dependenciesToSel.Count == 0) {
					dependencyTable.RowCount = 1 + dependencies.Count;
				} else {
					dependencyTable.RowCount = 2 + dependencies.Count + dependenciesToSel.Count;
				}
				while (dependencyTable.RowStyles.Count < dependencyTable.RowCount) {
					dependencyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				}
				int rowIndex = 0;
				if (dependencies.Count > 0) {
					AddLabelRow(rowIndex++, ResourceService.GetString("AddInManager.RequiredDependencies"));
					foreach (AddInReference dep in dependencies) {
						if (!AddDependencyRow(addInDict, dep, rowIndex++, null))
							allDepenciesOK = false;
					}
				}
				if (dependenciesToSel.Count > 0) {
					AddLabelRow(rowIndex++, ResourceService.GetString("AddInManager.RequiredBy"));
					foreach (KeyValuePair<AddIn, AddInReference> pair in dependenciesToSel) {
						if (!AddDependencyRow(addInDict, pair.Value, rowIndex++, pair.Key.Name))
							allDepenciesOK = false;
					}
				}
				dependencyTable.Visible = true;
			}
			return allDepenciesOK;
		}
		
		bool AddDependencyRow(Dictionary<string, Version> addInDict, AddInReference dep, int rowIndex, string requiredByName)
		{
			string text = requiredByName ?? GetDisplayName(dep.Name);
			Version versionFound;
			Label label = new Label();
			label.AutoSize = true;
			label.Text = text;
			PictureBox box = new PictureBox();
			box.BorderStyle = BorderStyle.None;
			box.Size = new Size(16, 16);
			bool isOK = dep.Check(addInDict, out versionFound);
			box.SizeMode = PictureBoxSizeMode.CenterImage;
			box.Image = isOK ? WinFormsResourceService.GetBitmap("Icons.16x16.OK") : WinFormsResourceService.GetBitmap("Icons.16x16.Clear");
			dependencyTable.Controls.Add(label, 1, rowIndex);
			dependencyTable.Controls.Add(box,   0, rowIndex);
			return isOK;
		}
		
		void AddLabelRow(int rowIndex, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Text = text;
			dependencyTable.Controls.Add(label, 0, rowIndex);
			dependencyTable.SetColumnSpan(label, 2);
		}
		
		string GetDisplayName(string identity)
		{
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Manifest.Identities.ContainsKey(identity))
					return addIn.Name;
			}
			return identity;
		}

		#endregion
		
		#region Install new AddIns

        public void TryInstall()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = ResourceService.GetString("AddInManager.FileFilter");
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (ShowInstallableAddIns(dlg.FileNames))
                    {
                        if (_actionsCommand.Visible && _actionsCommand.Enabled)
                            _actionsCommand.PerformClick();
                    }
                }
            }
        }

        public void BeginInstall(ICollection<string> fileNames)
        {
            if (fileNames == null && fileNames.Count == 0)
            {
                return;
            }

            installFileNames = fileNames;
        }

        public void EndInstall()
        {
            installFileNames = null;
        }
		
		public bool ShowInstallableAddIns(IEnumerable<string> fileNames)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = false;
			}
			UpdateActionBox();
			List<InstallableAddIn> list = new List<InstallableAddIn>();
			foreach (string file in fileNames) {
				try {
					// Same file-extension check is in Panel1DragEnter
					switch (Path.GetExtension(file).ToLowerInvariant()) {
						case ".addin":
							if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, file)) {
								MessageService.ShowMessage("${res:AddInManager.CannotInstallIntoApplicationDirectory}");
								return false;
							}
							list.Add(new InstallableAddIn(file, false));
							break;
						case ".sdaddin":
						case ".zip":
							list.Add(new InstallableAddIn(file, true));
							break;
						default:
							MessageService.ShowMessage("${res:AddInManager.UnknownFileFormat} " + Path.GetExtension(file));
							return false;
					}
				} catch (AddInLoadException ex) {
					MessageService.ShowMessage("Error loading " + file + ":\n" + ex.Message);
					return false;
				}
			}
			ShowInstallableAddIns(list);

			return true;
		}
		
		IList<InstallableAddIn> shownAddInPackages;
		
		void ShowInstallableAddIns(IList<InstallableAddIn> addInPackages)
		{
			shownAddInPackages = addInPackages;
			ignoreFocusChange = true;
			splitContainer.Panel2Collapsed = false;
			dependencyTable.Visible = false;
			_actionsCommand.Visible = true;
			_uninstallCommand.Visible = false;
			
			selectedAction = AddInAction.Install;
			List<string> installAddIns = new List<string>();
			List<string> updateAddIns = new List<string>();
			foreach (InstallableAddIn addInPackage in addInPackages) {
				string identity = addInPackage.AddIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn addIn in AddInTree.AddIns) {
					if (addIn.Action != AddInAction.Install
					    && addIn.Manifest.Identities.ContainsKey(identity))
					{
						foundAddIn = addIn;
						break;
					}
				}
				if (foundAddIn != null) {
					updateAddIns.Add(addInPackage.AddIn.Name);
				} else {
					installAddIns.Add(addInPackage.AddIn.Name);
				}
			}
			
			if (updateAddIns.Count == 0) {
				//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionInstall");
                _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionInstall");
			} else if (installAddIns.Count == 0) {
				//actionGroupBox.Text = _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionUpdate");
                _actionsCommand.Text = ResourceService.GetString("AddInManager.ActionUpdate");
			} else {
				//actionGroupBox.Text = _actionsCommand.Text =
                _actionsCommand.Text =
					ResourceService.GetString("AddInManager.ActionInstall")
					+ " + " +
					ResourceService.GetString("AddInManager.ActionUpdate");
			}
			List<AddIn> addInList = new List<AddIn>();
			StringBuilder b = new StringBuilder();
			if (installAddIns.Count == 1) {
				b.Append("Installs the AddIn " + installAddIns[0]);
			} else if (installAddIns.Count > 1) {
				b.Append("Installs the AddIns " + string.Join(",", installAddIns.ToArray()));
			}
			if (updateAddIns.Count > 0 && installAddIns.Count > 0)
				b.Append("; ");
			if (updateAddIns.Count == 1) {
				b.Append("Updates the AddIn " + updateAddIns[0]);
			} else if (updateAddIns.Count > 1) {
				b.Append("Updates the AddIns " + string.Join(",", updateAddIns.ToArray()));
			}
			
            //actionDescription.Text = b.ToString();
			_actionsCommand.Enabled = ShowDependencies(addInList, ShowDependencyMode.Enable);
		}
		
		void RunInstallation()
		{
			// install new AddIns
			foreach (InstallableAddIn addInPackage in shownAddInPackages) {
				string identity = addInPackage.AddIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn addIn in AddInTree.AddIns) {
					if (addIn.Manifest.Identities.ContainsKey(identity)) {
						foundAddIn = addIn;
						break;
					}
				}
				if (foundAddIn != null) {
					addInPackage.Install(true);
					if (foundAddIn.Action != AddInAction.Enable) {
						AddInManagerEx.Enable(new AddIn[] { foundAddIn });
					}
					if (foundAddIn.Action != AddInAction.Install) {
						foundAddIn.Action = AddInAction.Update;
					}
				} else {
					addInPackage.Install(false);
				}
			}
			RefreshAddInList();
		}
		#endregion
		
		#region Uninstall AddIns
		void UninstallButtonClick(object sender, EventArgs e)
		{
			AddInManagerEx.RemoveExternalAddIns(selected);
			InstallableAddIn.Uninstall(selected);
			RefreshAddInList();
		}
		#endregion
		
		#region Drag'N'Drop Operations

		void Panel1DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.None;
				return;
			}
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			int addInCount = 0;
			int packageCount = 0;
			foreach (string file in files) {
				switch (Path.GetExtension(file).ToLowerInvariant()) {
					case ".addin":
						addInCount += 1;
						break;
					case ".sdaddin":
					case ".zip":
						packageCount += 1;
						break;
					default:
						e.Effect = DragDropEffects.None;
						return;
				}
			}
			if (addInCount == 0 && packageCount == 0) {
				e.Effect = DragDropEffects.None;
			} else if (addInCount == 0) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.Link;
			}
		}
		
		void Panel1DragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			ShowInstallableAddIns((string[])e.Data.GetData(DataFormats.FileDrop));
		}

		#endregion

        #region AddIn Actions

        public AddInActionHelper ActionsCommand
        {
            get
            {
                return _actionsCommand;
            }
        }

        public bool CanUninstall
        {
            get
            {
                if (selected != null && selected.Count == 1)
                {
                    AddIn selItem = selected[0];
                    if (String.Equals(selItem.Manifest.PrimaryIdentity,
                        "ICSharpCode.AddInManager", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                else if (selected == null || selected.Count == 0)
                {
                    return false;
                }

                return (_uninstallCommand.Visible && _uninstallCommand.Enabled);
            }
        }

        public bool CanRun(AddInAction action)
        {
            if (action == AddInAction.Disable || action == AddInAction.Uninstall)
            {
                if (selected != null && selected.Count == 1)
                {
                    AddIn selItem = selected[0];
                    if (String.Equals(selItem.Manifest.PrimaryIdentity,
                        "ICSharpCode.AddInManager", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return (selectedAction == action &&
                _actionsCommand.Visible && _actionsCommand.Enabled);
        }

        public void TryRunAction(AddIn addIn, AddInAction action)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = ctl.AddIn == addIn;
			}
			UpdateActionBox();
			if (selectedAction == action && _actionsCommand.Visible && _actionsCommand.Enabled)
				_actionsCommand.PerformClick();
		}
		
		public void TryUninstall(AddIn addIn)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = ctl.AddIn == addIn;
			}
			UpdateActionBox();
			if (_uninstallCommand.Visible && _uninstallCommand.Enabled)
				_uninstallCommand.PerformClick();
		}
		
		private void RunActionButtonClick(object sender, EventArgs e)
		{
			switch (selectedAction) 
            {
				case AddInAction.Disable:
					for (int i = 0; i < selected.Count; i++) 
                    {
                        AddIn selItem = selected[i];
                        if (String.Equals(selItem.Manifest.PrimaryIdentity,
                            "ICSharpCode.AddInManager", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageService.ShowMessage(
                                "${res:AddInManager.CannotDisableAddInManager}");
							selected.RemoveAt(i--);
						}
					}
					AddInManagerEx.Disable(selected);
					break;
				case AddInAction.Enable:
					AddInManagerEx.Enable(selected);
					break;
				case AddInAction.Install:
					RunInstallation();
					return;
				case AddInAction.Uninstall:
					UninstallButtonClick(sender, e);
					return;
				case AddInAction.InstalledTwice: // used to cancel installation of update
					InstallableAddIn.CancelUpdate(selected);
					foreach (AddIn addIn in selected) {
						addIn.Action = addIn.Enabled ? AddInAction.Enable : AddInAction.Disable;
					}
					break;
				default:
					throw new NotImplementedException();
			}
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Invalidate();
			}
			UpdateActionBox();
        }

        #endregion

        #region About Add-In

        public bool IsAboutAddInVisible
        {
            get
            {
                return !aboutSplitContainer.Panel2Collapsed;
            }
        }

        public void TryToggleAboutAddIn()
        {
            bool isVisible = this.IsAboutAddInVisible;
            if (isVisible)
            {
                aboutSplitContainer.Panel2Collapsed = true;
            }
            else
            {
                aboutSplitContainer.Panel2Collapsed = false;

                if (selected != null && selected.Count == 1)
                {
                    this.ShowAboutAddIn(selected[0]);
                }
            }
            ToolbarService.UpdateToolbar(toolStrip);
        }

		private void ShowAboutAddIn(AddIn addIn)
		{
            if (addIn == null)
            {
                actionDescription.Text = String.Empty;
                return;
            }

            if (!String.IsNullOrEmpty(_wndTitle))
            {
                this.Text = _wndTitle + " > " + addIn.Name;
                actionDescription.Text = addIn.Name;
            }
            if (aboutSplitContainer.Panel2Collapsed)
            {
                return;
            }

            try
            {
                aboutTablePanel.SuspendLayout();

                if (boldFont == null)
                {
                    try
                    {
                        Font aFont = new Font("Tahoma", 8, FontStyle.Regular);
                        boldFont = new Font(aFont, FontStyle.Bold);

                        aboutTablePanel.Font = aFont;
                    }
                    catch
                    {
                        boldFont = new Font(aboutTablePanel.Font, FontStyle.Bold);
                    }
                }
                aboutTablePanel.Controls.Clear();

                List<string> titles = new List<string>();
                List<string> values = new List<string>();

                closeButton.Text = ResourceService.GetString("Global.CloseButtonText");

                titles.Add("AddIn Name");
                values.Add(addIn.Name);

                titles.Add("Internal Name");
                values.Add(addIn.Manifest.PrimaryIdentity);

                if (addIn.Manifest.PrimaryVersion != null && addIn.Manifest.PrimaryVersion.ToString() != "0.0.0.0")
                {
                    titles.Add("Version");
                    values.Add(addIn.Manifest.PrimaryVersion.ToString());
                }

                if (addIn.Properties["author"].Length > 0)
                {
                    string authors = addIn.Properties["author"];
                    string[] listAuthors = authors.Split(',');
                    for (int i = 0; i < listAuthors.Length; i++)
                    {
                        titles.Add("Author");
                        values.Add(listAuthors[i].Trim());
                    }
                }

                if (addIn.Properties["copyright"].Length > 0)
                {
                    if (!addIn.Properties["copyright"].StartsWith("prj:"))
                    {
                        titles.Add("Copyright");
                        values.Add(addIn.Properties["copyright"]);
                    }
                }

                if (addIn.Properties["license"].Length > 0)
                {
                    titles.Add("License");
                    values.Add(addIn.Properties["license"]);
                }

                if (addIn.Properties["url"].Length > 0)
                {
                    titles.Add("Website");
                    values.Add(addIn.Properties["url"]);
                }

                if (addIn.Properties["description"].Length > 0)
                {
                    titles.Add("Description");
                    values.Add(addIn.Properties["description"]);
                }

                titles.Add("AddIn File");
                values.Add(Path.GetFullPath(addIn.FileName));

                aboutTablePanel.RowCount = titles.Count + 1;
                aboutTablePanel.RowStyles.Clear();
                for (int i = 0; i < titles.Count; i++)
                {
                    aboutTablePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    AddRow(titles[i], values[i], i);
                }
            }
            finally
            {
                //aboutTablePanel.AutoScrollMargin  = new Size(3, 3);        
                Size availSize   = aboutSplitContainer.Panel2.ClientSize;
                Padding padding  = aboutTablePanel.Padding;
                availSize.Width  = availSize.Width  - padding.Left - padding.Right;
                availSize.Height = availSize.Height - padding.Top  - padding.Bottom;
                aboutTablePanel.AutoScrollMinSize = availSize;

                aboutTablePanel.ResumeLayout(true);
            }
		}
		
		void AddRow(string desc, string val, int rowIndex)
		{
			Label descLabel = new Label();
			//descLabel.AutoSize = true;
            descLabel.TextAlign = ContentAlignment.MiddleLeft;
            descLabel.Height = 18;
			descLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			descLabel.Font = boldFont;
			descLabel.Text = StringParser.Parse(desc) + ":";
			aboutTablePanel.Controls.Add(descLabel, 0, rowIndex);
			
			Label valLabel;
			string link = GetLink(val);
			if (link != null) 
            {
				LinkLabel linkLabel = new LinkLabel();
				linkLabel.LinkClicked += delegate 
                {
					try 
                    {
						Process.Start(link);
					} 
                    catch (Exception ex) 
                    {
						MessageService.ShowMessage(ex.ToString());
					}
				};
				valLabel = linkLabel;
			} 
			else if (String.Equals(desc, "AddIn File", 
                StringComparison.OrdinalIgnoreCase)) 
            {
				LinkLabel linkLabel = new LinkLabel();
				linkLabel.LinkClicked += delegate 
                {
					try 
                    {
                        if (!String.IsNullOrEmpty(val) && File.Exists(val))
                        {
                            OpenFolderContainingFile.OpenContainingFolderInExplorer(val);
                        }
					} 
                    catch (Exception ex) 
                    {
						MessageService.ShowMessage(ex.ToString());
					}
				};
                linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
				valLabel = linkLabel;
			} 
            else 
            {
				valLabel = new Label();
			}
			valLabel.AutoSize = true;
            valLabel.Dock = DockStyle.Fill;
            //valLabel.Height = 18;
            if (rowIndex == 0)
            {
                valLabel.Font = boldFont;
                valLabel.ForeColor = SystemColors.Highlight;
            }
            else
            {   
                valLabel.Font = aboutTablePanel.Font;
            }
			valLabel.Text = val;
            valLabel.BackColor = SystemColors.Window;
            valLabel.TextAlign = ContentAlignment.MiddleLeft;
			aboutTablePanel.Controls.Add(valLabel, 1, rowIndex);
		}
		
		string GetLink(string text)
		{
			if (text == null)
				return null;
			switch (text) 
            {
				case "GNU General Public License":
				case "GPL":
					return "http://www.gnu.org/licenses/gpl.html";
				case "LGPL":
				case "GNU Lesser General Public License":
					return "http://www.gnu.org/licenses/lgpl.html";
				default:
					if (text.StartsWith("http://"))
						return text;
					if (text.StartsWith("www."))
						return "http://" + text;
					return null;
			}
		}

        #endregion

        #region Form Events and Operations

        private void OnFormLoad(object sender, EventArgs e)
        {
            _wndTitle = this.Text;

            if (aboutSplitContainer != null && aboutTablePanel != null)
            {
                aboutSplitContainer.Panel2Collapsed = true;
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            if (installFileNames != null)
            {
                if (this.ShowInstallableAddIns(installFileNames))
                {
                    this.EndInstall();
                }
            }

            ToolbarService.UpdateToolbar(toolStrip);

            _actionsCommand.Enabled   = (selected != null && selected.Count == 1);
            _uninstallCommand.Enabled = (selected != null && selected.Count == 1);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            if (aboutSplitContainer != null && aboutSplitContainer.IsHandleCreated)
            {
                Size availSize = aboutSplitContainer.Panel2.ClientSize;
                Padding padding = aboutTablePanel.Padding;
                availSize.Width = availSize.Width - padding.Left - padding.Right;
                availSize.Height = availSize.Height - padding.Top - padding.Bottom;
                aboutTablePanel.AutoScrollMinSize = availSize;
            }
        }

        private void OnSplitContainerPanel1Paint(object sender, PaintEventArgs e)
        {
            if (visibleAddInCount == 0)
            {
                Rectangle rect = splitContainer.Panel1.ClientRectangle;
                rect.Offset(16, 16);
                rect.Inflate(-32, -32);
                e.Graphics.DrawString(ResourceService.GetString("AddInManager.NoAddInsInstalled"),
                                      this.Font, SystemBrushes.WindowText, rect);
            }
        }

        #endregion
    }
}
