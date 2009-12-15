// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2562 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner.Gui
{
    public partial class RenameCategoryDialog : Form
    {
        private string categoryName = String.Empty;

        public RenameCategoryDialog()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls.GetRecursive())
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);
        }


        public RenameCategoryDialog(string categoryName, Form owner)
            : this()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.FormsDesigner.Resources.RenameSidebarCategoryDialog.xfrm"));

            this.Owner = owner;

            if (categoryName == null)
            {
                categoryNameTextBox.Text = "New Category";
                this.Text = StringParser.Parse(
                    "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.NewCategoryDialogName}");
            }
            else
            {
                this.categoryName = categoryName;
                categoryNameTextBox.Text = categoryName;
                this.Text = StringParser.Parse(
                    "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.RenameCategoryDialogName}");
            }

            okButton.Click += new EventHandler(okButtonClick);
        }

        public string CategoryName
        {
            get
            {
                return categoryName;
            }
        }

        private void ShowDuplicateErrorMessage()
        {

            MessageService.ShowError(
                "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.DuplicateNameError}");
        }

        private void okButtonClick(object sender, System.EventArgs e)
        {
            if (categoryName != categoryNameTextBox.Text)
            {
                foreach (Category cat in ToolboxProvider.ComponentLibraryLoader.Categories)
                {
                    if (cat.Name == categoryNameTextBox.Text)
                    {
                        ShowDuplicateErrorMessage();
                        return;
                    }
                }

                foreach (SideTab tab in ToolboxProvider.FormsDesignerSideBar.Tabs)
                {
                    if (!(tab is SideTabDesigner) && !(tab is CustomComponentsSideTab))
                    {
                        if (tab.Name == categoryNameTextBox.Text)
                        {
                            ShowDuplicateErrorMessage();
                            return;
                        }
                    }
                }

                categoryName = categoryNameTextBox.Text;
            }
        }
    }
}
