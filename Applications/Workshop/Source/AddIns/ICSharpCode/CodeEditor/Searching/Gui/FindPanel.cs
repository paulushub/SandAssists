using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

using ICSharpCode.CodeEditor.Resources;

namespace ICSharpCode.TextEditor.Searching
{
    public partial class FindPanel : UserControl
    {
        private ISelection selection;
        private TextEditorControl textEditor;
        private bool ignoreSelectionChanges;
        private bool findFirst;

        private SearchPanel parentPanel;
        private VisualStyleRenderer _renderer;
        private List<DocumentIteratorType> _listIterators;

        public FindPanel()
        {               
            InitializeComponent();

            _listIterators = new List<DocumentIteratorType>();

            foreach (Control control in this.Controls.GetRecursive())
            {
                control.Text = StringParser.Parse(control.Text);
            }

            findRegexButton.Text = String.Empty;
            findRegexButton.Image = Resources.Arrow;
            lookInSelectButton.Text = String.Empty;
            lookInSelectButton.Image = Resources.DownArrow;

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

            pathComboBox.DropDown      += new EventHandler(OnComboDropDown);
            findComboBox.DropDown      += new EventHandler(OnComboDropDown);
            lookInComboBox.DropDown    += new EventHandler(OnComboDropDown);
            fileTypesComboBox.DropDown += new EventHandler(OnComboDropDown);

            pathComboBox.MaxDropDownItems      = 20;
            findComboBox.MaxDropDownItems      = 20;
            lookInComboBox.MaxDropDownItems    = 20;
            fileTypesComboBox.MaxDropDownItems = 20;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
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

        public DocumentIteratorType DocumentType
        {
            get
            {
                int selIndex = lookInComboBox.SelectedIndex;

                if (selIndex < 0)
                {
                    return DocumentIteratorType.Directory;
                }

                return _listIterators[selIndex];
            }
            set
            {
                lookInComboBox.SelectedIndex = _listIterators.IndexOf(value);
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

        private bool IsSelectionSearch
        {
            get
            {
                return (this.DocumentType == DocumentIteratorType.CurrentSelection);
            }
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

        public void Initialize()
        {
            if (parentPanel != null)
            {
                Form parentForm = parentPanel.ParentForm;
                if (parentForm != null)
                {
                    parentForm.AcceptButton = findNextButton;
                }
            }

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
            // Unknown changes, we save the current state
            this.WritebackOptions();

            // Now, initialize based on the current scope information...
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

        private void OnFindRegexButttonClicked(object sender, EventArgs e)
        {

        }

        private void OnLookInButtonClicked(object sender, EventArgs e)
        {
        }

        private void OnPathBrowseButtonClicked(object sender, EventArgs e)
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

        private void LookInSelectedIndexChanged(object sender, EventArgs e)
        {
            SearchScope selScope = lookInComboBox.SelectedItem as SearchScope;
            if (selScope == null)
            {
                return;
            }

            DocumentIteratorType iteratorType = selScope.IteratorType;
            bool isDirectory = (iteratorType == DocumentIteratorType.Directory);
            bool needsFilter = iteratorType == DocumentIteratorType.WholeSolution ||
                iteratorType == DocumentIteratorType.WholeProject ||
                iteratorType == DocumentIteratorType.Directory;

            includeSubFolderCheckBox.Enabled = isDirectory;
            pathComboBox.Enabled = isDirectory;
            lookInSelectButton.Enabled = isDirectory;
            pathBrowseButton.Enabled = isDirectory;
            labelPath.Enabled    = isDirectory;

            labelFileTypes.Enabled    = needsFilter;
            fileTypesComboBox.Enabled = needsFilter;

            this.DocumentType = iteratorType;
            SearchOptions.DocumentIteratorType = iteratorType;

            if (this.IsSelectionSearch)
            {
                InitSelectionSearch();
            }
            else
            {
                RemoveSelectionSearchHandlers();
            }

            EnableButtons(this.HasFindPattern);
        }

        private void FindNextButtonClicked(object sender, EventArgs e)
        {
            WritebackOptions();

            if (this.IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    FindNextInSelection();
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = 
                    AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.FindNext(monitor);
                }
            }

            this.Focus();
        }

        private void FindAllButtonClicked(object sender, EventArgs e)
        {
            WritebackOptions();

            if (this.IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    RunAllInSelection(0);
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchInFilesManager.FindAll(monitor);
                }
            }
        }

        private void BookmarkAllButtonClicked(object sender, EventArgs e)
        {
            this.WritebackOptions();

            if (this.IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    this.RunAllInSelection(1);
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = 
                    AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.MarkAll(monitor);
                }
            }
        }

        private void SetOptions()
        {
            findComboBox.Items.Clear();
            IList<string> searchPatterns = SearchOptions.FindPatterns;
            int itemCount = searchPatterns == null ? 0 : searchPatterns.Count;
            for (int i = 0; i < itemCount; i++)
            {
                findComboBox.Items.Add(searchPatterns[i]);
            }
            findComboBox.Text = SearchOptions.FindPattern;

            bool solutionOpened = (ProjectService.OpenSolution != null);
            bool projectExists = false;
            if (solutionOpened)
            {
                projectExists = ProjectService.OpenSolution.HasProjects;
            }
            ICollection<IViewContent> contents = 
                WorkbenchSingleton.Workbench.ViewContentCollection;
            bool documentExists = false;
            if (contents != null && contents.Count != 0)
            {
                foreach (IViewContent viewContent in contents)
                {
                    if (viewContent is ITextEditorControlProvider)
                    {
                        documentExists = true;
                        break;
                    }
                }
            }

            string[] lookInTexts = 
            {
				// must be in the same order as the DocumentIteratorType enum
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}",
				"${res:Global.Location.directories}"
			};
            lookInComboBox.Items.Clear();
            _listIterators.Clear();

            itemCount = lookInTexts.Length;
            for (int i = 0; i < itemCount; i++)
            {
                string lookInText = lookInTexts[i];
                DocumentIteratorType iterator = (DocumentIteratorType)i;
                string labelText = StringParser.Parse(lookInText);
                if (iterator == DocumentIteratorType.WholeSolution)
                {
                    if (solutionOpened && projectExists)
                    {
                        lookInComboBox.Items.Add(new SearchScope(labelText, iterator));
                        _listIterators.Add(iterator);
                    }
                }
                else if (iterator == DocumentIteratorType.WholeProject)
                {
                    if (projectExists)
                    {
                        lookInComboBox.Items.Add(new SearchScope(labelText, iterator));
                        _listIterators.Add(iterator);
                    }
                }
                else if (iterator == DocumentIteratorType.AllOpenFiles)
                {
                    if (documentExists)
                    {
                        lookInComboBox.Items.Add(new SearchScope(labelText, iterator));
                        _listIterators.Add(iterator);
                    }
                }
                else if (iterator == DocumentIteratorType.CurrentDocument ||
                    iterator == DocumentIteratorType.CurrentSelection)
                {
                    if (documentExists && SearchReplaceUtilities.IsTextAreaSelected)
                    {
                        lookInComboBox.Items.Add(new SearchScope(labelText, iterator));
                        _listIterators.Add(iterator);
                    }
                }
                else
                {
                    lookInComboBox.Items.Add(new SearchScope(labelText, iterator));
                    _listIterators.Add(iterator);
                }
            }

            pathComboBox.Items.Clear();

            IList<string> lookInDirs = SearchOptions.LookInDirs;
            itemCount = lookInDirs == null ? 0 : lookInDirs.Count;
            for (int i = 0; i < itemCount; i++)
            {
                pathComboBox.Items.Add(lookInDirs[i]);
            }
            pathComboBox.Text = SearchOptions.LookInDir;

            fileTypesComboBox.Items.Clear();

            IList<string> filterList = SearchOptions.LookInFiletypes;
            itemCount = filterList == null ? 0 : filterList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                fileTypesComboBox.Items.Add(filterList[i]);
            }
            fileTypesComboBox.Text = SearchOptions.LookInFiletype;
            
            matchCaseCheckBox.Checked = SearchOptions.MatchCase;
            matchWholeWordCheckBox.Checked = SearchOptions.MatchWholeWord;
            includeSubFolderCheckBox.Checked = SearchOptions.IncludeSubdirectories;

            // If there is only one item, we just select it...
            if (_listIterators.Count == 1)
            {
                DocumentIteratorType iterator = _listIterators[0];
                if (SearchOptions.DocumentIteratorType != iterator)
                {
                    SearchOptions.DocumentIteratorType = iterator;
                }
                this.DocumentType = iterator;
            }
            else
            {
                if (IsMultipleLineSelection(GetCurrentTextSelection()))
                {
                    this.DocumentType = DocumentIteratorType.CurrentSelection;
                }
                else
                {
                    if (SearchOptions.DocumentIteratorType ==
                        DocumentIteratorType.CurrentSelection)
                    {
                        SearchOptions.DocumentIteratorType =
                            DocumentIteratorType.CurrentDocument;
                    }
                    this.DocumentType = SearchOptions.DocumentIteratorType;
                }
            }
        }

