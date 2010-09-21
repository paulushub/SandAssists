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
	public sealed class ToolBarComboBox : ToolStripComboBox, IStatusUpdate
	{
		private object caller;
		private Codon  codon;
		private string description;
		private IComboBoxCommand menuCommand;
		
		public ToolBarComboBox(Codon codon, object caller)
		{
            description = String.Empty;
			this.RightToLeft = RightToLeft.Inherit;
			ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBox.SelectionChangeCommitted += new EventHandler(selectionChanged);
			ComboBox.KeyDown += new KeyEventHandler(ComboBoxKeyDown);
            this.DropDownClosed += new EventHandler(OnDropDownClosed);
			
			this.caller  = caller;
			this.codon   = codon;
			
			menuCommand = (IComboBoxCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
			if (menuCommand == null) {
				throw new NullReferenceException("Can't create combobox menu command");
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

        public IComboBoxCommand MenuCommand
        {
            get
            {
                return menuCommand;
            }
        }
		
		void ComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Control) {
				MenuCommand.Run();
			}
		}
		
		void selectionChanged(object sender, EventArgs e)
		{
			MenuCommand.Run();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null) {
					isEnabled &= menuCommand.IsEnabled;
				}
				
				return isEnabled;
			}
		}
		
		public void UpdateStatus()
		{
			bool isVisible = base.Visible;
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				isVisible &= failedAction != ConditionFailedAction.Exclude;
			}
			if (base.Visible != isVisible) {
				Visible = isVisible;
			}
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("label")) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (codon.Properties.Contains("tooltip")) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboxBox = this.ComboBox;
            Control parent = comboxBox.Parent;
            if (parent != null)
            {
                parent.Focus();
            }
        }
    }
}
