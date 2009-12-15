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
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class KeywordListsOptionPanel : NodeOptionPanel
    {
        public KeywordListsOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public KeywordListsOptionPanel(KeywordListsNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.KeywordLists.xfrm"));

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
            KeywordListsNode node = (KeywordListsNode)parent;
            listView.Items.Clear();

            foreach (KeywordListNode rn in node.Nodes)
            {
                ListViewItem lv = new ListViewItem(rn.Name);
                lv.Tag = rn;
                listView.Items.Add(lv);
            }
        }

        private void addClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("", 
                "${res:Dialog.HighlightingEditor.KeywordList.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == result)
                    return;
            }

            KeywordListNode kwn = new KeywordListNode(result);
            ListViewItem lv = new ListViewItem(result);
            lv.Tag = kwn;
            parent.Nodes.Add(kwn);
            listView.Items.Add(lv);
        }

        private void removeClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1) 
                return;

            ((TreeNode)listView.SelectedItems[0].Tag).Remove();
            listView.SelectedItems[0].Remove();
        }
    }
}
