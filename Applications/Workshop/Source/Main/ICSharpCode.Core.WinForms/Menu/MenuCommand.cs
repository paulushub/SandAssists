// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3763 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCommand : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		string description = "";
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ICommand Command {
			get {
				if (menuCommand == null) {
					CreateCommand();
				}
				return menuCommand;
			}
		}

        public object Caller
        {
            get
            {
                return caller;
            }
        }

        public Codon Codon
        {
            get
            {
                return codon;
            }
        }
		
		// HACK: find a better way to allow the host app to process link commands
		public static Converter<string, ICommand> LinkCommandCreator;
		
		void CreateCommand()
		{
			try {
				string link = codon.Properties["link"];
				if (link != null && link.Length > 0) {
					if (LinkCommandCreator == null)
						throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
					menuCommand = LinkCommandCreator(codon.Properties["link"]);
				} else {
					menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				if (menuCommand != null) {
					//menuCommand.Owner = caller;
					menuCommand.Owner = this;
				}
			} catch (Exception e) {
				MessageService.ShowError(e, "Can't create menu command : " + codon.Id);
			}
		}
		
		public MenuCommand(Codon codon, object caller) : this(codon, caller, false)
		{
			
		}
		
		public static Keys ParseShortcut(string shortcutString)
		{
			Keys shortCut = Keys.None;
			if (shortcutString.Length > 0) {
				try {
					foreach (string key in shortcutString.Split('|')) {
						shortCut  |= (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key);
					}
				} catch (Exception ex) {
					MessageService.ShowError(ex);
					return System.Windows.Forms.Keys.None;
				}
			}
			return shortCut;
		}
		
		public MenuCommand(Codon codon, object caller, bool createCommand)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			
			if (createCommand) {
				CreateCommand();
			}
			
			UpdateText();
			if (codon.Properties.Contains("shortcut")) {
				ShortcutKeys =  ParseShortcut(codon.Properties["shortcut"]);
			}
            if (codon.Properties.Contains("checkonclick"))
            {
                this.CheckOnClick = codon.Properties["checkonclick"] == "true";
			}
            if (codon.Properties.Contains("ischecked"))
            {
                this.Checked = codon.Properties["ischecked"] == "true";
			}
        }
		
		public MenuCommand(string label, EventHandler handler) : this(label)
		{
			this.Click  += handler;
		}
		
		public MenuCommand(string label)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.codon  = null;
			this.caller = null;
			Text = StringParser.Parse(label);
		}

        public MenuCommand(string label, ICommand command, bool isChecked)
        {
            this.RightToLeft = RightToLeft.Inherit;
            this.codon = null;
            this.caller = null;
            this.menuCommand = command;
            this.Text = StringParser.Parse(label);
            this.Checked = isChecked;
        }
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (codon != null) {
				if (GetVisible() && Enabled) {
					ICommand cmd = Command;
                    if (cmd != null)
                    {
                        LoggingService.Info("Run command " + cmd.GetType().FullName);
                        cmd.Run();
                    }
                }
			}
            else if (menuCommand != null)
            {
                menuCommand.Run();
            }
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			StatusBarService.SetMessage(description);
//		}
		
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
		}
		
		bool GetVisible()
		{
			if (codon == null)
				return true;
			else
				return codon.GetFailedAction(caller) != ConditionFailedAction.Exclude;
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
                if (this.Image == null && codon.Properties.Contains("icon"))
                {
					try {
                        this.Image = WinFormsResourceService.GetBitmap(codon.Properties["icon"]);
					} catch (ResourceNotFoundException) {}
				}
				this.Visible = GetVisible();
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
                this.Text = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
