// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;

namespace ICSharpCode.AddInManager
{
    public class AddInManagerAddInStateConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            string states = condition.Properties["states"];
            if (String.IsNullOrEmpty(states))
            {
                return false;
            }
            string action = ((AddInControl)caller).AddIn.Action.ToString();
            if (String.IsNullOrEmpty(action))
            {
                return false;
            }
            foreach (string state in states.Split(','))
            {
                if (String.Equals(state, action, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

    public class ShowCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm dlg   = new ManagerForm();
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.ShowDialog();
        }
    }
	
	public abstract class AddInCommand : AbstractMenuCommand
	{
        protected AddInCommand()
        {   
        }

        public AddIn AddIn
        {
            get
            {
                ToolBarCommand toolItem = this.Owner as ToolBarCommand;
                if (toolItem != null)
                {
                    ManagerForm form = toolItem.Caller as ManagerForm;
                    if (form != null)
                    {
                        return form.SelectedAddIn;
                    }

                    return null;
                }

                ToolBarCheckBox toolCheck = this.Owner as ToolBarCheckBox;
                if (toolCheck != null)
                {
                    ManagerForm form = toolCheck.Caller as ManagerForm;
                    if (form != null)
                    {
                        return form.SelectedAddIn;
                    }

                    return null;
                }

                MenuCheckBox menuCheck = this.Owner as MenuCheckBox;
                if (menuCheck != null)
                {
                    AddInControl addInControl = menuCheck.Caller as AddInControl;
                    if (addInControl != null)
                    {
                        return addInControl.AddIn;
                    }

                    return null;
                }

                MenuCommand menuItem = this.Owner as MenuCommand;
                if (menuItem != null)
                {
                    AddInControl addInControl = menuItem.Caller as AddInControl;
                    if (addInControl != null)
                    {
                        return addInControl.AddIn;
                    }

                    return null;
                }

                return null;
            }
        }

        public ManagerForm Form
        {
            get
            {
                ToolBarCommand toolItem = this.Owner as ToolBarCommand;
                if (toolItem != null)
                {
                    return toolItem.Caller as ManagerForm;
                }

                ToolBarCheckBox toolCheck = this.Owner as ToolBarCheckBox;
                if (toolCheck != null)
                {
                    return toolCheck.Caller as ManagerForm;
                }

                MenuCheckBox menuCheck = this.Owner as MenuCheckBox;
                if (menuCheck != null)
                {
                    AddInControl addInControl = menuCheck.Caller as AddInControl;
                    if (addInControl != null)
                    {
                        return addInControl.Form;
                    }

                    return null;
                }

                MenuCommand menuItem = this.Owner as MenuCommand;
                if (menuItem != null)
                {
                    AddInControl addInControl = menuItem.Caller as AddInControl;
                    if (addInControl != null)
                    {
                        return addInControl.Form;
                    }

                    return null;
                }

                return null;
            }
        }
	}

    public class InstallCommand : AddInCommand
    {
        public override void Run()
        {
            ToolBarCommand toolItem = this.Owner as ToolBarCommand;
            if (toolItem != null)
            {
                ManagerForm form = toolItem.Caller as ManagerForm;
                if (form != null)
                {
                    form.TryInstall();
                }
            }
        }
    }

    public class DisableCommand : AddInCommand
	{
        public override bool IsEnabled
        {
            get
            {
                ManagerForm form = this.Form;
                if (form != null)
                {
                    return form.CanRun(AddInAction.Disable);
                }

                return false;
            }
        }

        public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryRunAction(addIn, AddInAction.Disable);
		}
	}

    public class EnableCommand : AddInCommand
	{
        public override bool IsEnabled
        {
            get
            {
                AddIn addIn = this.AddIn;
                if (addIn != null)
                {
                    return addIn.Action == AddInAction.Disable;
                }

                return false;
            }
        }

