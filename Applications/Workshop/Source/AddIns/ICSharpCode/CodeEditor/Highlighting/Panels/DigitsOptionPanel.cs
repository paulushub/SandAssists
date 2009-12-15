// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 2487 $</version>
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
    partial class DigitsOptionPanel : NodeOptionPanel
    {
        EditorHighlightColor color = new EditorHighlightColor();

        public DigitsOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public DigitsOptionPanel(DigitsNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Digits.xfrm"));

            //button = (Button)ControlDictionary["button"];
            button.Click += new EventHandler(EditButtonClicked);
            //sampleLabel = (Label)ControlDictionary["sampleLabel"];
        }

        public override void StoreSettings()
        {
            DigitsNode node = (DigitsNode)this.ParentNode;

            node.Color = color;
        }

        public override void LoadSettings()
        {
            DigitsNode node = (DigitsNode)this.ParentNode;

            sampleLabel.Font = CodeEditorProperties.Instance.FontContainer.DefaultFont;
            color = node.Color;
            PreviewUpdate(sampleLabel, color);
        }

        private void EditButtonClicked(object sender, EventArgs e)
        {
            using (HighlightingColorDialog dlg = new HighlightingColorDialog(color))
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
