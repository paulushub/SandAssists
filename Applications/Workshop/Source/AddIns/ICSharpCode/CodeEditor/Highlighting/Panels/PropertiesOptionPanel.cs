// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 3118 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class PropertiesOptionPanel : NodeOptionPanel
    {
        public PropertiesOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public PropertiesOptionPanel(PropertiesNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Properties.xfrm"));

            //addBtn = (Button)ControlDictionary["addBtn"];
            addBtn.Click += new EventHandler(addClick);
            //editBtn = (Button)ControlDictionary["editBtn"];
            editBtn.Click += new EventHandler(editClick);
            //removeBtn = (Button)ControlDictionary["removeBtn"];
            removeBtn.Click += new EventHandler(removeClick);

            //listView = (ListView)ControlDictionary["listView"];
        }

        public override void StoreSettings()
        {
            PropertiesNode node = (PropertiesNode)parent;

            node.Properties.Clear();
            foreach (ListViewItem item in listView.Items)
            {
                node.Properties.Add(item.Text, item.SubItems[1].Text);
            }
        }

        public override void LoadSettings()
        {
            PropertiesNode node = (PropertiesNode)parent;
            listView.Items.Clear();

            foreach (DictionaryEntry de in node.Properties)
            {
                ListViewItem lv = new ListViewItem(new string[] { (string)de.Key, (string)de.Value });
                listView.Items.Add(lv);
            }
        }

        private void addClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("",
                "${res:Dialog.HighlightingEditor.Properties.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;

            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == result)
                    return;
            }

            listView.Items.Add(new ListViewItem(new string[] { result, "" }));
        }

        private void removeClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1) 
                return;

            listView.SelectedItems[0].Remove();
        }

        private void editClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1) 
                return;

            string result = MessageService.ShowInputBox(
                "${res:Dialog.HighlightingEditor.EnterText}",
                String.Format(ResourceService.GetString(
                "Dialog.HighlightingEditor.Properties.EnterValue"),
                listView.SelectedItems[0].Text), "");

            if (string.IsNullOrEmpty(result))
                return;

            listView.SelectedItems[0].SubItems[1].Text = result;
        } 
    }
}
