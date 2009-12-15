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
    partial class SchemeOptionPanel : NodeOptionPanel
    {
        public SchemeOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public SchemeOptionPanel(SchemeNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.Scheme.xfrm"));
            //nameBox = (TextBox)ControlDictionary["nameBox"];
            //extBox = (TextBox)ControlDictionary["extBox"];
        }

        public override void StoreSettings()
        {
            SchemeNode node = (SchemeNode)parent;
            node.Name = nameBox.Text;
            node.Extensions = extBox.Text.Split(';');
        }

        public override void LoadSettings()
        {
            SchemeNode node = (SchemeNode)parent;
            nameBox.Text = node.Name;
            extBox.Text = String.Join(";", node.Extensions);
        }

        public override bool ValidateSettings()
        {
            if (nameBox.Text == "")
            {
                ValidationMessage(
                    "${res:Dialog.HighlightingEditor.Scheme.NameEmpty}");
                return false;
            }

            return true;
        }
    }
}
