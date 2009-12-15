// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3702 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCheckBox : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		
		void CreateMenuCommand()
		{
			if (menuCommand == null) {
				try {
					menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
                    if (menuCommand != null)
                    {
                        menuCommand.Owner = this;
                    }
				} catch (Exception e) {
					MessageService.ShowError(e, "Can't create menu command : " + codon.Id);
				}
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
		
		public ICheckableMenuCommand MenuCommand {
			get {
				CreateMenuCommand();
				return menuCommand;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		public MenuCheckBox(string text)
		{
			this.RightToLeft = RightToLeft.Inherit;
			Text = text;
		}
		public MenuCheckBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			UpdateText();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (codon != null) {
				MenuCommand.Run();
				this.Checked = MenuCommand.IsChecked;
			}
		}
		
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				return failedAction != ConditionFailedAction.Disable;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {

                // There is no reason why we should not display the icon for the 
                // check-box menu item...
                if (this.Image == null && codon.Properties.Contains("icon"))
                {
                    try
                    {
                        this.Image = WinFormsResourceService.GetBitmap(codon.Properties["icon"]);
                    }
                    catch (ResourceNotFoundException) { }
                }

				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Visible = failedAction != ConditionFailedAction.Exclude;
				if (menuCommand == null && !string.IsNullOrEmpty(codon.Properties["checked"])) {
                    this.Checked = string.Equals(StringParser.Parse(codon.Properties["checked"]),
					                        bool.TrueString, StringComparison.OrdinalIgnoreCase);
				} else {
					CreateMenuCommand();
					if (menuCommand != null) {
						this.Checked = menuCommand.IsChecked;
					}
				}
			}

            // Now, toggle the icon/image display mode based on the checked state...
            this.DisplayStyle = this.Checked ? 
                ToolStripItemDisplayStyle.Text : ToolStripItemDisplayStyle.ImageAndText;
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
                this.Text = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
