using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Searching
{
    public partial class ReplacePanel : UserControl
    {
        private const int customDirectoryIndex = 5;

        private VisualStyleRenderer _renderer;

        private ISelection selection;
        private TextEditorControl textEditor;
        private bool ignoreSelectionChanges;
        private bool findFirst;

        private SearchPanel parentPanel;

        public ReplacePanel()
        {
            InitializeComponent();

            foreach (Control control in this.Controls.GetRecursive())
            {
                control.Text = StringParser.Parse(control.Text);
            }
            // For now...
            labelPath.Text = "Path:";

            //string[] lookInTexts = 
            //{
            //    // must be in the same order as the DocumentIteratorType enum
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}",
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}",
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}",
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}",
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}"
            //};
            //foreach (string lookInText in lookInTexts)
            //{
            //    string labelText = StringParser.Parse(lookInText);
            //    lookInComboBox.Items.Add(new SearchScope(labelText, true));
            //}

            //lookInComboBox.Items.Add(new SearchScope("Directories", false));

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable, true);

            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(
               VisualStyleElement.Status.Gripper.Normal))
            {
                _renderer = new VisualStyleRenderer(
                    VisualStyleElement.Status.Gripper.Normal);
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        public void Initialize()
        {
            //this.ParentForm.AcceptButton = replaceButton;
            if (parentPanel != null)
            {
                Form parentForm = parentPanel.ParentForm;
                if (parentForm != null)
                {
                    parentForm.AcceptButton = replaceButton;
                }
            }

            //findComboBox.TextChanged += FindPatternChanged;
            //findNextButton.Click += FindNextButtonClicked;
            //lookInBrowseButton.Click += LookInBrowseButtonClicked;
            //((Form)Parent).AcceptButton = findNextButton;
            SetOptions();
            EnableButtons(HasFindPattern);
            RightToLeftConverter.ReConvertRecursive(this);
        }

        public void Uninitialize(bool serialize)
        {
            if (serialize)
            {
                this.WritebackOptions();
            }
        }

        public void ScopeChanged(EventArgs args)
        {
            this.SetOptions();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int gripSize = 14;

            if (_renderer != null)
            {
                gripSize += 2;
                Rectangle rect = new Rectangle(
                    this.Width - gripSize, this.Height - gripSize, gripSize, gripSize);

                _renderer.DrawBackground(e.Graphics, rect);
            }
            else
            {
                Rectangle rect = new Rectangle(
                    this.Width - gripSize, this.Height - gripSize, gripSize, gripSize);
                ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rect);
            }
        }

        public DocumentIteratorType DocumentType
        {
            get
            {
                return (DocumentIteratorType)(lookInComboBox.SelectedIndex);
            }
            set
            {
                lookInComboBox.SelectedIndex = (int)value;
            }
        }

        public SearchPanel SearchPanel
        {
            get
            {
                return parentPanel;
            }
            set
            {
                parentPanel = value;
            }
        }


        private void WritebackOptions()
        {
            SearchOptions.FindPattern = findComboBox.Text;
            SearchOptions.ReplacePattern = replaceComboBox.Text;

            if (lookInComboBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                SearchOptions.LookIn = lookInComboBox.Text;
            }
            SearchOptions.LookInFiletype = fileTypesComboBox.Text;
            SearchOptions.MatchCase = matchCaseCheckBox.Checked;
            SearchOptions.MatchWholeWord = matchWholeWordCheckBox.Checked;
            SearchOptions.IncludeSubdirectories = includeSubFolderCheckBox.Checked;

            //SearchOptions.SearchStrategyType = (SearchStrategyType)useComboBox.SelectedIndex;
            if (lookInComboBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                SearchOptions.DocumentIteratorType = DocumentIteratorType.Directory;
            }
            else
            {
                SearchOptions.DocumentIteratorType = (DocumentIteratorType)lookInComboBox.SelectedIndex;
            }
        }

        private void LookInBrowseButtonClicked(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = FileService.CreateFolderBrowserDialog(
                "${res:Dialog.NewProject.SearchReplace.LookIn.SelectDirectory}", 
                lookInComboBox.Text))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pathComboBox.Items.Insert(0, dlg.SelectedPath);
                    pathComboBox.SelectedIndex = 0;
                }
            }
        }

        private void FindNextButtonClicked(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    FindNextInSelection();
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.FindNext(monitor);
                }
            }
            Focus();
        }

        private void ReplaceAllButtonClicked(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    RunAllInSelection(2);
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.ReplaceAll(monitor);
                }
            }
        }

        private void ReplaceButtonClicked(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    ReplaceInSelection();
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.Replace(monitor);
                }
            }
            Focus();
        }

        private void SetOptions()
        {
            findComboBox.Text = SearchOptions.FindPattern;
            findComboBox.Items.Clear();

            findComboBox.Text = SearchOptions.FindPattern;
            findComboBox.Items.Clear();
            foreach (string findPattern in SearchOptions.FindPatterns)
            {
                findComboBox.Items.Add(findPattern);
            }

            replaceComboBox.Text = SearchOptions.ReplacePattern;
            replaceComboBox.Items.Clear();
            foreach (string replacePattern in SearchOptions.ReplacePatterns)
            {
                replaceComboBox.Items.Add(replacePattern);
            }

            lookInComboBox.Text = SearchOptions.LookIn;
            string[] lookInTexts = {
				// must be in the same order as the DocumentIteratorType enum
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}",
				"Directories"
			};
            lookInComboBox.Items.Clear();
            int itemCount = lookInTexts.Length;
            for (int i = 0; i < itemCount; i++)
            {
                string lookInText = lookInTexts[i];
                //lookInComboBox.Items.Add(StringParser.Parse(lookInText));
                string labelText = StringParser.Parse(lookInText);
                lookInComboBox.Items.Add(
                    new SearchScope(labelText, (DocumentIteratorType)i));
            }
            //foreach (string lookInText in lookInTexts)
            //{
            //    //lookInComboBox.Items.Add(StringParser.Parse(lookInText));
            //    string labelText = StringParser.Parse(lookInText);
            //    lookInComboBox.Items.Add(new SearchScope(labelText, true));
            //}
            //lookInComboBox.Items.Add(new SearchScope("Directories", false));
            //lookInComboBox.Items.Add(SearchOptions.LookIn);
            //lookInComboBox.SelectedIndexChanged += new EventHandler(LookInSelectedIndexChanged);

            if (IsMultipleLineSelection(GetCurrentTextSelection()))
            {
                this.DocumentType = DocumentIteratorType.CurrentSelection;
            }
            else
            {
                if (SearchOptions.DocumentIteratorType == DocumentIteratorType.CurrentSelection)
                {
                    SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
                }
                this.DocumentType = SearchOptions.DocumentIteratorType;
            }

            fileTypesComboBox.Text = SearchOptions.LookInFiletype;
            matchCaseCheckBox.Checked = SearchOptions.MatchCase;
            matchWholeWordCheckBox.Checked = SearchOptions.MatchWholeWord;
            includeSubFolderCheckBox.Checked = SearchOptions.IncludeSubdirectories;

            //Get<ComboBox>("use").Items.Clear();
            //Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.Standard}"));
            //Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch}"));
            //Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.WildcardSearch}"));
            //switch (SearchOptions.SearchStrategyType)
            //{
            //    case SearchStrategyType.RegEx:
            //        Get<ComboBox>("use").SelectedIndex = 1;
            //        break;
            //    case SearchStrategyType.Wildcard:
            //        Get<ComboBox>("use").SelectedIndex = 2;
            //        break;
            //    default:
            //        Get<ComboBox>("use").SelectedIndex = 0;
            //        break;
            //}
        }

        private void LookInSelectedIndexChanged(object sender, EventArgs e)
        {
            SearchScope selScope = lookInComboBox.SelectedItem as SearchScope;
            if (selScope == null)
            {
                return;
            }

            //if (lookInComboBox.SelectedIndex == customDirectoryIndex)
            if (selScope.IteratorType != DocumentIteratorType.Directory)
            {
                includeSubFolderCheckBox.Enabled = false;
                fileTypesComboBox.Enabled = false;
                fileTypesComboBox.Enabled = false;
            }
            else       
            {
                includeSubFolderCheckBox.Enabled = true;
                fileTypesComboBox.Enabled = true;
                fileTypesComboBox.Enabled = true;
            }
            if (IsSelectionSearch)
            {
                InitSelectionSearch();
            }
            else
            {
                RemoveSelectionSearchHandlers();
            }
        }

        private bool IsSelectionSearch
        {
            get
            {
                return this.DocumentType == DocumentIteratorType.CurrentSelection;
            }
        }

        /// <summary>
        /// Checks whether the selection spans two or more lines.
        /// </summary>
        /// <remarks>Maybe the ISelection interface should have an
        /// IsMultipleLine method?</remarks>
        private static bool IsMultipleLineSelection(ISelection selection)
        {
            if (IsTextSelected(selection))
            {
                return selection.SelectedText.IndexOf('\n') != -1;
            }
            return false;
        }

        private static bool IsTextSelected(ISelection selection)
        {
            if (selection != null)
            {
                return !selection.IsEmpty;
            }
            return false;
        }

        private void FindNextInSelection()
        {
            int startOffset = Math.Min(selection.Offset, selection.EndOffset);
            int endOffset = Math.Max(selection.Offset, selection.EndOffset);

            if (findFirst)
            {
                SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
            }

            try
            {
                ignoreSelectionChanges = true;
                if (findFirst)
                {
                    findFirst = false;
                    SearchReplaceManager.FindFirstInSelection(startOffset, endOffset - startOffset, null);
                }
                else
                {
                    findFirst = !SearchReplaceManager.FindNextInSelection(null);
                    if (findFirst)
                    {
                        SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
                    }
                }
            }
            finally
            {
                ignoreSelectionChanges = false;
            }
        }

        /// <summary>
        /// Returns the first ISelection object from the currently active text editor
        /// </summary>
        private static ISelection GetCurrentTextSelection()
        {
            TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
            if (textArea != null)
            {
                SelectionManager selectionManager = textArea.ActiveTextAreaControl.SelectionManager;
                if (selectionManager.HasSomethingSelected)
                {
                    return selectionManager.SelectionCollection[0];
                }
            }
            return null;
        }

        private void WorkbenchActiveViewContentChanged(object source, EventArgs e)
        {
            TextEditorControl activeTextEditorControl = SearchReplaceUtilities.GetActiveTextEditor();
            if (activeTextEditorControl != this.textEditor)
            {
                AddSelectionChangedHandler(activeTextEditorControl);
                TextSelectionChanged(source, e);
            }
        }

        private void AddSelectionChangedHandler(TextEditorControl textEditor)
        {
            RemoveSelectionChangedHandler();

            this.textEditor = textEditor;
            if (textEditor != null)
            {
                this.textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged += TextSelectionChanged;
            }
        }

        private void RemoveSelectionChangedHandler()
        {
            if (textEditor != null)
            {
                textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged -= TextSelectionChanged;
            }
        }

        private void RemoveActiveWindowChangedHandler()
        {
            WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
        }

        /// <summary>
        /// When the selected text is changed make sure the 'Current Selection'
        /// option is not selected if no text is selected.
        /// </summary>
        /// <remarks>The text selection can change either when the user
        /// selects different text in the editor or the active window is
        /// changed.</remarks>
        private void TextSelectionChanged(object source, EventArgs e)
        {
            if (!ignoreSelectionChanges)
            {
                LoggingService.Debug("TextSelectionChanged.");
                selection = GetCurrentTextSelection();
                findFirst = true;
            }
        }

        private void SetCaretPosition(TextArea textArea, int offset)
        {
            textArea.Caret.Position = textArea.Document.OffsetToPosition(offset);
        }

        private void InitSelectionSearch()
        {
            findFirst = true;
            selection = GetCurrentTextSelection();
            AddSelectionChangedHandler(SearchReplaceUtilities.GetActiveTextEditor());
            WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
        }

        private void RemoveSelectionSearchHandlers()
        {
            RemoveSelectionChangedHandler();
            RemoveActiveWindowChangedHandler();
        }

        /// <summary>
        /// action: 0 = find, 1 = mark, 2 = replace
        /// </summary>
        private void RunAllInSelection(int action)
        {
            const IProgressMonitor monitor = null;

            int startOffset = Math.Min(selection.Offset, selection.EndOffset);
            int endOffset = Math.Max(selection.Offset, selection.EndOffset);

            SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
            SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);

            try
            {
                ignoreSelectionChanges = true;
                if (action == 0)
                    SearchInFilesManager.FindAll(startOffset, endOffset - startOffset, monitor);
                else if (action == 1)
                    SearchReplaceManager.MarkAll(startOffset, endOffset - startOffset, monitor);
                else if (action == 2)
                    SearchReplaceManager.ReplaceAll(startOffset, endOffset - startOffset, monitor);
                SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
            }
            finally
            {
                ignoreSelectionChanges = false;
            }
        }

        private void ReplaceInSelection()
        {
            int startOffset = Math.Min(selection.Offset, selection.EndOffset);
            int endOffset = Math.Max(selection.Offset, selection.EndOffset);

            if (findFirst)
            {
                SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
            }

            try
            {
                ignoreSelectionChanges = true;
                if (findFirst)
                {
                    findFirst = false;
                    SearchReplaceManager.ReplaceFirstInSelection(startOffset, endOffset - startOffset, null);
                }
                else
                {
                    findFirst = !SearchReplaceManager.ReplaceNextInSelection(null);
                    if (findFirst)
                    {
                        SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
                    }
                }
            }
            finally
            {
                ignoreSelectionChanges = false;
            }
        }

        /// <summary>
        /// Enables the various find, bookmark and replace buttons
        /// depending on whether any find string has been entered. The buttons
        /// are disabled otherwise.
        /// </summary>
        private void EnableButtons(bool enabled)
        {
            replaceButton.Enabled = enabled;
            replaceAllButton.Enabled = enabled;
            findNextButton.Enabled = enabled;
        }

        /// <summary>
        /// Returns true if the string entered in the find or replace text box
        /// is not an empty string.
        /// </summary>
        private bool HasFindPattern
        {
            get
            {
                return findComboBox.Text.Length != 0;
            }
        }

        /// <summary>
        /// Updates the enabled/disabled state of the search and replace buttons
        /// after the search or replace text has changed.
        /// </summary>
        private void FindPatternChanged(object source, EventArgs e)
        {
            EnableButtons(HasFindPattern);
        }
    }
}
