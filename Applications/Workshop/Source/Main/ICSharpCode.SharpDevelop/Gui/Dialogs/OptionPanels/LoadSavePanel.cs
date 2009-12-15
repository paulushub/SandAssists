// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2997 $</version>
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
    public partial class LoadSavePanel : DialogPanel
    {
        public LoadSavePanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.LoadSaveOptionPanel.xfrm"));

            loadUserDataCheckBox.Checked = PropertyService.Get(
                "SharpDevelop.LoadDocumentProperties", true);
            createBackupCopyCheckBox.Checked = FileService.SaveUsingTemporaryFile;

            lineTerminatorStyleComboBox.Items.Add(StringParser.Parse(
                "${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}"));
            lineTerminatorStyleComboBox.Items.Add(StringParser.Parse(
                "${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}"));
            lineTerminatorStyleComboBox.Items.Add(StringParser.Parse(
                "${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}"));

            lineTerminatorStyleComboBox.SelectedIndex = 
                (int)(LineTerminatorStyle)PropertyService.Get(
                "SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);

            //autoLoadExternalChangesCheckBox = Get<CheckBox>("autoLoadExternalChanges");
            //detectExternalChangesCheckBox = Get<CheckBox>("detectExternalChanges");
            //useRecycleBinCheckBox = Get<CheckBox>("useRecycleBin");

            detectExternalChangesCheckBox.CheckedChanged += delegate
            {
                autoLoadExternalChangesCheckBox.Enabled = detectExternalChangesCheckBox.Checked;
            };
            autoLoadExternalChangesCheckBox.Enabled = detectExternalChangesCheckBox.Checked;

            detectExternalChangesCheckBox.Checked = FileChangeWatcher.DetectExternalChangesOption;
            autoLoadExternalChangesCheckBox.Checked = FileChangeWatcher.AutoLoadExternalChangesOption;
            useRecycleBinCheckBox.Checked = FileService.DeleteToRecycleBin;
        }

        public override bool StorePanelContents()
        {
            PropertyService.Set("SharpDevelop.LoadDocumentProperties", loadUserDataCheckBox.Checked);
            PropertyService.Set("SharpDevelop.LineTerminatorStyle", (LineTerminatorStyle)lineTerminatorStyleComboBox.SelectedIndex);

            FileChangeWatcher.DetectExternalChangesOption = detectExternalChangesCheckBox.Checked;
            FileChangeWatcher.AutoLoadExternalChangesOption = autoLoadExternalChangesCheckBox.Checked;
            FileService.DeleteToRecycleBin = useRecycleBinCheckBox.Checked;
            FileService.SaveUsingTemporaryFile = createBackupCopyCheckBox.Checked;

            return true;
        }
    }

    public enum LineTerminatorStyle
    {
        Windows,
        Macintosh,
        Unix
    }
}
