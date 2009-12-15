// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1423 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using SortDirection = ICSharpCode.SharpDevelop.TextEditor.Commands.SortSelection.SortDirection;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class SortOptionsDialog: Form
	{
		public static readonly string removeDupesOption       = "ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.RemoveDuplicateLines";
		public static readonly string caseSensitiveOption     = "ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.CaseSensitive";
		public static readonly string ignoreWhiteSpacesOption = "ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.IgnoreWhitespaces";
		public static readonly string sortDirectionOption     = "ICSharpCode.SharpDevelop.Gui.SortOptionsDialog.SortDirection";
		
		public SortOptionsDialog()
		{
			InitializeComponent();

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            foreach (Control ctl in groupBox.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.SharpDevelop.Resources.SortOptionsDialog.xfrm"));

            //AcceptButton = (Button)ControlDictionary["okButton"];
            //CancelButton = (Button)ControlDictionary["cancelButton"];
            removeDupesCheckBox.Checked = PropertyService.Get(removeDupesOption, false);
            caseSensitiveCheckBox.Checked = PropertyService.Get(caseSensitiveOption, true);
            ignoreWhiteSpacesCheckBox.Checked = PropertyService.Get(ignoreWhiteSpacesOption, false);

            ascendingRadioButton.Checked = PropertyService.Get(sortDirectionOption, SortDirection.Ascending) == SortDirection.Ascending;
            descendingRadioButton.Checked = PropertyService.Get(sortDirectionOption, SortDirection.Ascending) == SortDirection.Descending;

            // insert event handlers
            okButton.Click += new EventHandler(OkEvent);
        }

        private void OkEvent(object sender, EventArgs e)
        {
            PropertyService.Set(removeDupesOption, removeDupesCheckBox.Checked);
            PropertyService.Set(caseSensitiveOption, caseSensitiveCheckBox.Checked);
            PropertyService.Set(ignoreWhiteSpacesOption, ignoreWhiteSpacesCheckBox.Checked);
            if (ascendingRadioButton.Checked)
            {
                PropertyService.Set(sortDirectionOption, SortDirection.Ascending);
            }
            else
            {
                PropertyService.Set(sortDirectionOption, SortDirection.Descending);
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            okButton.Focus();
        }
    }
}