        public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryRunAction(addIn, AddInAction.Enable);
		}
	}

    public class AbortInstallCommand : AddInCommand
	{
		public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryRunAction(addIn, AddInAction.Uninstall);
		}
	}

    public class AbortUpdateCommand : AddInCommand
	{
		public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryRunAction(addIn, AddInAction.InstalledTwice);
		}
	}

    public class AbortUninstallCommand : AddInCommand
	{
        public override bool IsEnabled
        {
            get
            {
                AddIn addIn = this.AddIn;
                if (addIn != null)
                {
                    return addIn.Action == AddInAction.Uninstall;
                }

                return false;
            }
        }

        public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryRunAction(addIn, AddInAction.Enable);
		}
	}

    public class UninstallCommand : AddInCommand
	{
        public override bool IsEnabled
        {
            get
            {
                ManagerForm form = this.Form;
                if (form != null)
                {
                    return form.CanUninstall;
                }

                return false;
            }
        }

        public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            form.TryUninstall(addIn);
		}
	}

    public class UndoAddInAction : AddInCommand
    {
        private Codon _codeOn;
        private string _itemText;
        private string _itemToolTip;
        private ToolBarCommand _toolItem;
        private AddInActionHelper _actionsRunner;

        public UndoAddInAction()
        {   
        }

        public override bool IsEnabled
        {
            get
            {
                if (_actionsRunner == null)
                {
                    ManagerForm form = this.Form;
                    if (form != null)
                    {
                        _actionsRunner = form.ActionsCommand;

                        if (_actionsRunner != null)
                        {
                            _actionsRunner.TextChanged += 
                                new EventHandler(OnActionsTextChanged);
                            _actionsRunner.UndoableChanged += 
                                new EventHandler(OnActionsUndoableChanged);
                        }
                    }
                }

                if (_actionsRunner == null)
                {
                    return false;
                }

                return _actionsRunner.Undoable;
            }
        }

        private void OnActionsUndoableChanged(object sender, EventArgs e)
        {
            if (_actionsRunner == null || _codeOn == null)
            {
                return;
            }

            if (!String.IsNullOrEmpty(_itemText) &&
                !String.IsNullOrEmpty(_itemToolTip))
            {
                if (!_actionsRunner.Undoable)
                {
                    _codeOn.Properties["label"] = _itemText;
                    _codeOn.Properties["tooltip"] = _itemToolTip;
                }
            }

            _toolItem.UpdateText();
        }

        private void OnActionsTextChanged(object sender, EventArgs e)
        {
            if (_actionsRunner == null || _codeOn == null)
            {
                return;
            }

            if (!String.IsNullOrEmpty(_itemText) &&
                !String.IsNullOrEmpty(_itemToolTip))
            {
                if (_actionsRunner.Undoable)
                {
                    _codeOn.Properties["label"] = _actionsRunner.Text;
                    _codeOn.Properties["tooltip"] = _actionsRunner.UndoText;
                }
                else
                {
                    _codeOn.Properties["label"] = _itemText;
                    _codeOn.Properties["tooltip"] = _itemToolTip;
                }
            }

            _toolItem.UpdateText();
        }

        public override void Run()
        {
            if (_actionsRunner != null)
            {
                _actionsRunner.PerformClick();
            }
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarCommand commandItem = this.Owner as ToolBarCommand;

            if (commandItem != null)
            {
                _toolItem = commandItem;

                Codon codeOn = commandItem.Codon;
                if (codeOn != null)
                {
                    _codeOn = codeOn;

                    Properties properties = codeOn.Properties;
                    if (properties.Contains("label"))
                    {
                        _itemText = properties["label"];
                    }
                    if (properties.Contains("tooltip"))
                    {
                        _itemToolTip = properties["tooltip"];
                    }
                }
            }
        }
    }

    public class OpenHomepageCommand : AddInCommand
	{
		public override bool IsEnabled 
        {
			get 
            {
                AddIn addIn = this.AddIn;

                if (addIn == null)
                {
                    // Most likely, there is no Add-In selection currently...
                    return false;
                }

                return !String.IsNullOrEmpty(addIn.Properties["url"]);
			}
		}
		
		public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            string linkUrl = addIn.Properties["url"];
            if (String.IsNullOrEmpty(linkUrl))
            {
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(linkUrl);
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.ToString());
            }
		}
	}

    public class AboutCommand : AddInCommand, ICheckableMenuCommand
	{
        public bool IsChecked
        {
            get
            {
                ManagerForm form = this.Form;
                if (form == null)
                {
                    return false;
                }

                return form.IsAboutAddInVisible;
            }
            set
            {               
            }
        } 

		public override void Run()
		{
            ManagerForm form = this.Form;
            if (form == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }

            form.TryToggleAboutAddIn();
		}
	}

    public class ShowOptionsCommand : AddInCommand
	{
		public override bool IsEnabled 
        {
			get 
            {
                AddIn addIn = this.AddIn;

                if (addIn == null)
                {
                    // Most likely, there is no Add-In selection currently...
                    return false;
                }
                if (addIn.Enabled)
                {
					foreach (KeyValuePair<string, ExtensionPath> pair in addIn.Paths) {
						if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog")) {
							return true;
						}
					}
				}
				return false;
			}
		}
		
		public override void Run()
		{
            AddIn addIn = this.AddIn;

            if (addIn == null)
            {
                LoggingService.Warn("The caller is not correctly set: " + this.Owner.ToString());
                return;
            }
            AddInTreeNode dummyNode = new AddInTreeNode();
			foreach (KeyValuePair<string, ExtensionPath> pair in addIn.Paths) {
				if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog")) {
					dummyNode.Codons.AddRange(pair.Value.Codons);
				}
			}
			OptionsCommand.ShowTabbedOptions(addIn.Name + " " + 
                ResourceService.GetString("AddInManager.Options"), dummyNode);
		}
	}
}
