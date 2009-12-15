using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class ChooseLayoutDialog : Form
    {
        public ChooseLayoutDialog()
        {
            InitializeComponent();

            listEditor.ManualOrder = false;
            listEditor.BrowseForDirectory = false;
            listEditor.TitleText = StringParser.Parse(
                "${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.Label}");
            listEditor.AddButtonText = StringParser.Parse(
                "${res:ICSharpCode.SharpDevelop.Commands.ChooseLayoutCommand.EditLayouts.AddLayout}");

            foreach (Control ctl in this.Controls.GetRecursive())
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);
        }

        public StringListEditor Editor
        {
            get
            {
                return listEditor;
            }
        }
    }
}
