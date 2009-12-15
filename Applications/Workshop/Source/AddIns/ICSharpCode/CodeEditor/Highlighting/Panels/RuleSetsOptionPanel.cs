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
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class RuleSetsOptionPanel : NodeOptionPanel
    {
        public RuleSetsOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public RuleSetsOptionPanel(RuleSetsNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //   "ICSharpCode.SharpDevelop.Gui.Resources.RuleSets.xfrm"));

            //addBtn = (Button)ControlDictionary["addBtn"];
            addBtn.Click += new EventHandler(addClick);
            //removeBtn = (Button)ControlDictionary["removeBtn"];
            removeBtn.Click += new EventHandler(removeClick);

            //listView = (ListView)ControlDictionary["listView"];
        }

        public override void StoreSettings()
        {
        }

        public override void LoadSettings()
        {
            RuleSetsNode node = (RuleSetsNode)parent;
            listView.Items.Clear();

            foreach (RuleSetNode rn in node.Nodes)
            {
                if (rn.Name == "") continue;
                ListViewItem lv = new ListViewItem(rn.Name);
                lv.Tag = rn;
                listView.Items.Add(lv);
            }
        }

        private void addClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("",
                "${res:Dialog.HighlightingEditor.RuleSets.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == result)
                    return;
            }

            RuleSetNode rsn = new RuleSetNode(result,
                "&<>~!@%^*()-+=|\\#/{}[]:;\"' ,	.?", "", '\0', false);
            ListViewItem lv = new ListViewItem(result);
            lv.Tag = rsn;
            parent.Nodes.Add(rsn);
            listView.Items.Add(lv);
        }

        private void removeClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1) return;

            ((TreeNode)listView.SelectedItems[0].Tag).Remove();
            listView.SelectedItems[0].Remove();
        }
    }
}
