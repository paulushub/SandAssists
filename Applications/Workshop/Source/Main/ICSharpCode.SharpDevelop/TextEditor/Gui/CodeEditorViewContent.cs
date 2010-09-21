﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4081 $</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Printing;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Undo;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
    public sealed class CodeEditorViewContent : AbstractViewContent,
        IMementoCapable, IPrintable, IEditable, IUndoHandler, IPositionable,
        ITextEditorControlProvider, IParseInformationListener, IClipboardHandler,
        IContextHelpProvider, IToolsHost
    {
        private CodeEditorControl textEditorControl;

        public CodeEditorViewContent(OpenedFile file)
            : base(file)
        {
            this.TabPageText = "${res:FormsDesigner.DesignTabPages.SourceTabPage}";

            textEditorControl = CreateTextAreaControl();

            textEditorControl.RightToLeft = RightToLeft.No;
            textEditorControl.Document.DocumentChanged += new DocumentEventHandler(TextAreaChangedEvent);
            textEditorControl.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
            textEditorControl.ActiveTextAreaControl.Enter += new EventHandler(CaretUpdate);
            textEditorControl.ActiveTextAreaControl.Caret.PositionChanged += CaretUpdate;

            textEditorControl.FileName = file.FileName;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (this.PrimaryFile.IsUntitled)
                {
                    ParserService.ClearParseInformation(this.PrimaryFile.FileName);
                }
                textEditorControl.Dispose();
            }

            base.Dispose(disposing);
        }

        public TextEditorControl TextEditorControl
        {
            get
            {
                return textEditorControl;
            }
        }

        public TextArea ActiveTextArea
        {
            get
            {
                return textEditorControl.ActiveTextAreaControl.TextArea;
            }
        }

        public ITextDocument GetDocumentForFile(OpenedFile file)
        {
            if (file == this.PrimaryFile)
            {
                return this.TextEditorControl.Document;
            }
            else
            {
                return null;
            }
        }

        public bool EnableUndo
        {
            get
            {
                return textEditorControl.EnableUndo;
            }
        }

        public bool EnableRedo
        {
            get
            {
                return textEditorControl.EnableRedo;
            }
        }

        // ParserUpdateThread uses the text property via IEditable, I had an exception
        // because multiple threads were accessing the GapBufferStrategy at the same time.

        private string GetText()
        {
            return textEditorControl.Document.TextContent;
        }

        private void SetText(string value)
        {
            textEditorControl.Document.Replace(0, textEditorControl.Document.TextLength, value);
        }

        public string Text
        {
            get
            {
                if (WorkbenchSingleton.InvokeRequired)
                    return WorkbenchSingleton.SafeThreadFunction<string>(GetText);
                else
                    return GetText();
            }
            set
            {
                if (WorkbenchSingleton.InvokeRequired)
                    WorkbenchSingleton.SafeThreadCall(SetText, value);
                else
                    SetText(value);
            }
        }

        #region IPrintable interface

        public PrintDocument PrintDocument
        {
            get
            {
                return textEditorControl.PrintDocument;
            }
        }

        bool IPrintable.IsSelfPrinting
        {
            get
            {
                return false;
            }
        }

        bool IPrintable.IsSelfPreviewing
        {
            get
            {
                return false;
            }
        }

        void IPrintable.Print()
        {
        }

        void IPrintable.PageSetup()
        {
            WinFormsPrinterService.ShowPageSettings();
        }

        void IPrintable.PrintPreview()
        {
        }

        #endregion

        public override Control Control
        {
            get
            {
                return textEditorControl;
            }
        }

        public void Undo()
        {
            this.textEditorControl.Undo();
        }

        public void Redo()
        {
            this.textEditorControl.Redo();
        }

        private CodeEditorControl CreateTextAreaControl()
        {
            return new CodeEditorControl();
        }

        public void ShowHelp()
        {
            // Resolve expression at cursor and show help
            TextArea textArea = this.ActiveTextArea;
            ITextDocument doc = textArea.Document;
            IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
            if (expressionFinder == null)
                return;
            LineSegment seg = doc.GetLineSegment(textArea.Caret.Line);
            string textContent = doc.TextContent;
            ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + textArea.Caret.Column);
            string expression = expressionResult.Expression;
            if (expression != null && expression.Length > 0)
            {
                ResolveResult result = ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textEditorControl.FileName, textContent);
                TypeResolveResult trr = result as TypeResolveResult;
                if (trr != null)
                {
                    HelpProvider.ShowHelp(trr.ResolvedClass);
                }
                MemberResolveResult mrr = result as MemberResolveResult;
                if (mrr != null)
                {
                    HelpProvider.ShowHelp(mrr.ResolvedMember);
                }
            }
        }

        private void TextAreaChangedEvent(object sender, DocumentEventArgs e)
        {
            ITextDocument document = sender as ITextDocument;
            if (document != null)
            {
                UndoStack undoStack = document.UndoStack;
                this.PrimaryFile.IsDirty = undoStack.CanUndo;
            }
            else
            {
                this.PrimaryFile.MakeDirty();
            }

            NavigationService.ContentChanging(this.textEditorControl, e);
        }

        public override void RedrawContent()
        {
            textEditorControl.OptionsChanged();
            textEditorControl.Refresh();
        }

        public override bool IsReadOnly
        {
            get
            {
                return textEditorControl.IsReadOnly;
            }
        }

        public override void Save(OpenedFile file, Stream stream)
        {
            if (file != PrimaryFile)
                throw new ArgumentException("file != PrimaryFile");

            if (!textEditorControl.CanSaveWithCurrentEncoding())
            {
                if (MessageService.AskQuestion("The file cannot be saved with the current encoding " +
                                               textEditorControl.Encoding.EncodingName + " without losing data." +
                                               "\nDo you want to save it using UTF-8 instead?"))
                {
                    textEditorControl.Encoding = System.Text.Encoding.UTF8;
                }
            }

            textEditorControl.SaveFile(stream);
        }

        public override void Load(OpenedFile file, Stream stream)
        {
            if (file != PrimaryFile)
                throw new ArgumentException("file != PrimaryFile");

            if (!file.IsUntitled)
            {
                textEditorControl.IsReadOnly = (File.GetAttributes(file.FileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }

            bool autodetectEncoding = true;
            textEditorControl.LoadFile(file.FileName, stream, true, autodetectEncoding);
            textEditorControl.FileLoaded();
            foreach (Bookmarks.BookmarkEx mark in
                Bookmarks.BookmarkExManager.GetBookmarks(file.FileName))
            {
                mark.Document = textEditorControl.Document;
                textEditorControl.Document.BookmarkManager.AddMark(mark);
            }
            ForceFoldingUpdate();
        }

        public Properties CreateMemento()
        {
            Properties properties = new Properties();
            TextAreaControl textAreaCtrl = textEditorControl.ActiveTextAreaControl;
            properties.Set("CaretOffset", textAreaCtrl.Caret.Offset);
            properties.Set("VisibleLine", textAreaCtrl.TextArea.TextView.FirstVisibleLine);
            if (textEditorControl.HighlightingExplicitlySet)
            {
                properties.Set("HighlightingLanguage",
                    textEditorControl.Document.HighlightingStrategy.Name);
            }

            return properties;
        }

        public void SetMemento(Properties properties)
        {
            textEditorControl.ActiveTextAreaControl.Caret.Position = textEditorControl.Document.OffsetToPosition(Math.Min(textEditorControl.Document.TextLength, Math.Max(0, properties.Get("CaretOffset", textEditorControl.ActiveTextAreaControl.Caret.Offset))));
            //			textAreaControl.SetDesiredColumn();

            string highlightingName = properties.Get("HighlightingLanguage", string.Empty);
            if (!string.IsNullOrEmpty(highlightingName))
            {
                if (highlightingName == textEditorControl.Document.HighlightingStrategy.Name)
                {
                    textEditorControl.HighlightingExplicitlySet = true;
                }
                else
                {
                    IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(highlightingName);
                    if (highlightingStrategy != null)
                    {
                        textEditorControl.HighlightingExplicitlySet = true;
                        textEditorControl.Document.HighlightingStrategy = highlightingStrategy;
                    }
                }
            }
            this.ActiveTextArea.TextView.FirstVisibleLine = properties.Get("VisibleLine", 0);

            //			// insane check for cursor position, may be required for document reload.
            //			int lineNr = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.Document.Caret.Offset);
            //			LineSegment lineSegment = textAreaControl.Document.GetLineSegment(lineNr);
            //			textAreaControl.Document.Caret.Offset = Math.Min(lineSegment.Offset + lineSegment.Length, textAreaControl.Document.Caret.Offset);
            //
            //			textAreaControl.OptionsChanged();
            //			textAreaControl.Refresh();
        }

        public override INavigationPoint BuildNavPoint()
        {
            int lineNumber = this.Line;
            LineSegment lineSegment = textEditorControl.Document.GetLineSegment(lineNumber);
            string txt = textEditorControl.Document.GetText(lineSegment);
            return new TextNavigationPoint(this.PrimaryFileName, lineNumber, this.Column, txt);
        }

        private void CaretUpdate(object sender, EventArgs e)
        {
            CaretChanged(null, null);
            CaretModeChanged(null, null);
        }

        private void CaretChanged(object sender, EventArgs e)
        {
            TextAreaControl activeTextAreaControl = textEditorControl.ActiveTextAreaControl;
            int line = activeTextAreaControl.Caret.Line;
            int col = activeTextAreaControl.Caret.Column;
            StatusBarService.SetCaretPosition(activeTextAreaControl.TextArea.TextView.GetVisualColumn(line, col) + 1, line + 1, col + 1);
            NavigationService.Log(this.BuildNavPoint());
        }

        private void CaretModeChanged(object sender, EventArgs e)
        {
            StatusBarService.SetInsertMode(textEditorControl.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
        }

        protected override void OnFileNameChanged(OpenedFile file)
        {
            base.OnFileNameChanged(file);
            Debug.Assert(file == this.Files[0]);

            string oldFileName = textEditorControl.FileName;
            string newFileName = file.FileName;

            if (Path.GetExtension(oldFileName) != Path.GetExtension(newFileName))
            {
                if (textEditorControl.Document.HighlightingStrategy != null)
                {
                    textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(newFileName);
                    textEditorControl.Refresh();
                }
            }

            SetIcon();

            ParserService.ClearParseInformation(oldFileName);
            textEditorControl.FileName = newFileName;
            ParserService.ParseViewContent(this);
        }

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();
            SetIcon();
        }

        private void SetIcon()
        {
            if (this.WorkbenchWindow != null)
            {
                System.Drawing.Icon icon = null;

                // Check for the missing icon, try to extract one from the shell...
                string iconKey = IconService.GetImageForFile(this.PrimaryFileName);
                if (String.Equals(iconKey, "Icons.16x16.MiscFiles",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    icon = IconExtractor.Extract(this.PrimaryFileName, false);
                    if (icon == null)
                    {
                        icon = WinFormsResourceService.GetIcon(iconKey);
                    }
                }
                else
                {
                    icon = WinFormsResourceService.GetIcon(iconKey);
                }

                if (icon != null)
                {
                    this.WorkbenchWindow.Icon = icon;
                }
            }
        }

        #region IPositionable implementation

        public void JumpTo(int line, int column)
        {
            textEditorControl.ActiveTextAreaControl.JumpTo(line, column);

            // we need to delay this call here because the text editor does not know its height if it was just created
            WorkbenchSingleton.SafeThreadAsyncCall(
                delegate
                {
                    textEditorControl.ActiveTextAreaControl.CenterViewOn(
                        line, (int)(0.3 * this.ActiveTextArea.TextView.VisibleLineCount));
                });
        }

        public int Line
        {
            get
            {
                return textEditorControl.ActiveTextAreaControl.Caret.Line;
            }
        }

        public int Column
        {
            get
            {
                return textEditorControl.ActiveTextAreaControl.Caret.Column;
            }
        }

        #endregion

        public void ForceFoldingUpdate()
        {
            if (textEditorControl.TextEditorProperties.EnableFolding)
            {
                string fileName = textEditorControl.FileName;
                ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
                if (parseInfo == null)
                {
                    parseInfo = ParserService.ParseFile(fileName,
                                                        textEditorControl.Document.TextContent, false);
                }
                textEditorControl.Document.FoldingManager.UpdateFoldings(fileName, parseInfo);
                UpdateClassMemberBookmarks(parseInfo);
            }
        }

        public void ParseInformationUpdated(ParseInformation parseInfo)
        {
            if (textEditorControl.TextEditorProperties.EnableFolding)
            {
                WorkbenchSingleton.SafeThreadAsyncCall(ParseInformationUpdatedInvoked, parseInfo);
            }
        }

        private void ParseInformationUpdatedInvoked(ParseInformation parseInfo)
        {
            try
            {
                textEditorControl.Document.FoldingManager.UpdateFoldings(TitleName, parseInfo);
                UpdateClassMemberBookmarks(parseInfo);
                this.ActiveTextArea.Refresh(this.ActiveTextArea.FoldMargin);
                this.ActiveTextArea.Refresh(this.ActiveTextArea.IconBarMargin);
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }

        private void UpdateClassMemberBookmarks(ParseInformation parseInfo)
        {
            BookmarkManager bm = textEditorControl.Document.BookmarkManager;
            bm.RemoveMarks(new Predicate<Bookmark>(IsClassMemberBookmark));
            if (parseInfo == null) return;
            Debug.Assert(textEditorControl.Document.TotalNumberOfLines >= 1);
            if (textEditorControl.Document.TotalNumberOfLines < 1)
            {
                return;
            }
            foreach (IClass c in parseInfo.MostRecentCompilationUnit.Classes)
            {
                AddClassMemberBookmarks(bm, c);
            }
        }

        private void AddClassMemberBookmarks(BookmarkManager bm, IClass c)
        {
            if (c.IsSynthetic) return;
            if (!c.Region.IsEmpty)
            {
                bm.AddMark(new Bookmarks.ClassBookmark(textEditorControl.Document, c));
            }
            foreach (IClass innerClass in c.InnerClasses)
            {
                AddClassMemberBookmarks(bm, innerClass);
            }
            foreach (IMethod m in c.Methods)
            {
                if (m.Region.IsEmpty || m.IsSynthetic) continue;
                bm.AddMark(new Bookmarks.MethodBookmark(textEditorControl.Document, m));
            }
            foreach (IProperty m in c.Properties)
            {
                if (m.Region.IsEmpty || m.IsSynthetic) continue;
                bm.AddMark(new Bookmarks.PropertyBookmark(textEditorControl.Document, m));
            }
            foreach (IField f in c.Fields)
            {
                if (f.Region.IsEmpty || f.IsSynthetic) continue;
                bm.AddMark(new Bookmarks.FieldBookmark(textEditorControl.Document, f));
            }
            foreach (IEvent e in c.Events)
            {
                if (e.Region.IsEmpty || e.IsSynthetic) continue;
                bm.AddMark(new Bookmarks.EventBookmark(textEditorControl.Document, e));
            }
        }

        private bool IsClassMemberBookmark(Bookmark b)
        {
            return b is Bookmarks.ClassMemberBookmark || b is Bookmarks.ClassBookmark;
        }

        #region IClipboardHandler Members

        public bool EnableCut
        {
            get
            {
                return !this.IsDisposed && this.ActiveTextArea.ClipboardHandler.EnableCut;
            }
        }

        public bool EnableCopy
        {
            get
            {
                return !this.IsDisposed && this.ActiveTextArea.ClipboardHandler.EnableCopy;
            }
        }

        public bool EnablePaste
        {
            get
            {
                return !this.IsDisposed && this.ActiveTextArea.ClipboardHandler.EnablePaste;
            }
        }

        public bool EnableDelete
        {
            get
            {
                return !this.IsDisposed && this.ActiveTextArea.ClipboardHandler.EnableDelete;
            }
        }

        public bool EnableSelectAll
        {
            get
            {
                return !this.IsDisposed && this.ActiveTextArea.ClipboardHandler.EnableSelectAll;
            }
        }

        public void SelectAll()
        {
            this.ActiveTextArea.ClipboardHandler.SelectAll(null, null);
        }

        public void Delete()
        {
            this.ActiveTextArea.ClipboardHandler.Delete(null, null);
        }

        public void Paste()
        {
            this.ActiveTextArea.ClipboardHandler.Paste(null, null);
        }

        public void Copy()
        {
            this.ActiveTextArea.ClipboardHandler.Copy(null, null);
        }

        public void Cut()
        {
            this.ActiveTextArea.ClipboardHandler.Cut(null, null);
        }
        #endregion

        Control IToolsHost.ToolsControl
        {
            get { return TextEditorSideBar.Instance; }
        }

        public override string ToString()
        {
            return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
        }
    }
}
