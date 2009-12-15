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
    partial class SpanOptionPanel : NodeOptionPanel
    {
        private EditorHighlightColor color;
        private EditorHighlightColor beginColor;
        private EditorHighlightColor endColor;

        public SpanOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public SpanOptionPanel(SpanNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Span.xfrm"));
            //nameBox = (TextBox)ControlDictionary["nameBox"];
            //beginBox = (TextBox)ControlDictionary["beginBox"];
            beginBox.TextChanged += BeginTextChanged;
            //endBox = (TextBox)ControlDictionary["endBox"];
            endBox.TextChanged += EndTextChanged;
            //ruleBox = (ComboBox)ControlDictionary["ruleBox"];

            //useBegin = (CheckBox)ControlDictionary["useBegin"];
            //useEnd = (CheckBox)ControlDictionary["useEnd"];

            //chgBegin = (Button)ControlDictionary["chgBegin"];
            //chgEnd = (Button)ControlDictionary["chgEnd"];
            //chgCont = (Button)ControlDictionary["chgCont"];

            //samBegin = (Label)ControlDictionary["samBegin"];
            //samEnd = (Label)ControlDictionary["samEnd"];
            //samCont = (Label)ControlDictionary["samCont"];

            //stopEolBox = (CheckBox)ControlDictionary["stopEolBox"];
            //beginSingleWordCheckBox = (CheckBox)ControlDictionary["beginSingleWordCheckBox"];
            //endSingleWordCheckBox = (CheckBox)ControlDictionary["endSingleWordCheckBox"];
            //escCharTextBox = (TextBox)ControlDictionary["escCharTextBox"];

            this.chgBegin.Click += new EventHandler(chgBeginClick);
            this.chgCont.Click  += new EventHandler(chgContClick);
            this.chgEnd.Click   += new EventHandler(chgEndClick);

            this.useBegin.CheckedChanged += new EventHandler(CheckedChanged);
            this.useEnd.CheckedChanged   += new EventHandler(CheckedChanged);
        }

        public override void StoreSettings()
        {
            SpanNode node = (SpanNode)parent;

            node.Name = nameBox.Text;
            node.Begin = beginBox.Text;
            node.End = endBox.Text;
            node.StopEOL = stopEolBox.Checked;
            node.IsBeginSingleWord = beginSingleWordCheckBox.Checked;
            node.IsEndSingleWord = endSingleWordCheckBox.Checked;
            node.EscapeCharacter = escCharTextBox.TextLength > 0 ? escCharTextBox.Text[0] : '\0';
            node.Rule = ruleBox.Text;

            node.Color = color;

            if (useBegin.Checked)
            {
                node.BeginColor = beginColor;
            }
            else
            {
                node.BeginColor = new EditorHighlightColor(true);
            }

            if (useEnd.Checked)
            {
                node.EndColor = endColor;
            }
            else
            {
                node.EndColor = new EditorHighlightColor(true);
            }
        }

        public override void LoadSettings()
        {
            SpanNode node = (SpanNode)parent;

            try
            {
                ruleBox.Items.Clear();
                foreach (RuleSetNode rn in node.Parent.Parent.Parent.Nodes)
                { // list rule sets
                    if (!rn.IsRoot) ruleBox.Items.Add(rn.Text);
                }
            }
            catch
            {
            }

            samBegin.Font = samEnd.Font = samCont.Font = ICSharpCode.SharpDevelop.TextEditor.Gui.CodeEditorProperties.Instance.FontContainer.DefaultFont;

            nameBox.Text = node.Name;
            ruleBox.Text = node.Rule;
            beginBox.Text = node.Begin;
            endBox.Text = node.End;
            stopEolBox.Checked = node.StopEOL;
            beginSingleWordCheckBox.Checked = node.IsBeginSingleWord;
            endSingleWordCheckBox.Checked = node.IsEndSingleWord;
            escCharTextBox.Text = (node.EscapeCharacter == '\0') ? "" : node.EscapeCharacter.ToString();

            color = node.Color;
            beginColor = node.BeginColor;
            endColor = node.EndColor;

            if (beginColor != null)
            {
                if (!beginColor.NoColor) useBegin.Checked = true;
            }
            if (endColor != null)
            {
                if (!endColor.NoColor) useEnd.Checked = true;
            }

            PreviewUpdate(samBegin, beginColor);
            PreviewUpdate(samEnd, endColor);
            PreviewUpdate(samCont, color);
            CheckedChanged(null, null);
            BeginTextChanged(null, null);
            EndTextChanged(null, null);
        }

        public override bool ValidateSettings()
        {
            if (nameBox.Text == String.Empty)
            {
                ValidationMessage(ResourceService.GetString(
                    "Dialog.HighlightingEditor.Span.NameEmpty"));
                return false;
            }
            if (beginBox.Text == String.Empty)
            {
                ValidationMessage(ResourceService.GetString(
                    "Dialog.HighlightingEditor.Span.BeginEmpty"));
                return false;
            }

            return true;
        }

        private void chgBeginClick(object sender, EventArgs e)
        {
            using (HighlightingColorDialog dlg = new HighlightingColorDialog(beginColor))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    beginColor = dlg.Color;
                    PreviewUpdate(samBegin, beginColor);
                }
            }
        }

        private void chgEndClick(object sender, EventArgs e)
        {
            using (HighlightingColorDialog dlg = new HighlightingColorDialog(endColor))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    endColor = dlg.Color;
                    PreviewUpdate(samEnd, endColor);
                }
            }
        }

        private void chgContClick(object sender, EventArgs e)
        {
            using (HighlightingColorDialog dlg = new HighlightingColorDialog(color))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    color = dlg.Color;
                    PreviewUpdate(samCont, color);
                }
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            chgEnd.Enabled = useEnd.Checked;
            chgBegin.Enabled = useBegin.Checked;
        }

        private void BeginTextChanged(object sender, EventArgs e)
        {
            beginSingleWordCheckBox.Enabled = beginBox.Text.Length > 0;
        }

        private void EndTextChanged(object sender, EventArgs e)
        {
            endSingleWordCheckBox.Enabled = endBox.Text.Length > 0;
        }
    }
}