        /// <summary>
        /// This writes back the current state into the storage.
        /// </summary>
        private void WritebackOptions()
        {
            string tempText = String.Empty;
            int selIndex = findComboBox.SelectedIndex;
            if (selIndex != -1)
            {
                tempText = (string)findComboBox.Items[selIndex];
            }
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = findComboBox.Text;
            }

            string searchText = SearchOptions.FindPattern;
            SearchOptions.FindPattern = tempText;

            if (!String.Equals(searchText, tempText, 
                StringComparison.CurrentCultureIgnoreCase))
            {
                // Reinitialize the find patterns to reflect current stored values...
                findComboBox.Items.Clear();
                IList<string> searchPatterns = SearchOptions.FindPatterns;
                int itemCount = searchPatterns == null ? 0 : searchPatterns.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    findComboBox.Items.Add(searchPatterns[i]);
                }
            }

            tempText = String.Empty;
            selIndex = pathComboBox.SelectedIndex;
            if (selIndex != -1)
            {
                tempText = (string)pathComboBox.Items[selIndex];
            }
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = pathComboBox.Text;
            }
            if (Directory.Exists(tempText))
            {
                tempText = FileUtility.StripEndBackSlash(tempText);
            }
            SearchOptions.LookInDir = tempText;
            //if (lookInComboBox.DropDownStyle == ComboBoxStyle.DropDown)
            //{
            //    SearchOptions.LookIn = lookInComboBox.Text;
            //}

            tempText = String.Empty;
            selIndex = fileTypesComboBox.SelectedIndex;
            if (selIndex != -1)
            {
                tempText = (string)fileTypesComboBox.Items[selIndex];
            }
            if (String.IsNullOrEmpty(tempText))
            {
                tempText = fileTypesComboBox.Text;
            }

            SearchOptions.LookInFiletype        = tempText;
            SearchOptions.MatchCase             = matchCaseCheckBox.Checked;
            SearchOptions.MatchWholeWord        = matchWholeWordCheckBox.Checked;
            SearchOptions.IncludeSubdirectories = includeSubFolderCheckBox.Checked;
            SearchOptions.DocumentIteratorType  = this.DocumentType;
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
            TextEditorControl activeTextEditorControl = 
                SearchReplaceUtilities.GetActiveTextEditor();
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
            WorkbenchSingleton.Workbench.ActiveViewContentChanged -= 
                WorkbenchActiveViewContentChanged;
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
            WorkbenchSingleton.Workbench.ActiveViewContentChanged += 
                WorkbenchActiveViewContentChanged;
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

        /// <summary>
        /// Enables the various find, bookmark and replace buttons
        /// depending on whether any find string has been entered. The buttons
        /// are disabled otherwise.
        /// </summary>
        private void EnableButtons(bool enabled)
        {   
            if (enabled)
            {
                DocumentIteratorType iterator = this.DocumentType;
            
                if (iterator == DocumentIteratorType.Directory)
                {
                    bool hasValidDir = false;
                    string dirText = pathComboBox.Text;
                    if (!String.IsNullOrEmpty(dirText))
                    {
                        string[] directories = dirText.Split(';');
                        int itemCount = directories.Length;
                        for (int i = 0; i < itemCount; i++)
                        {
                            if (Directory.Exists(directories[i]))
                            {
                                hasValidDir = true;
                                break;
                            }
                        }
                    }

                    bookmarkAllButton.Enabled = false;
                    findAllButton.Enabled     = hasValidDir;
                    findNextButton.Enabled    = false;
                }
                else if (iterator == DocumentIteratorType.WholeProject ||
                    iterator == DocumentIteratorType.WholeSolution)
                {
                    bookmarkAllButton.Enabled = false;
                    findAllButton.Enabled     = true;
                    findNextButton.Enabled    = false;
                }
                else if (iterator == DocumentIteratorType.AllOpenFiles)
                {
                    findAllButton.Enabled     = true;
                    bookmarkAllButton.Enabled = true;
                    findNextButton.Enabled    = false;
                }
                else
                {
                    bookmarkAllButton.Enabled = enabled;
                    findAllButton.Enabled     = enabled;
                    findNextButton.Enabled    = enabled;
                }   
            }
            else
            {
                bookmarkAllButton.Enabled = enabled;
                findAllButton.Enabled     = enabled;
                findNextButton.Enabled    = enabled;
            }
        }

        /// <summary>
        /// Updates the enabled/disabled state of the search and replace buttons
        /// after the search or replace text has changed.
        /// </summary>
        private void FindPatternChanged(object source, EventArgs e)
        {
            EnableButtons(this.HasFindPattern);
        }

        private void OnFileTypesIndexChanged(object sender, EventArgs e)
        {
            SearchOptions.LookInFiletype = fileTypesComboBox.Text;
        }

        private void OnFileTypesLeave(object sender, EventArgs e)
        {
            string curText = fileTypesComboBox.Text;
            if (!String.IsNullOrEmpty(curText))
            {
                bool isFound = false;
                SearchOptions.LookInFiletype = curText;
                IList<string> fileTypes = SearchOptions.LookInFiletypes;
                int itemCount = fileTypes == null ? 0 : fileTypes.Count;
                for (int i = 0; i < itemCount; i++)
                {   
                    if (String.Equals(fileTypes[i], curText, 
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        isFound = true;
                    }
                }

                if (!isFound)
                {
                    fileTypesComboBox.Items.Insert(0, curText);
                    List<string> updatedFileTypes = new List<string>(itemCount + 1);
                    updatedFileTypes.Add(curText);
                    updatedFileTypes.AddRange(fileTypes);

                    SearchOptions.LookInFiletypes = updatedFileTypes;
                }
            }
        }

        private void OnPathTextChanged(object sender, EventArgs e)
        {
            EnableButtons(this.HasFindPattern);
        }

        private void OnPathIndexChanged(object sender, EventArgs e)
        {
            EnableButtons(this.HasFindPattern);
        }

        private void OnComboDropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = sender as ComboBox;
            if (senderComboBox == null)
            {
                return;    
            }

            Graphics graphics = null;
            try
            {
                int width  = senderComboBox.Width;
                graphics   = senderComboBox.CreateGraphics();
                Font font  = senderComboBox.Font;

                //checks if a scrollbar will be displayed.
                //If yes, then get its width to adjust the size of the drop down list.
                int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ?
                SystemInformation.VerticalScrollBarWidth : 0;

                //Loop through list items and check size of each items.
                //set the width of the drop down list to the width of the largest item.

                int newWidth = 0;
                if (senderComboBox == lookInComboBox)
                {
                    foreach (SearchScope s in senderComboBox.Items)
                    {
                        if (s != null)
                        {
                            string text = s.LabelText.Trim();
                            newWidth = (int)graphics.MeasureString(text, font).Width + vertScrollBarWidth;
                            if (width < newWidth)
                            {
                                width = newWidth;
                            }
                        }
                    }
                }
                else
                {
                    foreach (string s in senderComboBox.Items)
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            string text = s.Trim();
                            newWidth = (int)graphics.MeasureString(text, font).Width + vertScrollBarWidth;
                            if (width < newWidth)
                            {
                                width = newWidth;
                            }
                        }
                    }
                }
                if (width > senderComboBox.DropDownWidth)
                {
                    senderComboBox.DropDownWidth = width + 3;
                }

                graphics.Dispose();
                graphics = null;
            }
            catch (Exception ex)
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }

                MessageService.ShowError(ex);
            }
        }
    }
}
