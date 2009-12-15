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
    partial class MarkerOptionPanel : NodeOptionPanel
    {
        private bool previous;
        private EditorHighlightColor color;

        public MarkerOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public MarkerOptionPanel(MarkerNode parent, bool prev)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Marker.xfrm"));

            //chgBtn = (Button)ControlDictionary["chgBtn"];
            chgBtn.Click += new EventHandler(chgBtnClick);

            //checkBox = (CheckBox)ControlDictionary["checkBox"];
            //nameBox = (TextBox)ControlDictionary["nameBox"];
            //sampleLabel = (Label)ControlDictionary["sampleLabel"];

            previous = prev;
            explLabel.Text = ResourceService.GetString(
                previous ? "Dialog.HighlightingEditor.Marker.ExplanationPrev"
                : "Dialog.HighlightingEditor.Marker.ExplanationNext");
        }

        public override void StoreSettings()
        {
            MarkerNode node = (MarkerNode)parent;

            node.What = nameBox.Text;
            node.Color = color;
            node.MarkMarker = checkBox.Checked;
        }

        public override void LoadSettings()
        {
            MarkerNode node = (MarkerNode)parent;

            sampleLabel.Font = CodeEditorProperties.Instance.FontContainer.DefaultFont;

            color = node.Color;
            nameBox.Text = node.What;
            checkBox.Checked = node.MarkMarker;
            PreviewUpdate(sampleLabel, color);
        }

        public override bool ValidateSettings()
        {
            if (nameBox.Text == "")
            {
                ValidationMessage(ResourceService.GetString(
                    "Dialog.HighlightingEditor.Marker.NameEmpty"));
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
    }
}
