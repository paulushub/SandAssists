// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 3586 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic; 

using ICSharpCode.SharpDevelop.Project;
using RadioBinding = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Project.StartAction, System.Windows.Forms.RadioButton>;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class DebugPanel : ProjectDialogPanel
    {
        public DebugPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlResource("ICSharpCode.SharpDevelop.Resources.ProjectOptions.DebugOptions.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ProjectOptions.DebugOptions.xfrm"));

            ConnectBrowseButton(startExternalProgramBrowseButton, startExternalProgramTextBox,
                                "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd",
                                TextBoxEditMode.EditRawProperty);
            ConnectBrowseFolder(workingDirectoryBrowseButton, workingDirectoryTextBox,
                                TextBoxEditMode.EditRawProperty);

            InitializeHelper();

            ConfigurationBinding b;
            ChooseStorageLocationButton locationButton;

            b = helper.BindRadioEnum("StartAction",
                new RadioBinding(StartAction.Project, startProjectRadioButton),
                new RadioBinding(StartAction.Program, startExternalProgramRadioButton),
                new RadioBinding(StartAction.StartURL, startBrowserInURLRadioButton));
            b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            locationButton = b.CreateLocationButtonInPanel(startActionGroupBox);

            b = helper.BindString(startExternalProgramTextBox, 
                "StartProgram", TextBoxEditMode.EditRawProperty);
            b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            b.RegisterLocationButton(locationButton);

            b = helper.BindString(startBrowserInURLTextBox, "StartURL", TextBoxEditMode.EditRawProperty);
            b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            b.RegisterLocationButton(locationButton);

            startExternalProgramRadioButton.CheckedChanged += UpdateEnabledStates;
            startBrowserInURLRadioButton.CheckedChanged += UpdateEnabledStates;

            b = helper.BindString(commandLineArgumentsTextBox, 
                "StartArguments", TextBoxEditMode.EditRawProperty);
            locationButton = b.CreateLocationButtonInPanel(startOptionsGroupBox);
            b = helper.BindString(workingDirectoryTextBox, 
                "StartWorkingDirectory", TextBoxEditMode.EditRawProperty);
            b.RegisterLocationButton(locationButton);

            UpdateEnabledStates(this, EventArgs.Empty);

            helper.AddConfigurationSelector(this);
        }

        private void UpdateEnabledStates(object sender, EventArgs e)
        {
            startExternalProgramTextBox.Enabled = startExternalProgramBrowseButton.Enabled = startExternalProgramRadioButton.Checked;
            startBrowserInURLTextBox.Enabled    = startBrowserInURLRadioButton.Checked;
        }
    }
}
