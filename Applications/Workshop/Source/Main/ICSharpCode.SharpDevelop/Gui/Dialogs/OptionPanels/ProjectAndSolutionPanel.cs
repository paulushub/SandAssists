// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3253 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class ProjectAndSolutionPanel : DialogPanel
    {
        public ProjectAndSolutionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ProjectAndSolutionOptionsPanel.xfrm"));

            // read properties
            projectLocationTextBox.Text = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath",
                Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal), "Workshop Projects")).ToString();

            loadPrevProjectCheckBox.Checked = PropertyService.Get(
                "SharpDevelop.LoadPrevProjectOnStartup", false);
            showErrorListCheckBox.Checked = Project.BuildOptions.ShowErrorListAfterBuild;
            parallelBuildNumericUpDown.Value = Project.BuildOptions.DefaultParallelProjectCount;

            selectProjectLocationButton.Click += new EventHandler(
                SelectProjectLocationButtonClicked);

            Type type = typeof(Project.BuildOnExecuteSetting);
            foreach (Project.BuildOnExecuteSetting element in Enum.GetValues(type))
            {
                object[] attr = type.GetField(Enum.GetName(type, element)).GetCustomAttributes(typeof(DescriptionAttribute), false);
                string description;
                if (attr.Length > 0)
                {
                    description = StringParser.Parse((attr[0] as DescriptionAttribute).Description);
                }
                else
                {
                    description = Enum.GetName(type, element);
                }
                onExecuteComboBox.Items.Add(description);
            }
            onExecuteComboBox.SelectedIndex = (int)Project.BuildModifiedProjectsOnlyService.Setting;
        }

        public override bool StorePanelContents()
        {
            // check for correct settings
            string projectPath = projectLocationTextBox.Text;
            if (projectPath.Length > 0)
            {
                if (!FileUtility.IsValidPath(projectPath))
                {
                    MessageService.ShowError(StringParser.Parse(
                        "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.InvalidProjectPathSpecified}"));
                    return false;
                }
            }

            // set properties
            PropertyService.Set(
                "ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);
            PropertyService.Set("SharpDevelop.LoadPrevProjectOnStartup",
                loadPrevProjectCheckBox.Checked);
            Project.BuildOptions.ShowErrorListAfterBuild =
                showErrorListCheckBox.Checked;
            Project.BuildOptions.DefaultParallelProjectCount = 
                (int)parallelBuildNumericUpDown.Value;

            Project.BuildModifiedProjectsOnlyService.Setting =
                (Project.BuildOnExecuteSetting)onExecuteComboBox.SelectedIndex;

            return true;
        }

        private void SelectProjectLocationButtonClicked(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(
                "${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.SelectDefaultProjectLocationDialog.Title}", 
                projectLocationTextBox.Text))
            {
                if (fdiag.ShowDialog() == DialogResult.OK)
                {
                    projectLocationTextBox.Text = fdiag.SelectedPath;
                }
            }
        }
    }
}
