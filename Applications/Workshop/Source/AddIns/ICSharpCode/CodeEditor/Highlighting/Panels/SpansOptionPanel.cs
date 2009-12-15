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
    partial class SpansOptionPanel : NodeOptionPanel
    {
        public SpansOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public SpansOptionPanel(SpansNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Spans.xfrm"));

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
            SpansNode node = (SpansNode)parent;
            listView.Items.Clear();

            foreach (SpanNode rn in node.Nodes)
            {
                ListViewItem lv = new ListViewItem(rn.Text);
                lv.Tag = rn;
                listView.Items.Add(lv);
            }
        }

        private void addClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("",
                "${res:Dialog.HighlightingEditor.Spans.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;
            SpanNode rsn = new SpanNode(result);
            ListViewItem lv = new ListViewItem(rsn.Text);
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
