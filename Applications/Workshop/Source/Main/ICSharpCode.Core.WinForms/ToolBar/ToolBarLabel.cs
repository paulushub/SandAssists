// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision: 3702 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarLabel : ToolStripLabel, IStatusUpdate
	{
		object caller;
		Codon  codon;
		ICommand menuCommand = null;
		
		public ToolBarLabel(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;			
			this.caller  = caller;
			this.codon   = codon;

			if (codon.Properties.Contains("class"))
			{
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				menuCommand.Owner = this;
			}

			UpdateText();
			UpdateStatus();
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
				
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
								
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null)
			{
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Enabled = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
		
		public virtual void UpdateText()
		{
            if (codon == null)
            {
                return;
            }
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
