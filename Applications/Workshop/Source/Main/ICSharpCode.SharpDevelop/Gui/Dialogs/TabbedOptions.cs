// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2487 $</version>
// </file>

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// Basic "tabbed" options dialog
    /// </summary>
    public partial class TabbedOptions : Form
    {
        private List<IDialogPanel> OptionPanels = new List<IDialogPanel>();

        public TabbedOptions()
        {
            InitializeComponent();

            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");
            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
        }

        public TabbedOptions(string dialogName, AddInTreeNode node)
            : this()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.TabbedOptionsDialog.xfrm"));

            if (!String.IsNullOrEmpty(dialogName))
            {
                this.Text = dialogName;
            }
            this.okButton.Click += new EventHandler(AcceptEvent);
            this.Owner = WorkbenchSingleton.MainForm;

            AddOptionPanels(node.BuildChildItems<IDialogPanelDescriptor>(this));
        }

        private void AcceptEvent(object sender, EventArgs e)
        {
            foreach (IDialogPanel pane in OptionPanels)
            {
                if (!pane.ReceiveDialogMessage(DialogMessage.OK))
                {
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void AddOptionPanels(
            IEnumerable<IDialogPanelDescriptor> dialogPanelDescriptors)
        {
            foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors)
            {
                if (descriptor != null && descriptor.DialogPanel != null && 
                    descriptor.DialogPanel.Control != null)
                { // may be null, if it is only a "path"
                    descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
                    descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
                    OptionPanels.Add(descriptor.DialogPanel);

                    TabPage page = new TabPage(descriptor.Label);
                    page.UseVisualStyleBackColor = true;
                    page.Controls.Add(descriptor.DialogPanel.Control);
                    optionPanelTabControl.TabPages.Add(page);
                }

                if (descriptor.ChildDialogPanelDescriptors != null)
                {
                    AddOptionPanels(descriptor.ChildDialogPanelDescriptors);
                }
            }
        }
    }
}
