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
	public sealed class ToolBarSeparator : ToolStripSeparator, IStatusUpdate
	{
		object caller;
		Codon codon;
		
		public ToolBarSeparator()
		{
			this.RightToLeft = RightToLeft.Inherit;
		}
		
		public ToolBarSeparator(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
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
		
		public void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Enabled = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
		
		public void UpdateText()
		{
		}
	}
}
