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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.TextEditor.Searching
{
    public partial class SearchDialog : Form
    {
        public static string SearchPattern = String.Empty;
        public static string ReplacePattern = String.Empty;

        private Keys searchKeyboardShortcut = Keys.None;
        private Keys replaceKeyboardShortcut = Keys.None;
        private const string SearchMenuAddInPath = "/SharpDevelop/Workbench/MainMenu/Edit/Search";

        private static SearchDialog Instance;
  
        public SearchDialog()
        {
            InitializeComponent();

            // We allow the horizontal resizing of the form, and prevent the vertical...
            Rectangle rect = Screen.PrimaryScreen.WorkingArea; 
            this.MaximumSize = new Size(rect.Width, this.Height);
        }

        public SearchDialog(SearchAndReplaceMode searchAndReplaceMode)
            : this()
        {
            searchPanel.Initialize();

            this.Owner = WorkbenchSingleton.MainForm;
            this.Text = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.Title}");

            searchButton.Text    = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.FindDialogName}");
            searchButton.Image   = IconService.GetBitmap("Icons.16x16.FindIcon");
            searchButton.Checked = searchAndReplaceMode == SearchAndReplaceMode.Search;
            searchButton.Click  += new EventHandler(SearchButtonClick);

            replaceButton.Text    = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.ReplaceDialogName}");
            replaceButton.Image   = IconService.GetBitmap("Icons.16x16.ReplaceIcon");
            replaceButton.Checked = searchAndReplaceMode == SearchAndReplaceMode.Replace;
            replaceButton.Click += new EventHandler(ReplaceButtonClick);

            useComboBox.Items.Clear();
            useComboBox.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.Standard}"));
            useComboBox.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch}"));
            useComboBox.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.WildcardSearch}"));
            switch (SearchOptions.SearchStrategyType)
            {
                case SearchStrategyType.RegEx:
                    useComboBox.SelectedIndex = 1;
                    break;
                case SearchStrategyType.Wildcard:
                    useComboBox.SelectedIndex = 2;
                    break;
                default:
                    useComboBox.SelectedIndex = 0;
                    break;
            }
            useComboBox.SelectedIndexChanged += new EventHandler(OnUseSelectedIndexChanged);

            useButton.Text   = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.UseMethodLabel}");

            searchHelp.Image = IconService.GetBitmap("Icons.16x16.ToolHelp");
            searchHelp.Text  = StringParser.Parse("${res:Global.HelpButtonText}");

            RightToLeftConverter.ConvertRecursive(this);

            SetSearchAndReplaceMode();
            FormLocationHelper.Apply(this, "ICSharpCode.SharpDevelop.Gui.SearchDialog.Location", false);

            searchKeyboardShortcut  = GetKeyboardShortcut(SearchMenuAddInPath, "Find");
            replaceKeyboardShortcut = GetKeyboardShortcut(SearchMenuAddInPath, "Replace");

            ToolStripRenderer stripRenderer = SearchOptions.StripRenderer;
            if (stripRenderer == null)
            {
                toolStrip.Renderer = ToolbarService.StretchedRenderer;
            }
            if (stripRenderer != null)
            {
                toolStrip.Renderer = stripRenderer;
            }
        }

        public static void ShowSingleInstance(SearchAndReplaceMode searchAndReplaceMode)
        {
            if (Instance == null)
            {
                Instance = new SearchDialog(searchAndReplaceMode);
                Instance.Show(WorkbenchSingleton.MainForm);
            }
            else
            {
                if (searchAndReplaceMode == SearchAndReplaceMode.Search)
                {
                    Instance.searchButton.PerformClick();
                }
                else
                {
                    Instance.replaceButton.PerformClick();
                }
                Instance.Focus();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (searchPanel != null)
            {
                searchPanel.Uninitialize();
            }

            base.OnClosing(e);
            Instance = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close();
            }
            else if (searchKeyboardShortcut == e.KeyData && !searchButton.Checked)
            {
                EnableSearchMode(true);
            }
            else if (replaceKeyboardShortcut == e.KeyData && !replaceButton.Checked)
            {
                EnableSearchMode(false);
            }
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            if (!searchButton.Checked)
            {
                EnableSearchMode(true);
            }
        }

        private void ReplaceButtonClick(object sender, EventArgs e)
        {
            if (!replaceButton.Checked)
            {
                EnableSearchMode(false);
            }
        }

        private void EnableSearchMode(bool enable)
        {
            searchButton.Checked = enable;
            replaceButton.Checked = !enable;
            SetSearchAndReplaceMode();

            this.Focus();
        }

        private void SetSearchAndReplaceMode()
        {
            SearchOptions.SearchStrategyType = (SearchStrategyType)useComboBox.SelectedIndex;
            searchPanel.Mode = searchButton.Checked ? SearchAndReplaceMode.Search : SearchAndReplaceMode.Replace;
            //searchAndReplacePanel.SearchAndReplaceMode = searchButton.Checked ? SearchAndReplaceMode.Search : SearchAndReplaceMode.Replace;
            //if (searchButton.Checked)
            //{
            //    this.ClientSize = new Size(430, 335);
            //}
            //else
            //{
            //    this.ClientSize = new Size(430, 385);
            //}
        }

        /// <summary>
        /// Gets the keyboard shortcut for the menu item with the given addin tree
        /// path and given codon id.
        /// </summary>
        private Keys GetKeyboardShortcut(string path, string id)
        {
            AddInTreeNode node = AddInTree.GetTreeNode(path);
            if (node != null)
            {
                foreach (Codon codon in node.Codons)
                {
                    if (codon.Id == id)
                    {
                        return MenuCommand.ParseShortcut(codon.Properties["shortcut"]);
                    }
                }
            }
            return Keys.None;
        }

        private void OnUseSelectedIndexChanged(object sender, EventArgs e)
        {
            SearchOptions.SearchStrategyType = (SearchStrategyType)useComboBox.SelectedIndex;
        }

        private void OnSearchHelp(object sender, EventArgs e)
        {

        }
    }
}
