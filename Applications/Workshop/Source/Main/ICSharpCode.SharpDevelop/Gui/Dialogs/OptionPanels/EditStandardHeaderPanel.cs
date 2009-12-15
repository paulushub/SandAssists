// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class EditStandardHeaderPanel : DialogPanel
    {
        public EditStandardHeaderPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.EditStandardHeaderPanel.xfrm"));

            headerTextBox.Font = WinFormsResourceService.DefaultMonospacedFont;
            foreach (StandardHeader header in StandardHeader.StandardHeaders)
            {
                headerChooser.Items.Add(header);
            }
            headerChooser.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            headerChooser.SelectedIndex = 0;
            headerTextBox.TextChanged += new EventHandler(TextChangedEvent);
        }

        public override bool StorePanelContents()
        {
            StandardHeader.StoreHeaders();
            return true;
        }

        private void TextChangedEvent(object sender, EventArgs e)
        {
            ((StandardHeader)(headerChooser.SelectedItem)).Header = headerTextBox.Text;
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            headerTextBox.TextChanged -= new EventHandler(TextChangedEvent);
            int idx = headerChooser.SelectedIndex;
            if (idx >= 0)
            {
                headerTextBox.Text = ((StandardHeader)headerChooser.SelectedItem).Header;
                headerTextBox.Enabled = true;
            }
            else
            {
                headerTextBox.Text = "";
                headerTextBox.Enabled = false;
            }
            headerTextBox.TextChanged += new EventHandler(TextChangedEvent);
        }
    }
}
