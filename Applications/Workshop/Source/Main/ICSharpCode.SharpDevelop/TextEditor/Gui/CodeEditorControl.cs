// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4869 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Gui.InsightWindow;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
	public class CodeEditorControl : TextEditorControl
	{
		protected string contextMenuPath             = "/SharpDevelop/ViewContent/TextEditor/ContextMenu";
        
        private const string editActionsPath         = "/AddIns/TextEditor/EditActions";
        private const string formatingStrategyPath   = "/AddIns/TextEditor/Formatters";
        private const string advancedHighlighterPath = "/AddIns/TextEditor/AdvancedHighlighter";

        private bool addQuickBrowser;
        private bool quickBrowserIsAdded;

        private Control customQuickClassBrowserPanel;
        private CodeErrorDrawer errorDrawer;
        private IAdvancedHighlighter advancedHighlighter;
        private QuickClassBrowserPanel quickClassBrowserPanel;

        private bool startedDelayedReparse;

        private bool inHandleKeyPress;
        private InsightWindow insightWindow;
        private CodeCompletionWindow codeCompletionWindow;

        private static List<ICodeCompletionBinding> codeCompletionBindings;
		
		public QuickClassBrowserPanel QuickClassBrowserPanel {
			get {
				return quickClassBrowserPanel;
			}
		}

		public Control CustomQuickClassBrowserPanel {
			get	{
				return customQuickClassBrowserPanel;
			}
			set	{
				if (customQuickClassBrowserPanel != null) {
					RemoveQuickClassBrowserPanel();
					customQuickClassBrowserPanel.Dispose();
				}
				customQuickClassBrowserPanel = value;
				ActivateQuickClassBrowserOnDemand();
			}
		}
		
		public CodeEditorControl()
			: this(true, true, true)
		{
			GenerateEditActions();
			
			this.TextEditorProperties = CodeEditorProperties.Instance;
		}
		
		protected CodeEditorControl(bool enableFolding, bool sdBookmarks,
            bool showQuickBrowser)
		{
            this.addQuickBrowser = showQuickBrowser;

            ITextDocument document = this.Document;
            if (enableFolding)
            {
                Document.FoldingManager.FoldingStrategy = new ParserFoldingStrategy();
            }

            if (sdBookmarks)
            {
                document.BookmarkManager.Factory  = new Bookmarks.BookmarkFactory(Document.BookmarkManager);
                document.BookmarkManager.Added   += new BookmarkEventHandler(BookmarkAdded);
                document.BookmarkManager.Removed += new BookmarkEventHandler(BookmarkRemoved);
                document.LineCountChanged        += BookmarkLineCountChanged;
            }
		}
		
		private void BookmarkAdded(object sender, BookmarkEventArgs e)
		{
			Bookmarks.BookmarkEx b = e.Bookmark as Bookmarks.BookmarkEx;
			if (b != null) {
                Bookmarks.BookmarkExManager.AddMark(b);
			}
		}

        private void BookmarkRemoved(object sender, BookmarkEventArgs e)
		{
			Bookmarks.BookmarkEx b = e.Bookmark as Bookmarks.BookmarkEx;
			if (b != null) {
                Bookmarks.BookmarkExManager.RemoveMark(b);
			}
		}

        private void BookmarkLineCountChanged(object sender, LineCountChangeEventArgs e)
		{
			foreach (Bookmark b in Document.BookmarkManager.Marks) {
				if (b.LineNumber >= e.LineStart) {
					Bookmarks.BookmarkEx sdb = b as Bookmarks.BookmarkEx;
					if (sdb != null) {
						sdb.RaiseLineNumberChanged();
					}
				}
			}
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			
			newControl.ShowContextMenu += delegate(object sender, MouseEventArgs e) {
				if (contextMenuPath != null) {
					MenuService.ShowContextMenu(this, contextMenuPath, (Control)sender, e.X, e.Y);
				}
			};
			newControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(HandleKeyPress);
			newControl.TextArea.ClipboardHandler.CopyText += new CopyTextEventHandler(ClipboardHandlerCopyText);
			
//			newControl.TextArea.IconBarMargin.Painted   += new MarginPaintEventHandler(PaintIconBarBreakPoints);
//			newControl.TextArea.IconBarMargin.MouseDown += new MarginMouseEventHandler(IconBarMouseDown);
			
			newControl.MouseWheel                       += new MouseEventHandler(TextAreaMouseWheel);
			newControl.DoHandleMousewheel = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				if (errorDrawer != null) {
					errorDrawer.Dispose();
					errorDrawer = null;
				}
				if (quickClassBrowserPanel != null) {
					quickClassBrowserPanel.Dispose();
					quickClassBrowserPanel = null;
				}
				if (customQuickClassBrowserPanel != null) {
					customQuickClassBrowserPanel.Dispose();
					customQuickClassBrowserPanel = null;
				}
				if (advancedHighlighter != null) {
					advancedHighlighter.Dispose();
					advancedHighlighter = null;
				}
				CloseCodeCompletionWindow(this, EventArgs.Empty);
				CloseInsightWindow(this, EventArgs.Empty);
			}
		}

        private void CloseCodeCompletionWindow(object sender, EventArgs e)
		{
			if (codeCompletionWindow != null) {
				codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
				codeCompletionWindow.Dispose();
				codeCompletionWindow = null;
			}
		}

        private void CloseInsightWindow(object sender, EventArgs e)
		{
			if (insightWindow != null) {
				insightWindow.Closed -= new EventHandler(CloseInsightWindow);
				insightWindow.Dispose();
				insightWindow = null;
			}
		}

        private void TextAreaMouseWheel(object sender, MouseEventArgs e)
		{
			TextAreaControl textAreaControl = (TextAreaControl)sender;
			if (insightWindow != null && !insightWindow.IsDisposed && insightWindow.Visible) {
				insightWindow.HandleMouseWheel(e);
			} else if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed && codeCompletionWindow.Visible) {
				codeCompletionWindow.HandleMouseWheel(e);
			} else {
				textAreaControl.HandleMouseWheel(e);
			}
		}

        private void ClipboardHandlerCopyText(object sender, CopyTextEventArgs e)
		{
			TextEditorSideBar.Instance.PutInClipboardRing(e.Text);
		}
		
		public override void OptionsChanged()
		{
			base.OptionsChanged();
			CodeEditorProperties sdtep = base.TextEditorProperties as CodeEditorProperties;
			
			if (sdtep == null) 
            {
                return;
			}

            if (!sdtep.ShowQuickClassBrowserPanel)
            {
                RemoveQuickClassBrowserPanel();
            }
            else
            {
                if (addQuickBrowser)
                {
                    ActivateQuickClassBrowserOnDemand();
                }
            }
            if (sdtep.UnderlineErrors)
            {
                if (errorDrawer == null)
                {
                    errorDrawer = new CodeErrorDrawer(this);
                }
            }
            else
            {
                if (errorDrawer != null)
                {
                    errorDrawer.Dispose();
                    errorDrawer = null;
                }
            }
		}
		
		internal void FileLoaded()
		{
			if (errorDrawer != null) {
				errorDrawer.UpdateErrors();
			}
		}

        protected void GenerateEditActions()
		{
#if DEBUG
            editactions[DebuggerService.DebugBreakModifiers | Keys.OemPeriod] = new DebugDotCompletionAction();
            editactions[DebuggerService.DebugBreakModifiers | Keys.Space] = new DebugCtrlSpaceCodeCompletionAction();
#endif
            try
            {
				List<IEditAction> actions = AddInTree.GetTreeNode(
                    editActionsPath).BuildChildItems<IEditAction>(this);
				
				foreach (IEditAction action in actions) 
                {
					foreach (Keys key in action.Keys) 
                    {
						editactions[key] = action;
					}
				}
			} 
            catch (TreePathNotFoundException) 
            {
				LoggingService.Warn("EditAction " + editActionsPath + " doesn't exists in the AddInTree");
			}
		}

        private void RemoveQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel != null) {
                Control editorParent = this.Parent;
                if (editorParent != null)
                {
                    editorParent.Controls.Remove(quickClassBrowserPanel);
				    quickClassBrowserPanel.Dispose();
				    quickClassBrowserPanel = null;
                    addQuickBrowser = false;
                    quickBrowserIsAdded = false;
				    textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
                }
			}
			if (customQuickClassBrowserPanel != null) {
				if (Controls.Contains(customQuickClassBrowserPanel)) {
					Controls.Remove(customQuickClassBrowserPanel);
					customQuickClassBrowserPanel.Enabled = false;
					textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
				}
			}
		}

        private void ShowQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel == null) {
				quickClassBrowserPanel = new QuickClassBrowserPanel(this);
                Control editorParent = this.Parent;
                addQuickBrowser = true;
                if (editorParent != null)
                {
                    editorParent.Controls.Add(quickClassBrowserPanel);
                    quickBrowserIsAdded = true;
                }
                else
                {
                    quickBrowserIsAdded = false;
                    //this.Controls.Add(quickClassBrowserPanel);
                }
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			}
			if (customQuickClassBrowserPanel != null) {
				if (quickClassBrowserPanel != null)
					RemoveQuickClassBrowserPanel();
				if (!Controls.Contains(customQuickClassBrowserPanel)) {
					Controls.Add(customQuickClassBrowserPanel);
					customQuickClassBrowserPanel.Enabled = true;
					textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				}
				return;
			}
		}

		public void ActivateQuickClassBrowserOnDemand()
		{
			CodeEditorProperties sdtep = base.TextEditorProperties as CodeEditorProperties;
			if (sdtep != null && sdtep.ShowQuickClassBrowserPanel && FileName != null) {
				
				bool quickClassPanelActive = ParserService.GetParser(FileName) != null;
				if (quickClassPanelActive) {
					ShowQuickClassBrowserPanel();
				} else {
					RemoveQuickClassBrowserPanel();
				}
			}
		}
		
		protected override void OnFileNameChanged(EventArgs e)
		{
			base.OnFileNameChanged(e);
			((Bookmarks.BookmarkFactory)Document.BookmarkManager.Factory).ChangeFilename(this.FileName);

            if (addQuickBrowser)
            {
                ActivateQuickClassBrowserOnDemand();
            }
		}
		
		public static IList<ICodeCompletionBinding> CodeCompletionBindings {
			get {
				if (codeCompletionBindings == null) {
					try {
						codeCompletionBindings = AddInTree.GetTreeNode(
                            "/AddIns/TextEditor/CodeCompletion").BuildChildItems<ICodeCompletionBinding>(null);
					} catch (TreePathNotFoundException) {
						codeCompletionBindings = new List<ICodeCompletionBinding>();
					}
				}
				return codeCompletionBindings;
			}
		}
		
        private bool HandleKeyPress(char ch)
		{
			if (inHandleKeyPress)
				return false;
			inHandleKeyPress = true;
			try {
				if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
					if (codeCompletionWindow.ProcessKeyEvent(ch)) {
						return true;
					}
					if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
						// code-completion window is still opened but did not want to handle
						// the keypress -> don't try to restart code-completion
						return false;
					}
				}
				
				if (CodeCompletionOptions.EnableCodeCompletion) {
					foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings) {
						if (ccBinding.HandleKeyPress(this, ch))
							return false;
					}
                    if (ch == '\n')
                        StartDelayedReparse();
                }
			} catch (Exception ex) {
				LogException(ex);
			} finally {
				inHandleKeyPress = false;
			}
			return false;
		}

        private void StartDelayedReparse()
        {
            if (startedDelayedReparse)
                return;
            startedDelayedReparse = true;
            WorkbenchSingleton.SafeThreadAsyncCall(
                delegate
                {
                    startedDelayedReparse = false;
                    if (!this.IsDisposed)
                    {
                        ParserService.StartAsyncParse(this.FileName, this.Document.TextContent);
                    }
                });
        }
		
		public void StartCtrlSpaceCompletion()
		{
			foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings) {
				if (ccBinding.CtrlSpace(this))
					return;
			}
		}

        public bool ExpandTemplateOnTab()
		{
			string word = GetWordBeforeCaret();
			if (word != null) {
				CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(FileName);
				if (templateGroup != null) {
					foreach (CodeTemplate template in templateGroup.Templates) {
						if (template.Shortcut == word) {
							if (word.Length > 0) {
								int newCaretOffset = DeleteWordBeforeCaret();
								//// set new position in text area
								ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(newCaretOffset);
							}
							
							InsertTemplate(template);
							return true;
						}
					}
				}
			}
			return false;
		}
		
		public void ShowInsightWindow(IInsightDataProvider insightDataProvider)
		{
			if (insightWindow == null || insightWindow.IsDisposed) {
				insightWindow = new InsightWindow(WorkbenchSingleton.MainForm, this);
				insightWindow.Closed += new EventHandler(CloseInsightWindow);
			}
			insightWindow.AddInsightDataProvider(insightDataProvider, this.FileName);
			insightWindow.ShowInsightWindow();
		}
		
		public bool InsightWindowVisible {
			get {
				return insightWindow != null;
			}
		}
		
		public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider, char ch)
		{
			codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(WorkbenchSingleton.MainForm, this, this.FileName, completionDataProvider, ch);
			if (codeCompletionWindow != null) {
				codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
			}
		}
		
		private void LogException(Exception ex)
		{
			ICSharpCode.Core.MessageService.ShowError(ex);
		}
		
		public string GetWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			return Document.GetText(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
		}
		
		public int DeleteWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			Document.Remove(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
			return start;
		}
		
		/// <remarks>
		/// This method inserts a code template at the current caret position
		/// </remarks>
		public void InsertTemplate(CodeTemplate template)
		{
			string selectedText = String.Empty;
			Document.UndoStack.StartUndoGroup();
			if (base.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected) {
				selectedText = base.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
				ActiveTextAreaControl.TextArea.Caret.Position = ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition;
				base.ActiveTextAreaControl.TextArea.SelectionManager.RemoveSelectedText();
			}
			
			// save old properties, these properties cause strange effects, when not
			// be turned off (like insert curly braces or other formatting stuff)
			
			string templateText = StringParser.Parse(template.Text, new string[,] { { "Selection", selectedText } });
			int finalCaretOffset = templateText.IndexOf('|');
			if (finalCaretOffset >= 0) {
				templateText = templateText.Remove(finalCaretOffset, 1);
			} else {
				finalCaretOffset = templateText.Length;
			}
			int caretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
			
			BeginUpdate();
			int beginLine = ActiveTextAreaControl.TextArea.Caret.Line;
			Document.Insert(caretOffset, templateText);
			
			ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(caretOffset + finalCaretOffset);
			int endLine = Document.OffsetToPosition(caretOffset + templateText.Length).Y;
			
			IndentStyle save1 = TextEditorProperties.IndentStyle;
			TextEditorProperties.IndentStyle = IndentStyle.Smart;
			
			Document.FormattingStrategy.IndentLines(ActiveTextAreaControl.TextArea, beginLine, endLine);
			
			Document.UndoStack.EndUndoGroup();
			EndUpdate();
			Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			Document.CommitUpdate();
			
			// restore old property settings
			TextEditorProperties.IndentStyle = save1;
		}
		
		protected override void OnReloadHighlighting(object sender, EventArgs e)
		{
			base.OnReloadHighlighting(sender, e);
			InitializeAdvancedHighlighter();
		}

        public bool HighlightingExplicitlySet { get; set; }

        /// <summary>
        /// Explicitly set the highlighting to use. Will be persisted.
        /// </summary>
        public override void SetHighlighting(string name)
        {
            base.SetHighlighting(name);
            this.HighlightingExplicitlySet = true;
            InitializeAdvancedHighlighter();
        }
		
		public void InitializeAdvancedHighlighter()
		{
			if (advancedHighlighter != null) {
				advancedHighlighter.Dispose();
				advancedHighlighter = null;
			}
			string highlighterPath = advancedHighlighterPath + "/" + Document.HighlightingStrategy.Name;
			if (AddInTree.ExistsTreeNode(highlighterPath)) {
				IList<IAdvancedHighlighter> highlighter = AddInTree.BuildItems<IAdvancedHighlighter>(highlighterPath, this);
				if (highlighter != null && highlighter.Count > 0) {
					advancedHighlighter = highlighter[0];
					advancedHighlighter.Initialize(this);
					Document.HighlightingStrategy = new AdvancedHighlightingStrategy((HighlightingStrategy)Document.HighlightingStrategy, advancedHighlighter);
				}
			}
		}
		
		public void InitializeFormatter()
		{
			string formatterPath = formatingStrategyPath + "/" + Document.HighlightingStrategy.Name;
			if (AddInTree.ExistsTreeNode(formatterPath)) {
				List<IFormattingStrategy> formatter = AddInTree.GetTreeNode(
                    formatterPath).BuildChildItems<IFormattingStrategy>(this);
				if (formatter != null && formatter.Count > 0) {
					Document.FormattingStrategy = formatter[0];
				}
			}
		}
		
		public override string GetRangeDescription(int selectedItem, int itemCount)
		{
			StringParser.Properties["CurrentMethodNumber"]  = selectedItem.ToString("##");
			StringParser.Properties["NumberOfTotalMethods"] = itemCount.ToString("##");
			return StringParser.Parse("${res:ICSharpCode.SharpDevelop.TextEditor.InsightWindow.NumberOfText}");
		}

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            Control editorParent = this.Parent;
            if (editorParent == null)
            {
                return;
            }
            if (quickClassBrowserPanel != null && addQuickBrowser)
            {
                if (quickBrowserIsAdded)
                {
                    Control curParent = quickClassBrowserPanel.Parent;
                    if (curParent != null)
                    {
                        if (curParent != editorParent)
                        {
                            curParent.Controls.Remove(quickClassBrowserPanel);
                            editorParent.Controls.Add(quickClassBrowserPanel);
                        }
                    }
                    else
                    {
                        editorParent.Controls.Add(quickClassBrowserPanel);
                    }
                }
                else
                {
                    if (editorParent != null)
                    {
                        editorParent.Controls.Add(quickClassBrowserPanel);
                        quickBrowserIsAdded = true;
                    }
                }
            }
        }
	}

#if DEBUG
    internal class DebugCtrlSpaceCodeCompletionAction : AbstractEditAction
    {
        public override void Execute(TextArea textArea)
        {
            CodeEditorControl sdtac = (CodeEditorControl)textArea.MotherTextEditorControl;
            CtrlSpaceCompletionDataProvider provider = new CtrlSpaceCompletionDataProvider();
            provider.AllowCompleteExistingExpression = true;
            provider.DebugMode = true;
            sdtac.ShowCompletionWindow(provider, '\0');
        }
    }

    internal class DebugDotCompletionAction : AbstractEditAction
    {
        public override void Execute(TextArea textArea)
        {
            CodeEditorControl sdtac = (CodeEditorControl)textArea.MotherTextEditorControl;
            CodeCompletionDataProvider ccdp = new CodeCompletionDataProvider();
            ccdp.DebugMode = true;
            sdtac.ShowCompletionWindow(ccdp, '.');
        }
    }
#endif
}
