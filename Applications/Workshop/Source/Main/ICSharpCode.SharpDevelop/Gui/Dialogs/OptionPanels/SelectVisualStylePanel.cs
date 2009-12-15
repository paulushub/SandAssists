// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class SelectVisualStylePanel : DialogPanel
    {
        public SelectVisualStylePanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.SelectStylePanel.xfrm"));

            showExtensionsCheckBox.Checked = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);

            AddInTreeNode treeNode = AddInTree.GetTreeNode(
                "/SharpDevelop/Workbench/Ambiences");
            foreach (Codon codon in treeNode.Codons)
            {
                selectAmbienceComboBox.Items.Add(codon.Id);
            }

            selectAmbienceComboBox.Text = PropertyService.Get(
                "SharpDevelop.UI.CurrentAmbience", "C#");
            preferProjectAmbienceCheckBox.Checked = AmbienceService.UseProjectAmbienceIfPossible;

            showStatusBarCheckBox.Checked = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
            showToolBarCheckBox.Checked = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
            useProfessionalStyleCheckBox.Checked = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", false);
        }

        public override bool StorePanelContents()
        {
            PropertyService.Set(
                "ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions",
                showExtensionsCheckBox.Checked);
            PropertyService.Set("SharpDevelop.UI.CurrentAmbience",
                selectAmbienceComboBox.Text);
            PropertyService.Set("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", 
                showStatusBarCheckBox.Checked);
            PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", 
                showToolBarCheckBox.Checked);
            PropertyService.Set(
                "ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", 
                useProfessionalStyleCheckBox.Checked);
            
            AmbienceService.UseProjectAmbienceIfPossible = 
                preferProjectAmbienceCheckBox.Checked;
            
            return true;
        }
    }
}
