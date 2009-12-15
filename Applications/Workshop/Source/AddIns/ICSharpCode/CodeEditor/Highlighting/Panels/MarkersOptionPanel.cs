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
    partial class MarkersOptionPanel : NodeOptionPanel
    {
        private bool previous;

        public MarkersOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public MarkersOptionPanel(MarkersNode parent, bool prev)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Markers.xfrm"));

            //addBtn = (Button)ControlDictionary["addBtn"];
            addBtn.Click += new EventHandler(addClick);
            //removeBtn = (Button)ControlDictionary["removeBtn"];
            removeBtn.Click += new EventHandler(removeClick);

            //listView = (ListView)ControlDictionary["listView"];

            previous = prev;
            label.Text = ResourceService.GetString(previous ? 
                "Dialog.HighlightingEditor.Markers.Previous" : 
                "Dialog.HighlightingEditor.Markers.Next");
        }

        public override void StoreSettings()
        {
        }

        public override void LoadSettings()
        {
            MarkersNode node = (MarkersNode)parent;
            listView.Items.Clear();

            foreach (MarkerNode rn in node.Nodes)
            {
                ListViewItem lv = new ListViewItem(rn.What);
                lv.Tag = rn;
                listView.Items.Add(lv);
            }
        }

        private void addClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("",
                "${res:Dialog.HighlightingEditor.Markers.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;

            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == result)
                    return;
            }

            MarkerNode rsn  = new MarkerNode(result, previous);
            ListViewItem lv = new ListViewItem(result);
            lv.Tag = rsn;
            parent.Nodes.Add(rsn);
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
