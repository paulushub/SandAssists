// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 2043 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace VBNetBinding.OptionPanels
{
    public partial class ProjectImportsPanel : ProjectDialogPanel
    {
        public ProjectImportsPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlResource("VBNetBinding.Resources.ProjectImports.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "VBNetBinding.Resources.ProjectImports.xfrm"));

            InitializeHelper();

            addImportButton.Click += new EventHandler(addImportButton_Click);
            removeImportButton.Click += new EventHandler(removeImportButton_Click);
            namespacesComboBox.TextChanged += new EventHandler(namespacesComboBox_TextCanged);
            importsListBox.SelectedIndexChanged += new EventHandler(importsListBox_SelectedIndexChanged);

            namespacesComboBox.Items.Clear();
            namespacesComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            namespacesComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            foreach (ProjectItem item in project.Items)
            {
                if (item.ItemType == ItemType.Import)
                {
                    importsListBox.Items.Add(item.Include);
                }
            }

            IProjectContent projectContent = ParserService.GetProjectContent(project);
            foreach (IProjectContent refProjectContent in projectContent.ReferencedContents)
            {
                AddNamespaces(refProjectContent);

            }
            AddNamespaces(projectContent);

            namespacesComboBox_TextCanged(null, EventArgs.Empty);
            importsListBox_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void AddNamespaces(IProjectContent projectContent)
        {
            foreach (string projectNamespace in projectContent.NamespaceNames)
            {
                if (projectNamespace != "")
                {
                    if (!namespacesComboBox.Items.Contains(projectNamespace))
                    {
                        namespacesComboBox.Items.Add(projectNamespace);
                    }
                }
            }
        }

        private void namespacesComboBox_TextCanged(object sender, EventArgs e)
        {
            addImportButton.Enabled = namespacesComboBox.Text != "" &&
                !importsListBox.Items.Contains(namespacesComboBox.Text);
        }

        private void importsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeImportButton.Enabled = importsListBox.SelectedIndex != -1;
        }

        private void removeImportButton_Click(object sender, EventArgs e)
        {
            importsListBox.Items.RemoveAt(importsListBox.SelectedIndex);
            IsDirty = true;
        }

        private void addImportButton_Click(object sender, EventArgs e)
        {
            importsListBox.Items.Add(namespacesComboBox.Text);
            namespacesComboBox.Text = "";
            IsDirty = true;
        }

        public override bool StorePanelContents()
        {
            List<ProjectItem> imports = new List<ProjectItem>();
            foreach (ProjectItem item in project.Items)
            {
                if (item.ItemType == ItemType.Import)
                {
                    imports.Add(item);
                }
            }

            foreach (ImportProjectItem item in imports)
            {
                ProjectService.RemoveProjectItem(project, item);
            }

            foreach (string importedNamespace in importsListBox.Items)
            {
                ProjectService.AddProjectItem(project, 
                    new ImportProjectItem(project, importedNamespace));
            }
            return base.StorePanelContents();
        }
    }
}
