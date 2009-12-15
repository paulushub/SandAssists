// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class PublishPanel : ProjectDialogPanel
    {
        public PublishPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlResource("ICSharpCode.SharpDevelop.Resources.ProjectOptions.Publish.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ProjectOptions.Publish.xfrm"));

            InitializeHelper();
        }
    }
}
