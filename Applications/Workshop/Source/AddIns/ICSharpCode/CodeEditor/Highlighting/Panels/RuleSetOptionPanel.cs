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
    partial class RuleSetOptionPanel : NodeOptionPanel
    {
        public RuleSetOptionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public RuleSetOptionPanel(RuleSetNode parent)
            : base(parent)
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Gui.Resources.RuleSet.xfrm"));

            //nameBox = (TextBox)ControlDictionary["nameBox"];
            //refBox = (TextBox)ControlDictionary["refBox"];
            //delimBox = (TextBox)ControlDictionary["delimBox"];

            //igcaseBox = (CheckBox)ControlDictionary["igcaseBox"];
            //escCharTextBox = (TextBox)ControlDictionary["escCharTextBox"];
        }

        public override void StoreSettings()
        {
            RuleSetNode node = (RuleSetNode)parent;
            if (!node.IsRoot) node.Name = nameBox.Text;
            node.Reference = refBox.Text;
            node.Delimiters = delimBox.Text;
            node.EscapeCharacter = (escCharTextBox.TextLength == 0) ? '\0' : escCharTextBox.Text[0];
            node.IgnoreCase = igcaseBox.Checked;
        }

        public override void LoadSettings()
        {
            RuleSetNode node = (RuleSetNode)parent;

            nameBox.Text = node.Name;

            if (node.IsRoot)
            {
                nameBox.Text = ResourceService.GetString(
                    "Dialog.HighlightingEditor.TreeView.RootRuleSet");
                nameBox.Enabled = false;
            }

            refBox.Text = node.Reference;
            delimBox.Text = node.Delimiters;

            escCharTextBox.Text = (node.EscapeCharacter == '\0') ? "" : node.EscapeCharacter.ToString();
            igcaseBox.Checked = node.IgnoreCase;
        }

        public override bool ValidateSettings()
        {
            if (nameBox.Text == "")
            {
                ValidationMessage(ResourceService.GetString(
                    "Dialog.HighlightingEditor.RuleSet.NameEmpty"));
                return false;
            }

            return true;
        }
    }
}
