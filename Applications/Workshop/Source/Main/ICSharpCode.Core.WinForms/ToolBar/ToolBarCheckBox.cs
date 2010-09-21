﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3702 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public sealed class ToolBarCheckBox : ToolStripButton , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description;
		ICheckableMenuCommand menuCommand;
		
		public ToolBarCheckBox(string text)
		{
            description = String.Empty;
			this.RightToLeft = RightToLeft.Inherit;
			Text = text;
		}
		
		public ToolBarCheckBox(Codon codon, object caller)
		{
            description = String.Empty;
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			try {
				menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			} catch (Exception) {
			}
			if (menuCommand == null) {
				MessageService.ShowError("Can't create toolbar checkbox : " + codon.Id);
			}
			menuCommand.Owner = this;
			
			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			UpdateText();
			UpdateStatus();
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Run();
				Checked = menuCommand.IsChecked;
			}
		}

        public ICheckableMenuCommand MenuCommand
        {
            get
            {
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

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
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
		
		public void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (isVisible != Visible)
					Visible = isVisible;
				if (menuCommand != null) {
					bool isChecked = menuCommand.IsChecked;
					if (isChecked != Checked)
						Checked = isChecked;
				}
				
				if (this.Visible && codon.Properties.Contains("icon")) {
					Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
				}
			}
		}
		
		public void UpdateText()
		{
			if (codon != null) 
            {
                if (codon.Properties.Contains("tooltip"))
                {
                    string tempText = StringParser.Parse(codon.Properties["tooltip"]);
                    this.ToolTipText = String.IsNullOrEmpty(tempText) ?
                    tempText : tempText.Replace("&", String.Empty);
                }

                if (codon.Properties.Contains("label"))
                {
                    string tempText = StringParser.Parse(codon.Properties["label"]);
                    this.Text = String.IsNullOrEmpty(tempText) ?
                    tempText : tempText.Replace("&", String.Empty);
                }
            }
		}
	}
}
