// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1963 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner.Gui
{
    public partial class ConfigureSideBarDialog : Form
    {
        private ArrayList oldComponents;

        public ConfigureSideBarDialog()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls.GetRecursive())
            {
                ctl.Text = StringParser.Parse(ctl.Text);

                ListView listView = ctl as ListView;
                if (listView != null)
                {
                    foreach (ColumnHeader header in listView.Columns)
                    {
                        header.Text = StringParser.Parse(header.Text);
                    }
                }
            }
            this.Text = StringParser.Parse(this.Text);

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.FormsDesigner.Resources.ConfigureSidebarDialog.xfrm"));

            oldComponents = ToolboxProvider.ComponentLibraryLoader.CopyCategories();

            FillCategories();

            categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
            componentListViewSelectedIndexChanged(this, EventArgs.Empty);

            //okButton.Click += new System.EventHandler(this.okButtonClick);
            //newCategoryButton.Click += new System.EventHandler(this.newCategoryButtonClick);
            //renameCategoryButton.Click += new System.EventHandler(this.renameCategoryButtonClick);
            //removeCategoryButton.Click += new System.EventHandler(this.removeCategoryButtonClick);
            //addComponentsButton.Click += new System.EventHandler(this.addComponentsButtonClick);
            //tbbRemoveComponents.Click += new System.EventHandler(this.removeComponentsButtonClick);

            //categoryListView.SelectedIndexChanged += new System.EventHandler(this.categoryListViewSelectedIndexChanged);
            //componentListView.SelectedIndexChanged += new System.EventHandler(this.componentListViewSelectedIndexChanged);
        }

        void FillCategories()
        {
            categoryListView.BeginUpdate();
            categoryListView.Items.Clear();
            foreach (Category category in ToolboxProvider.ComponentLibraryLoader.Categories)
            {
                ListViewItem newItem = new ListViewItem(category.Name);
                newItem.Checked = category.IsEnabled;
                newItem.Tag = category;
                categoryListView.Items.Add(newItem);
            }
            categoryListView.EndUpdate();
        }

        void FillComponents()
        {
            componentListView.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
            componentListView.BeginUpdate();
            componentListView.Items.Clear();
            componentListView.SmallImageList = new ImageList();

            if (categoryListView.SelectedItems != null && 
                categoryListView.SelectedItems.Count == 1)
            {
                Category category = (Category)categoryListView.SelectedItems[0].Tag;
                foreach (ToolComponent component in category.ToolComponents)
                {
                    Bitmap icon = null;
                    string loadError = null;
                    try
                    {
                        icon = ToolboxProvider.ComponentLibraryLoader.GetIcon(component);
                    }
                    catch (Exception e)
                    {

                        icon = IconService.GetBitmap("Icons.16x16.Warning");
                        loadError = e.Message;
                    }
                    if (icon != null)
                    {
                        componentListView.SmallImageList.Images.Add(icon);
                    }

                    ListViewItem newItem = new ListViewItem(component.Name);
                    newItem.SubItems.Add(component.Namespace);
                    newItem.SubItems.Add(loadError != null ? loadError : component.AssemblyName);

                    newItem.Checked = component.IsEnabled;
                    newItem.Tag = component;
                    newItem.ImageIndex = componentListView.SmallImageList.Images.Count - 1;
                    componentListView.Items.Add(newItem);
                }
            }
            componentListView.EndUpdate();
            componentListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
        }

        // THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
        // DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
        void categoryListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            tbbRenameCategory.Enabled = tbbAddComponents.Enabled = CurrentCategory != null;
            FillComponents();
        }

        Category CurrentCategory
        {
            get
            {
                if (categoryListView.SelectedItems != null && 
                    categoryListView.SelectedItems.Count == 1)
                {
                    return (Category)categoryListView.SelectedItems[0].Tag;
                }
                return null;
            }
        }

        void addComponentsButtonClick(object sender, System.EventArgs e)
        {
            AddComponentsDialog addComponentsDialog = new AddComponentsDialog();
            if (addComponentsDialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (ToolComponent component in addComponentsDialog.SelectedComponents)
                {
                    CurrentCategory.ToolComponents.Add(component);
                }
                FillComponents();
                categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
                componentListViewSelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        void newCategoryButtonClick(object sender, System.EventArgs e)
        {
            RenameCategoryDialog renameCategoryDialog = 
                new RenameCategoryDialog(null, this);
            if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK)
            {
                ToolboxProvider.ComponentLibraryLoader.Categories.Add(
                    new Category(renameCategoryDialog.CategoryName));
                FillCategories();
            }
        }

        void removeCategoryButtonClick(object sender, System.EventArgs e)
        {

            if (MessageService.AskQuestion(
                "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveCategoryQuestion}"))
            {
                categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
                ToolboxProvider.ComponentLibraryLoader.Categories.Remove(CurrentCategory);
                FillCategories();
                FillComponents();
                categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
                componentListViewSelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        void componentListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            tbbRemoveComponents.Enabled = 
                componentListView.SelectedItems != null && 
                componentListView.SelectedItems.Count > 0;
        }

        void removeComponentsButtonClick(object sender, System.EventArgs e)
        {

            if (MessageService.AskQuestion(
                "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSideBarDialog.RemoveComponentsQuestion}"))
            {
                foreach (ListViewItem item in componentListView.SelectedItems)
                {
                    CurrentCategory.ToolComponents.Remove((ToolComponent)item.Tag);
                }
                FillComponents();
                componentListViewSelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            if (oldComponents != null)
            {
                ToolboxProvider.ComponentLibraryLoader.Categories = oldComponents;
            }
        }

        void renameCategoryButtonClick(object sender, System.EventArgs e)
        {
            RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(this.CurrentCategory.Name, this);
            if (renameCategoryDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.CurrentCategory.Name = renameCategoryDialog.CategoryName;
                FillCategories();
            }
        }

        void okButtonClick(object sender, System.EventArgs e)
        {
            oldComponents = null;

            foreach (ListViewItem item in categoryListView.Items)
            {
                Category category = (Category)item.Tag;
                category.IsEnabled = item.Checked;
            }

            ToolboxProvider.SaveToolbox();
        }

        void componentListViewItemCheck(object sender, ItemCheckEventArgs e)
        {
            ToolComponent tc = (ToolComponent)componentListView.Items[e.Index].Tag;
            tc.IsEnabled = !tc.IsEnabled;
        }
    }
}
