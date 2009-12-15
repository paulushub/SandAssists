// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class CodeTemplatePanel : DialogPanel
    {
        private List<CodeTemplateGroup> templateGroups;
        private int currentSelectedGroup = -1;

        public CodeTemplatePanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public CodeTemplateGroup CurrentTemplateGroup
        {
            get
            {
                if (currentSelectedGroup < 0 || 
                    currentSelectedGroup >= templateGroups.Count)
                {
                    return null;
                }

                return templateGroups[currentSelectedGroup];
            }
        }

        public override void LoadPanelContents()
        {
            templateGroups = CopyCodeTemplateGroups(CodeTemplateLoader.TemplateGroups);

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.CodeTemplatePanel.xfrm"));

            removeButton.Click += new System.EventHandler(RemoveEvent);
            addButton.Click += new System.EventHandler(AddEvent);
            editButton.Click += new System.EventHandler(EditEvent);

            addGroupButton.Click += new System.EventHandler(AddGroupEvent);
            removeGroupButton.Click += new System.EventHandler(RemoveGroupEvent);


            templateTextBox.Font = WinFormsResourceService.DefaultMonospacedFont;
            templateTextBox.TextChanged += new EventHandler(TextChange);

            templateListView.Activation = ItemActivation.Standard;
            templateListView.ItemActivate += new System.EventHandler(EditEvent);
            templateListView.SelectedIndexChanged += new System.EventHandler(IndexChange);

            groupComboBox.DropDown += new EventHandler(FillGroupBoxEvent);

            if (templateGroups.Count > 0)
            {
                currentSelectedGroup = 0;
            }

            FillGroupComboBox();
            BuildListView();
            IndexChange(null, null);
            SetEnabledStatus();
        }

        public override bool StorePanelContents()
        {
            CodeTemplateLoader.TemplateGroups = templateGroups;
            CodeTemplateLoader.SaveTemplates();
            return true;
        }

        private void FillGroupBoxEvent(object sender, EventArgs e)
        {
            FillGroupComboBox();
        }

        private void SetEnabledStatus()
        {
            bool groupSelected = CurrentTemplateGroup != null;
            bool groupsEmpty = templateGroups.Count != 0;

            SetEnabledStatus(groupSelected, addButton, editButton, 
                removeButton, templateListView, templateTextBox);
            SetEnabledStatus(groupsEmpty, groupComboBox, extensionLabel);
            if (groupSelected)
            {
                bool oneItemSelected = templateListView.SelectedItems.Count == 1;
                bool isItemSelected = templateListView.SelectedItems.Count > 0;
                SetEnabledStatus(oneItemSelected, editButton, templateTextBox);
                SetEnabledStatus(isItemSelected, removeButton);
            }
        }

        #region GroupComboBox event handler

        private void SetGroupSelection(object sender, EventArgs e)
        {
            currentSelectedGroup = groupComboBox.SelectedIndex;
            BuildListView();
        }

        private void GroupComboBoxTextChanged(object sender, EventArgs e)
        {
            if (groupComboBox.SelectedIndex >= 0)
            {
                currentSelectedGroup = groupComboBox.SelectedIndex;
            }
            if (CurrentTemplateGroup != null)
            {
                CurrentTemplateGroup.ExtensionStrings = groupComboBox.Text.Split(';');
            }
        }

        #endregion

        #region Group Button events

        private void AddGroupEvent(object sender, EventArgs e)
        {
            templateGroups.Add(new CodeTemplateGroup(".???"));
            FillGroupComboBox();
            groupComboBox.SelectedIndex = templateGroups.Count - 1;
            SetEnabledStatus();
        }

        private void RemoveGroupEvent(object sender, EventArgs e)
        {
            if (CurrentTemplateGroup != null)
            {
                templateGroups.RemoveAt(currentSelectedGroup);
                if (templateGroups.Count == 0)
                {
                    currentSelectedGroup = -1;
                }
                else
                {
                    groupComboBox.SelectedIndex = Math.Min(
                        currentSelectedGroup, templateGroups.Count - 1);
                }
                FillGroupComboBox();
                BuildListView();
                SetEnabledStatus();
            }
        }
        #endregion

        #region Template Button events

        private void RemoveEvent(object sender, System.EventArgs e)
        {
            object[] selectedItems = new object[templateListView.SelectedItems.Count];
            templateListView.SelectedItems.CopyTo(selectedItems, 0);

            foreach (ListViewItem item in selectedItems)
            {
                templateListView.Items.Remove(item);
            }
            StoreTemplateGroup();
        }

        private void AddEvent(object sender, System.EventArgs e)
        {
            CodeTemplate newTemplate = new CodeTemplate();
            using (EditTemplateDialog etd = new EditTemplateDialog(newTemplate))
            {
                if (etd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK)
                {
                    CurrentTemplateGroup.Templates.Add(newTemplate);
                    templateListView.SelectedItems.Clear();
                    BuildListView();
                    templateListView.Select();
                }
            }
        }

        private void EditEvent(object sender, System.EventArgs e)
        {
            int i = GetCurrentIndex();
            if (i != -1)
            {
                ListViewItem item = templateListView.SelectedItems[0];
                CodeTemplate template = (CodeTemplate)item.Tag;
                template = new CodeTemplate(template.Shortcut, template.Description, template.Text);

                using (EditTemplateDialog etd = new EditTemplateDialog(template))
                {
                    if (etd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK)
                    {
                        item.Tag = template;
                        StoreTemplateGroup();
                    }
                }

                BuildListView();
            }
        }

        #endregion

        private void FillGroupComboBox()
        {
            groupComboBox.TextChanged -= new EventHandler(GroupComboBoxTextChanged);
            groupComboBox.SelectedIndexChanged -= new EventHandler(SetGroupSelection);

            groupComboBox.Items.Clear();
            foreach (CodeTemplateGroup templateGroup in templateGroups)
            {
                groupComboBox.Items.Add(String.Join(";", templateGroup.ExtensionStrings));
            }
            groupComboBox.Text = CurrentTemplateGroup != null ? 
                groupComboBox.Items[currentSelectedGroup].ToString() : String.Empty;
            if (currentSelectedGroup >= 0)
            {
                groupComboBox.SelectedIndex = currentSelectedGroup;
            }

            groupComboBox.SelectedIndexChanged += new EventHandler(SetGroupSelection);
            groupComboBox.TextChanged += new EventHandler(GroupComboBoxTextChanged);
        }

        private int GetCurrentIndex()
        {
            if (templateListView.SelectedItems.Count == 1)
            {
                return templateListView.SelectedItems[0].Index;
            }
            return -1;
        }

        private void IndexChange(object sender, System.EventArgs e)
        {
            int i = GetCurrentIndex();

            if (i != -1)
            {
                templateTextBox.Text = ((CodeTemplate)templateListView.SelectedItems[0].Tag).Text;
            }
            else
            {
                templateTextBox.Text = String.Empty;
            }
            SetEnabledStatus();
        }

        private void TextChange(object sender, EventArgs e)
        {
            int i = GetCurrentIndex();
            if (i != -1)
            {
                ((CodeTemplate)templateListView.SelectedItems[0].Tag).Text = 
                    templateTextBox.Text;
            }
        }

        private void StoreTemplateGroup()
        {
            if (CurrentTemplateGroup != null)
            {
                CurrentTemplateGroup.Templates.Clear();
                foreach (ListViewItem item in templateListView.Items)
                {
                    CurrentTemplateGroup.Templates.Add((CodeTemplate)item.Tag);
                }
            }
        }

        private void BuildListView()
        {
            templateListView.BeginUpdate();
            templateListView.Items.Clear();
            if (CurrentTemplateGroup != null)
            {
                IList<CodeTemplate> listTemplate = CurrentTemplateGroup.Templates;
                if (listTemplate != null && listTemplate.Count != 0)
                {
                    int itemCount = listTemplate.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        CodeTemplate template = listTemplate[i];
                        if (template == null)
                        {
                            continue;
                        }
                        ListViewItem newItem = new ListViewItem(
                            new string[] { template.Shortcut, template.Description });
                        newItem.Tag = template;
                        templateListView.Items.Add(newItem);
                    }
                }
            }
            templateListView.AutoResizeColumn(0, 
                ColumnHeaderAutoResizeStyle.HeaderSize);
            templateListView.AutoResizeColumn(1,
                ColumnHeaderAutoResizeStyle.ColumnContent);
            templateListView.EndUpdate();

            IndexChange(this, EventArgs.Empty);
        }

        private List<CodeTemplateGroup> CopyCodeTemplateGroups(IList<CodeTemplateGroup> groups)
        {
            List<CodeTemplateGroup> copiedGroups = new List<CodeTemplateGroup>();
            foreach (CodeTemplateGroup group in groups)
            {
                CodeTemplateGroup newGroup = new CodeTemplateGroup(String.Join(";", group.ExtensionStrings));
                foreach (CodeTemplate template in group.Templates)
                {
                    CodeTemplate newTemplate = new CodeTemplate(template.Shortcut, template.Description, template.Text);
                    newGroup.Templates.Add(newTemplate);
                }
                copiedGroups.Add(newGroup);
            }
            return copiedGroups;
        }
    }
}
