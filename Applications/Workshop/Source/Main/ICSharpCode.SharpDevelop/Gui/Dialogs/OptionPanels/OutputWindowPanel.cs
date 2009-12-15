// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    /// <summary>
    /// The Output Window options panel.
    /// </summary>
    public partial class OutputWindowPanel : DialogPanel
    {
        public const string OutputWindowsProperty = "SharpDevelop.UI.OutputWindowOptions";
        
        public OutputWindowPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.OutputWindowOptionsPanel.xfrm"));

            Properties properties = (Properties)PropertyService.Get(
                OutputWindowsProperty, new Properties());
            wordWrapCheckBox.Checked = properties.Get("WordWrap", true);

            this.fontSelectionPanel.CurrentFontString = properties.Get("DefaultFont",
                WinFormsResourceService.DefaultMonospacedFont.ToString()).ToString();
        }

        public override bool StorePanelContents()
        {
            Properties properties = (Properties)PropertyService.Get(
                OutputWindowsProperty, new Properties());
            properties.Set("WordWrap", wordWrapCheckBox.Checked);
            string currentFontString = this.fontSelectionPanel.CurrentFontString;
            if (currentFontString != null)
                properties.Set("DefaultFont", currentFontString);

            PropertyService.Set(OutputWindowsProperty, properties);

            return true;
        }
    }
}
