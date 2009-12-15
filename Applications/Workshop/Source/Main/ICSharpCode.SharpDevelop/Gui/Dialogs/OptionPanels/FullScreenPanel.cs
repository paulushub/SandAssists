// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
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
    public partial class FullScreenPanel : DialogPanel
    {
        private const string fullscreenProperty = "ICSharpCode.SharpDevelop.Gui.FullscreenOptions";

        public FullScreenPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.FullscreenPanel.xfrm"));

            Properties properties = PropertyService.Get(fullscreenProperty, 
                new Properties());

            HideMainMenuCheckBox.Checked = properties.Get("HideMainMenu", false);
            ShowMainMenuOnMouseMoveCheckBox.Checked = properties.Get("ShowMainMenuOnMouseMove", true);
            HideToolbarsCheckBox.Checked = properties.Get("HideToolbars", true);
            HideTabsCheckBox.Checked = properties.Get("HideTabs", false);
            HideVerticalScrollbarCheckBox.Checked = properties.Get("HideVerticalScrollbar", false);
            HideHorizontalScrollbarCheckBox.Checked = properties.Get("HideHorizontalScrollbar", false);
            HideStatusBarCheckBox.Checked = properties.Get("HideStatusBar", true);
            ShowStatusBarOnMouseMoveCheckBox.Checked = properties.Get("ShowStatusBarOnMouseMove", true);
            HideWindowsTaskbarCheckBox.Checked = properties.Get("HideWindowsTaskbar", true);

            HideMainMenuCheckBox.CheckedChanged += delegate
            {
                RefreshStatus();
            };
            HideStatusBarCheckBox.CheckedChanged += delegate
            {
                RefreshStatus();
            };

            RefreshStatus();
        }

        public override bool StorePanelContents()
        {
            Properties properties = PropertyService.Get(fullscreenProperty, new Properties());

            properties.Set("HideMainMenu", HideMainMenuCheckBox.Checked);
            properties.Set("ShowMainMenuOnMouseMove", ShowMainMenuOnMouseMoveCheckBox.Checked);
            properties.Set("HideToolbars", HideToolbarsCheckBox.Checked);
            properties.Set("HideTabs", HideTabsCheckBox.Checked);
            properties.Set("HideVerticalScrollbar", HideVerticalScrollbarCheckBox.Checked);
            properties.Set("HideHorizontalScrollbar", HideHorizontalScrollbarCheckBox.Checked);
            properties.Set("HideStatusBar", HideStatusBarCheckBox.Checked);
            properties.Set("ShowStatusBarOnMouseMove", ShowStatusBarOnMouseMoveCheckBox.Checked);
            properties.Set("HideWindowsTaskbar", HideWindowsTaskbarCheckBox.Checked);

            PropertyService.Set(fullscreenProperty, properties);

            return true;
        }

        private void RefreshStatus()
        {
            ShowMainMenuOnMouseMoveCheckBox.Enabled = HideMainMenuCheckBox.Checked;
            ShowStatusBarOnMouseMoveCheckBox.Enabled = HideStatusBarCheckBox.Checked;
        }
    }
}
