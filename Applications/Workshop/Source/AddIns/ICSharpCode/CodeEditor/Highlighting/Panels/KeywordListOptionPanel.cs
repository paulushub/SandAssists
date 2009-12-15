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
    partial class KeywordListOptionPanel : NodeOptionPanel
    {
        private EditorHighlightColor color;

        public KeywordListOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public KeywordListOptionPanel(KeywordListNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.KeywordList.xfrm"));

            //addBtn = (Button)ControlDictionary["addBtn"];
            addBtn.Click += new EventHandler(addBtnClick);
            //removeBtn = (Button)ControlDictionary["removeBtn"];
            removeBtn.Click += new EventHandler(removeBtnClick);
            //chgBtn = (Button)ControlDictionary["chgBtn"];
            chgBtn.Click += new EventHandler(chgBtnClick);

            //nameBox = (TextBox)ControlDictionary["nameBox"];
            //sampleLabel = (Label)ControlDictionary["sampleLabel"];
            //listBox = (ListBox)ControlDictionary["listBox"];
        }

        public override void StoreSettings()
        {
            KeywordListNode node = (KeywordListNode)this.ParentNode;
            List<string> col = new List<string>();

            foreach (string word in listBox.Items)
            {
                col.Add(word);
            }
            node.Words = col;
            node.Name = nameBox.Text;
            node.Color = color;
        }

        public override void LoadSettings()
        {
            KeywordListNode node = (KeywordListNode)this.ParentNode;
            listBox.Items.Clear();

            foreach (string word in node.Words)
            {
                listBox.Items.Add(word);
            }

            sampleLabel.Font = CodeEditorProperties.Instance.FontContainer.DefaultFont;

            color = node.Color;
            nameBox.Text = node.Name;
            PreviewUpdate(sampleLabel, color);
        }

        public override bool ValidateSettings()
        {
            if (nameBox.Text == "")
            {
                ValidationMessage(
                    "${res:Dialog.HighlightingEditor.KeywordList.NameEmpty}");
                return false;
            }

            return true;
        }

        private void chgBtnClick(object sender, EventArgs e)
        {
            using (HighlightingColorDialog dlg =
                new HighlightingColorDialog(color))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    color = dlg.Color;
                    PreviewUpdate(sampleLabel, color);
                }
            }
        }

        private void addBtnClick(object sender, EventArgs e)
        {
            string result = MessageService.ShowInputBox("",
                "${res:Dialog.HighlightingEditor.KeywordList.EnterName}", "");
            if (string.IsNullOrEmpty(result))
                return;
            foreach (string item in listBox.Items)
            {
                if (item == result)
                    return;
            }

            listBox.Items.Add(result);
        }

        private void removeBtnClick(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1) 
                return;

            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }
    }
}
