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
    public sealed class ToolBarCommand : ToolStripButton, IStatusUpdate
	{
		private object caller;
        private Codon codon;
        private ICommand menuCommand;
        private ICommandGroup group;
		
        public ToolBarCommand()
        {   
        }

        public ToolBarCommand(Image image)
            : base(image)
        {   
        }  
 
        public ToolBarCommand(string text)
            : base(text)
        {   
        }

        public ToolBarCommand(string text, Image image)
            : base(text, image)
        {   
        }

        public ToolBarCommand(string text, Image image, EventHandler onClick)
            : base(text, image, onClick)
        {
        }

        public ToolBarCommand(string text, Image image, EventHandler onClick, string name)
            : base(text, image, onClick, name)
        {
        }

		public ToolBarCommand(Codon codon, object caller, bool createCommand)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller      = caller;
			this.codon       = codon;
			
			if (createCommand) 
            {
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
                menuCommand.Owner = this;
            }
			
			if (codon.Properties.Contains("label"))
            {
                string tempText = StringParser.Parse(codon.Properties["label"]);
                this.Text = String.IsNullOrEmpty(tempText) ? 
                tempText : tempText.Replace("&", String.Empty);
			}
			if (this.Image == null && codon.Properties.Contains("icon")) 
            {
                this.Image = WinFormsResourceService.GetBitmap(StringParser.Parse(
                    codon.Properties["icon"]));
			}
			
			UpdateStatus();
			UpdateText();
		}

        public ICommandGroup Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
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

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);

            if (codon != null)
            {
			    if (menuCommand == null) 
                {
                    menuCommand = (ICommand)codon.AddIn.CreateObject(
                        codon.Properties["class"]);
                    menuCommand.Owner = this;
                }
			    if (menuCommand != null) 
                {
				    //menuCommand.Owner = caller;
                    LoggingService.Info("Run command " + menuCommand.GetType().FullName);
                    menuCommand.Run();
			    }
            }
		}
		
		public void UpdateStatus()
		{
			if (codon != null) 
            {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Visible = failedAction != ConditionFailedAction.Exclude;
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				if (isEnabled && menuCommand != null) 
                {
                    IMenuCommand menuItem = menuCommand as IMenuCommand;
                    if (menuItem != null)
                    {
                        isEnabled = menuItem.IsEnabled;
                    }
				}
				this.Enabled = isEnabled;
				
				if (this.Visible && codon.Properties.Contains("icon")) 
                {
                    this.Image = WinFormsResourceService.GetBitmap(StringParser.Parse(
                        codon.Properties["icon"]));
				}
			}
		}
		
		public void UpdateText()
		{
			if (codon != null) 
            {
				if (codon.Properties.Contains("tooltip")) 
                {
                    string tempText  = StringParser.Parse(codon.Properties["tooltip"]);
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
