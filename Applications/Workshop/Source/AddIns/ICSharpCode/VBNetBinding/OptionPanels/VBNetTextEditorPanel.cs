// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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
using ICSharpCode.SharpDevelop.Gui;

namespace VBNetBinding.OptionPanels
{
    public partial class VBNetTextEditorPanel : DialogPanel
    {
        public VBNetTextEditorPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "VBNetBinding.Resources.VBNetTextEditorOptions.xfrm"));

            enableEndConstructsCheckBox.Checked = PropertyService.Get<bool>(
                "VBBinding.TextEditor.EnableEndConstructs", true);
            enableCasingCheckBox.Checked = PropertyService.Get<bool>(
                "VBBinding.TextEditor.EnableCasing", true);
        }

        public override bool StorePanelContents()
        {
            PropertyService.Set<bool>("VBBinding.TextEditor.EnableEndConstructs", 
                enableEndConstructsCheckBox.Checked);
            PropertyService.Set<bool>("VBBinding.TextEditor.EnableCasing", 
                enableCasingCheckBox.Checked);

            return true;
        }
    }
}
