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
    partial class EnvironmentOptionPanel : NodeOptionPanel
    {
        public EnvironmentOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public EnvironmentOptionPanel(EnvironmentNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Environment.xfrm"));

            //button = (Button)ControlDictionary["button"];
            button.Click += new EventHandler(EditButtonClicked);
            //listView = (ListView)ControlDictionary["listView"];

            //listView.Font = new Font(listView.Font.FontFamily, 10);
        }

        public override void StoreSettings()
        {
            EnvironmentNode node = (EnvironmentNode)this.ParentNode;

            foreach (EnvironmentItem item in listView.Items)
            {
                node.Colors[item.arrayIndex] = item.Color;
            }
        }

        public override void LoadSettings()
        {
            EnvironmentNode node = (EnvironmentNode)this.ParentNode;
            listView.Items.Clear();

            for (int i = 0; i <= node.ColorNames.GetUpperBound(0); ++i)
            {
                listView.Items.Add(new EnvironmentItem(i, node.ColorDescs[i], node.Colors[i], listView.Font));
            }
        }

        private void EditButtonClicked(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1) 
                return;

            EnvironmentItem item = (EnvironmentItem)listView.SelectedItems[0];
            using (HighlightingColorDialog dlg = 
                new HighlightingColorDialog(item.Color))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    item.Color = dlg.Color;
                    item.ColorUpdate();
                }
            }
        }

        #region EnvironmentItem Class

        private class EnvironmentItem : ListViewItem
        {
            //public string Name;
            public EditorHighlightColor Color;
            public int arrayIndex;

            Font basefont;
            Font listfont;

            public EnvironmentItem(int index, string name, EditorHighlightColor color, Font listFont)
                : base(new string[] { name, "Sample" })
            {
                this.Name = name;
                Color = color;
                arrayIndex = index;

                this.UseItemStyleForSubItems = false;

                basefont = CodeEditorProperties.Instance.FontContainer.DefaultFont;
                listfont = listFont;

                ColorUpdate();
            }

            public void ColorUpdate()
            {
                FontStyle fs = FontStyle.Regular;
                if (Color.Bold) fs |= FontStyle.Bold;
                if (Color.Italic) fs |= FontStyle.Italic;

                this.Font = new Font(listfont.FontFamily, 8);

                Font font = new Font(basefont, fs);

                this.SubItems[1].Font = font;

                this.SubItems[1].ForeColor = Color.GetForeColor();
                this.SubItems[1].BackColor = Color.GetBackColor();
            }
        }

        #endregion
    }
}
