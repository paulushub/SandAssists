// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class CodeGenerationPanel : DialogPanel
    {
        private const string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";

        public CodeGenerationPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.CodeGenerationOptionsPanel.xfrm"));

            Properties p = (Properties)PropertyService.Get(
                codeGenerationProperty, new Properties());

            generateAdditonalCommentsCheckBox.Checked = p.Get("GenerateAdditionalComments", true);
            generateDocCommentsCheckBox.Checked = p.Get("GenerateDocumentComments", true);
            useFullTypeNamesCheckBox.Checked = p.Get("UseFullyQualifiedNames", true);

            blankLinesBetweenMemberCheckBox.Checked = p.Get("BlankLinesBetweenMembers", true);
            elseOnClosingCheckbox.Checked = p.Get("ElseOnClosing", true);
            startBlockOnTheSameLineCheckBox.Checked = p.Get("StartBlockOnSameLine", true);
        }

        public override bool StorePanelContents()
        {
            Properties p = (Properties)PropertyService.Get(codeGenerationProperty, new Properties());
            p.Set("GenerateAdditionalComments", generateAdditonalCommentsCheckBox.Checked);
            p.Set("GenerateDocumentComments", generateDocCommentsCheckBox.Checked);
            p.Set("UseFullyQualifiedNames", useFullTypeNamesCheckBox.Checked);
            p.Set("BlankLinesBetweenMembers", blankLinesBetweenMemberCheckBox.Checked);
            p.Set("ElseOnClosing", elseOnClosingCheckbox.Checked);
            p.Set("StartBlockOnSameLine", startBlockOnTheSameLineCheckBox.Checked);
            
            PropertyService.Set(codeGenerationProperty, p);
            
            return true;
        }
    }
}
